using System;

#pragma warning disable 1591

namespace Zlo4NET.Api.Shared
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ZLoggingLevel
    {
        Info = 2,
        Debug = 4,
        Warning = 8,
        Error = 16
    }
}