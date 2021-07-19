using MLAPI;
using MLAPI.Logging;
using TMPro;
using UnityEngine;

namespace DarkKey.Core
{
    public class CustomDebugger : NetworkBehaviour
    {
        [SerializeField] private bool isNetworkedDebug;
        [SerializeField] private bool outPutToCustomText;
        [SerializeField] private TMP_Text customTextField;

        public static CustomDebugger Instance { get; private set; }

        #region Unity Methods

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        #endregion

        #region Public Methods

        public void LogInfo(string scriptName, string msg)
        {
            if (outPutToCustomText)
            {
                CustomOutput(scriptName, msg, Color.white);
                return;
            }

            if (isNetworkedDebug)
            {
                if (NetworkManager == null) return;
                
                if (NetworkManager.IsConnectedClient || NetworkManager.IsServer)
                    NetworkLog.LogInfoServer($"[{scriptName}]: {msg}");
            }
            else
            {
                Debug.Log($"[{scriptName}]: {msg}");
            }
        }

        public void LogInfo(string msg)
        {
            if (outPutToCustomText)
            {
                CustomOutput(msg, Color.white);
                return;
            }

            if (isNetworkedDebug)
            {
                if (NetworkManager == null) return;
                
                if (NetworkManager.IsConnectedClient || NetworkManager.IsServer)
                    NetworkLog.LogInfoServer($"{msg}");
            }
            else
            {
                Debug.Log($"[{msg}");
            }
        }

        public void LogWarning(string scriptName, string msg)
        {
            if (outPutToCustomText)
            {
                CustomOutput(scriptName, msg, Color.yellow);
                return;
            }

            if (isNetworkedDebug)
            {
                if (NetworkManager == null) return;
                
                if (NetworkManager.IsConnectedClient || NetworkManager.IsServer)
                    NetworkLog.LogWarningServer($"[{scriptName}]: {msg}");
            }
            else
            {
                Debug.LogWarning($"[{scriptName}]: {msg}");
            }
        }

        public void LogWarning(string msg)
        {
            if (outPutToCustomText)
            {
                CustomOutput(msg, Color.yellow);
                return;
            }

            if (isNetworkedDebug)
            {
                if (NetworkManager == null) return;
                
                if (NetworkManager.IsConnectedClient || NetworkManager.IsServer)
                    NetworkLog.LogWarningServer($"{msg}");
            }
            else
            {
                Debug.LogWarning($"[{msg}");
            }
        }

        public void LogError(string scriptName, string msg)
        {
            if (outPutToCustomText)
            {
                CustomOutput(scriptName, msg, Color.red);
                return;
            }

            if (isNetworkedDebug)
            {
                if (NetworkManager == null) return;
                
                if (NetworkManager.IsConnectedClient || NetworkManager.IsServer)
                    NetworkLog.LogErrorServer($"[{scriptName}]: {msg}");
            }
            else
            {
                Debug.LogError($"[{scriptName}]: {msg}");
            }
        }

        public void LogError(string msg)
        {
            if (outPutToCustomText)
            {
                CustomOutput(msg, Color.red);
                return;
            }

            if (isNetworkedDebug)
            {
                if (NetworkManager == null) return;
                
                if (NetworkManager.IsConnectedClient || NetworkManager.IsServer)
                    NetworkLog.LogErrorServer($"{msg}");
            }
            else
            {
                Debug.LogError($"[{msg}");
            }
        }

        #endregion

        #region Private Methods

        private void CustomOutput(string scriptName, string msg, Color color)
        {
            customTextField.color = color;
            customTextField.text += $"[{scriptName}]: {msg}\n";
        }

        private void CustomOutput(string msg, Color color)
        {
            customTextField.color = color;
            customTextField.text += $"{msg}\n";
        }

        #endregion
    }
}