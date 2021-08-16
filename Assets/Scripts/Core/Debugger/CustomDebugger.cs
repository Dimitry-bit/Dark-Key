using System.IO;
using System.Linq;
using Mirror;
using UnityEngine;

namespace DarkKey.Core.Debugger
{
    public class CustomDebugger : NetworkBehaviour, IDebugger
    {
        [SerializeField] private DebugLogLevel currentLogLevel;

        private enum LogTypes : byte
        {
            Info,
            Warning,
            Error,
        }

        public void LogInfo(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            BaseLog(logText, LogTypes.Info);
        }

        public void LogWarning(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            BaseLog(logText, LogTypes.Warning);
        }

        public void LogError(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            BaseLog(logText, LogTypes.Error);
        }

        public void LogInfoToServer(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            BaseLogToServer(logText, LogTypes.Info);
        }

        public void LogWarningToServer(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";
            BaseLogToServer(logText, LogTypes.Warning);
        }

        public void LogErrorToServer(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            BaseLogToServer(logText, LogTypes.Error);
        }

        public void LogCriticalError(object msg,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            Debug.LogError($"[{callerFileName}.{memberName}()]: {msg}");
        }

        private bool IsAllowedToDebug(DebugLogLevel[] logLevels)
        {
            var isAllowedToDebug = logLevels.Any(logLevel => currentLogLevel.HasFlag(logLevel));
            return isAllowedToDebug;
        }

        [Command(requiresAuthority = false)]
        private void CmdLogToServer(string msg, LogTypes logType) =>
            BaseLog(msg, logType);

        private static void BaseLog(string logMessage, LogTypes logTypes)
        {
            switch (logTypes)
            {
                case LogTypes.Info:
                    Debug.Log(logMessage);
                    break;
                case LogTypes.Warning:
                    Debug.LogWarning(logMessage);
                    break;
                case LogTypes.Error:
                    Debug.LogError(logMessage);
                    break;
            }
        }

        private void BaseLogToServer(string logMessage, LogTypes logType)
        {
            BaseLog(logMessage, logType);

            if (NetworkManager.singleton != null && NetworkManager.singleton.isNetworkActive)
            {
                if (!NetworkClient.isHostClient)
                    CmdLogToServer(logMessage, LogTypes.Info);
            }
        }
    }
}