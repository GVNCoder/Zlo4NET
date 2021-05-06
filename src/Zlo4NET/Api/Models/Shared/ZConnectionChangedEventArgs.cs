using System;
using Zlo4NET.Api.Service;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Represents <see cref="IZConnection.ConnectionChanged"/> event args
    /// </summary>
    public class ZConnectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets current connection state with ZClient
        /// </summary>
        public bool IsConnected { get; }
        /// <summary>
        /// Creates an instance of <see cref="ZConnectionChangedEventArgs"/>
        /// </summary>
        public ZConnectionChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}