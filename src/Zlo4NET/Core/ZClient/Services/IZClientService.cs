using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.ZClient.Services
{
    internal interface IZClientService
    {
        /// <summary>
        /// The client
        /// </summary>
        IZClient Client { get; }
        /// <summary>
        /// The request factory
        /// </summary>
        IZRequestFactory RequestFactory { get; }
        /// <summary>
        /// </summary>
        Task<ZResponse> SendUserInfoRequestAsync();
        /// <summary>
        /// </summary>
        Task<ZResponse> SendPingRequestAsync();
        /// <summary>
        /// </summary>
        Task<ZResponse> SendInstalledGamesRequestAsync();
        /// <summary>
        /// </summary>
        /// <param name="runnableGameName">The runnable game string.</param>
        /// <param name="commandArgs">The run string.</param>
        Task<ZResponse> SendGameRunRequestAsync(string runnableGameName, string commandArgs);
        /// <summary>
        /// </summary>
        /// <param name="game">The game for inject.</param>
        /// <param name="dllPath">The dll paths.</param>
        Task<ZResponse> SendDllInjectRequestAsync(ZGame game, string dllPath);
        /// <summary>
        /// </summary>
        /// <param name="game">The target game</param>
        Task<ZResponse> SendStatsRequestAsync(ZGame game);
        /// <summary>
        /// Creates packet streaming tunnel
        /// </summary>
        /// <param name="openRequest">The request for open streaming</param>
        /// <param name="closeRequest">The request for close streaming</param>
        /// <returns><see cref="ZTunnel"/> instance</returns>
        ZTunnel CreateTunnel(ZRequest openRequest, ZRequest closeRequest);
    }
}