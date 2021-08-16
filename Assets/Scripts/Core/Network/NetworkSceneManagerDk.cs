using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using Mirror;
using UnityEngine;

namespace DarkKey.Core.Network
{
    public class NetworkSceneManagerDk : MonoBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [Header("Scene")]
        [SerializeField] [Scene]
        private string offlineScene = "OfflineScene";
        [SerializeField] [Scene]
        private string onlineScene = "Multiplayer_Test";

        #region Public Methods

        public void SwitchScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                ServiceLocator.Instance.GetDebugger().LogWarningToServer($"Scene -> {sceneName} is invalid.", ScriptLogLevel);
                return;
            }

            ServiceLocator.Instance.GetDebugger().LogInfoToServer($"Starting Scene Switch ({sceneName}).", ScriptLogLevel);
            NetPortal.Instance.ServerChangeScene(sceneName);
        }

        public void SwitchToOfflineScene() => SwitchScene(offlineScene);

        public void SwitchToOnlineScene() => SwitchScene(onlineScene);

        #endregion
    }
}