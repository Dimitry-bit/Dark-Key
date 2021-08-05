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

            if (NetworkManager.singleton.isNetworkActive)
            {
                if (NetworkClient.isHostClient)
                {
                    Debug.Log(logText);
                }
                else
                {
                    Debug.Log(logText);
                    CmdLogToServer(logText, LogTypes.Info);
                }
            }
            else
            {
                Debug.Log(logText);
            }
        }

        public void LogWarning(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            if (NetworkManager.singleton.isNetworkActive)
            {
                if (NetworkClient.isHostClient)
                {
                    Debug.LogWarning(logText);
                }
                else
                {
                    Debug.LogWarning(logText);
                    CmdLogToServer(logText, LogTypes.Warning);
                }
            }
            else
            {
                Debug.LogWarning(logText);
            }
        }

        public void LogError(object msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string logText = $"[NetId: ({netId})] [{callerFileName}.{memberName}()]: {msg}";

            if (NetworkManager.singleton.isNetworkActive)
            {
                if (NetworkClient.isHostClient)
                {
                    Debug.LogError(logText);
                }
                else
                {
                    Debug.LogError(logText);
                    CmdLogToServer(logText, LogTypes.Error);
                }
            }
            else
            {
                Debug.LogError(logText);
            }
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