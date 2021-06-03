using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using System;
using System.ComponentModel;
using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.ZClientAPI;

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZServersListParser : IZServersListParser
    {
        private readonly object _lock = new object();
        private readonly object _threadInstanceLock = new object();

        private readonly uint _authorizedUserId;
        private readonly ZGame _gameContext;
        private readonly Queue<ZPacket[]> _packetsQueue;
        private readonly ZLogger _logger;

        private readonly IDictionary<ZGame, Action<BinaryReader, ZServerBase>> _serverModelParsingMethods = new Dictionary<ZGame, Action<BinaryReader, ZServerBase>>
        {
            { ZGame.BF3, ZGameSpecificServerParserMethodsProvider.ParseBF3ServerModel },
            { ZGame.BF4, ZGameSpecificServerParserMethodsProvider.ParseBF4ServerModel },
            { ZGame.BFHL, ZGameSpecificServerParserMethodsProvider.ParseBFHLServerModel },
        };

        private Thread _thread;
        private bool _disposed;

        public ZServersListParser(uint authorizedUserId, ZGame gameContext, ZLogger logger)
        {
            _packetsQueue = new Queue<ZPacket[]>(5);

            _authorizedUserId = authorizedUserId;
            _gameContext = gameContext;
            _logger = logger;

            ZGameSpecificServerParserMethodsProvider.Configure(gameContext);
        }

        #region Private helpers

        // fabric method
        private static ZServerBase _CreateServerModelByGame(ZGame targetGame)
        {
            ZServerBase model;

            switch (targetGame)
            {
                case ZGame.BF3: model = new ZServerBF3();
                    break;
                case ZGame.BF4: model = new ZServerBF4();
                    break;
                case ZGame.BFHL: model = new ZServerBFHL();
                    break;

                case ZGame.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetGame), targetGame, null);
            }

            return model;
        }

        private void _ParsingLoop()
        {
            while (true)
            {
                ZPacket[] packets;

                lock (_lock)
                {
                    if (! _packetsQueue.Any())
                    {
                        continue;
                    }

                    packets = _packetsQueue.Dequeue();
                }

                foreach (var packet in packets)
                {
                    using (var memoryStream = new MemoryStream(packet.Payload, false))
                    using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
                    {
                        var action = (ZServerListAction) binaryReader.ReadByte();
                        var targetGame = (ZGame) binaryReader.ReadByte();
                        var serverModel = _CreateServerModelByGame(targetGame);

                        if (_gameContext != targetGame)
                        {
                            _logger.Warning("Received server list packet target game doesn't match with requested target game");

                            continue;
                        }

                        switch (action)
                        {
                            case ZServerListAction.ServerAddOrUpdate:

                                var serverModelParser = _serverModelParsingMethods[targetGame];

                                // parse into model
                                serverModelParser.Invoke(binaryReader, serverModel);
                                break;
                            case ZServerListAction.ServerPlayersList:
                                _ParsePlayersList(serverModel, binaryReader);
                                break;
                            case ZServerListAction.ServerRemove:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        // handle parsed data
                        OnParsingResultCallback?.Invoke(serverModel, action);
                    }
                }

                Thread.Sleep(100);
            }
        }
        private void _ParsePlayersList(ZServerBase model, BinaryReader binaryReader)
        {
            var playersList = new List<ZPlayer>();
            var countOfPlayers = binaryReader.ReadByte();

            for (var i = 0; i < countOfPlayers; i++)
            {
                var playerSlot = binaryReader.ReadByte();
                var playerId = binaryReader.ReadZUInt32();
                var playerName = binaryReader.ReadZString();

                var player = new ZPlayer
                {
                    Slot = playerSlot,
                    Id = playerId,
                    Name = playerName,
                    Role = ZPlayerRole.InServerPlayer
                };

                playersList.Add(player);
            }

            // set authorized player
            var authorizedPlayer = playersList.FirstOrDefault(p => p.Id == _authorizedUserId);
            if (authorizedPlayer != null)
            {
                authorizedPlayer.Role = ZPlayerRole.AuthorizedPlayer;
            }

            model.PlayersList = playersList;
            model.CurrentPlayersCount = playersList.Count;
        }

        #endregion

        #region IZServerListParser interface

        public Action<ZServerBase, ZServerListAction> OnParsingResultCallback { get; set; }

        public void Parse(ZPacket[] packets)
        {
            if (_disposed)
            {
                throw new InvalidOperationException("Object disposed");
            }

            // check and create thread if need
            lock (_threadInstanceLock)
            {
                _thread = _thread ?? new Thread(_ParsingLoop) { IsBackground = true, Name = "ServerListParserThread" };

                if (! _thread.IsAlive)
                {
                    _thread.Start();
                }
            }

            lock (_lock)
            {
                _packetsQueue.Enqueue(packets);
            }
        }

        public void Close()
        {
            _disposed = true;
            _thread?.Abort();
        }

        #endregion
    }
}