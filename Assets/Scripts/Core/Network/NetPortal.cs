using System;
using System.Collections.Generic;
using System.Text;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Core.Network
{
    public class NetPortal : NetworkBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string onlineScene = "Multiplayer_Test";
        [SerializeField] private string offlineScene = "OfflineScene";

        private string _passwordText;
        private static NetPortal _instance;

        public static NetPortal Instance
        {
            get
            {
                if (_instance == null) CustomDebugger.Instance.LogError("NetPortal", "NetPortal instance is null");
                return _instance;
            }
        }

        public List<LobbyPlayer> roomPlayers = new List<LobbyPlayer>();

        /// The callback to invoke once a client connects. This callback is only ran on the server and on the local client that connects.
        public event Action OnAnyConnection;
        /// The callback to invoke once a client connects. This callback is only ran on the local client that connects.
        public event Action OnLocalConnection;

        /// The callback to invoke once a client disconnects. This callback is only ran on the server and on the local client that connects.
        public event Action OnAnyDisconnection;
        /// The callback to invoke once a client disconnects. This callback is only ran on the local client that connects.
        public event Action OnLocalDisconnection;

        #region Unity Methods

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else if (_instance != this)
                Destroy(gameObject);
        }

        private void Start()
        {
            NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) return;

            NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }

        #endregion

        #region Public Methods

        public void Disconnect()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkSceneManager.SwitchScene(offlineScene);

                NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
                NetworkManager.Singleton.StopHost();
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StopClient();
                SceneManager.LoadScene(offlineScene);
            }
            else if (NetworkManager.Singleton.IsServer)
            {
                NetworkSceneManager.SwitchScene(offlineScene);

                NetworkManager.Singleton.StopServer();
            }

            OnLocalDisconnection?.Invoke();
        }

        public void Host(string ipAddress, string password)
        {
            _passwordText = password;

            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ipAddress;
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.StartHost();
        }

        public void Join(string ipAddress, string password)
        {
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ipAddress;
            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(password);
            NetworkManager.Singleton.StartClient();
        }

        public void AddLobbyPlayer(LobbyPlayer lobbyPlayer)
        {
            var playerID = new List<ulong>();

            foreach (var player in roomPlayers) playerID.Add(player.OwnerClientId);

            if (playerID.Contains(lobbyPlayer.OwnerClientId)) return;

            roomPlayers.Add(lobbyPlayer);
        }

        #endregion

        #region Private Methods

        private void ApprovalCheck(byte[] connectionData, ulong clientId,
            NetworkManager.ConnectionApprovedDelegate callback)
        {
            string password = Encoding.ASCII.GetString(connectionData);
            bool isApproved = password == _passwordText;

            CustomDebugger.Instance.LogInfo($"{isApproved}");

            callback(true, null, isApproved, null, null);
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            OnAnyDisconnection?.Invoke();

            if (clientId != NetworkManager.Singleton.LocalClientId) return;
            
            Disconnect();

            CustomDebugger.Instance.LogInfo("NetPortal", $"[Client] : ({clientId}) disconnected successfully");
            OnLocalDisconnection?.Invoke();
        }

        private void HandleClientConnected(ulong clientId)
        {
            OnAnyConnection?.Invoke();

            if (clientId != NetworkManager.Singleton.LocalClientId) return;

            CustomDebugger.Instance.LogInfo("NetPortal", $"[Client] : ({clientId}) connected successfully");
            OnLocalConnection?.Invoke();
        }

        private void HandleServerStarted()
        {
            OnAnyConnection?.Invoke();

            if (!NetworkManager.Singleton.IsHost) return;

            CustomDebugger.Instance.LogInfo("NetPortal", $"hosting started successfully");
            OnLocalConnection?.Invoke();
        }

        #endregion
    }
}