using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Zlo4NET.Api.Shared;
using Zlo4NET.Data;
using Zlo4NET.Extensions;

// ReSharper disable InvertIf
// ReSharper disable InconsistentNaming

namespace Zlo4NET.ZClientAPI
{
    internal class ZClientImpl : IZClient
    {
        #region Internal types

        private class SocketAsyncState
        {
            public SocketAsyncState(Socket workSocket)
            {
                WorkSocket = workSocket;
            }

            public SocketAsyncState(Socket workSocket, byte[] buffer) : this(workSocket)
            {
                Buffer = buffer;
            }

            public Socket WorkSocket { get; }
            public byte[] Buffer { get; }
            public long BufferSize => Buffer.Length;
        }

        #endregion

        #region Constants

        // size of static buffer 8 KBytes
        private const int BUFFER_SIZE = 8192;
        // size of packet header
        private const int HEADER_SIZE = sizeof(byte) + sizeof(uint);

        #endregion

        private readonly LingerOption _lingerOption;
        private readonly IPEndPoint _endPoint;
        private readonly ZBuffer _buffer;       // used for accumulate message data if it size is more then BUFFER_SIZE constant
        private readonly ZLoggerImpl _logger;

        private Socket _currentSocket;
        private bool _socketCloseInitiated = false;

        #region Ctors
         
        public ZClientImpl()
        {
            _lingerOption   = new LingerOption(false, 0);
            _endPoint       = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 48486);
            _buffer         = new ZBuffer();
            _logger         = ZLoggerImpl.Instance;
        }

        #endregion

        #region Socket operations

        private Socket _createSocket()
            => new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                { LingerState = _lingerOption };

        private IAsyncResult _socketBeginConnect(Socket workSocket)
            => _currentSocket.BeginConnect(_endPoint,
                new AsyncCallback(_EndConnectCallback),
                new SocketAsyncState(workSocket));

