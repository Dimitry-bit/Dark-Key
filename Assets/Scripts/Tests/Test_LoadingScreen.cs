using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Tests
{
    public class Test_LoadingScreen : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("LoadingScreen");
            StartCoroutine(LoadingScreen("LoadingScreenTest"));
        }

        private IEnumerator LoadingScreen(string sceneName)
        {
            yield return new WaitForSeconds(5f);

            AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName);
            while (sceneLoading.progress < 1)
            {
                Debug.Log(sceneLoading.progress);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}