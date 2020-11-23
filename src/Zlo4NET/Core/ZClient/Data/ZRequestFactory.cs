using System.Linq;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.ZClient.Data
{
    internal class ZRequestFactory : IZRequestFactory
    {
        private readonly IZClient _client;

        private ZRequest _BuildRequest()
        {
            return new ZRequest(_client);
        }

        public ZRequestFactory(IZClient client)
        {
            _client = client;
        }

        public ZRequest BuildUserInfoRequest()
        {
            var request = _BuildRequest();
            request.Id = ZCommand.UserInfo;
            request.Method = ZMethod.Get;

            return request;
        }

        public ZRequest BuildPingRequest()
        {
            var request = _BuildRequest();
            request.Id = ZCommand.Ping;
            request.Method = ZMethod.Get;

            return request;
        }

        public ZRequest BuildServerListSubscribeRequest(ZGame game)
        {
            var request = _BuildRequest();
            request.Id = ZCommand.ServerList;
            request.Method = ZMethod.Tunnel;
            request.Payload = new byte[] { 0, (byte) game };

            return request;
        }

        public ZRequest BuildServerListUnsubscribeRequest(ZGame game)
        {
            var request = _BuildRequest();
            request.Id = ZCommand.ServerList;
            request.Method = ZMethod.Tunnel;
            request.Payload = new byte[] { 1, (byte) game };

            return request;
        }

        public ZRequest BuildInstalledGamesRequest()
        {
            var request = _BuildRequest();
            request.Id = ZCommand.GameList;
            request.Method = ZMethod.Get;

            return request;
        }

        public ZRequest BuildRunGameRequest(string runnableGameName, string commandArgs)
        {
            var request = _BuildRequest();
            request.Id = ZCommand.RunGame;
            request.Method = ZMethod.Get;
            request.Payload = ZBitConverter.Convert(runnableGameName)
                .Concat(ZBitConverter.Convert(commandArgs))
                .ToArray();

            return request;
        }

        public ZRequest BuildDllInjectRequest(ZGame game, string dllPath)
        {
            var request = _BuildRequest();
            request.Id = ZCommand.Inject;
            request.Method = ZMethod.Put;
            request.Payload = new[] { (byte) game }
                .Concat(ZBitConverter.Convert(dllPath))
                .ToArray();

            return request;
        }

        public ZRequest BuildStatsRequest(ZGame game)
        {
            var request = _BuildRequest();
            request.Id = ZCommand.Stats;
            request.Method = ZMethod.Get;
            request.Payload = new[] { (byte) game };

            return request;
        }
    }
}