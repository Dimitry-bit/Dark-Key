using DarkKey.Core.Debugger;
using UnityEngine;

namespace DarkKey.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        public void StartGame() => ServiceLocator.Instance.networkSceneManager.SwitchToOnlineScene();

        public void QuitGame() => Application.Quit();

        public static void PauseGame() =>
            ServiceLocator.Instance.customDebugger.LogInfo("Game paused.", ScriptLogLevel);

        public static void SaveGame() => ServiceLocator.Instance.customDebugger.LogInfo("Game saved.", ScriptLogLevel);
    }
}