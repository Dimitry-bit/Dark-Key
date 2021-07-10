using DarkKey.Core;
using DarkKey.Core.Network;
using DarkKey.Gameplay;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Logging;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui
{
    public class UiPauseMenu : NetworkBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private TextMeshProUGUI pauseText;

        [Header("Debug")] [SerializeField] private bool debug;

        private InputHandler _inputHandler;
        private NetPortal _netPortal;
        private bool _isHidden;

        #region Unity Methods

        private void Start()
        {
            _netPortal = FindObjectOfType<NetPortal>();
            if (_netPortal == null)
                Debug.LogError("[UiPauseMenu]: NetPortal not found. Please place NetPortal script on an object.");

            _isHidden = !mainPanel.activeSelf;

            _netPortal.OnConnection += GetInputHandlerAndAssignEvent;
        }

        private void GetInputHandlerAndAssignEvent()
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.LocalClientId,
                out NetworkClient client)) return;

            if (client.PlayerObject == null) return;
            if (!client.PlayerObject.IsSpawned) return;

            if (client.PlayerObject.gameObject.TryGetComponent(out InputHandler inputHandler))
                _inputHandler = inputHandler;

            _inputHandler.OnEscape += Menu;
        }

        private void OnDestroy()
        {
            if (_netPortal != null)
                _netPortal.OnConnection -= GetInputHandlerAndAssignEvent;

            if (_inputHandler != null)
                _inputHandler.OnEscape -= Menu;
        }

        #endregion

        #region Public Methods

        public void Resume()
        {
            DisableMenu();
        }

        public void Disconnect()
        {
            DisableMenu();
            _netPortal.Disconnect();
        }

        public void Quit()
        {
            DisableMenu();
            _netPortal.Disconnect();
            GameManager.QuitGame();
        }

        #endregion

        #region Private Methods

        private void Menu()
        {
            if (_isHidden)
            {
                EnableMenu();
            }
            else
            {
                DisableMenu();
            }
        }

        private void EnableMenu()
        {
            mainPanel.SetActive(true);
            _isHidden = false;
            CursorManager.ShowCursor();
            _inputHandler.actionMap = InputHandler.InputActionMap.Ui;
            Log("EnableMenu Executed");
        }

        private void DisableMenu()
        {
            mainPanel.SetActive(false);
            _isHidden = true;
            CursorManager.HideCursor();
            _inputHandler.actionMap = InputHandler.InputActionMap.Gameplay;
            Log("Disable Executed");
        }

        private void Log(string msg)
        {
            if (!debug) return;
            if (NetworkManager.IsConnectedClient)
                NetworkLog.LogInfoServer($"[UiStartMenu]: {msg}");
            else
                Debug.Log($"[UiPauseMenu]: {msg}");
        }

        #endregion
    }
}