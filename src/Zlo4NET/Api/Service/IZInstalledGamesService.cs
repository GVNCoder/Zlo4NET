using System.Threading.Tasks;

using Zlo4NET.Api.DTOs;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZInstalledGamesService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<ZInstalledGames> GetInstalledGamesAsync();
    }
}