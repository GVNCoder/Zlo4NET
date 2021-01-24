using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZGameFactory : IZGameFactory
    {
        private const string _SingleKey = "single";
        private const string _MultiKey = "multi";
        private const string _CoopHostKey = "coop host";
        private const string _CoopJoinKey = "coop join";
        private const string _TestRangeKey = "test range";

        private const string _personaRefReplaceable = "[personaRef]";
        private const string _gameIdReplaceable = "[gameId]";
        private const string _isSpectatorReplaceable = "[isSpectator]";
        private const string _roleReplaceable = "[role]";
        private const string _friendIdReplaceable = "[friendId]";
        private const string _levelReplaceable = "[level]";
        private const string _difficultyReplaceable = "[difficulty]";

        private const string _spectatorValue = "isspectator=\\\"true\\\"";

        private readonly IZClientService _clientService;
        private readonly IZInstalledGamesService _installedGamesService;
        private readonly IZConnection _connection;

        private JObject __runStrings;
        private ZUser __userContext => _connection.AuthorizedUser;

        public ZGameFactory(IZClientService clientService, IZConnection connection)
        {
            _clientService = clientService;
            _installedGamesService = new ZInstalledGamesService(clientService);
            _connection = connection;

            _loadRunJSON();
        }

        #region Private methods

        private IZGameProcess _createRunGame(ZInstalledGame target, string command, ZGame game, ZGameArchitecture architecture)
        {
            switch (game)
            {
                case ZGame.BF3: return new ZGameProcess(_clientService, command, target, "venice_snowroller", "bf3");
                case ZGame.BF4:
                    return new ZGameProcess(_clientService, command, target, "warsaw_snowroller",
                        architecture == ZGameArchitecture.x64 ? "bf4" : "bf4_x86");
                case ZGame.BFH:
                    return new ZGameProcess(_clientService, command, target, "omaha_snowroller",
                        architecture == ZGameArchitecture.x64 ? "bfh" : "bfh_x86");

                case ZGame.None:
                default: throw new Exception();
            }
        }

        private void _loadRunJSON()
        {
            using (var sr = new StreamReader(ZInternalResource.GetResourceStream("run.json")))
            {
                var content = sr.ReadToEnd();
                __runStrings = JObject.Parse(content);
            }
        }

        #endregion

        public async Task<IZGameProcess> CreateSingleAsync(ZSingleParams args)
        {
            ZConnectionHelper.MakeSureConnection();

            var installedGames = await _installedGamesService.GetInstalledGamesAsync();
            if (installedGames == null)
            {
                throw new Exception("Installed games not received. Check your ZClient connection.");
            }

            var architecture = args.PreferredArchitecture ??
                               (installedGames.IsX64 ? ZGameArchitecture.x64 : ZGameArchitecture.x32);
            var compatibleGames = installedGames.InstalledGames
                .Where(insGame => insGame.EnumGame == args.Game)
                .ToArray();
            var targetGame = compatibleGames.Length > 1
                ? compatibleGames.FirstOrDefault(insGame => insGame.RunnableName.EndsWith(architecture.ToString()))
                : compatibleGames.FirstOrDefault();

            if (targetGame == null)
            {
                throw new InvalidOperationException($"The target game {args.Game} not found.");
            }

            var commandRun = __runStrings[_SingleKey].ToObject<string>();
            commandRun = commandRun.Replace(_personaRefReplaceable, __userContext.Id.ToString());

            var runGame = _createRunGame(targetGame, commandRun, args.Game, architecture);
            return runGame;
        }

        public async Task<IZGameProcess> CreateCoOpAsync(ZCoopParams args)
        {
            ZConnectionHelper.MakeSureConnection();

            if (args.Mode != ZPlayMode.CooperativeHost && args.Mode != ZPlayMode.CooperativeClient)
            {
                throw new ArgumentException("Mode contains wrong value. Allowed values is CooperativeHost or CooperativeClient.");
            }

            if (ZPlayMode.CooperativeClient == args.Mode && args.FriendId == null)
                throw new ArgumentException($"For this {args.Mode} mode need to specify {nameof(args.FriendId)} value.");
            else if (ZPlayMode.CooperativeHost == args.Mode && (args.Difficulty == null || args.Level == null))
                throw new ArgumentException($"For this {args.Mode} mode need to specify {nameof(args.Difficulty)}, {nameof(args.Level)} value.");

            var installedGames = await _installedGamesService.GetInstalledGamesAsync();
            if (installedGames == null)
            {
                throw new Exception("Installed games not received. Check your ZClient connection.");
            }

            var architecture = args.PreferredArchitecture ??
                               (installedGames.IsX64 ? ZGameArchitecture.x64 : ZGameArchitecture.x32);
            var compatibleGames = installedGames.InstalledGames
                .Where(insGame => insGame.EnumGame == ZGame.BF3)
                .ToArray();
            var targetGame = compatibleGames.Length > 1
                ? compatibleGames.FirstOrDefault(insGame => insGame.RunnableName.EndsWith(architecture.ToString()))
                : compatibleGames.FirstOrDefault();

            if (targetGame == null)
            {
                throw new InvalidOperationException($"The target game {ZGame.BF3} not found.");
            }

            string commandRun;
            if (args.Mode == ZPlayMode.CooperativeHost)
            {
                commandRun = __runStrings[_CoopHostKey].ToObject<string>();
                commandRun = commandRun.Replace(_levelReplaceable, args.Level.ToString().ToUpper());
                commandRun = commandRun.Replace(_difficultyReplaceable, args.Difficulty.ToString().ToUpper());
                commandRun = commandRun.Replace(_personaRefReplaceable, __userContext.Id.ToString());
            }
            else
            {
                commandRun = __runStrings[_CoopJoinKey].ToObject<string>();
                commandRun = commandRun.Replace(_friendIdReplaceable, args.FriendId.ToString());
                commandRun = commandRun.Replace(_personaRefReplaceable, __userContext.Id.ToString());
            }

            var runGame = _createRunGame(targetGame, commandRun, ZGame.BF3, architecture);
            return runGame;
        }

        public async Task<IZGameProcess> CreateTestRangeAsync(ZTestRangeParams args)
        {
            ZConnectionHelper.MakeSureConnection();

            if (args.Game == ZGame.BF3)
            {
                throw new NotSupportedException("Battlefield 3 TestRange play mode not supported.");
            }

            if (args.Game == ZGame.BFH)
            {
                throw new NotImplementedException("Battlefield Hardline TestRange not implemented in ZLOEmu.");
            }

            var installedGames = await _installedGamesService.GetInstalledGamesAsync();
            if (installedGames == null)
            {
                throw new Exception("Installed games not received. Check your ZClient connection.");
            }

            var architecture = args.PreferredArchitecture ??
                               (installedGames.IsX64 ? ZGameArchitecture.x64 : ZGameArchitecture.x32);
            var compatibleGames = installedGames.InstalledGames
                .Where(insGame => insGame.EnumGame == args.Game)
                .ToArray();
            var targetGame = compatibleGames.Length > 1
                ? compatibleGames.FirstOrDefault(insGame => insGame.RunnableName.EndsWith(architecture.ToString()))
                : compatibleGames.FirstOrDefault();

            if (targetGame == null)
            {
                throw new Exception($"The target game {args.Game} not found.");
            }

            var commandRun = __runStrings[_TestRangeKey].ToObject<string>();
            commandRun = commandRun.Replace(_personaRefReplaceable, __userContext.Id.ToString());

            var runGame = _createRunGame(targetGame, commandRun, args.Game, architecture);
            return runGame;
        }

        public async Task<IZGameProcess> CreateMultiAsync(ZMultiParams args)
        {
            ZConnectionHelper.MakeSureConnection();

            if ((args.Game == ZGame.BF3 || args.Game == ZGame.BFH) && args.Role == ZRole.Spectator)
            {
                throw new ArgumentException("BF3\\BFH is not support Spectator mode.");
            }

            var installedGames = await _installedGamesService.GetInstalledGamesAsync();
            if (installedGames == null)
            {
                throw new Exception("Installed games not received. Check your ZClient connection.");
            }

            var architecture = args.PreferredArchitecture ??
                               (installedGames.IsX64 ? ZGameArchitecture.x64 : ZGameArchitecture.x32);
            var compatibleGames = installedGames.InstalledGames
                .Where(insGame => insGame.EnumGame == args.Game)
                .ToArray();
            var targetGame = compatibleGames.Length > 1
                ? compatibleGames.FirstOrDefault(insGame => insGame.RunnableName.EndsWith(architecture.ToString()))
                : compatibleGames.FirstOrDefault();

            if (targetGame == null)
            {
                throw new Exception($"The target game {args.Game} not found.");
            }

            var commandRun = __runStrings[_MultiKey].ToObject<string>();
            commandRun = commandRun.Replace(_gameIdReplaceable, args.ServerId.ToString());
            commandRun = commandRun.Replace(_personaRefReplaceable, __userContext.Id.ToString());

            if (args.Role != ZRole.Spectator)
            {
                commandRun = commandRun.Replace(_roleReplaceable, args.Role.ToString().ToLower());
                commandRun = commandRun.Replace(_isSpectatorReplaceable, string.Empty);
            }
            else
            {
                commandRun = commandRun.Replace(_roleReplaceable, ZRole.Soldier.ToString().ToLower());
                commandRun = commandRun.Replace(_isSpectatorReplaceable, _spectatorValue);
            }

            var runGame = _createRunGame(targetGame, commandRun, args.Game, architecture);
            return runGame;
        }

        public Task<IZGameProcess> CreateByProcessNameAsync(ZProcessParameters args)
        {
            var gameInstance = new ZGameProcess(args.ProcessName);

            return Task.FromResult<IZGameProcess>(gameInstance);
        }
    }
}