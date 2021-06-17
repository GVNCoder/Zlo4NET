﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

using Zlo4NET.Api.Shared;
using Zlo4NET.Data;
using Zlo4NET.Extensions;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.ZClientAPI
{
    internal class ZClientImpl : IZClient
    {
        #region Constants

        // size of static buffer 8 KBytes
        private const int BUFFER_SIZE = 8192;
        // size of packet header
        private const int HEADER_SIZE = sizeof(byte) + sizeof(uint);

        #endregion

        private readonly IPEndPoint _endPoint;
        private readonly ZBuffer _buffer;       // used for accumulate message data if it size is more then BUFFER_SIZE constant
        private readonly ZLoggerImpl _logger;
        private readonly byte[] _readBuffer;    // used for each read operation

        private Socket _socket;
        private bool _socketCloseInitiated = false;

        #region Ctors

        public ZClientImpl()
        {
            _endPoint   = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 48486);
            _buffer     = new ZBuffer();
            _logger     = ZLoggerImpl.Instance;
            _readBuffer = new byte[BUFFER_SIZE];
        }

        #endregion

        #region Socket operations

        private Socket _createSocket()
            => new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        private IAsyncResult _socketBeginConnect()
            => _socket.BeginConnect(_endPoint,
                new AsyncCallback(_EndConnectCallback),
                null);
        private IAsyncResult _socketBeginReceive()
            => _socket.BeginReceive(_readBuffer, 0, BUFFER_SIZE, SocketFlags.None,
                new AsyncCallback(_EndReceiveCallback),
                null);
        private IAsyncResult _socketBeginSend(byte[] buffer)
            => _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                new AsyncCallback(_EndSendCallback),
                buffer);
        private void _closeSocket()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                // ignore, cuz we fully destroying a socket
            }
            finally
            {
                _socketCloseInitiated = false;

                _buffer.Clear();
                _socket.Close();
            }
        }

        #endregion

        #region Socket callbacks

        private void _EndConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                _socket.EndConnect(asyncResult);

                if (_socket.Connected)
                {
                    _socketBeginReceive();
                }
            }
            catch (SocketException ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket connection error {ex.SocketErrorCode} {ex.ErrorCode}");
            }
            catch (Exception ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket unexpected error {ex.Message}");
            }
            finally
            {
                // can`t connect to endPoint
                if (! _socket.Connected)
                {
                    _closeSocket();
                }

                _OnConnectionStateChanged(_socket.Connected);
            }
        }
        private void _EndReceiveCallback(IAsyncResult asyncResult)
        {
            var bytesReceived = 0;

            try
            {
                bytesReceived = _socket.EndReceive(asyncResult);

                if (bytesReceived == 0)
                {
                    throw new Exception("The sender has closed their connection");
                }

                var receivedBytes = _readBuffer.Take(bytesReceived);

                // append received bytes to internal buffer
                _buffer.Append(receivedBytes);

                // process internal buffer
                _OnBytesReceived();

                // I need more bytes! if we still connected
                if (_socket.Connected)
                {
                    _socketBeginReceive();
                }
            }
            catch (SocketException ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket receive error {ex.SocketErrorCode} {ex.ErrorCode}");
            }
            catch (ObjectDisposedException)
            {
                // ignore
                // we do not need to log it, see the next catch
            }
            catch (Exception ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket unexpected error {ex.Message}");
            }
            finally
            {
                // the sender has closed their connection
                if (!_socket.Connected || bytesReceived == 0)
                {
                    _closeSocket();
                    _OnConnectionStateChanged(false);
                }
            }
        }
        private void _EndSendCallback(IAsyncResult asyncResult)
        {
            var requestBytes = (byte[]) asyncResult.AsyncState;
            var bytesSent = 0;

            try
            {
                bytesSent = _socket.EndSend(asyncResult);

                if (bytesSent != requestBytes.Length)
                {
                    throw new Exception("The sender has closed their connection");
                }
            }
            catch (SocketException ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket send error {ex.ErrorCode} {ex.SocketErrorCode}");
            }
            catch (ObjectDisposedException)
            {
                // ignore it
                // we do not need to log it
            }
            catch (Exception ex)
            {
                _logSocketMessage(ZLoggingLevel.Error, $"Socket unexpected error {ex.Message}");
            }
            finally
            {
                if (! _socket.Connected || bytesSent == 0)
                {
                    _closeSocket();
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
        private void _logSocketMessage(ZLoggingLevel level, string message)
        {
            if (! _socketCloseInitiated)
            {
                _logger.Log(level, message);
            }
        }

        #endregion

        #region IZClient

        public event Action<bool> ConnectionStateChanged;

        public event ZPacketsReceivedHandler PacketsReceived;

        public void Run()
        {
            _socket = _createSocket();
            _socketBeginConnect();
        }
        public void Close()
        {
            _socketCloseInitiated = true;
            _closeSocket();
        }
        public void SendRequest(byte[] requestBytes)
        {
            _socketBeginSend(requestBytes);
        }

        #endregion
    }
}