using System.Threading.Tasks;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZPlayerStats
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        Task<ZPlayerStatsBase> GetStatsAsync(ZGame game);
    }
}