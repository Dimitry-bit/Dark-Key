using DarkKey.Core.Managers;
using DarkKey.Core.Network;
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

        public override void NetworkStart()
        {
            // TODO Get Player InputHandler
            if (!IsLocalPlayer) return;
            NetPortal.Instance.OnLocalConnection += GetInputHandlerAndAssignEvent;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                ShowMenu();
        }

        private void ShowMenu()
        {
            if (_isPanelEnabled)
            {
                CursorManager.HideCursor();
                debugBar.SetActive(false);
                _isPanelEnabled = false;
            }
            else
            {
                CursorManager.ShowCursor();
                debugBar.SetActive(true);
                _isPanelEnabled = true;
            }
        }

        private void GetInputHandlerAndAssignEvent()
        {
            Debug.Log("ass");
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.LocalClientId,
                out NetworkClient client)) return;

            if (client.PlayerObject == null) return;
            if (!client.PlayerObject.IsSpawned) return;

            if (client.PlayerObject.gameObject.TryGetComponent(out InputHandler inputHandler))
                _inputHandler = inputHandler;

            _inputHandler.OnConsole += ShowMenu;
        }
    }
}