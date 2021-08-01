using System.IO;
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

        public static void LogInfo(string msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);

            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
                NetworkLog.LogInfoServer($"[{callerFileName}.{memberName}()]: {msg}");
            else
                Debug.Log($"[{callerFileName}.{memberName}()]: {msg}");
        }

        public static void LogWarning(string msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);

            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
                NetworkLog.LogWarningServer($"[{callerFileName}.{memberName}()]: {msg}");
            else
                Debug.LogWarning($"[{callerFileName}.{memberName}()]: {msg}");
        }

        public static void LogError(string msg, DebugLogLevel[] logLevels,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            if (!IsAllowedToDebug(logLevels)) return;

            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);

            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
                NetworkLog.LogErrorServer($"[{callerFileName}.{memberName}()]: {msg}");
            else
                Debug.LogError($"[{callerFileName}.{memberName}()]: {msg}");
        }

        public static void LogCriticalError(string msg,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            string callerFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            Debug.LogError($"[{callerFileName}.{memberName}()]: {msg}");
        }

        private static bool IsAllowedToDebug(DebugLogLevel[] logLevels)
        {
            var isAllowedToDebug = logLevels.Any(logLevel => LogLevels.HasFlag(logLevel));
            return isAllowedToDebug;
        }
    }
}