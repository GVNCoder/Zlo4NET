using System.Collections.Generic;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal class ZInjectorService : IZInjector
    {
        private readonly ZLogger _logger;

        #region Ctor

        public ZInjectorService()
        {
            _logger = ZLogger.Instance;
        }

        #endregion

        public async void Inject(ZGame game, IEnumerable<string> dlls)
        {
            foreach (var dll in dlls)
            {
                var request = ZRequestFactory.CreateDllInjectRequest(game, dll);
                var response = await ZRouter.GetResponseAsync(request);

                if (response.StatusCode != ZResponseStatusCode.Ok)
                {
                    _logger.Warning($"Request failed {request}. Payload string {dll}");
                }
            }
        }
    }
}