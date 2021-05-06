using System;

using Zlo4NET.Api.Service;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZConnectionHelper
    {
        private static IZConnection _connection;

        internal static void Initialize(IZConnection connection) => _connection = connection;

        internal static void MakeSureConnection()
        {
            if (_connection.IsConnected) return;
            throw new InvalidOperationException("API not connected");
        }
    }
}