using System;
using System.Net;
using System.Net.Sockets;

using Zlo4NET.Core.Data;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.Client
{
    internal class ZClient : IZClient
    {
        public static int _InstanceCount = 0;

        private readonly IPEndPoint _endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 48486);
        private readonly ZBuffer _buffer = new ZBuffer();
        private readonly ZLogger _log = ZLogger.Instance;

        private Socket _socket;

        public ZClient()
        {
            // we can create only one instance of ZClient class
            // so we need track instance count
            if (_InstanceCount > 1)
            {
                throw new InvalidOperationException();
            }

            // track current instance
            _InstanceCount ++;
        }

        #region Socket

        private void _SocketCreate()
            => _socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        private IAsyncResult _SocketConnect()
            => _socket.BeginConnect(_endPoint,
                new AsyncCallback(_EndConnectCallback),
                null);

        private IAsyncResult _socketBeginSend(byte[] buffer, int offset, int size)
            => _socket.BeginSend(buffer, offset, size, SocketFlags.None,
                new AsyncCallback(_EndSendCallback),
                null);

        private IAsyncResult _socketBeginReceive(byte[] buffer, int offset, int size)
            => _socket.BeginReceive(buffer, offset, size,
                SocketFlags.None,
                new AsyncCallback(_EndReceiveCallback),
                null);

        private void _EndConnectCallback(IAsyncResult asyncResult)
        {

        }

        private void _EndSendCallback(IAsyncResult asyncResult)
        {

        }

        private void _EndReceiveCallback(IAsyncResult asyncResult)
        {

        }

        #endregion

        #region IZClient

        public event EventHandler<ZClientConnectionChangedArgs> ConnectionStatusChanged;
        public event EventHandler<ZClientDataReceivedArgs> DataReceived;

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void SendRequest(byte[] requestData)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}