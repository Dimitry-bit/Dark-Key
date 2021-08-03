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
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            Debug.Log(logText);
            if (NetworkManager.singleton.isNetworkActive)
                CmdLogToServer(logText, LogTypes.Info);
        }

        public void LogWarning(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            Debug.LogWarning(logText);
            if (NetworkManager.singleton.isNetworkActive)
                CmdLogToServer(logText, LogTypes.Warning);
        }

        public void LogError(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            Debug.LogError(logText);
            if (NetworkManager.singleton.isNetworkActive)
                CmdLogToServer(logText, LogTypes.Error);
        }

        public void LogCriticalError(object msg,
            [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath]
            string sourceFilePath = "")
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
        private void CmdLogToServer(string msg, LogTypes logType)
        {
            switch (logType)
            {
                case LogTypes.Info:
                {
                    Debug.Log(msg);
                    break;
                }
                case LogTypes.Warning:
                {
                    Debug.LogWarning(msg);
                    break;
                }
                case LogTypes.Error:
                {
                    Debug.LogError(msg);
                    break;
                }
            }
        }
    }
}