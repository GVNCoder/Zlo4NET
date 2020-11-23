using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Services
{
    internal interface IZInjectorService
    {
        void Inject(ZGame game, string[] dllPaths);
    }
}