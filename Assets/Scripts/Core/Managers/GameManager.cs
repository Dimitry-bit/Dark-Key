using DarkKey.Core.Debugger;
using MLAPI.SceneManagement;
using UnityEngine;

namespace DarkKey.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [Header("Scene Settings")]
        [SerializeField] private string onlineScene = "Multiplayer_Test";

        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null) CustomDebugger.LogError("Instance is null", ScriptLogLevel);
                return _instance;
            }
        }

        #region Unity Methods

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if (_instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        #endregion

        public void StartGame() => NetworkSceneManager.SwitchScene(onlineScene);

        public static void QuitGame() => Application.Quit();

        public static void PauseGame() => CustomDebugger.LogInfo("Game paused.", ScriptLogLevel);

        public static void SaveGame() => CustomDebugger.LogInfo("Game saved.", ScriptLogLevel);
    }
}