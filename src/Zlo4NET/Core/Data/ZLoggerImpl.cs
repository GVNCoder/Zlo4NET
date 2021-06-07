using System;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

// ReSharper disable InvertIf

namespace Zlo4NET.Core.Data
{
    internal class ZLoggerImpl : IZLogger
    {
        #region Singleton

        // https://csharpindepth.com/articles/singleton
        static ZLoggerImpl() { }

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<ZLoggerImpl> __lazyInstance = new Lazy<ZLoggerImpl>(() => new ZLoggerImpl(), true);

        public static ZLoggerImpl Instance => __lazyInstance.Value;

        #endregion

        private ZLoggingLevel _levelFilteringFlags = ZLoggingLevel.Warning | ZLoggingLevel.Error;
        private string _lastLogMessage;

        #region Private helpers

        private void _OnLogMessage(ZLoggingLevel level, string message, bool passDuplicates)
        {
            // check is it duplicate and can it pass filter
            if ((_lastLogMessage != message || passDuplicates) && _levelFilteringFlags.HasFlag(level))
            {
                ApiMessage?.Invoke(this, new ZLoggerMessageEventArgs(message));

                // save last log message
                _lastLogMessage = message;
            }
        }

        #endregion

        #region Internal public interface

        public void Log(ZLoggingLevel level, string message, bool passDuplicates = false)
        {
            _OnLogMessage(level, message, passDuplicates);
        }

        #endregion

        #region IZLogger interface

        public event EventHandler<ZLoggerMessageEventArgs> ApiMessage;

        public void SetLoggingLevelFiltering(ZLoggingLevel levelFlags)
        {
            _levelFilteringFlags = levelFlags;
        }

        #endregion
    }
}