using System;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZExceptionHelper
    {
        public static Exception EmptyReceive => new Exception("Receive return 0 bytes");
        public static Exception EmptySend => new Exception("Send return 0 bytes");
    }
}