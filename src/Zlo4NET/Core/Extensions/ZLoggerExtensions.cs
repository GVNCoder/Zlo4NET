using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;

namespace Zlo4NET.Core.Extensions
{
    internal static class ZLoggerExtensions
    {
        public static void Debug(this ZLoggerImpl loggerImpl, string message, bool passDuplicates = false)
        {
            loggerImpl.Log(ZLoggingLevel.Debug, message, passDuplicates);
        }

        public static void Info(this ZLoggerImpl loggerImpl, string message, bool passDuplicates = false)
        {
            loggerImpl.Log(ZLoggingLevel.Info, message, passDuplicates);
        }

        public static void Warning(this ZLoggerImpl loggerImpl, string message, bool passDuplicates = false)
        {
            loggerImpl.Log(ZLoggingLevel.Warning, message, passDuplicates);
        }

        public static void Error(this ZLoggerImpl loggerImpl, string message, bool passDuplicates = false)
        {
            loggerImpl.Log(ZLoggingLevel.Error, message, passDuplicates);
        }
    }
}