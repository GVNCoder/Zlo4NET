using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Zlo4NET.Api;
using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

// ReSharper disable ClassNeverInstantiated.Global

namespace Examples
{
    public class Program
    {
        private static IZApi _zloApi;
        private static IZGameFactory _gameFactory;

        private static void Main(string[] args)
        {
            // setup internal state
            _zloApi = ZApi.Instance;
            _gameFactory = _zloApi.GameFactory;

            var logger = _zloApi.Logger;

            // configure logging
            logger.SetLogLevelFiltering(ZLogLevel.Debug | ZLogLevel.Warning | ZLogLevel.Error | ZLogLevel.Info);
            logger.LogMessage += (sender, messageArgs) => Console.WriteLine(messageArgs.Message);

            var connection = _zloApi.Connection;

            // configure api thread synchronization
            _zloApi.Configure(new ZConfiguration
            {
                SynchronizationContext = new SynchronizationContext()
            });

            Console.WriteLine("Connecting to ZClient...");

            // create connection
            var resetEvent = new ManualResetEvent(false);

            connection.ConnectionChanged += (sender, changedArgs) => resetEvent.Set();
            connection.Connect();

            // wait for connection
            resetEvent.WaitOne();

            if (_zloApi.Connection.IsConnected)
            {
                Console.WriteLine("Connected\n");

                // call async version of Main(...)                  
                MainAsync(args).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Cannot connect to ZClient\n");
            }

            Console.ReadKey();
        }

        private static async Task MainAsync(string[] args)
        {
            #region Get target game from User

            // select game
            Console.Write($"Select target game where {ZGame.BF3}[1] {ZGame.BF4}[2] {ZGame.BFH}[3]: ");

            // get user input
            var gameSelectUserInput = Console.ReadLine();

            // validate input
            if (! int.TryParse(gameSelectUserInput, out var targetGame) || targetGame <= 0 || targetGame > 3)
                throw new InvalidOperationException("Invalid input!");

            // cuz BF3 = 0
            var game = (ZGame)targetGame - 1;

            #endregion

            #region Get target game mode

            // select game mode
            Console.Write($"Select game mode where {ZPlayMode.Singleplayer}[1] {ZPlayMode.Multiplayer}[2] \n");

            // get user input
            var gameModeSelectUserInput = Console.ReadLine();

            // validate input
            if (! int.TryParse(gameModeSelectUserInput, out var targetGameMode) || targetGameMode <= 0 || targetGameMode > 2)
                throw new InvalidOperationException("Invalid input!");

            // cuz Singleplayer = 0
            var gameMode = (ZPlayMode) targetGameMode;

            #endregion

            switch (gameMode)
            {
                // create and run singleplayer game
                case ZPlayMode.Singleplayer:

                    // create game
                    var gameProcess = await _gameFactory.CreateSingleAsync(new ZSingleParams { Game = game });

                    // run and track game pipe
                    await _RunAndTrack(gameProcess);

                    break;
                case ZPlayMode.Multiplayer:

                    await _MultiplayerHandler(game);

                    break;

                case ZPlayMode.CooperativeHost:
                case ZPlayMode.CooperativeClient:
                case ZPlayMode.TestRange:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static async Task _RunAndTrack(IZGameProcess gameProcess)
        {
            var resetEvent = new ManualResetEvent(false);

            // track game pipe
            gameProcess.StateChanged += (sender, pipeArgs) =>
            {
                Console.WriteLine(pipeArgs.RawFullMessage);

                // return from _RunAndTrack if game closed
                if (pipeArgs.Event == ZGameEvent.StateChanged && pipeArgs.States.Contains(ZGameState.State_GameClose))
                {
                    resetEvent.Set();
                }
            };

            // run game process
            var runResult = await gameProcess.RunAsync();

            // the result will be an enumeration
            // that will help determine if the game was launched successfully (returned directly by the ZClient)
            if (runResult != ZRunResult.Success)
            {
                // TODO: Do some stuff here
            }

            resetEvent.WaitOne();
        }

        private static async Task _MultiplayerHandler(ZGame game)
        {
            // build the server list service instance
            var serverListService = await _zloApi.CreateServersListAsync(game);
            var serverListCollection = new List<ZServerDto>();
            var resetEvent = new ManualResetEvent(false);

            // configure server list service
            serverListService.ServerListActionCallback = (action, id, server) =>
            {
                switch (action)
                {
                    case ZServerListAction.ServerAddOrUpdate:
                        serverListCollection.Add(server);
                        break;
                    case ZServerListAction.ServerPlayersList:

                        resetEvent.Set();

                        var serverModel = serverListCollection.FirstOrDefault(i => i.Id == id);
                        if (serverModel != null)
                        {
                            serverModel.PlayersList = server.PlayersList;
                        }
                        break;
                    case ZServerListAction.ServerRemove:
                        var index = serverListCollection.FindIndex(i => i.Id == id);
                        if (index != -1)
                        {
                            serverListCollection.RemoveAt(index);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(action), action, null);
                }
            };

            await serverListService.StartReceivingAsync();
            
            // wait to server list full load
            resetEvent.WaitOne();

            // draw servers table
            const string straightLine = "_______________________________________________________________________________________";

            // configure console
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n {0,88} \n {1,5} {2,50}| {3,30}| \n {4,88}", straightLine, "ID:", "ServerName", "Map:", straightLine);

            foreach (var item in serverListCollection)
            {
                Console.WriteLine("{0,5}| {1,50}| {2,30}|", item.Id, item.Name, item.MapRotation.Current.Name);
            }

            Console.Write($"{straightLine} \n \nTo join, Enter a server ID: ");

            #region Get target server Id

            // get user input
            var serverIdUserInput = Console.ReadLine();

            // validate input
            if (! uint.TryParse(serverIdUserInput, out var targetServerId))
                throw new InvalidOperationException("Invalid input!");

            #endregion

            // add some space between user input and game log
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();

            // create game
            var gameProcess = await _gameFactory.CreateMultiAsync(new ZMultiParams { Game = game, ServerId = targetServerId });

            // run and track game pipe
            await _RunAndTrack(gameProcess);
        }
    }
}
