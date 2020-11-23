using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZServersListService : IZServersListService
    {
        private readonly ZLogger _logger;
        private readonly IZServersListParser _parser;
        private readonly IZChangesMapper _changesMapper;
        private readonly ZTunnel _packetsStream;
        private readonly ZCollectionWrapper _collectionWrapper;

        private int _serversCount;
        private int? _initialCount;
        private bool __disposed;

        public ZServersListService(IZClientService clientService, uint myId, ZGame game)
        {
            _logger = ZLogger.Instance;
            _parser = ZParsersFactory.BuildServersListInfoParser(myId, game, _logger);
            _parser.ResultCallback = _ActionHandler;

            _collectionWrapper = new ZCollectionWrapper(new ObservableCollection<ZServerBase>());
            _changesMapper = new ZChangesMapper();
            _packetsStream = clientService.CreateTunnel(
                clientService.RequestFactory.BuildServerListSubscribeRequest(game),
                clientService.RequestFactory.BuildServerListUnsubscribeRequest(game));
            _packetsStream.PacketsReceived += _packetsReceivedHandler;

            _serversCount = 0;
        }

        public ObservableCollection<ZServerBase> ServersCollection => _collectionWrapper.Collection;

        public event EventHandler InitialSizeReached;

        public bool CanUse => ! __disposed;

        public void StartReceiving()
        {
            if (__disposed) throw new InvalidOperationException("Object disposed.");
            if (_packetsStream.IsOpen) return;

            _packetsStream.Open();
        }

        public void StopReceiving()
        {
            if (__disposed) throw new InvalidOperationException("Object disposed.");

            Dispose();
        }

        public void Dispose()
        {
            if (__disposed) return;

            _packetsStream.Close();
            _parser.Close();
            _flushServerCollection();

            __disposed = true;
        }

        #region Private methods

        private void _packetsReceivedHandler(object sender, ZPacket[] e)
        {
            if (e == null)
            {
                _flushServerCollection();
                _logger.Error($"Servers collection was flushed.");
            }
            else
            {
                if (_initialCount == null)
                {
                    _initialCount = _getCountOfPacketsByType(e, ZServerParserAction.Add);
                    if (_initialCount != e.Length && _initialCount != (e.Length / 2))
                    {
                        var PL = _getCountOfPacketsByType(e, ZServerParserAction.PlayersList);
                        _logger.Warning($"Input servers packets not match with player_list packets. COUNT_OF_A {_initialCount} COUNT_OF_PL {PL}");
                    }
                }

                _parser.ParseAsync(e);
            }
        }

        private void _OnInitialSizeReached()
        {
            InitialSizeReached?.Invoke(this, EventArgs.Empty);
        }

        private void _flushServerCollection()
        {
            _serversCount = 0;
            _collectionWrapper.Flush();
        }

        private int _getCountOfPacketsByType(IEnumerable<ZPacket> packets, ZServerParserAction actionType) =>
            packets
                .Where(p => p.Length > 0)
                .Count(p => p.Content.First() == (byte) actionType);

        private void _ActionHandler(ZServerBase model, ZServerParserAction action)
        {
            switch (action)
                {
                    case ZServerParserAction.Add:
                        _AddActionHandler(model);

                        break;
                    case ZServerParserAction.PlayersList:
                        _PlayerListActionHandler(model);

                        break;
                    case ZServerParserAction.Remove:
                        _RemoveActionHandler(model);

                        break;
                    case ZServerParserAction.Ignore:
                    default:
                        break;
                }
        }

        private void _AddActionHandler(ZServerBase model)
        {
            var target = _collectionWrapper.Collection.FirstOrDefault(s => s.Id == model.Id);
            if (target != null)
            {
                ZSynchronizationWrapper.Send<object>(o =>
                {
                    var source = model;

                    _changesMapper.MapCollection(source.MapRotation.Rotation, target.MapRotation.Rotation);

                    _changesMapper.MapChanges(source.MapRotation, target.MapRotation);
                    _changesMapper.MapChanges(source, target);

                    target.UpdateAll();
                });
            }
            else
            {
                _collectionWrapper.Add(model);
                _serversCount++;
            }

            if (_serversCount >= _initialCount)
            {
                _OnInitialSizeReached();
            }
        }

        private void _PlayerListActionHandler(ZServerBase model)
        {
            var target = _collectionWrapper.Collection.FirstOrDefault(s => s.Id == model.Id);
            if (target != null)
            {
                ZSynchronizationWrapper.Send<object>(o => _changesMapper.MapCollection(model.Players, target.Players));

                target.CurrentPlayersNumber = model.CurrentPlayersNumber;
                target.UpdateByName("CurrentPlayersNumber");
            }
            else
            {
                _logger.Warning($"Parsed players for server id: {model.Id} not found this server.");
            }
        }

        private void _RemoveActionHandler(ZServerBase model)
        {
            var item = _collectionWrapper.Collection.FirstOrDefault(s => s.Id == model.Id);
            if (item != null)
            {
                _serversCount--;
                _collectionWrapper.Remove(item);
            }
            else
            {
                _logger.Warning($"Server remove request for id: {model.Id} not found this server.");
            }
        }

        #endregion
    }
}