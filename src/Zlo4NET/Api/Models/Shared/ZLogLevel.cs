using System;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines log levels enum
    /// </summary>
    [Flags]
    public enum ZLogLevel
    {
        /// <summary>
        /// Info log level
        /// </summary>
        Info = 2,
        /// <summary>
        /// Debug log level
        /// </summary>
        Debug = 4,
        /// <summary>
        /// Warnings log level
        /// </summary>
        Warning = 8,
        /// <summary>
        /// Error log level
        /// </summary>
        Error = 16
    }
}