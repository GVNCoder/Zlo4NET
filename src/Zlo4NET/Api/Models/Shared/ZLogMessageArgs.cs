using System;

// ReSharper disable MemberCanBePrivate.Global

namespace Zlo4NET.Api.Models.Shared
{
    /// <inheritdoc />
    /// <summary>
    /// Defines log event args
    /// </summary>
    public class ZLogMessageArgs : EventArgs
    {
        /// <summary>
        /// Gets log level
        /// </summary>
        public ZLogLevel Level { get; }
        /// <summary>
        /// Gets message
        /// </summary>
        public string Message { get; }

        /// <inheritdoc />
        /// <summary>
        /// Default ctor
        /// </summary>
        public ZLogMessageArgs(ZLogLevel level, string message)
        {
            Level = level;
            Message = message;
        }
    }
}