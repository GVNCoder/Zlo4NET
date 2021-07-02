using System;
using System.Collections.Generic;

namespace Zlo4NET.ZClientAPI
{
    /// <summary>
    /// Encapsulates a method that handle received packets from ZClient
    /// </summary>
    /// <param name="packets">Received packets</param>
    internal delegate void ZPacketsReceivedHandler(IEnumerable<ZPacket> packets);

    /// <summary>
    /// Represents the ZClient client
    /// </summary>
    internal interface IZClient
    {
        /// <summary>
        /// Occurs when connection state changed
        /// </summary>
        event Action<bool> ConnectionStateChanged;
        /// <summary>
        /// Occurs when packets was received from ZClient
        /// </summary>
        event ZPacketsReceivedHandler PacketsReceived;
        /// <summary>
        /// Runs client to attempt establish connection with ZClient
        /// </summary>
        void Run();
        /// <summary>
        /// Closes current connection with ZClient
        /// </summary>
        void Close();
        /// <summary>
        /// Sends request bytes to ZClient
        /// </summary>
        /// <param name="requestBytes">Request bytes to send</param>
        void SendRequest(byte[] requestBytes);
    }
}