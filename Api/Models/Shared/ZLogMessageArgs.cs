using System;

namespace Zlo4NET.Api.Models.Shared
{
    /// <inheritdoc />
    /// <summary>
    /// Defines log event args
    /// </summary>
    public class ZLogMessageArgs : EventArgs
    {
        /// <summary>
        /// Gets message
        /// </summary>
        public string Message { get; }
        /// <inheritdoc />
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="message">Message string</param>
        public ZLogMessageArgs(string message)
        {
            Message = message;
        }
    }
}