using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal delegate void _PipeHandler(_GameState state);

    internal class _GamePipe
    {
        private const int _messageHeaderSize = 4;

        private readonly ZLogger _logger;
        private readonly Thread _readThread;
        private readonly NamedPipeClientStream _pipe;
        private readonly ZBuffer _buffer;

        public _GamePipe(ZLogger logger, string pipeName)
        {
            _logger = logger;
            _buffer = new ZBuffer();
            _pipe = new NamedPipeClientStream(".", pipeName, PipeDirection.In);
            _readThread = new Thread(_Reader)
            {
                IsBackground = true
            };
        }

        public event _PipeHandler PipeEvent;

        public void Begin() => _readThread.Start();

        #region Private helpers

        private void _OnPipeEvent(_GameState state) => PipeEvent?.Invoke(state);

        private void _Reader()
        {
            _pipe.Connect();

            // read data from pipe
            while (_pipe.IsConnected && _pipe.CanRead)
            {
                var buffer = new byte[4096];
                var bytesRead = _pipe.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    // create message data block
                    var messageData = buffer.Take(bytesRead);

                    // append message data
                    _buffer.Append(messageData);

                    // parse message
                    _parseData();
                }

                Thread.Sleep(50); // wait to data availability
            }
        }

        private void _parseData()
        {
            var eventString = string.Empty;
            var stateString = string.Empty;

            try
            {
                using (var memoryStream = new MemoryStream(_buffer, false))
                using (var br = new BinaryReader(memoryStream, Encoding.ASCII))
                {
                    br.ReadBytes(2); // skip two unknown bytes Approved by ZLOFENIX

                    // read message length
                    var messageLength = br.ReadUInt16();

                    // check, we got the full message or not
                    if (_buffer.Size < messageLength - _messageHeaderSize)
                    {
                        return;
                    }

                    // read and parse message data
                    var eventStringLength = br.ReadByte();

                    eventString = br.ReadCountedString(eventStringLength)
                        .Trim();

                    var stateStringLength = br.ReadUInt16();

                    stateString = br.ReadCountedString(stateStringLength)
                        .Trim();

                    //    .Replace('\0'.ToString(), string.Empty);

                    // normalize state string
                    stateString = Uri.UnescapeDataString(stateString);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(_parseData)} message {ex.Message}");
            }

            _buffer.Clear();

            // parse string to game state
            var state = _GameStateParser.ParseStates(eventString, stateString);

            // raise event
            _OnPipeEvent(state);
        }

        #endregion
    }
}