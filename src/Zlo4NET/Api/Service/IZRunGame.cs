using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Defines game ready for run
    /// </summary>
    public interface IZRunGame
    {
        /// <summary>
        /// Occurs when a game generates a message
        /// </summary>
        event EventHandler<ZGamePipeArgs> Pipe;
        /// <summary>
        /// Makes an asynchronous request to run game
        /// </summary>
        /// <returns>A task that represents the asynchronous run game operation</returns>
        Task<ZRunResult> RunAsync();
        /// <summary>
        /// Gets game process
        /// </summary>
        Process GameProcess { get; }

        /// <summary>
        /// Gets a value indicating the current state of the game, running or not.
        /// </summary>
        bool IsRun { get; }
        /// <summary>
        /// Attempts to maximize the game window and give it focus.
        /// </summary>
        /// <returns>Success value</returns>
        bool TryUnfoldGameWindow();
        /// <summary>
        /// Attempts to kill game process.
        /// </summary>
        /// <returns>Success value</returns>
        bool TryClose();
    }
}