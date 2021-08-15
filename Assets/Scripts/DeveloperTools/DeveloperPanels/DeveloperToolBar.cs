using DarkKey.Core.Managers;
using DarkKey.Gameplay;
using Mirror;
using UnityEngine;

namespace DarkKey.DeveloperTools.DeveloperPanels
{
    public class DeveloperToolBar : MonoBehaviour
    {
        [SerializeField] private GameObject developerToolBar;
        private InputHandler _inputHandler;

        #region Unity Methods

        private void Update()
        {
            if (_inputHandler == null)
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                    ShowMenu();
            }
        }

        #endregion

        #region Private Methods

        private void ShowMenu()
        {
            if (_inputHandler == null)
                GetInputHandlerAndAssignEvent();

            if (developerToolBar.activeSelf)
                DisablePanel();
            else
                EnablePanel();
        }


        private void EnablePanel()
        {
            if (_inputHandler != null)
                _inputHandler.actionMap = InputHandler.InputActionMap.Ui;

            CursorManager.ShowCursor();
            developerToolBar.SetActive(true);
        }

        private void DisablePanel()
        {
            if (_inputHandler != null)
                _inputHandler.actionMap = InputHandler.InputActionMap.Gameplay;

            CursorManager.HideCursor();
            developerToolBar.SetActive(false);
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

        #endregion
    }
}