using System;
using System.Text;
using DarkKey.Core;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Guide
{
    public class NetworkingGuide : NetworkBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string onlineScene = "Multiplayer_Test";
        [SerializeField] private string offlineScene = "OfflineScene";
        
        [Header("Custom Settings")]
        [SerializeField] private bool useBuiltinGui = true;

        private string _passwordText = "";
        private static NetworkingGuide _instance;

        public static NetworkingGuide Instance
        {
            get
            {
                if (_instance == null) CustomDebugger.Instance.LogError("NetPortal", "NetPortal instance is null");
                return _instance;
            }
        }

        /// The callback to invoke once a client connects. This callback is only ran on the server and on the local client that connects.
        public event Action OnAnyConnection;
        /// The callback to invoke once a client connects. This callback is only ran on the local client that connects.
        public event Action OnLocalConnection;

        /// The callback to invoke once a client connects. This callback is only ran on the server and on the local client that connects.
        public event Action OnAnyDisconnection;
        /// The callback to invoke once a client connects. This callback is only ran on the local client that connects.
        public event Action OnLocalDisconnection;
        
        public event Action OnSceneLoaded;

        [Header("Gui Settings")]
        [SerializeField] private string ipAddress;

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

        private void OnGUI()
        {
            if (!useBuiltinGui) return;
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
            {
                InputFields();
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
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

        #endregion

        #region Private Methods

        private void InputFields()
        {
            GUILayout.Label("IP Address");
            ipAddress = GUILayout.TextField(ipAddress);
            GUILayout.Label("Password ");
            _passwordText = GUILayout.TextField(_passwordText);
            ipAddress = string.IsNullOrEmpty(ipAddress) ? "127.0.0.1" : ipAddress;
        }

        private void StartButtons()
        {
            if (GUILayout.Button("Host")) Host(ipAddress, _passwordText);
            if (GUILayout.Button("Join")) Join(ipAddress, _passwordText);
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        private void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ? "Host" :
                NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
            GUILayout.Label("IP: " + ipAddress);
            GUILayout.Label("Password: " + _passwordText);

            var disconnectButtonName = NetworkManager.Singleton.IsHost ? "Stop Hosting" :
                NetworkManager.Singleton.IsServer ? "Stop Server" : "Disconnect";

            if (GUILayout.Button(disconnectButtonName)) Disconnect();
        }

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
            if (!NetworkManager.Singleton.IsHost) return;

            CustomDebugger.Instance.LogInfo("NetPortal", $"hosting started successfully");
            OnLocalConnection?.Invoke();
        }

        #endregion
    }
}