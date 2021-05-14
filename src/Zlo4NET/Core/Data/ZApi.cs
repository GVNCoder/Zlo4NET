using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using Zlo4NET.Api;
using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Api.Models.Shared;

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
        private readonly IZInjectorService _injector;
        private readonly IZPlayerStatsService _playerStatsService;
        private readonly IZGameFactory _gameFactory;

        private ZConfiguration _config;
        private IZServersList _lastCreatedServerListInstance;

        private ZApi()
        {
            // creating a client services
            _connection         = new ZConnection();
            _playerStatsService = new ZPlayerStatsService();
            _gameFactory        = new ZGameFactory(_connection);
            _injector           = new ZInjectorService();

            // initializing the static helpers
            ZConnectionHelper.Initialize(_connection);
        }

        #region IZApi

        public IZGameFactory GameFactory => _gameFactory;

        public IZConnection Connection => _connection;

        public IZLogger Logger => ZLogger.Instance;

        public async Task<ZPlayerStatsDto> GetPlayerStatsAsync(ZGame game)
        {
            // pre-validation
            ZConnectionHelper.ThrowIfNotConnected();

            if (game == ZGame.None)
            {
                throw new ArgumentException("Argument out of range", nameof(game));
            }

            // get player stats
            var result = await _playerStatsService.GetStatsAsync(game);

            return result;
        }

        public async Task<IZServersList> CreateServersListAsync(ZGame game)
        {
            // pre-validation
            ZConnectionHelper.ThrowIfNotConnected();

            if (game == ZGame.None)
            {
                throw new InvalidEnumArgumentException(nameof(game), (int)game, typeof(ZGame));
            }

            // first of all need to destroy last created instance
            if (_lastCreatedServerListInstance != null && _lastCreatedServerListInstance.IsInstanceAvailable)
            {
                await _lastCreatedServerListInstance.StopReceivingAsync();
            }

            // now, we can create a new instance ;)
            _lastCreatedServerListInstance = new ZServersList(game, Connection);

            return _lastCreatedServerListInstance;
        }

        public void InjectDll(ZGame game, IEnumerable<string> paths)
        {
            ZConnectionHelper.MakeSureConnection();

            _injector.Inject(game, paths);
        }

        public void Configure(ZConfiguration config)
        {
            if (_config != null) throw new InvalidOperationException("This method can only be called once.");
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (config.SynchronizationContext == null) throw new ArgumentException(nameof(config.SynchronizationContext));

            ZSynchronizationWrapper.Initialize(config);
            _config = config;
        }

        #endregion
    }
}