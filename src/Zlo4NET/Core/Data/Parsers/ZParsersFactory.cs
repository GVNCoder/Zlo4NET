using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Services;

namespace Zlo4NET.Core.Data.Parsers
{
    internal static class ZParsersFactory
    {
        public static IZUserInfoParser CreateUserInfoParser() => new ZUserInfoParser();
        public static IZServersListParser CreateServersListInfoParser(uint authorizedUserId, ZGame gameContext, ZLogger logger) => new ZServersListParser(authorizedUserId, gameContext, logger);
        public static IZInstalledGamesParser CreateInstalledGamesInfoParser() => new ZInstalledGamesParser();
        public static IZGameRunParser CreateGameRunInfoParser() => new ZGameRunParser();
        public static IZPlayerStatsParser CreateStatsInfoParser() => new ZPlayerStatsParser();
    }
}