using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

namespace Zlo4NET.Core.Data
{
    internal class ZPlayerStatsImpl : IZPlayerStats
    {
        private readonly IZPlayerStatsParser _parser;
        private readonly ZLogger _logger;

        #region Ctor

        public ZPlayerStatsImpl()
        {
            _parser = ZParsersFactory.CreateStatsInfoParser();
            _logger = ZLogger.Instance;
        }

        #endregion

        #region IZPlayerStats interface

        public async Task<ZPlayerStatsBase> GetStatsAsync(ZGame game)
        {
            ZConnectionHelper.ThrowIfNotConnected();
            ZGameHelper.ThrowIfOutOfRange(game);

            var request = ZRequestFactory.CreateStatsRequest(game);
            var response = await ZRouter.GetResponseAsync(request);

            if (response.StatusCode != ZResponseStatusCode.Ok)
            {
                _logger.Error($"Request fail {request}");
            }
            else
            {
                return _ParsePlayerStats(response.ResponsePackets);
            }

            return null;
        }

        #endregion

        #region Private helpers

        private ZPlayerStatsBase _ParsePlayerStats(IEnumerable<ZPacket> packets)
        {
            var responsePacket = packets.Single();
            var stats = _parser.Parse(responsePacket);

            return stats;
        }

        #endregion
    }
}