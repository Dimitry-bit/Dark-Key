using System.Linq;
using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using DarkKey.Core.Network;
using DarkKey.Ui.UiHandlers;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.CorePlayer
{
    [RequireComponent(typeof(LobbyPlayerUiHandler))]
    public class LobbyPlayer : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Network, DebugLogLevel.Player};

        public LobbyPlayerUiHandler LobbyPlayerUiHandler { get; private set; }
        public PlayerData PlayerData { get; private set; }

        // [HideInInspector]
        // public NetworkVariableBool isReady = new NetworkVariableBool(
        //     new NetworkVariableSettings
        //     {
        //         ReadPermission = NetworkVariablePermission.Everyone,
        //         WritePermission = NetworkVariablePermission.OwnerOnly
        //     });

        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool isReady;

        #region Unity Methods

        public void NetworkStart()
        {
            LobbyPlayerUiHandler = GetComponent<LobbyPlayerUiHandler>();

            // TODO: Doing Initialization this way will only sync between the server and the recent client only. (Needs Rework)
            NetPortal.Instance.OnAnyConnection += InitInstance;
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance != null)
            {
                NetPortal.Instance.OnAnyConnection -= InitInstance;
                NetPortal.Instance.LobbyPlayers.Remove(this);
            }
        }

        #endregion

        #region Public Methods

        public void StartGame() => GameManager.Instance.StartGame();

        public void Ready()
        {
            if (!isLocalPlayer) return;
            isReady= !isReady;
        }

        public void LeaveGame()
        {
            CmdHandleLeaveLobby(OwnerClientId);
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

            if (!isLocalPlayer || !hasAuthority) return;

            LobbyPlayerUiHandler.InitializeLobbyUi();

            CustomDebugger.LogInfo("Lobby player initialized.", ScriptLogLevel);
        }

        private void HandleReadyStatusChanged(bool previousValue, bool newValue)
        {
            if (!hasAuthority) return;
            LobbyPlayerUiHandler.CmdUpdateLobbyUI();
        }

        [Command]
        private void CmdHandleLeaveLobby(ulong clientId)
        {
            HandleLeaveLobbyClientRpc(clientId);

            // TODO: Need to be moved in HandleLeaveLobbyClientRpc (Maybe IDK)
            LobbyPlayerUiHandler.CmdUpdateLobbyUI();
        }

        [ClientRpc]
        private void HandleLeaveLobbyClientRpc(ulong clientId)
        {
            if (OwnerClientId != clientId)
            {
                LobbyPlayer ownedLobbyPlayer = GetOwnedLobbyPlayer(clientId);
                ownedLobbyPlayer.CmdHandleLeaveLobby(clientId);
            }
            else
            {
                NetPortal.Instance.LobbyPlayers.Remove(this);

                if (!isLocalPlayer) return;
                NetPortal.Instance.Disconnect();
            }
        }

        // TODO: Fix UpdateLobbyUi is broken
        private LobbyPlayer GetOwnedLobbyPlayer(ulong clientId) =>
            NetPortal.Instance.LobbyPlayers.FirstOrDefault(lobbyPlayer => clientId == lobbyPlayer.OwnerClientId);

        #endregion
    }
}