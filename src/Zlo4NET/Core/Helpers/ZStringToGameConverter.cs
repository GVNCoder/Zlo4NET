using System;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZStringToGameConverter
    {
        private static readonly string[] _GameStrings = new[]
        {
            ZGame.BF3.ToString(),
            ZGame.BF4.ToString(),
            ZGame.BFH.ToString()
        };

        public static ZGame Convert(string source)
        {
            if (source.IndexOf(ZGame.BF3.ToString(), StringComparison.OrdinalIgnoreCase) != -1)
            {
                return ZGame.BF3;
            }
            else if (source.IndexOf(ZGame.BF4.ToString(), StringComparison.OrdinalIgnoreCase) != -1)
            {
                return ZGame.BF4;
            }
            else if (source.IndexOf(ZGame.BFH.ToString(), StringComparison.OrdinalIgnoreCase) != -1)
            {
                return ZGame.BFH;
            }
            else
            {
                return ZGame.None;
            }
        }
    }
}