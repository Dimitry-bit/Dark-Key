using DarkKey.Core.Managers;
using DarkKey.Gameplay;
using Mirror;
using UnityEngine;

namespace DarkKey.Ui.Debug_Panels
{
    public class DebugBar : MonoBehaviour
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

            if (_inputHandler != null)
                _inputHandler.actionMap = InputHandler.InputActionMap.Ui;

            CursorManager.ShowCursor();
            debugBar.SetActive(true);
            _isPanelEnabled = true;
        }

        private void DisablePanel()
        {
            if (_inputHandler == null)
                GetInputHandlerAndAssignEvent();

            if (_inputHandler != null)
                _inputHandler.actionMap = InputHandler.InputActionMap.Gameplay;

            CursorManager.HideCursor();
            debugBar.SetActive(false);
            _isPanelEnabled = false;
        }

        private void GetInputHandlerAndAssignEvent()
        {
            if (NetworkManager.singleton == null || !NetworkManager.singleton.isNetworkActive) return;

            NetworkIdentity localPlayer = NetworkClient.localPlayer;
            if (localPlayer != null && localPlayer.TryGetComponent(out InputHandler inputHandler))
            {
                _inputHandler = inputHandler;
                _inputHandler.OnConsole += ShowMenu;
            }
        }
    }
}