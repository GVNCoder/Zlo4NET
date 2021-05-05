using System;
using System.Linq;
using System.Timers;

using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Service;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

//public async Task<bool> CheckMonolithAsync()
//{
//    string stringContent;
//    using (var client = new WebClient())
//    {
//        stringContent = await client.DownloadStringTaskAsync("http://zloemu.net/z-test");
//    }

//    var monolithStatusObject = _phpObjectDeserializer.Deserialize(stringContent) as Hashtable;

//    return true;
//}

namespace Zlo4NET.Core.Data
{
    internal class ZConnection : IZConnection
    {
        #region Constants

        // ReSharper disable once InconsistentNaming
        private const int PING_INTERVAL = 15000;

        #endregion

        private readonly IZUserInfoParser _userInfoParser;
        private readonly Timer _pingTimer;
        private readonly ZLogger _logger;

        private ZUserDto _currentUserInfo;
        private bool _raiseOnConnectionChangedEvent = true;

        public ZConnection()
        {
            _userInfoParser = ZParsersFactory.CreateUserInfoParser();
            _logger         = ZLogger.Instance;
            _pingTimer      = new Timer(PING_INTERVAL) { Enabled = false, AutoReset = true };

            _pingTimer.Elapsed += _OnPingTimerElapsedCallback;

            // track client connection state
            ZRouter.Initialize();
            ZRouter.ConnectionChanged += _OnClientConnectionStateChangedCallback;
        }

        #region Private helpers

        // https://stackoverflow.com/questions/19415646/should-i-avoid-async-void-event-handlers
        private async void _OnClientConnectionStateChangedCallback(bool clientConnectionState)
        {
            IsConnected = clientConnectionState;

            // if client connected then start ping timer
            // and try get authorized user
            if (clientConnectionState)
            {
                // get authorized user from ZClient
                var userRequest = ZRequestFactory.CreateUserInfoRequest();
                var response = await ZRouter.GetResponseAsync(userRequest);

                // check response
                if (response.StatusCode == ZResponseStatusCode.Ok)
                {
                    _currentUserInfo = _ParseUserInfo(response.ResponsePackets);
                    _pingTimer.Start();

                    _RaiseOnAuthorizedEvent(_currentUserInfo);
                }
                else
                {
                    _logger.Warning($"Request failed {userRequest}");
                }
            }
            else
            {
                _pingTimer.Stop();
                _currentUserInfo = null;
            }

            // ReSharper disable once InvertIf
            if (_raiseOnConnectionChangedEvent)
            {
                _RaiseOnConnectionChangedEvent(clientConnectionState);
                _raiseOnConnectionChangedEvent = true;
            }
        }

        private ZUserDto _ParseUserInfo(ZPacket[] responsePackets)
        {
            var payloadPacket = responsePackets.Single();
            var user = _userInfoParser.Parse(payloadPacket);

            return user;
        }
        private async void _OnPingTimerElapsedCallback(object sender, ElapsedEventArgs e)
        {
            // we are playing Ping-Pong with ZClient
            var pingRequest = ZRequestFactory.CreatePingRequest();
            var pongResponse = await ZRouter.GetResponseAsync(pingRequest);

            // check and set current connection state
            if (pongResponse.StatusCode != ZResponseStatusCode.Ok)
            {
                // burning bridges, completely :)
                Disconnect(true);
            }
        }

        private void _RaiseOnConnectionChangedEvent(bool connectionState) => ConnectionChanged?.Invoke((IZConnection) this, new ZConnectionChangedEventArgs(connectionState));
        private void _RaiseOnAuthorizedEvent(ZUserDto userInfo) => Authorized?.Invoke((IZConnection) this, new ZAuthorizedEventArgs(userInfo));

        #endregion

        #region IZConnection interface

        public event EventHandler<ZConnectionChangedEventArgs> ConnectionChanged;
        public event EventHandler<ZAuthorizedEventArgs> Authorized;

        public void Connect() => ZRouter.Start();
        public void Disconnect(bool raiseEvent = true)
        {
            // prepare to disconnect
            _raiseOnConnectionChangedEvent = raiseEvent;

            // disconnect
            ZRouter.Stop();
        }
        public ZUserDto GetCurrentUserInfo() => _currentUserInfo;

        public bool IsConnected { get; private set; }

        #endregion
    }
}
