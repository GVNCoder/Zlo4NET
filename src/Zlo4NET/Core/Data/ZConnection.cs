using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Service;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

using Timer = System.Timers.Timer;

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

        // this means that only 1 thread can be granted access at a time
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private readonly IZUserInfoParser _userInfoParser;
        private readonly Timer _pingTimer;
        private readonly ZLogger _logger;

        private ZUserDto _currentUserInfo;
        private bool _raiseOnConnectionChangedEvent = true;
        private bool? _internalConnectionState = null; // where [null] - initial state, [true/false] - concrete state

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
            // lock current execution context
            await _semaphore.WaitAsync();

            // TODO: Explain logic, cuz this code is difficult to understand

            // check if this is the first processing of the connection state, or not
            // where internalConnectionState.Value is last connection state
            // if this is not the first processing attempt, and nothing has changed from the previous state, then we simply exit the method
            if (_internalConnectionState == null || _internalConnectionState.Value != clientConnectionState)
            {
                var isAuthorized = false;

                // so, if we have connected to the host, this does not mean that the connection is active (for example, the user has not yet logged into the ZClient)
                // therefore, before we say in the affirmative that the connection has been established, we will try to get an authorized user
                if (clientConnectionState)
                {
                    var userRequest = ZRequestFactory.CreateUserInfoRequest();
                    var response = await ZRouter.GetResponseAsync(userRequest);

                    if (response.StatusCode == ZResponseStatusCode.Ok)
                    {
                        _currentUserInfo = _ParseUserInfo(response.ResponsePackets);
                        _pingTimer.Start();

                        isAuthorized = true;
                    }
                    else
                    {
                        _logger.Warning($"Request failed {userRequest}");
                    }

                    // set current connection state
                    _internalConnectionState = IsConnected = isAuthorized;
                }
                else
                {
                    // reset internal connection state
                    _internalConnectionState = null;
                    _currentUserInfo = null;

                    _pingTimer.Stop();
                }

                // ReSharper disable once InvertIf
                if (_raiseOnConnectionChangedEvent)
                {
                    _RaiseOnConnectionChangedEvent(clientConnectionState, _currentUserInfo);
                    _raiseOnConnectionChangedEvent = true;
                }
            }

            _semaphore.Release();
        }

        private ZUserDto _ParseUserInfo(IEnumerable<ZPacket> responsePackets)
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

        private void _RaiseOnConnectionChangedEvent(bool connectionState, ZUserDto authorizedUserDto)
            => ConnectionChanged?.Invoke((IZConnection) this, new ZConnectionChangedEventArgs(connectionState, authorizedUserDto));

        #endregion

        #region IZConnection interface

        public event EventHandler<ZConnectionChangedEventArgs> ConnectionChanged;

        public void Connect()
        {
            // already connected ?
            if (IsConnected)
            {
                return;
            }

            ZRouter.Start();
        }
        public void Disconnect(bool raiseEvent = true)
        {
            // already disconnected ?
            if (! IsConnected)
            {
                return;
            }

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
