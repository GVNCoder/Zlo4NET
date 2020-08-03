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
        Task<IZRunGame> CreateSingleAsync(ZSingleParams args);
        /// <summary>
        /// Asynchronously creates a game to run
        /// </summary>
        /// <param name="args">Options for creating a game</param>
        /// <returns>A task that represents the asynchronous game creation operation</returns>
        Task<IZRunGame> CreateMultiAsync(ZMultiParams args);
        /// <summary>
        /// Asynchronously creates a game to run
        /// </summary>
        /// <param name="args">Options for creating a game</param>
        /// <returns>A task that represents the asynchronous game creation operation</returns>
        Task<IZRunGame> CreateTestRangeAsync(ZTestRangeParams args);
        /// <summary>
        /// Asynchronously creates a game to run
        /// </summary>
        /// <param name="args">Options for creating a game</param>
        /// <returns>A task that represents the asynchronous game creation operation</returns>
        Task<IZRunGame> CreateCoOpAsync(ZCoopParams args);
    }
}