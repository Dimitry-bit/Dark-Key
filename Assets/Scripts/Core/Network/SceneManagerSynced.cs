using DarkKey.Core.Debugger;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Core.Network
{
    [RequireComponent(typeof(NetworkSceneManagerDk))]
    public class SceneManagerSynced : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [SyncVar] private bool _isAllReady;

        #region Unity Methods

        public void Start()
        {
            NetPortal.Instance.OnServerSceneChangeStart += SwitchToLoadingScreen;
            NetPortal.Instance.OnClientSceneChangeStart += SwitchToLoadingScreen;

            NetPortal.Instance.OnServerBeforeSceneActive += SceneOperationHandler;
            NetPortal.Instance.OnClientBeforeSceneActive += SceneOperationHandler;
        }


        public void OnDestroy()
        {
            NetPortal.Instance.OnServerSceneChangeStart -= SwitchToLoadingScreen;
            NetPortal.Instance.OnClientSceneChangeStart -= SwitchToLoadingScreen;

            NetPortal.Instance.OnServerBeforeSceneActive -= SceneOperationHandler;
            NetPortal.Instance.OnClientBeforeSceneActive -= SceneOperationHandler;
        }

        #endregion

        #region Private Methods

        private void SwitchToLoadingScreen()
        {
            var loadingScene = GetComponent<NetworkSceneManagerDk>().LoadingScene;
            SceneManager.LoadSceneAsync(loadingScene);
        }

        private void SceneOperationHandler()
        {
            if (NetworkManager.loadingSceneAsync == null) return;

            NetworkManager.loadingSceneAsync.allowSceneActivation = true;

            // ServiceLocator.Instance.GetDebugger().LogInfo("Finished loading scene.", ScriptLogLevel);
            Debug.Log("Finished loading scene.");

            NetPortal.Instance.ResetSceneOperation();
        }

        #endregion
    }
}