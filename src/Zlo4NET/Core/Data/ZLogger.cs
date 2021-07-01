using System;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

// ReSharper disable InvertIf

namespace Zlo4NET.Core.Data
{
    internal class ZLogger : IZLogger
    {
        #region Singleton

        // https://csharpindepth.com/articles/singleton
        static ZLogger() { }

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<ZLogger> __lazyInstance = new Lazy<ZLogger>(() => new ZLogger(), true);

        public static ZLogger Instance => __lazyInstance.Value;

        #endregion

        private ZLogLevel _levelFilter = ZLogLevel.Warning | ZLogLevel.Error;
        private string _lastLogMessage;

        #region Private helpers

        private void OnLogMessage(ZLogLevel level, string message, bool passDuplicates)
        {
            if ((_lastLogMessage != message || passDuplicates) && _levelFilter.HasFlag(level))
            {
                LogMessage?.Invoke(this, new ZLogMessageArgs(level, message));

                // save last log message
                _lastLogMessage = message;
            }
        }

        #endregion

        #region Internal Interface

        public void Debug(string message, bool passDuplicates = false) => OnLogMessage(ZLogLevel.Debug, message, passDuplicates);
        public void Info(string message, bool passDuplicates = false) => OnLogMessage(ZLogLevel.Info, message, passDuplicates);
        public void Warning(string message, bool passDuplicates = false) => OnLogMessage(ZLogLevel.Warning, message, passDuplicates);
        public void Error(string message, bool passDuplicates = false) => OnLogMessage(ZLogLevel.Error, message, passDuplicates);
        public void Log(ZLogLevel level, string message, bool passDuplicates = false) => OnLogMessage(level, message, passDuplicates);
        public void SetLogLevelFiltering(ZLogLevel level) => _levelFilter = level;

        public event EventHandler<ZLogMessageArgs> LogMessage;

        #endregion
    }
}