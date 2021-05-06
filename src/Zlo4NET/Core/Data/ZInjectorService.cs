using System.Collections.Generic;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data
{
    internal class ZInjectorService : IZInjectorService
    {
        public async void Inject(ZGame game, IEnumerable<string> dllPaths)
        {
            foreach (var dllPath in dllPaths)
            {
                var request = ZRequestFactory.CreateDllInjectRequest(game, dllPath);
                var response = await ZRouter.GetResponseAsync(request);
            }
        }
    }
}