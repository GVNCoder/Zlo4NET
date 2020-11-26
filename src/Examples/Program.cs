using System;
using System.Threading;
using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

namespace Examples
{
    public class Program
    {
        public static bool received = false;

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
                Console.WriteLine("Connected\n");

                // call async version of Main(...)
                MainAsync(args).GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine("Cannot connect to ZClient\n");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
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
                // create and run singleplayer game
                case ZPlayMode.Singleplayer:

                    // create game
                    var gameProcess = await gameFactory.CreateSingleAsync(new ZSingleParams { Game = game });

                    // run and track game pipe
                    await _RunAndTrack(gameProcess);

                    break;
                case ZPlayMode.Multiplayer:
                    Mulitplayer(game);
                    // TODO: Add multiplayer handling here

                    break;

                case ZPlayMode.CooperativeHost:
                case ZPlayMode.CooperativeClient:
                case ZPlayMode.TestRange:
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

            Console.WriteLine($"Run result {runResult}");

            resetEvent.WaitOne();
        }

        public static async void Mulitplayer(ZGame game)
        {
            string strainthline = "_______________________________________________________________________________________";
            var Api = ZApi.Instance;
            var service = Api.CreateServersListService(game);
            var factory = Api.GameFactory;
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            service.InitialSizeReached += (s, e) => resetEvent.Set();
            

            service.StartReceiving();
            resetEvent.WaitOne();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n {0,88} \n {1,5} {2,50}| {3,20}| {4,5}| \n {5,88}",strainthline,"ID:", "SERVERNAME" , "MAP:" ,"Players",strainthline);
            foreach (var item in service.ServersCollection)
            {
                Console.WriteLine("{0,5}| {1,50}| {2,20}| {3,2} / {4,2} |",item.Id,item.Name,item.MapRotation.Current.Name,item.CurrentPlayersNumber,item.PlayersCapacity);
            }
            Console.WriteLine($"{strainthline} \n \n SERVER ID : \n");
            var id = Console.ReadLine();
            var gameProcess = await factory.CreateMultiAsync(new ZMultiParams { Game = game, ServerId = uint.Parse(id) });
            await _RunAndTrack(gameProcess);

        }

        public static void list_recieved(object sender, EventArgs e)
        {
            Console.WriteLine("Список получен");
            received = true;   
        }
    }
}