        private IAsyncResult _socketBeginReceive(Socket workSocket, byte[] buffer)
            => _currentSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                new AsyncCallback(_EndReceiveCallback),
                new SocketAsyncState(workSocket, buffer));

        private IAsyncResult _socketBeginSend(Socket workSocket, byte[] buffer)
            => _currentSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                new AsyncCallback(_EndSendCallback),
                new SocketAsyncState(workSocket, buffer));

        private void _closeSocket(Socket workSocket)
        {
            try
            {
                workSocket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                // ignore, cuz we fully destroying a socket
            }
            finally
            {
                workSocket.Close();

                _buffer.Clear();
                _socketCloseInitiated = false;
            }
        }

        #endregion

        #region Socket callbacks

        private void _EndConnectCallback(IAsyncResult asyncResult)
        {
            var socketState = (SocketAsyncState) asyncResult.AsyncState;
            var socket = socketState.WorkSocket;

            try
            {
                // complete socket connection
                socket.EndConnect(asyncResult);

                // wait to data receive if connected
                if (socket.Connected)
                {
                    // create read buffer
                    var buffer = new byte[BUFFER_SIZE];

                    // lets go
                    _socketBeginReceive(socket, buffer);
                }
            }
            catch (SocketException ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket connection error {ex.SocketErrorCode} {ex.ErrorCode}");
                _closeSocket(socket);
            }
            catch (ObjectDisposedException)
            {
                // ignore
                // we do not need to log it, see the next catch
            }
            catch (Exception ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket unexpected error {ex.Message}");
                _closeSocket(socket);
            }
            finally
            {
                // it is the same socket
                if (_currentSocket == socket)
                {
                    _OnConnectionStateChanged(socket.Connected);
                }
            }
        }
        private void _EndReceiveCallback(IAsyncResult asyncResult)
        {
            var socketState = (SocketAsyncState) asyncResult.AsyncState;
            var socket = socketState.WorkSocket;

            try
            {
                // complete socket receive
                var bytesReceived = socket.EndReceive(asyncResult);
                if (bytesReceived == 0)
                {
                    throw new Exception("The sender has closed their connection");
                }

                var readBuffer = socketState.Buffer;
                var receivedBytes = readBuffer.Take(bytesReceived);

                // append received bytes to internal buffer
                _buffer.Append(receivedBytes);

                // process internal buffer
                _OnBytesReceived();

                // I need more bytes! if we still connected ofcourse
                if (socket.Connected)
                {
                    _socketBeginReceive(socket, readBuffer);
                }
            }
            catch (SocketException ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket receive error {ex.SocketErrorCode} {ex.ErrorCode}");
                _closeSocket(socket);
            }
            catch (ObjectDisposedException)
            {
                // ignore
                // we do not need to log it, see the next catch
            }
            catch (Exception ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket unexpected error {ex.Message}");
                _closeSocket(socket);
            }
            finally
            {
                // it is the same socket
                if (_currentSocket == socket && socket.Connected == false)
                {
                    _OnConnectionStateChanged(false);
                }
            }
        }
        private void _EndSendCallback(IAsyncResult asyncResult)
        {
            var socketState = (SocketAsyncState) asyncResult.AsyncState;
            var socket = socketState.WorkSocket;

            try
            {
                // complete socket send
                var bytesSent = socket.EndSend(asyncResult);
                if (bytesSent != socketState.BufferSize)
                {
                    throw new Exception("The sender has closed their connection");
                }
            }
            catch (SocketException ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket send error {ex.ErrorCode} {ex.SocketErrorCode}");
                _closeSocket(socket);
            }
            catch (ObjectDisposedException)
            {
                // ignore it
                // we do not need to log it
            }
            catch (Exception ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket unexpected error {ex.Message}");
                _closeSocket(socket);
            }
            finally
            {
                // it is the same socket
                if (_currentSocket == socket && socket.Connected == false)
                {
                    _OnConnectionStateChanged(false);
                }
            }
        }

        #endregion

        #region Private methods

        private void _OnBytesReceived()
        {
            // parse received packets
            var receivedPackets = new List<ZPacket>(1);

            using (var memoryStream = new MemoryStream(_buffer, false))
            using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
            {
                // read until can
                while (binaryReader.PeekChar() != -1 && binaryReader.BytesRemaining() >= HEADER_SIZE)
                {
                    var id = (ZCommand) binaryReader.ReadByte();
                    var length = (int) binaryReader.ReadZUInt32();

                    // check, are we get full packet ?
                    if (length <= binaryReader.BytesRemaining())
                    {
                        var payload = new byte[length];

                        binaryReader.Read(payload, 0, length);
                        receivedPackets.Add(new ZPacket
                        {
                            Id = id,
                            Payload = payload
                        });

                        // we need to remove used bytes from internal buffer
                        _buffer.RemoveBytes(HEADER_SIZE + length);
                    }
                    else
                    {
                        // go to get more bytes
                        break;
                    }
                }
            }

            if (receivedPackets.Count != 0)
            {
                _OnPacketsReceived(receivedPackets);
            }
        }

        private void _OnConnectionStateChanged(bool connectionState) => ConnectionStateChanged?.Invoke(connectionState);
        private void _OnPacketsReceived(IEnumerable<ZPacket> packets) => PacketsReceived?.Invoke(packets);

        // solves the problem that when we disconnect the socket of our own free will, then it is not necessary to log related errors
        private void _logSocketMessage(ZLogLevel level, string message, [CallerMemberName] string callerName = null)
        {
            if (_socketCloseInitiated == false)
            {
                _logger.Log(level, $"{callerName} {message}");
            }
        }

        #endregion

        #region IZClient

        public event Action<bool> ConnectionStateChanged;

        public event ZPacketsReceivedHandler PacketsReceived;

        public void Run()
        {
            // this helps complete all asynchronous pendings for the current socket
            if (_currentSocket != null)
            {
                Close();
            }

            _currentSocket = _createSocket();
            _socketBeginConnect(_currentSocket);
        }
        public void Close()
        {
            _socketCloseInitiated = true;
            _closeSocket(_currentSocket);
        }
        public void SendRequest(byte[] requestBytes)
        {
            _socketBeginSend(_currentSocket, requestBytes);
        }

        #endregion
    }
}