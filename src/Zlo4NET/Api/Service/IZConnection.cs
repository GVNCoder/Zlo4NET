using System;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Defines API connection
    /// </summary>
    public interface IZConnection
    {
        /// <summary>
        /// Initiates an asynchronous ZClient connection process.
        /// </summary>
        void Connect();
        /// <summary>
        /// Terminates the connection with ZClient.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Occurs when the connection changes state.
        /// </summary>
        event EventHandler<ZConnectionChangedArgs> ConnectionChanged;

        /// <summary>
        /// Gets connection state
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// Gets authorized user
        /// </summary>
        ZUser AuthorizedUser { get; }
    }
}