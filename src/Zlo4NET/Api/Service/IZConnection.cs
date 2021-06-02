using System;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Represents a service that allows you to manage and control the connection to the ZClient
    /// </summary>
    public interface IZConnection
    {
        /// <summary>
        /// Initiates an asynchronous connection to the ZClient
        /// </summary>
        void Connect();
        /// <summary>
        /// Initiates a disconnection to the ZClient. Raises a <see cref="ConnectionChanged"/> event by default
        /// </summary>
        /// <param name="raiseEvent">Indicates whether the connection state change event should be raised or not</param>
        void Disconnect(bool raiseEvent = true);
        /// <summary>
        /// Gets current authorized in ZClient user
        /// </summary>
        /// <returns>Returns current authorized in ZClient user</returns>
        ZUser GetCurrentUserInfo();
        /// <summary>
        /// Occurs when the connection changes state
        /// </summary>
        event EventHandler<ZConnectionChangedEventArgs> ConnectionChanged;
        /// <summary>
        /// Gets current connection state with ZClient
        /// </summary>
        bool IsConnected { get; }
    }
}