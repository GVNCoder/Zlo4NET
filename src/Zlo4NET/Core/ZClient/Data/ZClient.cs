using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Zlo4NET.Core.Data;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.ZClient.Data
{
    // ReSharper disable UseObjectOrCollectionInitializer
    #pragma warning disable 168 // used for unused exceptions => catch (e)

    internal class ZClient : IZClient
    {
        #region Internal types

        internal class _ZStateObject
        {
            public const int Size = 8192;
            public byte[] Buffer { get; } = new byte[Size];
            public byte[] RequestBuffer { get; set; }
        }

        #endregion

        private readonly Queue<byte[]> __buffersQueue;
        private bool __isBusy;

        private readonly Queue<ZRequest> _requestsQueue;
        private readonly List<byte> _globalBuffer;
        private readonly IPEndPoint _endPoint;
        private readonly ZLogger _logger;

        private ZRequest _currentRequest;
        private Socket _socket;
        private ZTunnel _tunnel;

        public ZClient()
        {
            __buffersQueue = new Queue<byte[]>(5);
            __isBusy = false;

            _logger = ZLogger.Instance;
            _endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 48486);
            _requestsQueue = new Queue<ZRequest>(10);
            _globalBuffer = new List<byte>(30 * 1024); // 30Kb
        }

        #region Socket operations

        private Socket _createSocket()
            => new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        private IAsyncResult _socketBeginConnect(_ZStateObject state)
            => _socket.BeginConnect(_endPoint,
                new AsyncCallback(_EndConnectCallback),
                state);

        private IAsyncResult _socketBeginReceive(_ZStateObject state)
            => _socket.BeginReceive(state.Buffer, 0, _ZStateObject.Size, SocketFlags.None,
                new AsyncCallback(_EndReceiveCallback),
                state);

        private IAsyncResult _socketBeginSend(byte[] buffer, int offset, int size, _ZStateObject state)
            => _socket.BeginSend(buffer, offset, size, SocketFlags.None,
                new AsyncCallback(_EndSendCallback),
                state);

        #endregion

        #region Socket callbacks

        private void _EndConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                _socket.EndConnect(asyncResult);

                if (_socket.Connected)
                {
                    _socketBeginReceive(_GetNewState()); // dropped connection handle in received callback
                }
            }
            catch (SocketException ex)
            {
                _logger.Warning($"SOCK_CONN SOCK_EX RSN_CODE {ex.SocketErrorCode} {ex.ErrorCode}");
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            finally
            {
                // can`t connect to endPoint handling
                if (! _socket.Connected)
                {
                    _closeSocket();
                }

                _OnConnectionChanged(_socket.Connected);
            }
        }

        private void _EndReceiveCallback(IAsyncResult asyncResult)
        {
            var state = (_ZStateObject) asyncResult.AsyncState;
            var bytesRead = 0;
            try
            {
                bytesRead = _socket.EndReceive(asyncResult);
                if (bytesRead == 0)
                {
                    throw ZExceptionHelper.EmptyReceive;
                }

                var receivedBuffer = state.Buffer;
                _globalBuffer.AddRange(receivedBuffer.Take(bytesRead)); // take received bytes from buffer

                if (bytesRead < receivedBuffer.Length)
                {
                    var responseBytes = _globalBuffer.ToArray();
                    _globalBuffer.Clear();

                    if (! _socket.Connected) return; // return if socket not connected

                    // parse response bytes to packets and move next request
                    _OnBytesReceived(responseBytes);
                }

                _socketBeginReceive(state);
            }
            catch (SocketException ex)
            {
                _logger.Warning($"SOCK_RECEIVE SOCK_EX RSN_CODE {ex.SocketErrorCode} {ex.ErrorCode}");
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                _logger.Warning($"SOCK_RECEIVE UNEXPECTED_EX {ex.Message}");
            }
            finally
            {
                if (! _socket.Connected || bytesRead == 0)
                {
                    _OnConnectionChanged(false);
                    _closeSocket();
                    _closeAllRequests();
                }
            }
        }

        private void _EndSendCallback(IAsyncResult asyncResult)
        {
            var state = (_ZStateObject) asyncResult.AsyncState;
            var bytesSent = 0;
            try
            {
                bytesSent = _socket.EndSend(asyncResult);
                if (bytesSent != state.RequestBuffer.Length)
                {
                    throw ZExceptionHelper.EmptySend;
                }
            }
            catch (SocketException ex)
            {
                _logger.Warning($"SOCK_SEND SOCK_EX {ex.ErrorCode} {ex.SocketErrorCode}");
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                _logger.Warning($"SOCK_SEND UNEXPECTED_EX {ex.Message}");
            }
            finally
            {
                if (! _socket.Connected || bytesSent == 0)
                {
                    _OnConnectionChanged(false);
                    _closeSocket();
                    _closeAllRequests();
                }
            }
        }

        #endregion

        #region Private methods

        private void _OnConnectionChanged(bool connectionState)
            => ConnectionChanged?.Invoke(this, new ZClientConnectionChangedArgs(connectionState));

        private void _closeSocket()
        {
            try
            {
                if (! _socket.Connected) return;
                _socket.Shutdown(SocketShutdown.Both);
            }
            finally
            {
                _socket.Close();
            }
        }

        private static _ZStateObject _GetNewState() => new _ZStateObject();

        private void _sendNextRequest()
        {
            if (_requestsQueue.Count == 0) return;

            _currentRequest = _requestsQueue.Dequeue();
            var requestBytes = _currentRequest.GetBytes();
            if (_currentRequest.Method != ZMethod.Get)
            {
                _currentRequest = null;
            }

            _socketBeginSend(requestBytes, 0, requestBytes.Length, new _ZStateObject { RequestBuffer = requestBytes });
        }

        private void _processTunnelPackets(IEnumerable<ZPacket> packets)
        {
            if (! _canGetTunnelAccess(_tunnel)) return;

            var packetsForTunnel = _getPacketsByCommandId(packets, _tunnel.Id);
            if (packetsForTunnel.Any())
            {
                _tunnel.OnPacketsReceived(packetsForTunnel);
            }
        }

        private void _processRequestPackets(IEnumerable<ZPacket> packets)
        {
            if (_currentRequest == null) return;

            var packetsForResponse = _getPacketsByCommandId(packets, _currentRequest.Id);
            if (packetsForResponse.Any())
            {
                var response = _buildResponseFromRequest(_currentRequest);

                response.Packets = packetsForResponse;
                response.Status = ZResponseStatusCode.Ok;

                _closeRequestByResponse(_currentRequest, response);
                _currentRequest = null;
            }
        }

        private ZPacket[] _getPacketsByCommandId(IEnumerable<ZPacket> packets, ZCommand commandId)
        {
            var value = packets
                .Where(p => p.Id == commandId)
                .ToArray();
            return value;
        }

        private void _OnBytesReceived(byte[] bytes)
        { // parse all packets and detect they is Reply or Event

            __buffersQueue.Enqueue(bytes);
            if (__isBusy) return;

            __isBusy = true;

            do
            {
                var packets = _ParsePackets(__buffersQueue.Dequeue());

                _processTunnelPackets(packets);
                _processRequestPackets(packets);
            }
            while (__buffersQueue.Any());

            __isBusy = false;
            _sendNextRequest();
        }

        private static IList<ZPacket> _ParsePackets(byte[] bytes)
        {
            var packets = new List<ZPacket>(1);

            using (var memStream = new MemoryStream(bytes, false))
            using (var reader = new BinaryReader(memStream, Encoding.ASCII))
            {
                while (reader.PeekChar() != -1)
                { // read when data available in stream
                    var id = (ZCommand)reader.ReadByte();
                    var contentLength = (int)reader.ReadZUInt32();
                    var content = new byte[contentLength];
                    reader.Read(content, 0, contentLength);

                    packets.Add(new ZPacket
                    {
                        Id = id,
                        Length = contentLength,
                        Content = content
                    });
                }
            }

            return packets;
        }

        private void _closeAllRequests()
        {
            ZResponse l_BuildDeclinedResponseFromRequest(ZRequest request)
            {
                var response = _buildResponseFromRequest(_currentRequest);
                response.Status = ZResponseStatusCode.Declined;

                return response;
            }

            if (_currentRequest != null)
            {
                _closeRequestByResponse(_currentRequest, l_BuildDeclinedResponseFromRequest(_currentRequest));
                _currentRequest = null;
            }

            foreach (var zRequest in _requestsQueue)
            {
                _closeRequestByResponse(_currentRequest, l_BuildDeclinedResponseFromRequest(_currentRequest));
            }

            if (_canGetTunnelAccess(_tunnel))
            {
                _tunnel.OnPacketsReceived(null);
            }

            _requestsQueue.Clear();
            _globalBuffer.Clear();
            __buffersQueue.Clear();

            __isBusy = false;
        }

        private ZResponse _buildResponseFromRequest(ZRequest request)
        {
            var response = new ZResponse 
            {
                Id = request.Id,
                Request = request
            };
            return response;
        }

        private void _closeRequestByResponse(ZRequest request, ZResponse response)
        {
            request.SetResponse(response);
        }

        private bool _canGetTunnelAccess(ZTunnel tunnel)
        {
            return tunnel != null && tunnel.IsOpen;
        }

        #endregion

        public void StartClient()
        {
            // create new socket
            _socket = _createSocket();
            // begin connect
            _socketBeginConnect(null);
        }

        public void StopClient()
        {
            _closeSocket();
            _closeAllRequests();
        }

        public void SendRequest(ZRequest request)
        {
            if (_socket.Connected)
            {
                // add request to requests queue
                _requestsQueue.Enqueue(request);
                if (_currentRequest == null) // if we no have any request in execution
                {
                    // move next request
                    _sendNextRequest();
                }
            }
            else // decline incoming request
            {
                var response = _buildResponseFromRequest(request);
                response.Status = ZResponseStatusCode.Declined;

                _closeRequestByResponse(request, response);
            }
        }

        public void RegisterTunnel(ZTunnel tunnel)
        {
            if (! _socket.Connected) return;

            if (_canGetTunnelAccess(_tunnel))
            {
                throw new InvalidOperationException("Cannot register. Current tunnel not closed.");
            }

            _tunnel = tunnel;
        }

        public event EventHandler<ZClientConnectionChangedArgs> ConnectionChanged;
    }
}