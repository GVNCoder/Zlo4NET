using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Services;

namespace Zlo4NET.Core.Data.Parsers
{
    internal static class ZParsersFactory
    {
        public static IZUserInfoParser BuildUserInfoParser() => new ZUserInfoParser();
        public static IZServersListParser BuildServersListInfoParser(uint myId, ZGame gameContext, ZLogger logger) => new ZServersListParser(myId, gameContext, logger);
        public static IZInstalledGamesParser BuildInstalledGamesInfoParser() => new ZInstalledGamesParser();
        public static IZGameRunParser BuildGameRunInfoParser() => new ZGameRunParser();
        public static IZStatsParser BuildStatsInfoParser() => new ZStatsParser();
    }
}