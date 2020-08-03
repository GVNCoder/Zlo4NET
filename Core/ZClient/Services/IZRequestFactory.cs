using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.ZClient.Services
{
    /// <summary>
    /// Defines request factory
    /// </summary>
    internal interface IZRequestFactory
    {
        /// <summary>
        /// Builds user info request model
        /// </summary>
        ZRequest BuildUserInfoRequest();
        /// <summary>
        /// Builds ping-pong request model
        /// </summary>
        ZRequest BuildPingRequest();
        /// <summary>
        /// Builds servers list subscribe request model
        /// </summary>
        /// <param name="game">The target game</param>
        ZRequest BuildServerListSubscribeRequest(ZGame game);
        /// <summary>
        /// Builds servers list unsubscribe request model
        /// </summary>
        /// <param name="game">The target game</param>
        ZRequest BuildServerListUnsubscribeRequest(ZGame game);
        /// <summary>
        /// Builds installed games request model
        /// </summary>
        ZRequest BuildInstalledGamesRequest();
        /// <summary>
        /// Builds game run request model
        /// </summary>
        /// <param name="runnableGameName">The runnable game string.</param>
        /// <param name="commandArgs">The run string.</param>
        ZRequest BuildRunGameRequest(string runnableGameName, string commandArgs);
        /// <summary>
        /// Builds dll inject request model
        /// </summary>
        /// <param name="game">The game for inject.</param>
        /// <param name="dllPath">The dll paths.</param>
        ZRequest BuildDllInjectRequest(ZGame game, string dllPath);
        /// <summary>
        /// Builds stats request model
        /// </summary>
        /// <param name="game">The target game</param>
        ZRequest BuildStatsRequest(ZGame game);
    }
}