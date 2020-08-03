using System;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.ZClient.Services
{
    /// <summary>
    /// Defines ZClient abstraction
    /// </summary>
    internal interface IZClient
    {
        /// <summary>
        /// Stops client asynchronously
        /// </summary>
        void StopClient();
        /// <summary>
        /// Starts client asynchronously
        /// </summary>
        void StartClient();
        /// <summary>
        /// Sends request to remote host
        /// </summary>
        /// <param name="request">The request for send</param>
        void SendRequest(ZRequest request);
        /// <summary>
        /// Register tunnel instance
        /// </summary>
        /// <param name="tunnel">The tunnel instance</param>
        void RegisterTunnel(ZTunnel tunnel);
        /// <summary>
        /// Occurs, when client connection changed
        /// </summary>
        event EventHandler<ZClientConnectionChangedArgs> ConnectionChanged;
    }
}