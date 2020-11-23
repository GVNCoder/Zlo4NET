using System;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;

namespace Zlo4NET.Core.Data
{
    internal class ZLogger : IZLogger
    {
        #region Singleton

        // https://csharpindepth.com/articles/singleton
        static ZLogger() { }

        private static readonly Lazy<ZLogger> __lazyInstance = new Lazy<ZLogger>(() => new ZLogger(), true);

        public static ZLogger Instance => __lazyInstance.Value;

        #endregion

        private ZLogLevel _levelFilter = ZLogLevel.Warning | ZLogLevel.Error;
        public event EventHandler<ZLogMessageArgs> OnMessage;

        public void SetMessageFilter(ZLogLevel level)
        {
            _levelFilter = level;
        }

        private void OnLogMessage(ZLogLevel level, string message)
        {
            if (_levelFilter.HasFlag(level)) OnMessage?.Invoke(this, new ZLogMessageArgs(message));
        }

        internal void Debug(string message) => OnLogMessage(ZLogLevel.Debug, message);
        internal void Info(string message) => OnLogMessage(ZLogLevel.Info, message);
        internal void Warning(string message) => OnLogMessage(ZLogLevel.Warning, message);
        internal void Error(string message) => OnLogMessage(ZLogLevel.Error, message);
    }
}