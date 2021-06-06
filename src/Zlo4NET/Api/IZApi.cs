using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Services;

namespace Zlo4NET.Api
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZApi
    {
        #region Async methods

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
        /// Gets the game factory
        /// </summary>
        IZGameFactory GameFactory { get; }
        /// <summary>
        /// Gets the API connection
        /// </summary>
        IZConnection Connection { get; }
        /// <summary>
        /// Gets the API logger
        /// </summary>
        IZLogger Logger { get; }
        /// <summary>
        /// Gets the installed games service
        /// </summary>
        IZInstalledGames InstalledGamesService { get; }
        /// <summary>
        /// Gets the injector
        /// </summary>
        IZInjector Injector { get; }

        #endregion

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