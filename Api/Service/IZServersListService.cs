using System;
using System.Collections.ObjectModel;
using Zlo4NET.Api.Models.Server;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Defines servers list service
    /// </summary>
    public interface IZServersListService : IDisposable
    {
        /// <summary>
        /// Makes a request to receive data about servers and starts processing them.
        /// </summary>
        void StartReceiving();
        /// <summary>
        /// Releases all resources used by this instance. After calling this method, this instance can no longer be used.
        /// </summary>
        void StopReceiving();
        /// <summary>
        /// Gets observable collection of servers
        /// </summary>
        ObservableCollection<ZServerBase> ServersCollection { get; }
        /// <summary>
        /// Occurs, when server list initial size reached
        /// </summary>
        event EventHandler InitialSizeReached;
        /// <summary>
        /// Gets a value indicating the availability of this instance. 
        /// </summary>
        bool CanUse { get; }
    }
}