using DarkKey.Core;
using DarkKey.Core.Network;
using DarkKey.Gameplay;
using MLAPI;
using MLAPI.Connection;
using UnityEngine;

namespace DarkKey.Ui
{
    public class UiPauseMenu : NetworkBehaviour
    {
        [SerializeField] private GameObject mainPanel;

        private InputHandler _inputHandler;
        private bool _isHidden;

        #region Unity Methods

        private void Start()
        {
            _isHidden = !mainPanel.activeSelf;
            NetPortal.Instance.OnLocalConnection += GetInputHandlerAndAssignEvent;
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
            if (NetPortal.Instance != null)
                NetPortal.Instance.OnLocalConnection -= GetInputHandlerAndAssignEvent;

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
            NetPortal.Instance.Disconnect();
        }

        public void Quit()
        {
            DisableMenu();
            NetPortal.Instance.Disconnect();
            GameManager.QuitGame();
        }

        #endregion

        #region Private Methods

        private void Menu()
        {
            if (_isHidden)
                EnableMenu();
            else
                DisableMenu();
        }

        private void EnableMenu()
        {
            mainPanel.SetActive(true);
            _isHidden = false;
            CursorManager.ShowCursor();
            _inputHandler.actionMap = InputHandler.InputActionMap.Ui;
            CustomDebugger.Instance.LogInfo("UiPauseMenu", "EnableMenu Executed");
        }

        private void DisableMenu()
        {
            mainPanel.SetActive(false);
            _isHidden = true;
            CursorManager.HideCursor();
            _inputHandler.actionMap = InputHandler.InputActionMap.Gameplay;
            CustomDebugger.Instance.LogInfo("UiPauseMenu", "Disable Executed");
        }

        #endregion
    }
}