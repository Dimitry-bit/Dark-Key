using DarkKey.Core.Managers;
using DarkKey.Gameplay;
using MLAPI;
using MLAPI.Connection;
using UnityEngine;

namespace DarkKey.Ui.Debug_Panels
{
    public class DebugBar : NetworkBehaviour
    {
        [SerializeField] private GameObject debugBar;
        private InputHandler _inputHandler;
        private bool _isPanelEnabled;

        private void Update()
        {
            if (_inputHandler == null)
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                    ShowMenu();
            }
        }

        private void ShowMenu()
        {
            if (_isPanelEnabled)
                DisablePanel();
            else
                EnablePanel();
        }

        private void EnablePanel()
        {
            if (_inputHandler == null)
                GetInputHandlerAndAssignEvent();
            else
                _inputHandler.actionMap = InputHandler.InputActionMap.Ui;

            CursorManager.ShowCursor();
            debugBar.SetActive(true);
            _isPanelEnabled = true;
        }

        private void DisablePanel()
        {
            if (_inputHandler == null)
                GetInputHandlerAndAssignEvent();
            else
                _inputHandler.actionMap = InputHandler.InputActionMap.Gameplay;


            CursorManager.HideCursor();
            debugBar.SetActive(false);
            _isPanelEnabled = false;
        }

        private void GetInputHandlerAndAssignEvent()
        {
            if (NetworkManager.Singleton == null) return;

            foreach (var networkClient in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(networkClient.ClientId,
                    out NetworkClient client)) continue;

                if (client.PlayerObject == null) continue;
                if (!client.PlayerObject.IsLocalPlayer) continue;
                if (!client.PlayerObject.IsSpawned) continue;
                if (client.PlayerObject.gameObject.TryGetComponent(out InputHandler inputHandler))
                    _inputHandler = inputHandler;
            }

            _inputHandler.OnConsole += ShowMenu;
        }
    }
}