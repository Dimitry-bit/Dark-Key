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
        private NetworkClient _localClient;

        #region Unity Methods

        public override void NetworkStart()
        {
            _netPortal = FindObjectOfType<NetPortal>();
            if (_netPortal == null)
                Debug.LogError("[UiPauseMenu]: NetPortal not found. Please place NetPortal script on an object.");

            _isHidden = !mainPanel.activeSelf;

            // Get LocalClient GameObject
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.LocalClientId,
                out NetworkClient client))
            {
                _localClient = client;
                if (client.PlayerObject == null)
                {
                    // Waits 5sec To make sure player has spawned.
                    Invoke(nameof(GetInputHandler), 5);
                }
                else
                {
                    GetInputHandler();
                }
            }
        }

        private void GetInputHandler()
        {
            if (_localClient == null) return;
            if (_localClient.PlayerObject == null) return;
            if (!_localClient.PlayerObject.IsSpawned) return;

            if (_localClient.PlayerObject.gameObject.TryGetComponent(out InputHandler inputHandler))
                _inputHandler = inputHandler;

            _inputHandler.OnEscape += Menu;
        }

        private void OnDestroy()
        {
            if (_netPortal == null) return;
            if (_inputHandler == null) return;

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
            _inputHandler.disabledInput = DisableInput.All;
            Log("EnableMenu Executed");
        }

        private void DisableMenu()
        {
            mainPanel.SetActive(false);
            _isHidden = true;
            CursorManager.HideCursor();
            _inputHandler.disabledInput = DisableInput.None;
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