using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.ZClient.Data
{
    internal class ZClientService : IZClientService
    {
        public ZClientService()
        {
            Client = new ZClient();
            RequestFactory = new ZRequestFactory(Client);
        }

        public IZClient Client { get; }

        public IZRequestFactory RequestFactory { get; }

        public Task<ZResponse> SendDllInjectRequestAsync(ZGame game, string dllPath)
        {
            var request = RequestFactory.BuildDllInjectRequest(game, dllPath);
            return request.GetResponseAsync();
        }

        public Task<ZResponse> SendGameRunRequestAsync(string runnableGameName, string commandArgs)
        {
            var request = RequestFactory.BuildRunGameRequest(runnableGameName, commandArgs);
            return request.GetResponseAsync();
        }

        public Task<ZResponse> SendInstalledGamesRequestAsync()
        {
            var request = RequestFactory.BuildInstalledGamesRequest();
            return request.GetResponseAsync();
        }

        public Task<ZResponse> SendPingRequestAsync()
        {
            var request = RequestFactory.BuildPingRequest();
            return request.GetResponseAsync();
        }

        public Task<ZResponse> SendStatsRequestAsync(ZGame game)
        {
            var request = RequestFactory.BuildStatsRequest(game);
            return request.GetResponseAsync();
        }

        public Task<ZResponse> SendUserInfoRequestAsync()
        {
            var request = RequestFactory.BuildUserInfoRequest();
            return request.GetResponseAsync();
        }

        public ZTunnel CreateTunnel(ZRequest openRequest, ZRequest closeRequest)
        {
            var tunnel = new ZTunnel(Client, openRequest, closeRequest);
            Client.RegisterTunnel(tunnel);
            return tunnel;
        }
    }
}