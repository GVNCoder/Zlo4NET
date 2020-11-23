using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZStatsService : IZStatsService
    {
        private readonly IZClientService _clientService;
        private readonly IZStatsParser _parser;
        private readonly ZLogger _logger;

        public ZStatsService(IZClientService clientService)
        {
            _clientService = clientService;
            _parser = ZParsersFactory.BuildStatsInfoParser();
            _logger = ZLogger.Instance;
        }

        public async Task<ZStatsBase> GetStatsAsync(ZGame game)
        {
            ZStatsBase stats = null;
            var response = await _clientService.SendStatsRequestAsync(game);

            if (response.Status != ZResponseStatusCode.Ok)
            {
                _logger.Error($"Received response by id: {response.Request.Id} with {response.Status}.");
            }
            else
            {
                switch (game)
                {
                    case ZGame.BF3: stats = _parser.ParseBF3Stats(response.Packets); break;
                    case ZGame.BF4: stats = _parser.ParseBF4Stats(response.Packets); break;
                }
            }

            return stats;
        }
    }
}