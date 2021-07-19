using System;

using Zlo4NET.Api.Service;
using Zlo4NET.Api.Shared;
using Zlo4NET.Data;

namespace Zlo4NET.Helpers
{
    internal static class ZThrowHelper
    {
        private static readonly IZConnection _connection;

        #region Ctor

        static ZThrowHelper()
        {
            var api = ZApi.Instance;
            var connection = api.GetApiConnection();

            _connection = connection;
        }

        #endregion

        #region Public interface

        public static void ThrowIfNotConnected()
        {
            if (_connection.IsConnected || _connection.IsPending)
            {
                throw new InvalidOperationException("There is no connection to the ZClient.");
            }
        }

        public static void ThrowIfNone(ZGame targetGame)
        {
            if (targetGame == ZGame.None)
            {
                throw new ArgumentOutOfRangeException(nameof(targetGame), targetGame, nameof(ZGame));
            }
        }

        #endregion
    }
}