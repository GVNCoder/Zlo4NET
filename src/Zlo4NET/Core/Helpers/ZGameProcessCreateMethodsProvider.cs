using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.Core.Helpers
{
    internal static class ZGameProcessCreateMethodsProvider
    {
        public static IZGameProcess CreateBF3GameProcess(ZInstalledGame targetGame, string commandArguments)
        {
            return new ZGameProcess(commandArguments, targetGame, "venice_snowroller", "bf3");
        }

        public static IZGameProcess CreateBF4GameProcess(ZInstalledGame targetGame, string commandArguments)
        {
            return new ZGameProcess(commandArguments, targetGame, "warsaw_snowroller",
                targetGame.Architecture == ZGameArchitecture.x64 ? "bf4" : "bf4_x86");
        }

        public static IZGameProcess CreateBFHLGameProcess(ZInstalledGame targetGame, string commandArguments)
        {
            return new ZGameProcess(commandArguments, targetGame, "omaha_snowroller", "bfh");
        }
    }
}