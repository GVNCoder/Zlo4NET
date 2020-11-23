using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Services
{
    public interface IZStatsService
    {
        Task<ZStatsBase> GetStatsAsync(ZGame game);
    }
}