using System;
using System.Threading.Tasks;

using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal class ZServersList : IZServersList
    {
        private readonly ZLogger _logger;
        private readonly IZServersListParser _parser;
        private readonly ZGame _gameContext;

        private bool _isDisposed;

        public ZServersList(ZGame game, IZConnection connection)
        {
            _gameContext = game;
            _logger = ZLogger.Instance;

            var authorizedUser = connection.GetCurrentUserInfo();

            _parser = ZParsersFactory.CreateServersListInfoParser(authorizedUser.UserId, game, _logger);
            _parser.OnParsingResultCallback = _OnParsingResultCallback;
        }

        #region IZServerList

        public async Task StartReceivingAsync()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("Object disposed");
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
                throw new InvalidOperationException("Object disposed");
            }

            // set disposed internal state
            _isDisposed = true;

            // free resources
            _parser.OnParsingResultCallback = null;
            _parser.Close();

            // close stream
            var closeStreamRequest = ZRequestFactory.CreateServerListCloseStreamRequest(_gameContext);
            await ZRouter.CloseStreamAsync(closeStreamRequest);
        }

        public ZServerListActionCallback ServerListActionCallback { get; set; }

        #endregion

        #region Private helpers

        private void _OnStreamRejectedCallback()
        {
            _logger.Info("The server list stream was rejected");
        }

        private void _OnStreamPacketsCallback(ZPacket[] packets)
        {
            _parser.Parse(packets);
        }

        private void _OnParsingResultCallback(ZServerDto server, ZServerListAction serverListAction)
        {
            ServerListActionCallback?.Invoke(serverListAction, server.Id, server);
        }

        #endregion
    }
}