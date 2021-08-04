using System;
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
        private LobbyPlayer _lobbyPlayer;

        #region Unity Methods

        public override void OnStartClient()
        {
            if (!hasAuthority || !isLocalPlayer) return;
            
            _lobbyPlayer = GetComponentInParent<LobbyPlayer>();

            if (_lobbyPlayer != null)
                _lobbyPlayer.OnLobbyUpdate += UpdateLobbyUI;

            InitializeLobbyUi();
        }

        public override void OnStopClient()
        {
            if (_lobbyPlayer != null)
                _lobbyPlayer.OnLobbyUpdate -= UpdateLobbyUI;
        }

        private void OnDestroy()
        {
            if (_lobbyPlayer != null)
                _lobbyPlayer.OnLobbyUpdate -= UpdateLobbyUI;
        }

        #endregion

        #region Public Methods

        private void InitializeLobbyUi()
        {
            lobbyPanel.gameObject.SetActive(hasAuthority);
            startButton.gameObject.SetActive(false);

            CmdInitStartButton();
        }

        private void UpdateLobbyUI()
        {
            ResetLobbyUI();
            AssignPlayersToUI();
            UpdateStartButtonStatus();
        }

        #endregion

        #region Private Methods

        [Command]
        private void CmdInitStartButton()
        {
            if (NetworkClient.isHostClient)
                startButton.gameObject.SetActive(true);
        }

        private void AssignPlayersToUI()
        {
            for (int i = 0; i < NetPortal.Instance.LobbyPlayers.Count; i++)
            {
                playerNames[i].text = "P_" + i;
                playerNames[i].color = Color.black;

                readyStatus[i].text = NetPortal.Instance.LobbyPlayers[i].isReady ? "Ready" : "Not Ready";
                readyStatus[i].color = NetPortal.Instance.LobbyPlayers[i].isReady ? Color.green : Color.red;
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
                if (!roomPlayer.isReady)
                    isAllReady = false;
            }

            startButton.interactable = isAllReady;
        }

        #endregion
    }
}