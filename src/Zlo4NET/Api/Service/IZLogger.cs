using System;

using Zlo4NET.Api.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IZLogger
    {
        /// <summary>
        /// Occurs when a message has been received
        /// </summary>
        event EventHandler<ZLoggerMessageEventArgs> ApiMessage;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelFlags"></param>
        void SetLoggingLevelFiltering(ZLoggingLevel levelFlags);
    }
}