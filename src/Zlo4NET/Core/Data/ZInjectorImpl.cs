using System.Threading.Tasks;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal class ZInjectorImpl : IZInjector
    {
        private readonly ZLogger _logger;

        #region Ctor

        public ZInjectorImpl()
        {
            _logger = ZLogger.Instance;
        }

        #endregion

        #region IZInjector interface

        public async Task InjectAsync(ZGame targetGame, string filePath)
        {
            ZConnectionHelper.ThrowIfNotConnected();
            ZGameHelper.ThrowIfOutOfRange(targetGame);

            await _InjectInternal(targetGame, filePath);
        }

        #endregion

        #region Private helpers

        private async Task _InjectInternal(ZGame targetGame, string filePath)
        {
            var request = ZRequestFactory.CreateDllInjectRequest(targetGame, filePath);
            var response = await ZRouter.GetResponseAsync(request);

            if (response.StatusCode != ZResponseStatusCode.Ok)
            {
                _logger.Info($"Request failed {request}. File path {filePath}");
            }
        }

        #endregion
    }
}