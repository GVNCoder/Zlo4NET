using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZInstalledGamesService : IZInstalledGamesService
    {
        private readonly IZInstalledGamesParser _installedGamesParser;
        private readonly IZClientService _clientService;
        private readonly ZLogger _logger;

        public ZInstalledGamesService(IZClientService clientService)
        {
            _clientService = clientService;
            _installedGamesParser = ZParsersFactory.BuildInstalledGamesInfoParser();
            _logger = ZLogger.Instance;
        }

        public async Task<ZInstalledGames> GetInstalledGamesAsync()
        {
            ZInstalledGames installedGames = null;
            var response = await _clientService.SendInstalledGamesRequestAsync();

            if (response.Status != ZResponseStatusCode.Ok)
            {
                _logger.Warning($"Received response id: {response.Request.Id} with {response.Status}.");
            }
            else
            {
                installedGames = _installedGamesParser.Parse(response.Packets);
            }

            return installedGames;
        }
    }
}