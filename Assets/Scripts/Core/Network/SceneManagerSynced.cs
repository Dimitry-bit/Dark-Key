using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Core.Network
{
    [RequireComponent(typeof(NetworkSceneManagerDk))]
    public class SceneManagerSynced : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.SceneManagement};

        [SyncVar] private bool _isAllReady;

        #region Unity Methods

        public override void OnStartClient()
        {
            NetPortal.Instance.OnServerSceneChangeStart += SwitchToLoadingScreen;
            NetPortal.Instance.OnClientSceneChangeStart += SwitchToLoadingScreen;

            NetPortal.Instance.OnServerBeforeSceneActive += SceneOperationHandler;
            NetPortal.Instance.OnClientBeforeSceneActive += SceneOperationHandler;
            
            ServiceLocator.Instance.GetDebugger().LogInfo("Registered scene switch events.", ScriptLogLevel);
        }

        public void OnDestroy()
        {
            NetPortal.Instance.OnServerSceneChangeStart -= SwitchToLoadingScreen;
            NetPortal.Instance.OnClientSceneChangeStart -= SwitchToLoadingScreen;

            NetPortal.Instance.OnServerBeforeSceneActive -= SceneOperationHandler;
            NetPortal.Instance.OnClientBeforeSceneActive -= SceneOperationHandler;
            
            ServiceLocator.Instance.GetDebugger().LogInfo("UnRegistered scene switch events.", ScriptLogLevel);
        }

        #endregion

        #region Private Methods

        private void SwitchToLoadingScreen()
        {
            ServiceLocator.Instance.GetDebugger().LogInfo("Started LoadingScreen.", ScriptLogLevel);
            var loadingScene = GetComponent<NetworkSceneManagerDk>().LoadingScene;
            SceneManager.LoadSceneAsync(loadingScene);
            ServiceLocator.Instance.GetDebugger().LogInfo("Finished LoadingScreen.", ScriptLogLevel);
        }

        private void SceneOperationHandler()
        {
            if (NetworkManager.loadingSceneAsync == null) return;

            NetworkManager.loadingSceneAsync.allowSceneActivation = true;
            NetPortal.Instance.ResetSceneOperation();

            ServiceLocator.Instance.GetDebugger().LogInfo("Finished loading scene.", ScriptLogLevel);
        }

        #endregion
    }
}