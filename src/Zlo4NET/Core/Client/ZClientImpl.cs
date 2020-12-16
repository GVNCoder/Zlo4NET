using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using Zlo4NET.Core.Data;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.Client
{
    internal class ZClientImpl : IZClient
    {
        private static int _InstanceCount = 0;

        private const int _StateBufferSize = 8192;

        private readonly IPEndPoint _endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 48486);
        private readonly ZBuffer _buffer = new ZBuffer();
        private readonly ZLogger _log = ZLogger.Instance;
        private readonly byte[] _stateBuffer = new byte[_StateBufferSize];

        private Socket _socket;

        public ZClientImpl()
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

        private void _SocketDestroy()
        {
            try
            {
                // check if socket already disconnected
                if (! _socket.Connected)
                {
                    return;
                }

                _socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                _socket.Close();
            }
        }

        private IAsyncResult _SocketConnect()
            => _socket.BeginConnect(_endPoint,
                new AsyncCallback(_EndConnectCallback),
                null);

        private IAsyncResult _SocketSend(byte[] buffer, int size)
            => _socket.BeginSend(buffer, 0, size, SocketFlags.None,
                new AsyncCallback(_EndSendCallback),
                buffer);

        private IAsyncResult _SocketReceive(byte[] buffer, int size)
            => _socket.BeginReceive(buffer, 0, size,
                SocketFlags.None,
                new AsyncCallback(_EndReceiveCallback),
                buffer);

        private void _EndConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                // close async pending
                _socket.EndConnect(asyncResult);

                // check connection
                if (_socket.Connected)
                {
                    _SocketReceive(_stateBuffer, _StateBufferSize);
                }
            }
            catch (SocketException exception)
            {
                _log.Warning($"{nameof(_EndConnectCallback)} disconnected by {exception.SocketErrorCode}");
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            finally
            {
                if (! _socket.Connected)
                {
                    _SocketDestroy();
                }

                _OnConnectionChanged(_socket.Connected);
            }
        }

        private void _EndReceiveCallback(IAsyncResult asyncResult)
        {
            var receivedBytes = (byte[]) asyncResult.AsyncState;
            var receivedBytesCount = 0;

            try
            {
                // end async read operation and get received bytes count
                receivedBytesCount = _socket.EndReceive(asyncResult);

                // check count of received bytes
                // because if we get a byte equal to 0, then this will mean the connection is broken
                // in this case, we need to stop further execution
                if (receivedBytesCount == 0)
                {
                    throw ZExceptionHelper.EmptyReceive;
                }

                // get payload data
                var payloadData = receivedBytes
                    .Take(receivedBytesCount)
                    .ToArray();

                // append response bytes to buffer
                _buffer.Append(payloadData);

                // check if we get full message or not
                // TODO: Also, probably we have bug here. What if massage length is equal to receive buffer size
                if (receivedBytesCount < receivedBytes.Length)
                {
                    // copy received message bytes to a new array
                    var messageBytes = _buffer.InternalBuffer
                        .ToArray();

                    // raise event
                    _OnDataReceived(messageBytes);
                }

                // reading should be constant and cyclical
                _SocketReceive(_stateBuffer, _StateBufferSize);
            }
            catch (SocketException exception)
            {
                _log.Warning($"{nameof(_EndReceiveCallback)} disconnected by {exception.SocketErrorCode}");
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            finally
            {
                if (! _socket.Connected || receivedBytesCount == 0)
                {
                    _OnConnectionChanged(false);
                    _SocketDestroy();
                }
            }
        }

        private void _EndSendCallback(IAsyncResult asyncResult)
        {
            var sendMessageBuffer = (byte[]) asyncResult.AsyncState;
            var sentBytesCount = 0;

            try
            {
                // end async send operation and get sent bytes count
                sentBytesCount = _socket.EndSend(asyncResult);

                // check count of sent bytes
                // because if we get a byte equal to 0, then this will mean the connection is broken
                // in this case, we need to stop further execution
                // check if we send full message or not
                if (sentBytesCount == 0)
                {
                    throw ZExceptionHelper.EmptySend;
                }
                else if (sentBytesCount != sendMessageBuffer.Length)
                {
                    throw ZExceptionHelper.SendMessageNotFull;
                }
            }
            catch (SocketException exception)
            {
                _log.Warning($"{nameof(_EndSendCallback)} disconnected by {exception.SocketErrorCode}");
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            finally
            {
                if (! _socket.Connected || sentBytesCount == 0)
                {
                    _OnConnectionChanged(false);
                    _SocketDestroy();
                }
            }
        }

        #endregion

        #region IZClient

        public event EventHandler<ZClientConnectionChangedArgs> ConnectionStatusChanged;
        public event EventHandler<ZClientDataReceivedArgs> DataReceived;

        public void Connect()
        {
            _SocketCreate();
            _SocketConnect();
        }

        public void Disconnect()
        {
            _SocketDestroy();
        }

        public void SendRequest(byte[] requestData)
        {
            _SocketSend(requestData, requestData.Length);
        }

        #endregion

        #region Private helpers

        private void _OnConnectionChanged(bool connectionState)
            => ConnectionStatusChanged?.Invoke(this, new ZClientConnectionChangedArgs(connectionState));

        private void _OnDataReceived(byte[] messageBytes)
            => DataReceived?.Invoke(this, new ZClientDataReceivedArgs(messageBytes));

        #endregion
    }
}