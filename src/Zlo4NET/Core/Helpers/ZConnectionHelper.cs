using System;

using Zlo4NET.Api.Service;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZConnectionHelper
    {
        private static IZConnection _connection;

        public static void Initialize(IZConnection connection) => _connection = connection;
        public static void ThrowIfNotConnected()
        {
            if (_connection.IsConnected)
            {
                return;
            }

            throw new InvalidOperationException("There is no connection to the ZClient.");
        }
    }
}