using System;

// ReSharper disable InheritdocConsiderUsage
// ReSharper disable MemberCanBePrivate.Global

namespace Zlo4NET.Api.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class ZLoggerMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets logger message level
        /// </summary>
        public ZLoggingLevel Level { get; }
        /// <summary>
        /// Gets message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates an instance of <see cref="ZLoggerMessageEventArgs"/> class
        /// </summary>
        public ZLoggerMessageEventArgs(ZLoggingLevel level, string message)
        {
            Level = level;
            Message = message;
        }
    }
}