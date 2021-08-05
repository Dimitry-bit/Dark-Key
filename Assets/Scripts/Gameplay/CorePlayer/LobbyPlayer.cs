using System;
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

        public PlayerData PlayerData { get; private set; }

        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool isReady;

        public event Action OnLobbyUpdate;

        #region Unity Methods

        public override void OnStartClient()
        {
            if (!hasAuthority || !isLocalPlayer) return;

            AddLobbyPlayers();
            CmdInitializationNotify();
            OnLobbyUpdate?.Invoke();

            ServiceLocator.Instance.customDebugger.LogInfo("Lobby player initialized.", ScriptLogLevel);
        }

        private void OnDestroy() => RemoveInstanceFromListAndUpdate();

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
        private void CmdInitializationNotify() => InitializeInstance();

        [ClientRpc]
        private void InitializeInstance()
        {
            NetPortal.Instance.AddLobbyPlayer(this);
            NetworkClient.localPlayer.GetComponent<LobbyPlayer>().OnLobbyUpdate?.Invoke();

            // PlayerData = new PlayerData(netIdentity.connectionToClient.connectionId, string.Empty, string.Empty);
        }

        [Command]
        private void CmdHandleLeaveLobby() => HandleLeaveLobbyClientRpc();

        [ClientRpc]
        private void HandleLeaveLobbyClientRpc()
        {
            RemoveInstanceFromListAndUpdate();

            if (isLocalPlayer)
                NetPortal.Instance.Disconnect();
        }


        private void AddLobbyPlayers()
        {
            LobbyPlayer[] lobbyPlayers = FindObjectsOfType<LobbyPlayer>();

            foreach (var lobbyPlayer in lobbyPlayers)
                NetPortal.Instance.AddLobbyPlayer(lobbyPlayer);
        }

        private void RemoveInstanceFromListAndUpdate()
        {
            if (NetPortal.Instance.LobbyPlayers.Contains(this))
                NetPortal.Instance.LobbyPlayers.Remove(this);

            NetworkIdentity localPlayer = NetworkClient.localPlayer;
            if (localPlayer == null) return;
            
            if (localPlayer.TryGetComponent(out LobbyPlayer lobbyPlayer))
                lobbyPlayer.OnLobbyUpdate?.Invoke();
        }

        private void HandleReadyStatusChanged(bool previousValue, bool newValue) =>
            NetworkClient.localPlayer.GetComponent<LobbyPlayer>().OnLobbyUpdate?.Invoke();

        #endregion
    }
}