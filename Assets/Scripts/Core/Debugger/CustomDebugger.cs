using System.Linq;
using DarkKey.Core.Network;
using MLAPI;
using MLAPI.Logging;
using UnityEngine;

namespace DarkKey.Core.Debugger
{
    public static class CustomDebugger
    {
        private static DebugLogLevel[] LogLevels
        {
            get
            {
                return NetPortal.Instance == null
                    ? new DebugLogLevel[1] {DebugLogLevel.All}
                    : NetPortal.Instance.selectedLogs;
            }
        }

        public static void LogInfo(string scriptName, string msg, DebugLogLevel[] logLevels)
        {
            if (!LogLevels.Contains(DebugLogLevel.All))
            {
                var isAllowedToLog = logLevels.Any(level => LogLevels.Contains(level));
                if (!isAllowedToLog) return;
            }

            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
            {
                NetworkLog.LogInfoServer($"[{scriptName}]: {msg}");
            }
            else
            {
                Debug.Log($"[{scriptName}]: {msg}");
            }
        }

        public static void LogWarning(string scriptName, string msg, DebugLogLevel[] logLevels)
        {
            if (!LogLevels.Contains(DebugLogLevel.All))
            {
                var isAllowedToLog = logLevels.Any(level => LogLevels.Contains(level));
                if (!isAllowedToLog) return;
            }

            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
            {
                NetworkLog.LogWarningServer($"[{scriptName}]: {msg}");
            }
            else
            {
                Debug.LogWarning($"[{scriptName}]: {msg}");
            }
        }

        public static void LogError(string scriptName, string msg, DebugLogLevel[] logLevels)
        {
            if (!LogLevels.Contains(DebugLogLevel.All))
            {
                var isAllowedToLog = logLevels.Any(level => LogLevels.Contains(level));
                if (!isAllowedToLog) return;
            }

            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
            {
                NetworkLog.LogErrorServer($"[{scriptName}]: {msg}");
            }
            else
            {
                Debug.LogError($"[{scriptName}]: {msg}");
            }
        }

        public static void LogCriticalError(string scriptName, string msg)
        {
            Debug.LogError($"[{scriptName}]: {msg}");
        }
    }
}