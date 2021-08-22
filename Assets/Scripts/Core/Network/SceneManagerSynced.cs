using System.Collections;
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

        private bool _isLoadingLoadingScreen;
        private Coroutine _loadingScreenCoroutine;

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

            ServiceLocator.Instance.GetDebugger().LogInfo("Unregistered scene switch events.", ScriptLogLevel);
        }

        #endregion

        #region Private Methods

        private void SwitchToLoadingScreen()
        {
            // NOTE(Dimitry): To prevent multiple LoadingScreens loading at once.
            if (_isLoadingLoadingScreen) return;

            ServiceLocator.Instance.GetDebugger().LogInfo("Started loading LoadingScreen.", ScriptLogLevel);

            var loadingScene = GetComponent<NetworkSceneManagerDk>().LoadingScene;
            var loadingOperation = SceneManager.LoadSceneAsync(loadingScene);

            if (_loadingScreenCoroutine != null)
            {
                StopCoroutine(_loadingScreenCoroutine);
            }
            else
            {
                _loadingScreenCoroutine = StartCoroutine(LoadingScreenHandler(loadingOperation));
            }
        }

        private IEnumerator LoadingScreenHandler(AsyncOperation loadingOperation)
        {
            while (loadingOperation != null && !loadingOperation.isDone)
            {
                _isLoadingLoadingScreen = true;
                yield return null;
            }

            _isLoadingLoadingScreen = false;
            ServiceLocator.Instance.GetDebugger().LogInfo("Finished loading LoadingScreen.", ScriptLogLevel);
        }

        private void SceneOperationHandler()
        {
            if (NetworkManager.loadingSceneAsync == null) return;

            NetworkManager.loadingSceneAsync.allowSceneActivation = true;

            if (NetworkManager.loadingSceneAsync.isDone)
            {
                NetPortal.Instance.UpdateScene();
                ServiceLocator.Instance.GetDebugger().LogInfo("Finished loading scene.", ScriptLogLevel);
            }
        }

        #endregion
    }
}