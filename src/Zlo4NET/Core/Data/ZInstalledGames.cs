using System.Linq;
using System.Threading.Tasks;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal class ZInstalledGames : IZInstalledGames
    {
        private readonly IZInstalledGamesParser _installedGamesParser;
        private readonly ZLogger _logger;

        public ZInstalledGames()
        {
            _installedGamesParser = ZParsersFactory.CreateInstalledGamesInfoParser();
            _logger = ZLogger.Instance;
        }

        public async Task<ZGameCollection> GetGamesCollectionAsync()
        {
            ZGameCollection installedGames = null;

            var request = ZRequestFactory.CreateInstalledGamesRequest();
            var response = await ZRouter.GetResponseAsync(request);

            if (response.StatusCode == ZResponseStatusCode.Ok)
            {
                var responsePacket = response.ResponsePackets.Single();
                installedGames = _installedGamesParser.Parse(responsePacket);
            }
            else
            {
                _logger.Warning($"Request fail {request}");
            }

            return installedGames;
        }
    }
}