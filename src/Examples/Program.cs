using System;
using System.Threading;
using System.Threading.Tasks;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;

namespace Examples
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            var zloApi = ZApi.Instance;

            // configure api thread synchronization
            zloApi.Configure(new ZConfiguration
            {
                SynchronizationContext = new SynchronizationContext()
            });

            Console.WriteLine("Connecting to ZClient...");

            // create connection
            var resetEvent = new ManualResetEvent(false);

            zloApi.Connection.ConnectionChanged += (sender, changedArgs) => resetEvent.Set();
            zloApi.Connection.Connect();

            // wait for connection
            resetEvent.WaitOne();

            if (zloApi.Connection.IsConnected)
            {
                // call async version of Main(...)
                MainAsync(args).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Cannot connection to ZClient");
            }
        }

        internal static async Task MainAsync(string[] args)
        {
            var zloApi = ZApi.Instance;
            var connection = zloApi.Connection;
            var gameFactory = zloApi.GameFactory;

            #region Get target game from User

            // select game
            Console.Write($"Select target game where {ZGame.BF3}[1] {ZGame.BF4}[2] {ZGame.BFH}[3]: ");

            // get user input
            var gameSelectUserInput = Console.ReadLine();

            // validate input
            if (!int.TryParse(gameSelectUserInput, out var targetGame) || targetGame <= 0 || targetGame > 3)
                throw new InvalidOperationException("Invalid input!");

            // cuz BF3 = 0
            var game = (ZGame) targetGame - 1;

            #endregion

            #region Get target game mode

            // select game mode
            Console.Write($"Select game mode where {ZPlayMode.Singleplayer}[1] {ZPlayMode.Multiplayer}[2]: ");

            // get user input
            var gameModeSelectUserInput = Console.ReadLine();

            // validate input
            if (!int.TryParse(gameModeSelectUserInput, out var targetGameMode) || targetGameMode <= 0 || targetGameMode > 2)
                throw new InvalidOperationException("Invalid input!");

            // cuz Singleplayer = 0
            var gameMode = (ZPlayMode) targetGameMode - 1;

            #endregion

            switch (gameMode)
            {
                case ZPlayMode.Singleplayer:
                    var gameProcess = await gameFactory.CreateSingleAsync(new ZSingleParams { Game = game });

                    // track game pipe
                    gameProcess.Pipe += (sender, pipeArgs) => Console.WriteLine(pipeArgs.FullMessage);

                    // run game process
                    var runResult = await gameProcess.RunAsync();

                    Console.WriteLine($"Run result {runResult}");

                    break;
                case ZPlayMode.Multiplayer:
                    break;

                case ZPlayMode.CooperativeHost:
                case ZPlayMode.CooperativeClient:
                case ZPlayMode.TestRange:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
