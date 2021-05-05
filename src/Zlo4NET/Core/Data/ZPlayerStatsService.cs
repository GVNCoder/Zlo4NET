using System.Linq;
using System.Threading.Tasks;

using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal class ZPlayerStatsService : IZPlayerStatsService
    {
        private readonly IZPlayerStatsParser _parser;
        private readonly ZLogger _logger;

        public ZPlayerStatsService()
        {
            _parser = ZParsersFactory.CreateStatsInfoParser();
            _logger = ZLogger.Instance;
        }

        public async Task<ZPlayerStatsDto> GetStatsAsync(ZGame game)
        {
            var request = ZRequestFactory.CreateStatsRequest(game);
            var response = await ZRouter.GetResponseAsync(request);

            if (response.StatusCode != ZResponseStatusCode.Ok)
            {
                _logger.Error($"Request fail {request}");
            }
            else
            {
                return _ParsePlayerStats(game, response.ResponsePackets);
            }

            return null;
        }

        private ZPlayerStatsDto _ParsePlayerStats(ZGame gameContext, ZPacket[] packets)
        {
            var responsePacket = packets.Single();
            var stats = _parser.Parse(gameContext, responsePacket);

            return stats;
        }
    }
}