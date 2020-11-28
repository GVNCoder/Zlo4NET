using System;
using System.Threading;
using System.Threading.Tasks;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

namespace Examples
{
    public class Program
    {
        private static IZApi _zloApi;
        private static IZGameFactory _gameFactory;

        internal static void Main(string[] args)
        {
            // setup internal state
            _zloApi = ZApi.Instance;
            _gameFactory = _zloApi.GameFactory;

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
        }

        internal static async Task MainAsync(string[] args)
        {
            #region Get target game from User

            // select game
            Console.Write($"Select target game where {ZGame.BF3}[1] {ZGame.BF4}[2] {ZGame.BFH}[3]: ");

            // get user input
            var gameSelectUserInput = Console.ReadLine();

            // validate input
            if (!int.TryParse(gameSelectUserInput, out var targetGame) || targetGame <= 0 || targetGame > 3)
                throw new InvalidOperationException("Invalid input!");

            // cuz BF3 = 0
            var game = (ZGame)targetGame - 1;

            #endregion

            #region Get target game mode

            // select game mode
            Console.Write($"Select game mode where {ZPlayMode.Singleplayer}[1] {ZPlayMode.Multiplayer}[2] {ZPlayMode.TestRange}[5]: \n");

            // get user input
            var gameModeSelectUserInput = Console.ReadLine();

            // validate input
            if (!int.TryParse(gameModeSelectUserInput, out var targetGameMode) || targetGameMode <= 0 || targetGameMode > 2)
                throw new InvalidOperationException("Invalid input!");

            // cuz Singleplayer = 0
            var gameMode = (ZPlayMode)targetGameMode - 1;

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
                    //testrange process 
             var testRange = await _gameFactory.CreateTestRangeAsync(new ZTestRangeParams { Game = game });

                  //run test range process handler 
             await _RunAndTrack(testRange);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static async Task _RunAndTrack(IZRunGame gameProcess)
        {
            var resetEvent = new ManualResetEvent(false);

            // track game pipe
            gameProcess.Pipe += (sender, pipeArgs) =>
            {
                Console.WriteLine(pipeArgs.FullMessage);

                // return from _RunAndTrack if game closed
                if (pipeArgs.SecondPart.Contains("Closed"))
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

        internal static async Task _MultiplayerHandler(ZGame game)
        {
            //table line for top and bottom 
            string straithLine = "_______________________________________________________________________________________";
            var service = _zloApi.CreateServersListService(game);
            var factory = _zloApi.GameFactory;
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            service.InitialSizeReached += (s, e) => resetEvent.Set();

            service.StartReceiving();
            resetEvent.WaitOne();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n {0,88} \n {1,5} {2,50}| {3,20}| \n {4,88}", straithLine, "ID:", "SERVERNAME", "MAP:", straithLine);
            foreach (var item in service.ServersCollection)
            {
                Console.WriteLine("{0,5}| {1,50}| {2,20}|", item.Id, item.Name, item.MapRotation.Current.Name);
            }
            Console.WriteLine($"{straithLine} \n \n SERVER ID : \n");
            var id = Console.ReadLine();
            var gameProcess = await factory.CreateMultiAsync(new ZMultiParams { Game = game, ServerId = uint.Parse(id) });

            // run and track game pipe
            await _RunAndTrack(gameProcess);
        }
    }
}
