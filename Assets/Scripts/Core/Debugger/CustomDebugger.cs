using System.Linq;
using DarkKey.Core.Network;
using MLAPI;
using MLAPI.Logging;
using UnityEngine;

namespace DarkKey.Core.Debugger
{
    public static class CustomDebugger
    {
        private static DebugLogLevel LogLevels =>
            NetPortal.Instance == null
                ? DebugLogLevel.Core | DebugLogLevel.Network | DebugLogLevel.Player
                : NetPortal.Instance.logLevel;

        public static void LogInfo(string scriptName, string msg, DebugLogLevel[] logLevels)
        {
            var isAllowedToDebug = logLevels.Any(logLevel => LogLevels.HasFlag(logLevel));
            if (!isAllowedToDebug) return;

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
            var isAllowedToDebug = logLevels.Any(logLevel => LogLevels.HasFlag(logLevel));
            if (!isAllowedToDebug) return;

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
            var isAllowedToDebug = logLevels.Any(logLevel => LogLevels.HasFlag(logLevel));
            if (!isAllowedToDebug) return;

            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
            {
                NetworkLog.LogErrorServer($"[{scriptName}]: {msg}");
            }
            else
            {
                Debug.LogError($"[{scriptName}]: {msg}");
            }
        }

        public static void LogCriticalError(string scriptName, string msg) => Debug.LogError($"[{scriptName}]: {msg}");
    }
}