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

            // due to the peculiarities of the work of the old and new client
            // where the old one, in case of an unauthorized state, generates two events of changing the state of the connection at once
            // the first is positive, and the second, negative
            // while a negative one is generated after receiving the first request (which is a request to obtain data about an authorized user)
            // the new client, in an unauthorized state, generates only one event, negative

            // so the code is structured to be compatible with these two situations

            // to solve this problem, the concept of an internal state was introduced (there are three of them at once)
            // no decisions about the state of the connection will be made until the incoming state is different from the previous one
            if (_internalConnectionState == null || _internalConnectionState.Value != clientConnectionState)
            {
                var isAuthorized = false;

                // so, if we have connected to the host, this does not mean that the connection is active (for example, the user has not yet logged into the ZClient)
                // therefore, before we say in the affirmative that the connection has been established, we will try to get an authorized user
                if (clientConnectionState)
                {
                    var userRequest = ZRequestFactory.CreateUserInfoRequest();
                    var response = await ZRouter.GetResponseAsync(userRequest);
                    //var response = new ZResponse(null) { StatusCode = ZResponseStatusCode.Declined };

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
                    _internalConnectionState = isAuthorized;
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
                    _RaiseOnConnectionChangedEvent(IsConnected, _currentUserInfo);
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

        public bool IsConnected => _internalConnectionState.GetValueOrDefault(false);

        #endregion
    }
}
