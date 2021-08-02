using System;
using System.Linq;
using DarkKey.Core.Network;
using DarkKey.Gameplay.CorePlayer;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Ui.UiHandlers
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
            lobbyPanel.gameObject.SetActive(hasAuthority);
            startButton.gameObject.SetActive(false);

            CmdUpdateLobbyUI();
            CmdInitStartButton();
        }

        [Command]
        public void CmdUpdateLobbyUI() => UpdateLobbyUIClientRpc();

        #endregion

        #region Private Methods

        [Command]
        private void CmdInitStartButton()
        {
            if (OwnerClientId == NetworkManager.ServerClientId)
                startButton.gameObject.SetActive(true);
        }


        [ClientRpc]
        private void UpdateLobbyUIClientRpc()
        {
            if (!hasAuthority)
            {
                LobbyPlayer ownedLobbyPlayer = GetOwnedLobbyPlayer();
                ownedLobbyPlayer.LobbyPlayerUiHandler.CmdUpdateLobbyUI();
            }
            else
            {
                ResetLobbyUI();
                AssignPlayersToUI();
                UpdateStartButtonStatus();
            }
        }

        private LobbyPlayer GetOwnedLobbyPlayer() =>
            NetPortal.Instance.LobbyPlayers.FirstOrDefault(lobbyPlayer => lobbyPlayer.hasAuthoirty);

        private void AssignPlayersToUI()
        {
            for (int i = 0; i < NetPortal.Instance.LobbyPlayers.Count; i++)
            {
                playerNames[i].text = NetPortal.Instance.LobbyPlayers[i].OwnerClientId == NetworkManager.ServerClientId
                    ? "P_1"
                    : "P_2";
                playerNames[i].color = Color.black;

                readyStatus[i].text = NetPortal.Instance.LobbyPlayers[i].isReady.Value ? "Ready" : "Not Ready";
                readyStatus[i].color = NetPortal.Instance.LobbyPlayers[i].isReady.Value ? Color.green : Color.red;
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
            if (!isServer) return;

            var isAllReady = true;

            foreach (var roomPlayer in NetPortal.Instance.LobbyPlayers)
            {
                if (!roomPlayer.isReady.Value)
                    isAllReady = false;
            }

            startButton.interactable = isAllReady;
        }

        #endregion
    }
}