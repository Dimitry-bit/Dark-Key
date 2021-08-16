using DarkKey.Core.Debugger;

namespace DarkKey.Core
{
    public interface IDebugger
    {
        public void LogInfoToServer(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "");

        public void LogWarningToServer(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "");

        public void LogErrorToServer(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "");

        public void LogCriticalError(object msg,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "");
    }
}