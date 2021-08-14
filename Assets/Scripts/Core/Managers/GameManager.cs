using DarkKey.Core.Debugger;
using UnityEngine;

namespace DarkKey.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        public void StartGame() => ServiceLocator.Instance.GetNetworkSceneManager().SwitchToOnlineScene();

        public void QuitGame() => Application.Quit();

        public static void PauseGame() =>
            ServiceLocator.Instance.GetDebugger().LogInfo("Game paused.", ScriptLogLevel);

        public static void SaveGame() => ServiceLocator.Instance.GetDebugger().LogInfo("Game saved.", ScriptLogLevel);
    }
}