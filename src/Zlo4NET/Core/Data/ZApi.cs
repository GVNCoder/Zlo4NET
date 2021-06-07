using System;
using System.Threading.Tasks;

using Zlo4NET.Api;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Api.Models.Shared;

// ReSharper disable ConvertToAutoProperty
// disable Missing xml doc
#pragma warning disable 1591

namespace Zlo4NET.Core.Data
{
    /// <inheritdoc />
    public class ZApi : IZApi
    {
        #region Singleton

        // https://csharpindepth.com/articles/singleton
        static ZApi() { }

        private static readonly Lazy<IZApi> Lazy = new Lazy<IZApi>(() => new ZApi(), true);

        /// <summary>
        /// Creates and returns an single instance of <see cref="IZApi"/> implementation
        /// </summary>
        public static IZApi Instance => Lazy.Value;

        #endregion

        private readonly IZConnection _connection;
        private readonly IZInjector _injector;
        private readonly IZPlayerStats _playerStats;
        private readonly IZGameFactory _gameFactory;
        private readonly IZInstalledGames _installedGames;

        private IZServersList _lastCreatedServerListInstance;

        private ZApi()
        {
            // creating a client services
            _connection      = new ZConnection();
            _playerStats     = new ZPlayerStatsImpl();
            _gameFactory     = new ZGameFactory(_connection);
            _injector        = new ZInjectorImpl();
            _installedGames  = new ZInstalledGames();

            // initializing the static helpers
            ZConnectionHelper.Initialize(_connection);
        }

        #region IZApi

        public async Task<IZServersList> CreateServersListServiceAsync(ZGame game)
        {
            ZGameHelper.ThrowIfOutOfRange(game);

            // first of all need to destroy last created instance
            if (_lastCreatedServerListInstance != null && _lastCreatedServerListInstance.IsInstanceAvailable)
            {
                await _lastCreatedServerListInstance.StopReceivingAsync();
            }

            // now, we can create a new instance ;)
            _lastCreatedServerListInstance = new ZServersListImpl(game, _connection);

            return _lastCreatedServerListInstance;
        }

        public IZInjector GetInjectorService()
        {
            return _injector;
        }

        public IZInstalledGames GetInstalledGamesService()
        {
            return _installedGames;
        }

        public IZLogger GetApiLogger()
        {
            return ZLoggerImpl.Instance;
        }

        public IZConnection GetApiConnection()
        {
            return _connection;
        }

        public IZGameFactory GetGameFactory()
        {
            return _gameFactory;
        }

        public IZPlayerStats GetPlayerStatsService()
        {
            return _playerStats;
        }

        #endregion
    }
}