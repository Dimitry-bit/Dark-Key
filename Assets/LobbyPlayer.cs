using DarkKey.Core;
using DarkKey.Core.Network;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey
{
    public class LobbyPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private TMP_Text[] playerNames = new TMP_Text[2];
        [SerializeField] private TMP_Text[] readyStatus = new TMP_Text[2];
        [SerializeField] private Button startButton;

        public NetworkVariableBool isReady = new NetworkVariableBool(
            new NetworkVariableSettings
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.OwnerOnly
            });

        #region Unity Methods

        public override void NetworkStart()
        {
            base.NetworkStart();

            NetPortal.Instance.OnConnection += DisableUnusedLobbyUi;
            NetPortal.Instance.OnConnection += UpdateReadyStatus;
            isReady.OnValueChanged += HandleReadyStatusChanged;
            isReady.OnValueChanged += HandleNameChanged;
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance != null)
            {
                NetPortal.Instance.OnConnection -= DisableUnusedLobbyUi;
                NetPortal.Instance.OnConnection -= UpdateReadyStatus;
            }

            isReady.OnValueChanged -= HandleReadyStatusChanged;
            isReady.OnValueChanged -= HandleNameChanged;
        }

        #endregion

        #region Public Methods

        public void StartGame()
        {
            lobbyPanel.SetActive(false);
            NetPortal.Instance.StartGame();
        }

        public void Ready()
        {
            if (!IsLocalPlayer) return;

            isReady.Value = !isReady.Value;
            UpdateReadyStatus();
        }

        public void LeaveGame() => NetPortal.Instance.Disconnect();

        public void QuitGame() => GameManager.QuitGame();

        #endregion

        #region Private Methods

        private bool IsAllReady()
        {
            foreach (var client in NetworkManager.ConnectedClientsList)
            {
                var lobbyObject = client.PlayerObject.GetComponent<LobbyPlayer>();
                if (!lobbyObject.isReady.Value) return false;
            }

            return true;
        }

        private void HandleReadyStatusChanged(bool previousValue, bool newValue) => UpdateReadyStatus();

        private void HandleNameChanged(bool previousValue, bool newValue) => UpdateName();

        private void UpdateReadyStatus()
        {
            // if (IsLocalPlayer)
            // {
            //     foreach (var client in NetworkManager.ConnectedClientsList)
            //     {
            //         var lobbyObject = client.PlayerObject.GetComponent<LobbyPlayer>();
            //         var panelId = client.PlayerObject.NetworkManager.IsHost ? 0 : 1;
            //
            //         var panelText = lobbyObject.isReady.Value ? "Ready" : "Not Ready";
            //         var color = lobbyObject.isReady.Value ? Color.green : Color.red;
            //
            //         readyStatus[panelId].text = panelText;
            //         readyStatus[panelId].color = color;
            //     }
            // }

            if (!IsHost) return;
            if (!IsAllReady()) return;

            startButton.gameObject.SetActive(true);
        }

        private void UpdateName()
        {
            foreach (var client in NetworkManager.ConnectedClientsList)
            {
                var lobbyObject = client.PlayerObject.GetComponent<LobbyPlayer>();
                var panelId = client.PlayerObject.NetworkManager.IsHost ? 0 : 1;
                var panelText = lobbyObject.NetworkManager.IsHost ? "P_1" : "P_2";

                playerNames[panelId].text = panelText;
            }
        }

        private void DisableUnusedLobbyUi()
        {
            if (!IsLocalPlayer) return;
            CustomDebugger.Instance.LogInfo("asss");
            lobbyPanel.gameObject.SetActive(true);
        }

        #endregion
    }
}