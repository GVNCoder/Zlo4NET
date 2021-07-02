using System;
using System.Linq;
using System.Timers;
using System.Threading;
using System.Collections.Generic;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Helpers;
using Zlo4NET.Services;
using Zlo4NET.ZClientAPI;
using Zlo4NET.Api.Shared;
using Zlo4NET.Extensions;
using Timer = System.Timers.Timer;

namespace Zlo4NET.Data
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
        private readonly ZLoggerImpl _logger;

        private ZUser _currentUserInfo;
        private bool _raiseOnConnectionChangedEvent = true;
        private bool? _internalConnectionState = null; // where [null] - initial state, [true/false] - concrete state
        private int _connectionOperationLock = 0;

        public ZConnection()
        {
            _userInfoParser = ZParsersFactory.CreateUserInfoParser();
            _logger         = ZLoggerImpl.Instance;
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

                // clear Connect() method lock
                Interlocked.Exchange(ref _connectionOperationLock, 0);

                // ReSharper disable once InvertIf
                if (_raiseOnConnectionChangedEvent)
                {
                    _RaiseOnConnectionChangedEvent(IsConnected, _currentUserInfo);
                    _raiseOnConnectionChangedEvent = true;
                }
            }

            _semaphore.Release();
        }

        private ZUser _ParseUserInfo(IEnumerable<ZPacket> responsePackets)
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

        private void _RaiseOnConnectionChangedEvent(bool connectionState, ZUser authorizedUserDto)
            => ConnectionChanged?.Invoke((IZConnection) this, new ZConnectionChangedEventArgs(connectionState, authorizedUserDto));

        #endregion

        #region IZConnection interface

        public event EventHandler<ZConnectionChangedEventArgs> ConnectionChanged;

        public void Connect()
        {
            // check, if someone trying to connect multiple times
            // and set connection pending state
            if (Interlocked.Exchange(ref _connectionOperationLock, 1) != 0)
            {
                throw new InvalidOperationException("The connection operation has already been initiated");
            }

            // check, if we already connected
            if (IsConnected)
            {
                throw new InvalidOperationException("The connection has already been established");
            }

            ZRouter.Start();
        }
        public void Disconnect(bool raiseEvent = true)
        {
            // check, if we already disconnected
            if (! IsConnected)
            {
                throw new InvalidOperationException("The connection has already been dropped or not yet established");
            }

            // prepare to disconnect
            _raiseOnConnectionChangedEvent = raiseEvent;

            // disconnect
            ZRouter.Stop();
        }
        public ZUser GetCurrentUserInfo() => _currentUserInfo;

        public bool IsConnected => _internalConnectionState.GetValueOrDefault(false);
        public bool IsPending => _connectionOperationLock == 1;

        #endregion
    }
}
