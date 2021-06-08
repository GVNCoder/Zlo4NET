using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Shared;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal abstract class ZServerListParserBase : IZServerListParser
    {
        #region Constants

        private const int WAIT_FOR_DATA_TIMEOUT = 100;
        private const int RESERVED_QUEUE_SIZE = 10;

        #endregion

        private readonly object _queueLockObj = new object();

        private readonly CancellationTokenSource _threadCancellationTokenSource;
        private readonly CancellationToken _threadCancellationToken;
        private readonly Queue<ZPacket[]> _packetsQueue;
        private readonly Thread _thread;
        
        private bool _disposed;

        protected readonly ZGameModesConverter _gameModeConverter;
        protected readonly ZMapNameConverter _mapNameConverter;
        protected readonly ZLoggerImpl _logger;
        protected readonly uint _currentUserId;

        #region Ctor

        protected ZServerListParserBase(uint currentUserId, ZGame gameContext)
        {
            _gameModeConverter = new ZGameModesConverter(gameContext);
            _mapNameConverter = new ZMapNameConverter(gameContext);

            _currentUserId = currentUserId;
            _logger = ZLoggerImpl.Instance;
            _disposed = false;

            _threadCancellationTokenSource = new CancellationTokenSource();
            _threadCancellationToken = _threadCancellationTokenSource.Token;

            _packetsQueue = new Queue<ZPacket[]>(RESERVED_QUEUE_SIZE);

            // create a thread but do not run it yet
            _thread = new Thread(new ParameterizedThreadStart(_ThreadLoop)) { IsBackground = true };
        }

        #endregion

        #region Abstract methods

        protected abstract ZServerBase ParseServerModel(BinaryReader binaryReader);
        protected abstract ZServerBase ParseServerPlayers(BinaryReader binaryReader);
        protected abstract ZServerBase ParseRemovedServerModel(BinaryReader binaryReader);

        #endregion

        #region Private helpers

        // server list parsing loop
        private void _ThreadLoop(object state)
        {
            var cancellationToken = (CancellationToken) state;
            
            while (! cancellationToken.IsCancellationRequested)
            {
                // try get packets from queue
                ZPacket[] packets;

                lock (_queueLockObj)
                {
                    if (!_packetsQueue.Any())
                    {
                        continue;
                    }

                    packets = _packetsQueue.Dequeue();
                }

                // parse packets
                foreach (var packet in packets)
                {
                    using (var memoryStream = new MemoryStream(packet.Payload, false))
                    using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
                    {
                        var action = (ZServerListAction) binaryReader.ReadByte();
                        var targetGame = (ZGame) binaryReader.ReadByte();

                        ZServerBase model;

                        switch (action)
                        {
                            case ZServerListAction.ServerAddOrUpdate: model = ParseServerModel(binaryReader);
                                break;
                            case ZServerListAction.ServerPlayersList: model = ParseServerPlayers(binaryReader);
                                break;
                            case ZServerListAction.ServerRemove: model = ParseRemovedServerModel(binaryReader);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(action), action, null);
                        }

                        // try pass results
                        if (! cancellationToken.IsCancellationRequested)
                        {
                            ResultCallback?.Invoke(model, action);
                        }
                    }
                }

                Thread.Sleep(WAIT_FOR_DATA_TIMEOUT);
            }
        }

        #endregion

        #region IZServerListParser interface

        public Action<ZServerBase, ZServerListAction> ResultCallback { get; set; }

        public void Parse(ZPacket[] packets)
        {
            if (_disposed)
            {
                throw new InvalidOperationException("Object disposed");
            }

            if (! _thread.IsAlive)
            {
                _thread.Start(_threadCancellationToken);
            }

            lock (_queueLockObj)
            {
                _packetsQueue.Enqueue(packets);
            }
        }

        public void Close()
        {
            // make an instance unusable
            _disposed = true;

            // request cancellation of thread
            _threadCancellationTokenSource.Cancel();
            _threadCancellationTokenSource.Dispose();
        }

        #endregion
    }
}