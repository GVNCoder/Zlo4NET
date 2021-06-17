using System;
using System.Threading.Tasks;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Helpers;
using Zlo4NET.Services;
using Zlo4NET.ZClientAPI;
using Zlo4NET.Api.Shared;
using Zlo4NET.Extensions;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter

namespace Zlo4NET.Data
{
    internal class ZServersListImpl : IZServersList
    {
        private readonly IZServerListParser _parser;
        private readonly ZGame _targetGame;
        private readonly ZLoggerImpl _logger;

        private bool _isDisposed;

        #region Ctor

        public ZServersListImpl(ZGame targetGame, IZConnection connection)
        {
            _targetGame = targetGame;
            _logger = ZLoggerImpl.Instance;

            // create and configure server list parser
            var currentUser = connection.GetCurrentUserInfo();

            _parser = ZParsersFactory.CreateServersListInfoParser(currentUser.UserId, targetGame);
            _parser.ResultCallback = _OnParsingResultCallback;
        }

        #endregion

        #region IZServerList interface

        public bool IsInstanceAvailable => _isDisposed;
        
        public async Task StartReceivingAsync()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("This server list instance is disposed. Please, create a new one.");
            }

            ZThrowHelper.ThrowIfNotConnected();

            // open stream
            var request = ZRequestFactory.CreateServerListOpenStreamRequest(_targetGame);
            await ZRouter.OpenStreamAsync(request, _OnStreamPacketsCallback, _OnStreamRejectedCallback);
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
            var closeStreamRequest = ZRequestFactory.CreateServerListCloseStreamRequest(_targetGame);
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
            // I do not wrap it in try ... catch, because the developer must know what is happening in his code and what needs to be fixed
            ServerListActionCallback?.Invoke(serverListAction, server.Id, server);
        }

        #endregion
    }
}