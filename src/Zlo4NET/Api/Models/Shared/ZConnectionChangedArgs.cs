using System;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines connection changed event args
    /// </summary>
    public class ZConnectionChangedArgs : EventArgs
    {
        /// <summary>
        /// Gets connection state
        /// </summary>
        public bool IsConnected { get; }
        /// <summary>
        /// Gets authorized user
        /// </summary>
        public ZUser AuthorizedUser { get; }

        /// <inheritdoc />
        public ZConnectionChangedArgs(bool isConnected, ZUser user)
        {
            IsConnected = isConnected;
            AuthorizedUser = user;
        }
    }
}