﻿using System;
using System.Collections.ObjectModel;
using System.Linq;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal class ZServersListService : IZServersListService
    {
        private readonly ZLogger _logger;
        private readonly IZServersListParser _parser;
        private readonly IZChangesMapper _changesMapper;
        private readonly ZCollectionWrapper _collectionWrapper;

        private int _initialCount;
        private int _initialCountPlayerList;
        private bool __disposed;
        private ZGame _gameContext;

        public ZServersListService(uint myId, ZGame game)
        {
            _logger = ZLogger.Instance;
            _parser = ZParsersFactory.CreateServersListInfoParser(myId, game, _logger);
            _parser.ResultCallback = _ActionHandler;
            _gameContext = game;

            _collectionWrapper = new ZCollectionWrapper(new ObservableCollection<ZServerBase>());
            _changesMapper = new ZChangesMapper();
        }

        public ObservableCollection<ZServerBase> ServersCollection => _collectionWrapper.Collection;

        public event EventHandler InitialSizeReached;

        public bool CanUse => ! __disposed;

        public void StartReceiving()
        {
            if (__disposed) throw new InvalidOperationException("Object disposed.");

            var openStreamRequest = ZRequestFactory.CreateServerListOpenStreamRequest(_gameContext);
            var response = ZRouter.OpenStreamAsync(openStreamRequest, _packetsReceivedHandler).Result;
        }

        public void StopReceiving()
        {
            if (__disposed) throw new InvalidOperationException("Object disposed.");

            Dispose();
        }

        public void Dispose()
        {
            if (__disposed) return;

            var request = ZRequestFactory.CreateServerListCloseStreamRequest(_gameContext);
            var response = ZRouter.CloseStreamAsync(request).Result;

            _parser.Close();

            __disposed = true;
        }

        #region Private methods

        private void _packetsReceivedHandler(ZPacket[] e)
        {
            _parser.ParseAsync(e);
        }

        private void _OnInitialSizeReached()
        {
            InitialSizeReached?.Invoke(this, EventArgs.Empty);
        }

        private void _ActionHandler(ZServerBase model, ZServerParserAction action)
        {
            switch (action)
                {
                    case ZServerParserAction.Add:
                        _AddActionHandler(model);
                        _initialCount++;

                        break;
                    case ZServerParserAction.PlayersList:
                        _PlayerListActionHandler(model);

                        _initialCountPlayerList++;

                    break;
                    case ZServerParserAction.Remove:
                        _RemoveActionHandler(model);

                        break;
                    case ZServerParserAction.Ignore:
                    default:
                        break;
                }

            if (_initialCountPlayerList >= _initialCount)
            {
                _OnInitialSizeReached();
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