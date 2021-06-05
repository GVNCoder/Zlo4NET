using System;

using Zlo4NET.Core.Services;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Parsers;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZParsersFactory
    {
        #region Factory methods

        public static IZUserInfoParser CreateUserInfoParser() => new ZUserInfoParser();
        public static IZServerListParser CreateServersListInfoParser(uint currentUserId, ZGame game) => _CreateParserByGame(game, currentUserId);
        public static IZInstalledGamesParser CreateInstalledGamesInfoParser() => new ZInstalledGamesParser();
        public static IZGameRunParser CreateGameRunInfoParser() => new ZGameRunParser();
        public static IZPlayerStatsParser CreateStatsInfoParser() => new ZPlayerStatsParser();

        #endregion

        #region Private helpers

        private static IZServerListParser _CreateParserByGame(ZGame game, uint currentUserId)
        {
            IZServerListParser parser;

            switch (game)
            {
                case ZGame.BF3:
                    parser = new ZBF3ServerListParser(currentUserId);
                    break;
                case ZGame.BF4:
                    parser = new ZBF4ServerListParser(currentUserId);
                    break;
                case ZGame.BFHL:
                    parser = new ZBFHLServerListParser(currentUserId);
                    break;

                case ZGame.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(game), game, null);
            }

            return parser;
        }

        #endregion
    }
}