using System;
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

            NetPortal.Instance.OnConnection += InitInstance;
            NetPortal.Instance.OnConnection += UpdateReadyStatus;
            NetPortal.Instance.OnDisconnection += RemoveInstanceFromRoomPlayers;
            isReady.OnValueChanged += HandleReadyStatusChanged;
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance != null)
            {
                NetPortal.Instance.OnConnection -= InitInstance;
                NetPortal.Instance.OnConnection -= UpdateReadyStatus;
                NetPortal.Instance.OnDisconnection -= RemoveInstanceFromRoomPlayers;
            }

            isReady.OnValueChanged -= HandleReadyStatusChanged;
        }

        #endregion

        #region Public Methods

        public void Ready()
        {
            if (!IsLocalPlayer) return;
            isReady.Value = !isReady.Value;
        }

        public void LeaveGame() => NetPortal.Instance.Disconnect();

        public void QuitGame() => GameManager.QuitGame();

        #endregion

        #region Private Methods

        private void HandleReadyStatusChanged(bool previousValue, bool newValue) => UpdateReadyStatus();

        private void UpdateReadyStatus()
        {
            if (!IsOwner)
            {
                foreach (var client in NetPortal.Instance.roomPlayer)
                {
                    if (client.IsOwner)
                    {
                        client.UpdateReadyStatus();
                        break;
                    }
                }

                return;
            }

            for (int i = 0; i < playerNames.Length; i++)
            {
                playerNames[i].text = "Waiting For Player";
                readyStatus[i].text = String.Empty;
            }

            for (int i = 0; i < NetPortal.Instance.roomPlayer.Count; i++)
            {
                playerNames[i].text = NetPortal.Instance.roomPlayer[i].IsHost ? "P_1" : "P_2";

                readyStatus[i].text = NetPortal.Instance.roomPlayer[i].isReady.Value ? "Ready" : "Not Ready";
                readyStatus[i].color = NetPortal.Instance.roomPlayer[i].isReady.Value ? Color.green : Color.red;
            }
        }

        private void InitInstance()
        {
            NetPortal.Instance.AddLobbyPlayer(this);

            if (!IsLocalPlayer) return;
            lobbyPanel.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);

            if (!IsHost) return;
            startButton.gameObject.SetActive(true);
        }

        private void RemoveInstanceFromRoomPlayers()
        {
            if (!IsLocalPlayer) return;
            
            NetPortal.Instance.roomPlayer.Remove(this);
        }

        #endregion
    }
}