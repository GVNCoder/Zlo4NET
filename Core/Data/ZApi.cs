using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    /// <inheritdoc />
    public class ZApi : IZApi
    {
        #region Singleton

        // https://csharpindepth.com/articles/singleton
        static ZApi() { }

        private static readonly Lazy<IZApi> lazy = new Lazy<IZApi>(() => new ZApi(), true);

        public static IZApi Instance => lazy.Value;

        #endregion

        private readonly IZClientService _clientService;

        private readonly IZConnection _connection;
        private readonly IZInjectorService _injector;
        private readonly IZStatsService _statsService;
        private readonly IZGameFactory _gameFactory;

        private ZConfiguration _config;

        internal ZApi()
        {
            // creating a base client
            _clientService = new ZClientService();

            // creating a needed services (local too, for to resolve dependencies)
            var userService = new ZUserService(_clientService);

            // creating a client services
            _connection = new ZConnection(userService, _clientService);
            _statsService = new ZStatsService(_clientService);
            _gameFactory = new ZGameFactory(_clientService, _connection);
            _injector = new ZInjectorService(_clientService);

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
            => new ZServersListService(_clientService, Connection.AuthorizedUser.Id, game);

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

            if (_config == null) throw new InvalidOperationException("You cannot create a service until the api is configured.");
            if (game == ZGame.None) throw new InvalidEnumArgumentException(nameof(game), (int) game, typeof(ZGame));

            var service = _BuildServerListService(game);
            return service;
        }

        /// <inheritdoc />
        public void InjectDll(ZGame game, string[] paths)
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