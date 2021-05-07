using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using Zlo4NET.Api;
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
        private readonly IZStatsService _statsService;
        private readonly IZGameFactory _gameFactory;

        private ZConfiguration _config;
        private IZServersListService _lastCreatedServerListInstance;

        private ZApi()
        {
            // creating a client services
            _connection = new ZConnection();
            _statsService = new ZStatsService();
            _gameFactory = new ZGameFactory(_connection);
            _injector = new ZInjectorService();

            // initializing the static helpers
            ZConnectionHelper.Initialize(_connection);
        }

        #region Impl

        private async Task<ZStatsBase> _GetStatsImpl(ZGame game)
        {
            var result = await _statsService.GetStatsAsync(game);
            return result;
        }

        private IZServersListService _BuildServerListService(ZGame game)
            => new ZServersListService(Connection.GetCurrentUserInfo().UserId, game);

        #endregion

        /// <inheritdoc />
        public IZGameFactory GameFactory => _gameFactory;

        /// <inheritdoc />
        public IZConnection Connection => _connection;

        /// <inheritdoc />
        public IZLogger Logger => ZLogger.Instance;

        /// <inheritdoc />
        public Task<ZStatsBase> GetStatsAsync(ZGame game)
        {
            ZConnectionHelper.MakeSureConnection();
            if (game == ZGame.BFH) throw new NotSupportedException("Stats not implemented for Battlefield Hardline.");

            return _GetStatsImpl(game);
        }

        /// <inheritdoc />
        public IZServersListService CreateServersListService(ZGame game)
        {
            ZConnectionHelper.MakeSureConnection();

            if (_config == null)
            {
                throw new InvalidOperationException("You cannot create a service until the api is configured.");
            }

            if (game == ZGame.None)
            {
                throw new InvalidEnumArgumentException(nameof(game), (int)game, typeof(ZGame));
            }

            if (_lastCreatedServerListInstance != null && _lastCreatedServerListInstance.CanUse)
            {
                _lastCreatedServerListInstance.StopReceiving();
            }

            var service = _BuildServerListService(game);

            _lastCreatedServerListInstance = service;

            return service;
        }

        /// <inheritdoc />
        public void InjectDll(ZGame game, IEnumerable<string> paths)
        {
            ZConnectionHelper.MakeSureConnection();

            _injector.Inject(game, paths);
        }

        /// <inheritdoc />
        public void Configure(ZConfiguration config)
        {
            if (_config != null) throw new InvalidOperationException("This method can only be called once.");
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (config.SynchronizationContext == null) throw new ArgumentException(nameof(config.SynchronizationContext));

            ZSynchronizationWrapper.Initialize(config);
            _config = config;
        }
    }
}