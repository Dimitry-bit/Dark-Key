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
        [SerializeField] [Scene] private string offlineScene = "OfflineScene";
        [SerializeField] [Scene] private string onlineScene = "Multiplayer_Test";
        [SerializeField] [Scene] private string loadingScene = "Loading";

        public string OfflineScene => offlineScene;
        public string OnlineScene => onlineScene;
        public string LoadingScene => loadingScene;

        #region Public Methods

        public void SwitchScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                ServiceLocator.Instance.GetDebugger()
                    .LogWarningToServer($"Scene -> {sceneName} is invalid.", ScriptLogLevel);
                return;
            }

            NetworkManager.singleton.ServerChangeScene(sceneName);
        }

        public void SwitchToOfflineScene() => SwitchScene(offlineScene);

        public void SwitchToOnlineScene() => SwitchScene(onlineScene);

        #endregion
    }
}