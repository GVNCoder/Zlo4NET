using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Zlo4NET.Api.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZApi
    {
        /// <summary>
        /// Makes an asynchronous request to create <see cref="IZServersList"/> instance
        /// </summary>
        /// <param name="game">The game context</param>
        /// <exception cref="InvalidOperationException">Occurs when Api is not connected to ZClient</exception>
        /// <exception cref="InvalidEnumArgumentException">Occurs when specifying the unsupported parameter <see cref="ZGame.None"/></exception>
        /// <returns></returns>
        Task<IZServersList> CreateServersListServiceAsync(ZGame game);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IZInjector GetInjectorService();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IZInstalledGames GetInstalledGamesService();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IZLogger GetApiLogger();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IZConnection GetApiConnection();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IZGameFactory GetGameFactory();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IZPlayerStats GetPlayerStatsService();
    }
}