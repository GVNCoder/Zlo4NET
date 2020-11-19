using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Defines game factory
    /// </summary>
    public interface IZGameFactory
    {
        /// <summary>
        /// Asynchronously creates a game to run
        /// </summary>
        /// <param name="args">Options for creating a game</param>
        /// <returns>A task that represents the asynchronous game creation operation</returns>
        Task<IZGameProcess> CreateSingleAsync(ZSingleParams args);
        /// <summary>
        /// Asynchronously creates a game to run
        /// </summary>
        /// <param name="args">Options for creating a game</param>
        /// <returns>A task that represents the asynchronous game creation operation</returns>
        Task<IZGameProcess> CreateMultiAsync(ZMultiParams args);
        /// <summary>
        /// Asynchronously creates a game to run
        /// </summary>
        /// <param name="args">Options for creating a game</param>
        /// <returns>A task that represents the asynchronous game creation operation</returns>
        Task<IZGameProcess> CreateTestRangeAsync(ZTestRangeParams args);
        /// <summary>
        /// Asynchronously creates a game to run
        /// </summary>
        /// <param name="args">Options for creating a game</param>
        /// <returns>A task that represents the asynchronous game creation operation</returns>
        Task<IZGameProcess> CreateCoOpAsync(ZCoopParams args);
        /// <summary>
        /// Asynchronously creates a game to track
        /// </summary>
        /// <param name="processName">The game process name</param>
        /// <returns>A task that represents the asynchronous game creation operation</returns>
        Task<IZGameProcess> CreateByProcessNameAsync(string processName);
    }
}