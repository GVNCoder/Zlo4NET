﻿using System;
using System.Threading.Tasks;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace Zlo4NET.Core.Data
{
    internal class ZServersList : IZServersList
    {
        private readonly ZLogger _logger;
        private readonly IZServerListParser _parser;
        private readonly ZGame _gameContext;

        private bool _isDisposed;

        public ZServersList(ZGame game, IZConnection connection)
        {
            _gameContext = game;
            _logger = ZLogger.Instance;

            var authorizedUser = connection.GetCurrentUserInfo();

            _parser = ZParsersFactory.CreateServersListInfoParser(authorizedUser.UserId, game);
            _parser.ResultCallback = _OnParsingResultCallback;
        }

        #region IZServerList

        public bool IsInstanceAvailable => _isDisposed;
        
        public async Task StartReceivingAsync()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("This server list instance is disposed. Please, create a new one.");
            }

            ZConnectionHelper.ThrowIfNotConnected();

            // open stream
            var openStreamRequest = ZRequestFactory.CreateServerListOpenStreamRequest(_gameContext);
            await ZRouter.OpenStreamAsync(openStreamRequest, _OnStreamPacketsCallback, _OnStreamRejectedCallback);
        }
        public async Task StopReceivingAsync()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("This server list instance is disposed. Please, create a new one.");
            }

            // set disposed internal state
            _isDisposed = true;

            // free resources
            _parser.ResultCallback = null;
            _parser.Close();

            // close stream
            var closeStreamRequest = ZRequestFactory.CreateServerListCloseStreamRequest(_gameContext);
            await ZRouter.CloseStreamAsync(closeStreamRequest);
        }

        public ZServerListActionCallback ServerListActionCallback { get; set; }

        #endregion

        #region Private helpers

        private async void _OnStreamRejectedCallback()
        {
            _logger.Info("The server list stream was rejected");
            
            // we should free all resources
            await StopReceivingAsync();
        }

        private void _OnStreamPacketsCallback(ZPacket[] packets)
        {
            _parser.Parse(packets);
        }

        private void _OnParsingResultCallback(ZServerBase server, ZServerListAction serverListAction)
        {
            ServerListActionCallback?.Invoke(serverListAction, server.Id, server);
        }

        #endregion
    }
}