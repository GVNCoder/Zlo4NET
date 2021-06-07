using System;

// ReSharper disable InheritdocConsiderUsage
// ReSharper disable MemberCanBePrivate.Global

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class ZLoggerMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates an instance of <see cref="ZLoggerMessageEventArgs"/> class
        /// </summary>
        public ZLoggerMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}