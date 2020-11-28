using System;

using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.Client
{
    internal interface IZClient
    {
        void Connect();
        void Disconnect();

        void SendRequest(byte[] requestData);

        event EventHandler<ZClientConnectionChangedArgs> ConnectionStatusChanged;
        event EventHandler<ZClientDataReceivedArgs> DataReceived;
    }
}