using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Zlo4NET.Api;
using Zlo4NET.Core.Data;
using Zlo4NET.Api.Service;
using Zlo4NET.Api.Models.Shared;

// ReSharper disable InconsistentNaming
// ReSharper disable AssignNullToNotNullAttribute
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

                RunGameExampleAsync(args).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Cannot connect to ZClient\n");
            }

            Console.ReadKey();
        }

        #region Examples

        private static async Task RunGameExampleAsync(string[] args)
        {
            // in fact, the ZApi.Instance class is a layer of access to all the services and capabilities of the API
            var api = ZApi.Instance;

            // using a factory, you can create a game process to run it
            var gameFactory = api.GameFactory;

            // using this service, you can get a list of all available games
            var installedGames = api.InstalledGamesService;

            // you cannot create a process for a game that is not installed, so first, you need to get a list of all available and supported API games
            var availableGames = await installedGames.GetGamesCollectionAsync();

            // print available info
            Console.WriteLine($"Your operating system: {(availableGames.IsX64OperatingSystem ? ZGameArchitecture.x64 : ZGameArchitecture.x32)}\n");

            for (var i = 0; i < availableGames.Games.Length; i++)
            {
                var installedGame = availableGames.Games[i];

                Console.WriteLine($"\tId: {i + 1} Game: {installedGame.ReadableName} Arch: {installedGame.Architecture}");
            }

            Console.Write("\nPlease, select game: ");

            // get user input
            var userInput = Console.ReadLine();
            var gameIndex = int.Parse(userInput) - 1;
            var availableGame = availableGames.Games[gameIndex];

            // create game process
            var gameProcess = gameFactory.CreateSingle(new ZSingleLaunchParameters { TargetGame = availableGame });

            await _RunAndTrack(gameProcess);
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

            // the result will be an enum
            // that will help determine if the game was launched successfully (returned directly by the ZClient)
            if (runResult != ZRunResult.Success)
            {
                // TODO: Do some stuff here
            }

            resetEvent.WaitOne();
        }

        #endregion

        //private static async Task MainAsync(string[] args)
        //{
        //    #region Get target game from User

        //    // select game
        //    Console.Write($"Select target game where {ZGame.BF3}[1] {ZGame.BF4}[2] {ZGame.BFHL}[3]: ");

        //    // get user input
        //    var gameSelectUserInput = Console.ReadLine();

        //    // validate input
        //    if (! int.TryParse(gameSelectUserInput, out var targetGame) || targetGame <= 0 || targetGame > 3)
        //        throw new InvalidOperationException("Invalid input!");

        //    // cuz BF3 = 0
        //    var game = (ZGame)targetGame - 1;

        //    #endregion

        //    #region Get target game mode

        //    // select game mode
        //    Console.Write($"Select game mode where {ZPlayMode.Singleplayer}[1] {ZPlayMode.Multiplayer}[2] \n");

        //    // get user input
        //    var gameModeSelectUserInput = Console.ReadLine();

        //    // validate input
        //    if (! int.TryParse(gameModeSelectUserInput, out var targetGameMode) || targetGameMode <= 0 || targetGameMode > 2)
        //        throw new InvalidOperationException("Invalid input!");

        //    // cuz Singleplayer = 0
        //    var gameMode = (ZPlayMode) targetGameMode;

        //    #endregion

        //    switch (gameMode)
        //    {
        //        // create and run singleplayer game
        //        case ZPlayMode.Singleplayer:

        //            // create game
        //            var gameProcess = await _gameFactory.CreateSingleAsync(new ZSingleLaunchParameters { Game = game });

        //            // run and track game pipe
        //            await _RunAndTrack(gameProcess);

        //            break;
        //        case ZPlayMode.Multiplayer:

        //            await _MultiplayerHandler(game);

        //            break;

        //        case ZPlayMode.CooperativeHost:
        //        case ZPlayMode.CooperativeClient:
        //        case ZPlayMode.TestRange:
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        //private static async Task _RunAndTrack(IZGameProcess gameProcess)
        //{
        //    var resetEvent = new ManualResetEvent(false);

        //    // track game pipe
        //    gameProcess.StateChanged += (sender, pipeArgs) =>
        //    {
        //        Console.WriteLine(pipeArgs.RawFullMessage);

        //        // return from _RunAndTrack if game closed
        //        if (pipeArgs.Event == ZGameEvent.StateChanged && pipeArgs.States.Contains(ZGameState.State_GameClose))
        //        {
        //            resetEvent.Set();
        //        }
        //    };

        //    // run game process
        //    var runResult = await gameProcess.RunAsync();

        //    // the result will be an enumeration
        //    // that will help determine if the game was launched successfully (returned directly by the ZClient)
        //    if (runResult != ZRunResult.Success)
        //    {
        //        // TODO: Do some stuff here
        //    }

        //    resetEvent.WaitOne();
        //}

        //private static async Task _MultiplayerHandler(ZGame game)
        //{
        //    // build the server list service instance
        //    var serverListService = await _zloApi.CreateServersListAsync(game);
        //    var serverListCollection = new List<ZServerDto>();
        //    var resetEvent = new ManualResetEvent(false);

        //    // configure server list service
        //    serverListService.ServerListActionCallback = (action, id, server) =>
        //    {
        //        switch (action)
        //        {
        //            case ZServerListAction.ServerAddOrUpdate:
        //                serverListCollection.Add(server);
        //                break;
        //            case ZServerListAction.ServerPlayersList:

        //                resetEvent.Set();

        //                var serverModel = serverListCollection.FirstOrDefault(i => i.Id == id);
        //                if (serverModel != null)
        //                {
        //                    serverModel.PlayersList = server.PlayersList;
        //                }
        //                break;
        //            case ZServerListAction.ServerRemove:
        //                var index = serverListCollection.FindIndex(i => i.Id == id);
        //                if (index != -1)
        //                {
        //                    serverListCollection.RemoveAt(index);
        //                }
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        //        }
        //    };

        //    await serverListService.StartReceivingAsync();

        //    // wait to server list full load
        //    resetEvent.WaitOne();

        //    // draw servers table
        //    const string straightLine = "_______________________________________________________________________________________";

        //    // configure console
        //    Console.ForegroundColor = ConsoleColor.DarkYellow;
        //    Console.WriteLine("\n {0,88} \n {1,5} {2,50}| {3,30}| \n {4,88}", straightLine, "ID:", "ServerName", "Map:", straightLine);

        //    foreach (var item in serverListCollection)
        //    {
        //        Console.WriteLine("{0,5}| {1,50}| {2,30}|", item.Id, item.Name, item.MapRotation.Current.Name);
        //    }

        //    Console.Write($"{straightLine} \n \nTo join, Enter a server ID: ");

        //    #region Get target server Id

        //    // get user input
        //    var serverIdUserInput = Console.ReadLine();

        //    // validate input
        //    if (! uint.TryParse(serverIdUserInput, out var targetServerId))
        //        throw new InvalidOperationException("Invalid input!");

        //    #endregion

        //    // add some space between user input and game log
        //    Console.ForegroundColor = ConsoleColor.Gray;
        //    Console.WriteLine();

        //    // create game
        //    var gameProcess = await _gameFactory.CreateMultiAsync(new ZMultiLaunchParameters { Game = game, ServerId = targetServerId });

        //    // run and track game pipe
        //    await _RunAndTrack(gameProcess);
        //}
    }
}
