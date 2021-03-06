using System.Linq;
using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using DarkKey.Core.Network;
using DarkKey.Ui.UiHandlers;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace DarkKey.Gameplay.CorePlayer
{
    [RequireComponent(typeof(LobbyPlayerUiHandler))]
    public class LobbyPlayer : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Network, DebugLogLevel.Player};

        public LobbyPlayerUiHandler LobbyPlayerUiHandler { get; private set; }
        public PlayerData PlayerData { get; private set; }

        [HideInInspector]
        public NetworkVariableBool isReady = new NetworkVariableBool(
            new NetworkVariableSettings
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.OwnerOnly
            });

        #region Unity Methods

        public override void NetworkStart()
        {
            LobbyPlayerUiHandler = GetComponent<LobbyPlayerUiHandler>();

            // TODO: Doing Initialization this way will only sync between the server and the recent client only. (Needs Rework)
            NetPortal.Instance.OnAnyConnection += InitInstance;

            isReady.OnValueChanged += HandleReadyStatusChanged;
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance != null)
            {
                NetPortal.Instance.OnAnyConnection -= InitInstance;
                NetPortal.Instance.LobbyPlayers.Remove(this);
            }

            isReady.OnValueChanged -= HandleReadyStatusChanged;
        }

        #endregion

        #region Public Methods

        public void StartGame() => GameManager.Instance.StartGame();

        public void Ready()
        {
            if (!IsLocalPlayer) return;
            isReady.Value = !isReady.Value;
        }

        public void LeaveGame()
        {
            HandleLeaveLobbyServerRpc(OwnerClientId);
            CustomDebugger.LogInfo($"Client({OwnerClientId}) left lobby.", ScriptLogLevel);
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
            PlayerData = new PlayerData(OwnerClientId, string.Empty, string.Empty);

            if (!IsLocalPlayer || !IsOwner) return;

            LobbyPlayerUiHandler.InitializeLobbyUi();

            CustomDebugger.LogInfo("Lobby player initialized.", ScriptLogLevel);
        }

        private void HandleReadyStatusChanged(bool previousValue, bool newValue)
        {
            if (!IsOwner) return;
            LobbyPlayerUiHandler.UpdateLobbyUIServerRpc();
        }

        [ServerRpc]
        private void HandleLeaveLobbyServerRpc(ulong clientId)
        {
            HandleLeaveLobbyClientRpc(clientId);

            // TODO: Need to be moved in HandleLeaveLobbyClientRpc (Maybe IDK)
            LobbyPlayerUiHandler.UpdateLobbyUIServerRpc();
        }

        [ClientRpc]
        private void HandleLeaveLobbyClientRpc(ulong clientId)
        {
            if (OwnerClientId != clientId)
            {
                LobbyPlayer ownedLobbyPlayer = GetOwnedLobbyPlayer(clientId);
                ownedLobbyPlayer.HandleLeaveLobbyServerRpc(clientId);
            }
            else
            {
                NetPortal.Instance.LobbyPlayers.Remove(this);

                if (!IsLocalPlayer) return;
                NetPortal.Instance.Disconnect();
            }
        }

        // TODO: Fix UpdateLobbyUi is broken
        private LobbyPlayer GetOwnedLobbyPlayer(ulong clientId) =>
            NetPortal.Instance.LobbyPlayers.FirstOrDefault(lobbyPlayer => clientId == lobbyPlayer.OwnerClientId);

        #endregion
    }
}