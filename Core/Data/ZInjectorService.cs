using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZInjectorService : IZInjectorService
    {
        private readonly IZClientService _clientService;

        public ZInjectorService(IZClientService clientService)
        {
            _clientService = clientService;
        }

        public async void Inject(ZGame game, string[] dllPaths)
        {
            foreach (var dllPath in dllPaths)
            {
                await _clientService.SendDllInjectRequestAsync(game, dllPath);
            }
        }
    }
}