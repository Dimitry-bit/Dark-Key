using System;
using System.Collections.Generic;
using System.Text;
using MLAPI;
using MLAPI.Transports.UNET;

namespace DarkKey.Core.Network
{
    public class NetPortal : NetworkBehaviour
    {
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

        public List<LobbyPlayer> roomPlayer = new List<LobbyPlayer>();

        public event Action OnDisconnection;
        public event Action OnConnection;
        public event Action OnSceneLoaded;

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
                NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
                NetworkManager.Singleton.StopHost();
            }
            else if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.StopClient();
            }

            OnDisconnection?.Invoke();
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
            
            foreach (var player in roomPlayer) playerID.Add(player.OwnerClientId);

            if (playerID.Contains(lobbyPlayer.OwnerClientId)) return;
            
            roomPlayer.Add(lobbyPlayer);
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
            OnDisconnection?.Invoke();

            if (clientId != NetworkManager.Singleton.LocalClientId) return;
            CustomDebugger.Instance.LogInfo("NetPortal", $"[Client] : ({clientId}) disconnected successfully");
        }

        private void HandleClientConnected(ulong clientId)
        {
            OnConnection?.Invoke();

            if (clientId != NetworkManager.Singleton.LocalClientId) return;
            CustomDebugger.Instance.LogInfo("NetPortal", $"[Client] : ({clientId}) connected successfully");
        }

        private void HandleServerStarted()
        {
            if (!NetworkManager.Singleton.IsHost) return;

            CustomDebugger.Instance.LogInfo("NetPortal", $"hosting started successfully");
            OnConnection?.Invoke();
        }

        #endregion
    }
}