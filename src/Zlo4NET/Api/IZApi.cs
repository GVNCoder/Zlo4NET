using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zlo4NET.Api.Service;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api
{
    /// <summary>
    /// Defines the ZloApi
    /// </summary>
    public interface IZApi
    {
        #region Async methods

        /// <summary>
        /// Makes an asynchronous request to get current soldier statistics
        /// </summary>
        /// <param name="game">Game context</param>
        /// <exception cref="NotSupportedException">Occurs when specifying the Battlefield Hardline parameter</exception>
        /// <exception cref="InvalidOperationException">Occurs when Api is not connected</exception>
        /// <returns>A task that represents the asynchronous get soldier statistics operation</returns>
        Task<ZStatsBase> GetStatsAsync(ZGame game);

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
        /// Creates and returns an implementation of <see cref="IZServersListService"/>
        /// </summary>
        /// <param name="game">Game context</param>
        /// <exception cref="InvalidOperationException">Occurs when trying to create a service in an non-configured Api or Api is not connected</exception>
        /// <exception cref="InvalidEnumArgumentException">Occurs when a parameter is set to an invalid value</exception>
        /// <returns>Implementation of <see cref="IZServersListService"/></returns>
        IZServersListService CreateServersListService(ZGame game);
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