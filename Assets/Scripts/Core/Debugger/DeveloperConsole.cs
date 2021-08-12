using DarkKey.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Core.Debugger
{
    public class DeveloperConsole : MonoBehaviour
    {
        [SerializeField] private TMP_Text consoleText;
        [SerializeField] private ScrollRect scrollRect;
        private bool _isPanelEnabled;
        private InputHandler _inputHandler;

        #region Unity Methods

        private void Awake()
        {
            Application.logMessageReceived += HandleLogReceived;
            DontDestroyOnLoad(gameObject);

            consoleText.text = "";
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private void OnDestroy() => Application.logMessageReceived -= HandleLogReceived;

        #endregion

        #region Private Methods

        private void HandleLogReceived(string logMessage, string stacktrace, LogType type)
        {
            var message = $"[{type.ToString()}]: {logMessage}";

            string textColor = type switch
            {
                LogType.Log => "white",
                LogType.Warning => "yellow",
                LogType.Error => "red",
                LogType.Assert => "red",
                LogType.Exception => "red",
                _ => "white"
            };

            consoleText.text += $"<color={textColor}> {message} </color>" + "\n";
            scrollRect.verticalNormalizedPosition = 0f;
        }

        #endregion
    }
}