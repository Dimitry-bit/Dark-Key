using System;
using System.Linq;
using DarkKey.Core.Network;
using MLAPI;
using MLAPI.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Core
{
    public class LobbyPlayerUiHandler : NetworkBehaviour
    {
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private TMP_Text[] playerNames = new TMP_Text[2];
        [SerializeField] private TMP_Text[] readyStatus = new TMP_Text[2];
        [SerializeField] private Button startButton;

        #region Public Methods

        public void InitializeLobbyUi()
        {
            lobbyPanel.gameObject.SetActive(IsOwner);
            startButton.gameObject.SetActive(false);

            UpdateLobbyUIServerRpc();
            InitStartButtonServerRpc();
        }

        [ServerRpc]
        public void UpdateLobbyUIServerRpc() => UpdateLobbyUIClientRpc();

        #endregion

        #region Private Methods

        [ServerRpc]
        private void InitStartButtonServerRpc()
        {
            if (OwnerClientId == NetworkManager.ServerClientId)
                startButton.gameObject.SetActive(true);
        }


        [ClientRpc]
        private void UpdateLobbyUIClientRpc()
        {
            if (!IsOwner)
            {
                LobbyPlayer ownedLobbyPlayer = GetOwnedLobbyPlayer();
                ownedLobbyPlayer.LobbyPlayerUiHandler.UpdateLobbyUIServerRpc();
            }
            else
            {
                ResetLobbyUI();
                AssignPlayersToUI();
                UpdateStartButtonStatus();
            }
        }

        private LobbyPlayer GetOwnedLobbyPlayer() =>
            NetPortal.Instance.lobbyPlayers.FirstOrDefault(lobbyPlayer => lobbyPlayer.IsOwner);

        private void AssignPlayersToUI()
        {
            for (int i = 0; i < NetPortal.Instance.lobbyPlayers.Count; i++)
            {
                playerNames[i].text = NetPortal.Instance.lobbyPlayers[i].OwnerClientId == NetworkManager.ServerClientId
                    ? "P_1"
                    : "P_2";
                playerNames[i].color = Color.black;

                readyStatus[i].text = NetPortal.Instance.lobbyPlayers[i].isReady.Value ? "Ready" : "Not Ready";
                readyStatus[i].color = NetPortal.Instance.lobbyPlayers[i].isReady.Value ? Color.green : Color.red;
            }
        }

        private void ResetLobbyUI()
        {
            for (int i = 0; i < playerNames.Length; i++)
            {
                playerNames[i].text = "Waiting For Player";
                playerNames[i].color = Color.white;

                readyStatus[i].text = String.Empty;
            }
        }

        private void UpdateStartButtonStatus()
        {
            if (!IsHost) return;

            var isAllReady = true;

            foreach (var roomPlayer in NetPortal.Instance.lobbyPlayers)
            {
                if (!roomPlayer.isReady.Value)
                    isAllReady = false;
            }

            startButton.interactable = isAllReady;
        }

        #endregion
    }
}