using System;
using System.Timers;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Core.ZClient.Services;

//public async Task<bool> CheckMonolithAsync()
//{
//    string stringContent;
//    using (var client = new WebClient())
//    {
//        stringContent = await client.DownloadStringTaskAsync("http://zloemu.net/z_test");
//    }

//    var monolithStatusObject = _phpObjectDeserializer.Deserialize(stringContent) as Hashtable;

//    return true;
//}

namespace Zlo4NET.Core.Data
{
    internal class ZConnection : IZConnection 
    {
        private readonly IZUserService _userService;
        private readonly IZClientService _clientService;
        private readonly IZClient _client;
        private readonly Timer _pingTimer;

        private bool __enabled;
        private bool? __curConState;

        public ZConnection(IZUserService userService, IZClientService clientService)
        {
            _userService = userService;
            _clientService = clientService;
            _client = clientService.Client;

            _pingTimer = new Timer(TimeSpan.FromSeconds(15).TotalMilliseconds) { Enabled = false, AutoReset = true };

            _pingTimer.Elapsed += _pingTimerElapsedHandler;
            _client.ConnectionChanged += _clientConnectionChangedHandler;
        }

        private void _OnConnectionChanged(bool state, ZUser user)
            => ConnectionChanged?.Invoke((IZConnection) this, new ZConnectionChangedArgs(state, user));

        private void _resetConnection() 
        {
            __curConState = default(bool?);
            __enabled = false;
            _pingTimer.Stop();
        }
        
        private void _clientConnectionChangedHandler(object sender, ZClientConnectionChangedArgs e)
        {
            if (e.ConnectionState)
            {
                _pingTimer.Start();
                _pingTimerElapsedHandler(null, null); // initial fire
            }
            else
            {
                _resetConnection();
                _OnConnectionChanged(false, null);
            }
        }

        private async void _pingTimerElapsedHandler(object sender, ElapsedEventArgs e)
        {
            if (_userService.AuthorizedUser == null)
            {
                await _userService.GetAuthorizedUserAsync();
            }

            var pingReply = await _clientService.SendPingRequestAsync();
            var newState = pingReply.Status == ZResponseStatusCode.Ok;
            if (__curConState != newState && __enabled)
            {
                __curConState = newState;
                _OnConnectionChanged(newState, _userService.AuthorizedUser);
            }

            if (newState) return;

            _resetConnection();
        }

        public void Connect()
        {
            if (__enabled) return;
            __enabled = true;

            _client.StartClient();
        }

        public void Disconnect()
        {
            if (! __enabled) return;
            __enabled = false;

            _client.StopClient();
        }

        public bool IsConnected => __curConState ?? false;
        public ZUser AuthorizedUser => _userService.AuthorizedUser;

        public event EventHandler<ZConnectionChangedArgs> ConnectionChanged;
    }
}
