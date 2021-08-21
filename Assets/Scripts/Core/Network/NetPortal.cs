using System.Collections.Generic;
using System.Linq;
using DarkKey.Core.Managers;
using DarkKey.Gameplay.CorePlayer;
using Mirror;
using Mirror.Authenticators;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Core.Network
{
    public class NetPortal : NetworkManager
    {
        [Header("Custom Settings")]
        [SerializeField] private bool useAuthentication;
        private BasicAuthenticator _authenticator;

        public static NetPortal Instance { get; private set; }
        public List<LobbyPlayer> LobbyPlayers { get; private set; }

        public override void Awake()
        {
            base.Awake();

            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        public override void Start()
        {
            base.Start();

            if (useAuthentication)
            {
                if (!TryGetComponent(out BasicAuthenticator networkAuthenticator))
                {
                    networkAuthenticator = gameObject.AddComponent<BasicAuthenticator>();
                }

                if (authenticator == null)
                {
                    authenticator = networkAuthenticator;
                }

                _authenticator = networkAuthenticator;
            }
        }

        #region Public Methods

        public void Host(string password)
        {
            if (NetworkClient.active) return;

            if (useAuthentication)
            {
                _authenticator.password = password;
            }

            StartHost();
        }

        public void Join(string ipAddress, string password)
        {
            if (NetworkClient.active) return;

            if (useAuthentication)
            {
                _authenticator.password = password;
            }

            networkAddress = ipAddress;

            StartClient();
        }

        public void Disconnect()
        {
            if (NetworkClient.isHostClient) // stop host if host mode
            {
                StopHost();
            }
            else if (NetworkClient.isConnected) // stop client if client-only
            {
                StopClient();
            }

            SceneManager.LoadScene(offlineScene);
            CursorManager.ShowCursor();
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
            Debug.Log($"[Client {conn.connectionId}]: connected successfully.");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            Debug.Log($"[Client {conn.connectionId}]: disconnected successfully.");
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            Debug.Log("Joined successfully.");
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            Debug.Log("Disconnected successfully.");
        }

        public override void OnStartServer()
        {
            Debug.Log("Server started successfully.");
        }

        public override void OnStopServer()
        {
            Debug.Log("Server stopped successfully.");
        }

        #endregion
    }
}