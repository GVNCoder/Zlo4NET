using System;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.Threading;

using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.ZClientAPI;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.Core.Data
{
    internal class ZGamePipe
    {
        #region Constants

        private const int MESSAGE_HEADER_SIZE = 4;
        private const int MESSAGE_BUFFER_SIZE = 4096;
        #endregion

        private readonly ZLoggerImpl _logger;
        private readonly Thread _readThread;
        private readonly NamedPipeClientStream _pipe;
        private readonly ZBuffer _buffer;

        #region Ctor

        public ZGamePipe(string pipeName)
        {
            _logger = ZLoggerImpl.Instance;
            _buffer = new ZBuffer();
            _pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.In);
            _readThread = new Thread(_GamePipeReader)
            {
                IsBackground = true
            };
        }

        #endregion

        #region Interface

        public event Action<ZGameStateModel> PipeEvent;

        public void Begin() => _readThread.Start();

        #endregion

        #region Private helpers

        private void _OnPipeEvent(ZGameStateModel state) => PipeEvent?.Invoke(state);

        private void _GamePipeReader()
        {
            _pipe.Connect();

            // read data from pipe
            while (_pipe.IsConnected && _pipe.CanRead)
            {
                var buffer = new byte[MESSAGE_BUFFER_SIZE];
                var bytesRead = _pipe.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    // create message data block
                    var messageData = buffer.Take(bytesRead);

                    // append message data
                    _buffer.Append(messageData);

                    // parse message
                    _ParseMessage();
                }

                // wait to data availability
                Thread.Sleep(50);
            }
        }

        private void _ParseMessage()
        {
            var eventString = string.Empty;
            var stateString = string.Empty;

            try
            {
                using (var memoryStream = new MemoryStream(_buffer, false))
                using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
                {
                    binaryReader.ReadBytes(2); // skip two unknown bytes Approved by ZLOFENIX

                    // read message length
                    var messageLength = binaryReader.ReadUInt16();

                    // check, we got the full message or not
                    if (_buffer.Size < messageLength - MESSAGE_HEADER_SIZE)
                    {
                        return;
                    }

                    // read and parse message data
                    var eventStringLength = binaryReader.ReadByte();
                    eventString = binaryReader.ReadCountedString(eventStringLength)
                        .Trim();

                    var stateStringLength = binaryReader.ReadUInt16();
                    stateString = binaryReader.ReadCountedString(stateStringLength)
                        .Trim();

                    //    .Replace('\0'.ToString(), string.Empty);

                    // normalize state string
                    stateString = Uri.UnescapeDataString(stateString);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(_ParseMessage)} message {ex.Message}");
            }

            _buffer.Clear();

            // parse string to game state
            var state = ZGameStateParser.ParseStates(eventString, stateString);

            // raise event
            _OnPipeEvent(state);
        }

        #endregion
    }
}