using System;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Defines logger
    /// </summary>
    public interface IZLogger
    {
        /// <summary>
        /// Occurs when a message has been received for the log.
        /// </summary>
        event EventHandler<ZLogMessageArgs> LogMessage;
        /// <summary>
        /// Sets message display filtering
        /// </summary>
        /// <param name="level">Levels for filtering</param>
        void SetLogLevelFiltering(ZLogLevel level);
    }
}