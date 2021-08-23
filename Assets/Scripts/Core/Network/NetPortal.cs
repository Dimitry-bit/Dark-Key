using System;
using System.Collections.Generic;
using System.Linq;
using DarkKey.Core.Managers;
using DarkKey.Gameplay.CorePlayer;
using Mirror;
using Mirror.Authenticators;
using UnityEngine;

namespace DarkKey.Core.Network
{
    public class NetPortal : NetworkManager
    {
        [Header("Custom Settings")]
        [SerializeField] private bool useAuthentication;
        public bool isCustomHandlingSceneSwitch;
        private BasicAuthenticator _authenticator;

        public event Action<string> OnSceneChangeStart;
        public event Action OnSceneActivateHalt;

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
                InitializeAuthenticator();
            }
        }


        public override void LateUpdate()
        {
            HandleSceneUpdateRequest();
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
            if (NetworkClient.isHostClient) // Stop host if host mode.
            {
                StopHost();
            }
            else if (NetworkClient.isConnected) // Stop client if client-only.
            {
                StopClient();
            }

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

        private void InitializeAuthenticator()
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

        public void UpdateScene()
        {
            if (loadingSceneAsync != null && loadingSceneAsync.isDone)
            {
                try
                {
                    FinishLoadScene();
                }
                finally
                {
                    loadingSceneAsync.allowSceneActivation = true;
                    loadingSceneAsync = null;
                }
            }
        }

        private void HandleSceneUpdateRequest()
        {
            if (loadingSceneAsync == null) return;

            // NOTE(Dimitry): Checking isNetworkActive to fix a loadingScreen issue. Where NetworkManager doesn't Persist To Offline Scene.
            // This creates collisions between NetworkManagers which have to be resolved manually. 
            if (isCustomHandlingSceneSwitch && isNetworkActive)
            {
                loadingSceneAsync.allowSceneActivation = false;

                if (OnSceneActivateHalt == null)
                {
                    Debug.LogWarning(
                        "(IsCustomHandlingSceneSwitch) is enable, But OnSceneActivateHalt event isn't hooked. (May result in scene not switching)");
                }

                OnSceneActivateHalt?.Invoke();
            }
            else
            {
                UpdateScene();
            }
        }

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

        public override void OnServerChangeScene(string newSceneName)
        {
            OnSceneChangeStart?.Invoke(newSceneName);
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation,
            bool customHandling)
        {
            if (NetworkClient.isHostClient) return;

            OnSceneChangeStart?.Invoke(newSceneName);
        }

        #endregion
    }
}