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

        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool isReady;

        #region Unity Methods

        public override void OnStartClient()
        {
            LobbyPlayerUiHandler = GetComponent<LobbyPlayerUiHandler>();

            AddLobbyPlayers();
            CmdNotifyInitialization();

            if (!isLocalPlayer || !hasAuthority) return;
            LobbyPlayerUiHandler.InitializeLobbyUi();
            ServiceLocator.Instance.customDebugger.LogInfo("Lobby player initialized.", ScriptLogLevel);
        }

        private void AddLobbyPlayers()
        {
            LobbyPlayer[] lobbyPlayers = FindObjectsOfType<LobbyPlayer>();

            for (int index = 0; index < lobbyPlayers.Length; index++)
            {
                NetPortal.Instance.AddLobbyPlayer(lobbyPlayers[index]);
            }
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance != null)
            {
                NetPortal.Instance.LobbyPlayers.Remove(this);
            }
        }

        #endregion

        #region Public Method

        public void StartGame() => ServiceLocator.Instance.gameManager.StartGame();

        [Command]
        public void CmdReady() => isReady = !isReady;

        public void LeaveGame()
        {
            CmdHandleLeaveLobby();
            ServiceLocator.Instance.customDebugger.LogInfo($"Client left lobby.", ScriptLogLevel);
        }

        public void QuitGame()
        {
            LeaveGame();
            ServiceLocator.Instance.gameManager.QuitGame();
        }

        #endregion

        #region Private Methods

        [Command]
        private void CmdNotifyInitialization() => InitializeInstance();

        [ClientRpc]
        private void InitializeInstance()
        {
            NetPortal.Instance.AddLobbyPlayer(this);
            NetworkClient.localPlayer.GetComponent<LobbyPlayer>().LobbyPlayerUiHandler.CmdUpdateLobbyUI();
            PlayerData = new PlayerData(netIdentity.connectionToClient.connectionId, string.Empty, string.Empty);
        }

        private void HandleReadyStatusChanged(bool previousValue, bool newValue)
        {
            if (!hasAuthority) return;
            LobbyPlayerUiHandler.CmdUpdateLobbyUI();
        }

        [Command]
        private void CmdHandleLeaveLobby()
        {
            HandleLeaveLobbyClientRpc();

            // TODO: Need to be moved in HandleLeaveLobbyClientRpc (Maybe IDK)
            LobbyPlayerUiHandler.CmdUpdateLobbyUI();
        }

        [ClientRpc]
        private void HandleLeaveLobbyClientRpc()
        {
            if (!hasAuthority)
            {
                NetworkClient.localPlayer.GetComponent<LobbyPlayer>().LobbyPlayerUiHandler.CmdUpdateLobbyUI();
            }
            else
            {
                NetPortal.Instance.LobbyPlayers.Remove(this);

                if (!isLocalPlayer) return;
                NetPortal.Instance.Disconnect();
            }
        }

        #endregion
    }
}