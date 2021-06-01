using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZGameProcess
    {
        /// <summary>
        /// Occurs when a game state changed (Pipe)
        /// </summary>
        event EventHandler<ZGameStateChangedEventArgs> StateChanged;
        /// <summary>
        /// Makes an asynchronous request to run game
        /// </summary>
        /// <returns>A task that represents the asynchronous run game operation</returns>
        Task<ZRunResult> RunAsync();
        /// <summary>
        /// Gets game process
        /// </summary>
        Process Process { get; }
        /// <summary>
        /// Attempts to kill game process
        /// </summary>
        /// <returns>Success value</returns>
        bool TryClose();
    }
}