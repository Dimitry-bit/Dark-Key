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

        private bool _hasInitialized;
        private bool _isLoadingLoadingScreen;
        private Coroutine _loadingScreenCoroutine;

        #region Unity Methods

        private void Start() => RegisterEvents();
        public void OnDestroy() => UnRegisterEvents();

        public override void OnStartClient() => RegisterEvents();
        public override void OnStopClient() => UnRegisterEvents();

        #endregion

        #region Private Methods

        private void RegisterEvents()
        {
            if (_hasInitialized) return;

            NetPortal.Instance.OnSceneChangeStart += SwitchToLoadingScreen;
            NetPortal.Instance.OnSceneActivateHalt += SceneOperationHandler;
            _hasInitialized = true;

            ServiceLocator.Instance.GetDebugger().LogInfo("Registered scene switch events.", ScriptLogLevel);
        }

        private void UnRegisterEvents()
        {
            if (_hasInitialized) return;

            NetPortal.Instance.OnSceneChangeStart -= SwitchToLoadingScreen;
            NetPortal.Instance.OnSceneActivateHalt -= SceneOperationHandler;
            _hasInitialized = false;

            ServiceLocator.Instance.GetDebugger().LogInfo("Unregistered scene switch events.", ScriptLogLevel);
        }

        private void SwitchToLoadingScreen(string newScene)
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