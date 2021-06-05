using System;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZGameHelper
    {
        public static void ThrowIfOutOfRange(ZGame game)
        {
            if (game == ZGame.None)
            {
                throw new ArgumentOutOfRangeException(nameof(game), game, nameof(ZGame));
            }
        }
    }
}