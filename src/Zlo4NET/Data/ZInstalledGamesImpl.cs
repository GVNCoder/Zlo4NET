using System.Linq;
using System.Threading.Tasks;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Extensions;
using Zlo4NET.Helpers;
using Zlo4NET.Services;
using Zlo4NET.ZClientAPI;

namespace Zlo4NET.Data
{
    internal class ZInstalledGamesImpl : IZInstalledGames
    {
        private readonly IZInstalledGamesParser _installedGamesParser;
        private readonly ZLoggerImpl _logger;

        #region Ctor

        public ZInstalledGamesImpl()
        {
            _installedGamesParser = ZParsersFactory.CreateInstalledGamesInfoParser();
            _logger = ZLoggerImpl.Instance;
        }

        #endregion

        #region IZInstalledGames interface

        public async Task<ZInstalledGamesCollection> GetGamesCollectionAsync()
        {
            ZConnectionHelper.ThrowIfNotConnected();

            return await _GetGamesCollectionInternal();
        }

        #endregion

        #region Private helpers

        private async Task<ZInstalledGamesCollection> _GetGamesCollectionInternal()
        {
            ZInstalledGamesCollection installedGames = null;

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

        #endregion
    }
}