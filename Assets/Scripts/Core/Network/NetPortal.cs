using System.Collections.Generic;
using System.Linq;
using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using DarkKey.Gameplay.CorePlayer;
using Mirror;
using Mirror.Authenticators;
using UnityEngine.SceneManagement;

namespace DarkKey.Core.Network
{
    public class NetPortal : NetworkManager
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core, DebugLogLevel.Network};

        public static NetPortal Instance { get; private set; }
        public List<LobbyPlayer> LobbyPlayers { get; private set; }

        public override void Awake()
        {
            base.Awake();

            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);
        }

        #region Public Methods

        public void Disconnect()
        {
            if (NetworkClient.isHostClient)
                StopHost();
            else if (NetworkClient.isConnected)
                StopClient();

            SceneManager.LoadScene("OfflineScene");
            CursorManager.ShowCursor();
        }

        public void Host(string password)
        {
            GetComponent<BasicAuthenticator>().password = password;
            StartHost();
        }

        public void Join(string ipAddress, string password)
        {
            networkAddress = ipAddress;

            if (TryGetComponent(out BasicAuthenticator basicAuthenticator))
                basicAuthenticator.password = password;

            StartClient();
        }

        public void AddLobbyPlayer(LobbyPlayer lobbyPlayer)
        {
            LobbyPlayers ??= new List<LobbyPlayer>();

            LobbyPlayers.Add(lobbyPlayer);
            LobbyPlayers = LobbyPlayers.Distinct().ToList();
        }

        #endregion

        #region Private Methods

        public override void OnServerConnect(NetworkConnection conn)
        {
            ServiceLocator.Instance.GetDebugger().LogInfo($"[Client {conn.connectionId}]: connected successfully.",
                ScriptLogLevel);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            ServiceLocator.Instance.GetDebugger().LogInfo($"[Client {conn.connectionId}]: disconnected successfully.",
                ScriptLogLevel);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            ServiceLocator.Instance.GetDebugger().LogInfo($"Joined successfully.", ScriptLogLevel);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ServiceLocator.Instance.GetDebugger().LogInfo($"Disconnected successfully.", ScriptLogLevel);
        }

        public override void OnStartServer()
        {
            ServiceLocator.Instance.GetDebugger().LogInfo($"Server started successfully.", ScriptLogLevel);
        }

        public override void OnStopServer()
        {
            ServiceLocator.Instance.GetDebugger().LogInfo($"Server stopped successfully.", ScriptLogLevel);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            onlineScene = sceneName;
            networkSceneName = sceneName;
        }

        #endregion
    }
}