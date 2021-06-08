using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.Core.Helpers
{
    internal static class ZGameProcessCreateMethodsProvider
    {
        public static IZGameProcess CreateBF3GameProcess(ZInstalledGame targetGame, string commandArguments)
        {
            return new ZGameProcess(targetGame, commandArguments, "venice_snowroller", "bf3");
        }

        public static IZGameProcess CreateBF4GameProcess(ZInstalledGame targetGame, string commandArguments)
        {
            return new ZGameProcess(targetGame, commandArguments, "warsaw_snowroller",
                targetGame.Architecture == ZGameArchitecture.x64 ? "bf4" : "bf4_x86");
        }

        public static IZGameProcess CreateBFHLGameProcess(ZInstalledGame targetGame, string commandArguments)
        {
            return new ZGameProcess(targetGame, commandArguments, "omaha_snowroller", "bfh");
        }
    }
}