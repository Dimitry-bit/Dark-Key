using System;
using DarkKey.Core.Debugger;
using DarkKey.Gameplay.CorePlayer;
using Mirror;
using Mirror.Authenticators;
using UnityEngine;

namespace DarkKey.Core.Network
{
    public class NetPortal : NetworkManager
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core, DebugLogLevel.Network};

        [Header("Debug")]
        public DebugLogLevel logLevel;

        [Header("Connection Data")]
        private string _passwordText;

        // public List<LobbyPlayer> LobbyPlayers { get; private set; }

        public event Action OnAnyConnection;
        public event Action OnLocalConnection;
        public event Action OnAnyDisconnection;
        public event Action OnLocalDisconnection;
        public event Action<PlayerData> OnSceneSwitch;

        #region Public Methods

        public void Disconnect()
        {
            // if (NetworkManager.Singleton.IsHost)
            // {
            //     NetworkSceneManager.SwitchScene(offlineScene);
            //
            //     NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            //     NetworkManager.Singleton.StopHost();
            // }
            // else if (NetworkManager.Singleton.IsClient)
            // {
            //     NetworkManager.Singleton.StopClient();
            //     SceneManager.LoadScene(offlineScene);
            // }
            // else if (NetworkManager.Singleton.IsServer)
            // {
            //     NetworkSceneManager.SwitchScene(offlineScene);
            //     NetworkManager.Singleton.StopServer();
            // }

            OnLocalDisconnection?.Invoke();
        }

        public void Host(string password)
        {
            _passwordText = password;
            StartHost();
        }

        public void Join(string ipAddress, string password)
        {
            if (TryGetComponent(out BasicAuthenticator basicAuthenticator))
                basicAuthenticator.password = password;

            networkAddress = ipAddress;

            StartClient();

            // Invoke(nameof(CheckServerAvailability), 1f);
        }

        // public void AddLobbyPlayer(LobbyPlayer lobbyPlayer)
        // {
        //     if (LobbyPlayers == null)
        //         LobbyPlayers = new List<LobbyPlayer>();
        //
        //     LobbyPlayers.Add(lobbyPlayer);
        //     LobbyPlayers = LobbyPlayers.Distinct().ToList();
        // }

        #endregion

        #region Private Methods

        // private void HandleClientDisconnect(ulong clientId)
        // {
        //     OnAnyDisconnection?.Invoke();
        //
        //     if (clientId != NetworkManager.Singleton.LocalClientId) return;
        //
        //     Disconnect();
        //
        //     CustomDebugger.LogInfo($"[Client] : ({clientId}) disconnected successfully", ScriptLogLevel);
        //     OnLocalDisconnection?.Invoke();
        // }
        //
        // private void HandleClientConnected(ulong clientId)
        // {
        //     OnAnyConnection?.Invoke();
        //
        //     if (clientId != NetworkManager.Singleton.LocalClientId) return;
        //
        //     CustomDebugger.LogInfo($"[Client] : ({clientId}) connected successfully", ScriptLogLevel);
        //     OnLocalConnection?.Invoke();
        // }
        //
        // private void HandleServerStarted()
        // {
        //     OnAnyConnection?.Invoke();
        //
        //     if (!NetworkManager.Singleton.IsHost) return;
        //
        //     CustomDebugger.LogInfo("hosting started successfully", ScriptLogLevel);
        //     OnLocalConnection?.Invoke();
        // }
        //
        // private void HandleSceneSwitched()
        // {
        //     if (!NetworkManager.Singleton.IsHost) return;
        //
        //     CustomDebugger.LogInfo("Switched Scene", ScriptLogLevel);
        //
        //     if (SceneManager.GetActiveScene().name != "Multiplayer_Test") return;
        //
        //     foreach (var roomPlayer in LobbyPlayers)
        //     {
        //         CustomDebugger.LogInfo("Player1 Switched Scene", ScriptLogLevel);
        //         OnSceneSwitch?.Invoke(roomPlayer.PlayerData);
        //     }
        // }
        //
        // private void CheckServerAvailability()
        // {
        //     if (NetworkManager.Singleton.IsConnectedClient) return;
        //     if (!NetworkManager.Singleton.IsClient) return;
        //
        //     NetworkManager.Singleton.StopClient();
        //     OnLocalDisconnection?.Invoke();
        // }

        #endregion
    }
}