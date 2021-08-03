using DarkKey.Core.Debugger;

namespace DarkKey.Core
{
    public interface IDebugger
    {
        public void LogInfo(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "");

        public void LogWarning(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "");

        public void LogError(object msg, DebugLogLevel[] logLevels,
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