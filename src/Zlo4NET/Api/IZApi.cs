using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZApi
    {
        #region Async methods

        /// <summary>
        /// Makes an asynchronous request to get authorized player statistics
        /// </summary>
        /// <param name="game">The game context</param>
        /// <exception cref="NotSupportedException">Occurs when specifying the unsupported parameter (like Battlefield Hardline)</exception>
        /// <exception cref="InvalidOperationException">Occurs when Api is not connected to ZClient</exception>
        /// <returns>A task that represents the asynchronous get soldier statistics operation</returns>
        Task<ZPlayerBaseStats> GetPlayerStatsAsync(ZGame game);
        /// <summary>
        /// Makes an asynchronous request to create <see cref="IZServersList"/> instance
        /// </summary>
        /// <param name="game">The game context</param>
        /// <exception cref="InvalidOperationException">Occurs when Api is not connected to ZClient</exception>
        /// <exception cref="InvalidEnumArgumentException">Occurs when specifying the unsupported parameter <see cref="ZGame.None"/></exception>
        /// <returns></returns>
        Task<IZServersList> CreateServersListAsync(ZGame game);

        #endregion

        #region Properties

        /// <summary>
        /// Gets game factory
        /// </summary>
        IZGameFactory GameFactory { get; }
        /// <summary>
        /// Gets API connection
        /// </summary>
        IZConnection Connection { get; }
        /// <summary>
        /// Gets API logger
        /// </summary>
        IZLogger Logger { get; }

        #endregion

        /// <summary>
        /// Makes asynchronous requests to inject specified dlls into the game process
        /// </summary>
        /// <param name="game">Game context</param>
        /// <param name="paths">An array of dll paths for injection</param>
        /// <exception cref="InvalidOperationException">Occurs when Api is not connected</exception>
        void InjectDll(ZGame game, IEnumerable<string> paths);
        /// <summary>
        /// Configures api
        /// </summary>
        /// <param name="config">Configuration instance</param>
        /// <exception cref="InvalidOperationException">Occurs when this method is called more than once</exception>
        /// <exception cref="ArgumentNullException">Occurs when <paramref name="config"/> is null</exception>
        /// <exception cref="ArgumentException">Occurs when <see cref="ZConfiguration.SynchronizationContext"/> is not specified</exception>
        void Configure(ZConfiguration config);
    }
}