using System;
using DarkKey.Core.Debugger;
using DarkKey.Core.Network;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Core
{
    public class LobbyPlayer : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Network, DebugLogLevel.Player};

        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private TMP_Text[] playerNames = new TMP_Text[2];
        [SerializeField] private TMP_Text[] readyStatus = new TMP_Text[2];
        [SerializeField] private Button startButton;

        [HideInInspector]
        public NetworkVariableBool isReady = new NetworkVariableBool(
            new NetworkVariableSettings
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.OwnerOnly
            });

        private GameObject _gameObject;

        #region Unity Methods

        public override void NetworkStart()
        {
            base.NetworkStart();

            // TODO: Doing Initialization this way will only sync between the server and the recent client only. (Needs Rework)
            NetPortal.Instance.OnAnyConnection += InitInstance;

            isReady.OnValueChanged += HandleReadyStatusChanged;
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance != null)
                NetPortal.Instance.OnAnyConnection -= InitInstance;

            isReady.OnValueChanged -= HandleReadyStatusChanged;
        }

        #endregion

        #region Public Methods

        public void Ready()
        {
            if (!IsLocalPlayer) return;
            isReady.Value = !isReady.Value;
        }

        public void LeaveGame()
        {
            CustomDebugger.LogInfo("LobbyPlayer", $"Client({OwnerClientId}) left lobby.", ScriptLogLevel);
            HandleLeaveLobbyServerRpc(OwnerClientId);
        }

        public void QuitGame()
        {
            LeaveGame();
            GameManager.QuitGame();
        }

        #endregion

        #region Private Methods

        private void InitInstance()
        {
            NetPortal.Instance.AddLobbyPlayer(this);

            if (!IsLocalPlayer) return;
            if (!IsOwner) return;

            lobbyPanel.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);

            UpdateLobbyUIServerRpc();
            InitStartButtonServerRpc();

            CustomDebugger.LogInfo("LobbyPlayer", "Lobby player initialized.", ScriptLogLevel);
        }

        [ServerRpc]
        private void InitStartButtonServerRpc()
        {
            if (OwnerClientId == NetworkManager.ServerClientId)
                startButton.gameObject.SetActive(true);
        }

        [ServerRpc]
        private void UpdateLobbyUIServerRpc()
        {
            UpdateLobbyUIClientRpc();
        }

        [ClientRpc]
        private void UpdateLobbyUIClientRpc()
        {
            if (!IsOwner)
            {
                foreach (var client in NetPortal.Instance.roomPlayers)
                {
                    if (client.IsOwner)
                    {
                        client.UpdateLobbyUIServerRpc();
                        break;
                    }
                }

                return;
            }

            ResetLobbyUI();
            AssignPlayersToUI();
        }

        private void AssignPlayersToUI()
        {
            for (int i = 0; i < NetPortal.Instance.roomPlayers.Count; i++)
            {
                playerNames[i].text = NetPortal.Instance.roomPlayers[i].OwnerClientId == NetworkManager.ServerClientId
                    ? "P_1"
                    : "P_2";
                playerNames[i].color = Color.black;

                readyStatus[i].text = NetPortal.Instance.roomPlayers[i].isReady.Value ? "Ready" : "Not Ready";
                readyStatus[i].color = NetPortal.Instance.roomPlayers[i].isReady.Value ? Color.green : Color.red;
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

        private void HandleReadyStatusChanged(bool previousValue, bool newValue)
        {
            if (!IsOwner) return;
            UpdateLobbyUIServerRpc();
        }

        [ServerRpc]
        private void HandleLeaveLobbyServerRpc(ulong clientId)
        {
            HandleLeaveLobbyClientRpc(clientId);
            UpdateLobbyUIServerRpc();
        }

        [ClientRpc]
        private void HandleLeaveLobbyClientRpc(ulong clientId)
        {
            if (OwnerClientId != clientId)
            {
                foreach (var client in NetPortal.Instance.roomPlayers)
                {
                    if (client.OwnerClientId == clientId)
                    {
                        client.HandleLeaveLobbyServerRpc(clientId);
                        break;
                    }
                }

                return;
            }

            NetPortal.Instance.roomPlayers.Remove(this);

            if (!IsLocalPlayer) return;
            NetPortal.Instance.Disconnect();
        }

        #endregion
    }
}