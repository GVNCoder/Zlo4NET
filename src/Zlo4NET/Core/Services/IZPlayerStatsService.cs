using System.Threading.Tasks;

using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Services
{
    public interface IZPlayerStatsService
    {
        Task<ZPlayerStatsDto> GetStatsAsync(ZGame game);
    }
}