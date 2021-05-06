using System.Linq;

using Zlo4NET.Core.Helpers;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.ZClientAPI
{
    /// <summary>
    /// Represents a request factory
    /// </summary>
    internal static class ZRequestFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="ZRequest"/> for <see cref="ZCommand.UserInfo"/>
        /// </summary>
        /// <returns>An instance of <see cref="ZRequest"/> for <see cref="ZCommand.UserInfo"/></returns>
        public static ZRequest CreateUserInfoRequest() => new ZRequest
        {
            RequestCommand = ZCommand.UserInfo,
            Method = ZRequestMethod.Get
        };
        /// <summary>
        /// Creates an instance of <see cref="ZRequest"/> for <see cref="ZCommand.Ping"/>
        /// </summary>
        /// <returns>An instance of <see cref="ZRequest"/> for <see cref="ZCommand.Ping"/></returns>
        public static ZRequest CreatePingRequest() => new ZRequest
        {
            RequestCommand = ZCommand.Ping,
            Method = ZRequestMethod.Get
        };
        /// <summary>
        /// Creates an instance of <see cref="ZRequest"/> for <see cref="ZCommand.ServerList"/> to Open Stream
        /// </summary>
        /// <param name="game">The target game</param>
        /// <returns>An instance of <see cref="ZRequest"/> for <see cref="ZCommand.ServerList"/></returns>
        public static ZRequest CreateServerListOpenStreamRequest(ZGame game) => new ZRequest
        {
            RequestCommand = ZCommand.ServerList,
            Method = ZRequestMethod.Put,
            RequestPayload = new byte[] { 0, (byte) game }
        };
        /// <summary>
        /// Creates an instance of <see cref="ZRequest"/> for <see cref="ZCommand.ServerList"/> to Close Stream
        /// </summary>
        /// <param name="game">The target game</param>
        /// <returns>An instance of <see cref="ZRequest"/> for <see cref="ZCommand.ServerList"/></returns>
        public static ZRequest CreateServerListCloseStreamRequest(ZGame game) => new ZRequest
        {
            RequestCommand = ZCommand.ServerList,
            Method = ZRequestMethod.Put,
            RequestPayload = new byte[] { 1, (byte)game }
        };
        /// <summary>
        /// Creates an instance of <see cref="ZRequest"/> for <see cref="ZCommand.GameList"/>
        /// </summary>
        /// <returns>An instance of <see cref="ZRequest"/> for <see cref="ZCommand.GameList"/></returns>
        public static ZRequest CreateInstalledGamesRequest() => new ZRequest
        {
            RequestCommand = ZCommand.GameList,
            Method = ZRequestMethod.Get
        };
        /// <summary>
        /// Creates an instance of <see cref="ZRequest"/> for <see cref="ZCommand.RunGame"/>
        /// </summary>
        /// <param name="runnableGameName">The runnable (sys) game name</param>
        /// <param name="commandArgs">The game run command args</param>
        /// <returns>An instance of <see cref="ZRequest"/> for <see cref="ZCommand.RunGame"/></returns>
        public static ZRequest CreateRunGameRequest(string runnableGameName, string commandArgs) => new ZRequest
        {
            RequestCommand = ZCommand.RunGame,
            Method = ZRequestMethod.Get,
            RequestPayload = ZBitConverter.Convert(runnableGameName)
                .Concat(ZBitConverter.Convert(commandArgs))
                .ToArray()
        };
        /// <summary>
        /// Creates an instance of <see cref="ZRequest"/> for <see cref="ZCommand.Inject"/>
        /// </summary>
        /// <param name="game">The target game</param>
        /// <param name="dllPath">The path to dll to inject</param>
        /// <returns>An instance of <see cref="ZRequest"/> for <see cref="ZCommand.Inject"/></returns>
        public static ZRequest CreateDllInjectRequest(ZGame game, string dllPath) => new ZRequest
        {
            RequestCommand = ZCommand.Inject,
            Method = ZRequestMethod.Get,
            RequestPayload = new[] { (byte) game }
                .Concat(ZBitConverter.Convert(dllPath))
                .ToArray()
        };
        /// <summary>
        /// Creates an instance of <see cref="ZRequest"/> for <see cref="ZCommand.Stats"/>
        /// </summary>
        /// <param name="game">The target game</param>
        /// <returns>An instance of <see cref="ZRequest"/> for <see cref="ZCommand.Stats"/></returns>
        public static ZRequest CreateStatsRequest(ZGame game) => new ZRequest
        {
            RequestCommand = ZCommand.Stats,
            Method = ZRequestMethod.Get,
            RequestPayload = new[] { (byte) game }
        };
    }
}