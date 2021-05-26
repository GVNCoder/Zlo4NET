using System.Threading.Tasks;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZPlayerStatsService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        Task<ZPlayerStatsBase> GetStatsAsync(ZGame game);
    }
}