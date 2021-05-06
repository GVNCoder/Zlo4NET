using System.Threading.Tasks;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal class ZStatsService : IZStatsService
    {
        private readonly IZStatsParser _parser;
        private readonly ZLogger _logger;

        public ZStatsService()
        {
            _parser = ZParsersFactory.CreateStatsInfoParser();
            _logger = ZLogger.Instance;
        }

        public async Task<ZStatsBase> GetStatsAsync(ZGame game)
        {
            ZStatsBase stats = null;

            var request = ZRequestFactory.CreateStatsRequest(game);
            var response = await ZRouter.GetResponseAsync(request);

            if (response.StatusCode != ZResponseStatusCode.Ok)
            {
                _logger.Error($"Request fail {request}");
            }
            else
            {
                switch (game)
                {
                    case ZGame.BF3: stats = _parser.ParseBF3Stats(response.ResponsePackets); break;
                    case ZGame.BF4: stats = _parser.ParseBF4Stats(response.ResponsePackets); break;
                }
            }

            return stats;
        }
    }
}