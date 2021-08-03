using DarkKey.Core.Debugger;
using UnityEngine;

namespace DarkKey.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        // public void StartGame() => NetworkSceneManager.SwitchScene(onlineScene);

        public static void QuitGame() => Application.Quit();

        public static void PauseGame() =>
            ServiceLocator.Instance.cutomeDebugger.LogInfo("Game paused.", ScriptLogLevel);

        public static void SaveGame() => ServiceLocator.Instance.cutomeDebugger.LogInfo("Game saved.", ScriptLogLevel);
    }
}