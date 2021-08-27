using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Core.Managers
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] [Scene] private string bootScene;
        [SerializeField] [Scene] private string developerToolsScene;
        [SerializeField] private bool isDeveloper = true;

        private static BootLoader _instance;
        private string _currentScenePath;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _currentScenePath = SceneManager.GetActiveScene().path;
            StartCoroutine(Boot());
        }

        private IEnumerator Boot()
        {
            InitializeAScene(bootScene, out AsyncOperation serviceOperation);

            while (serviceOperation != null && !serviceOperation.isDone)
            {
                yield return null;
            }

            if (isDeveloper)
            {
                InitializeAScene(developerToolsScene, out AsyncOperation developerOperation);

                while (developerOperation != null && !developerOperation.isDone)
                {
                    yield return null;
                }
            }

            SceneManager.LoadSceneAsync(_currentScenePath);
        }

        private void InitializeAScene(string scenePath, out AsyncOperation loadingAsyncOperation)
        {
            if (SceneManager.GetActiveScene().path == scenePath || string.IsNullOrEmpty(scenePath))
            {
                loadingAsyncOperation = null;
                return;
            }

            loadingAsyncOperation = SceneManager.LoadSceneAsync(scenePath);
            StartCoroutine(LoadingOperationHandler(scenePath, loadingAsyncOperation));
        }

        private IEnumerator LoadingOperationHandler(string scenePath, AsyncOperation asyncOperation)
        {
            if (asyncOperation == null)
            {
                Debug.LogError($"{scenePath} failed to boot, check inspector references.");
                yield return null;
            }

            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            Debug.Log($"{scenePath} booted successfully.");
        }
    }
}