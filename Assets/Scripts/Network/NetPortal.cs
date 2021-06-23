using System;
using System.Text;
using MLAPI;
using MLAPI.Logging;
using MLAPI.Transports.UNET;
using UnityEngine;

namespace DarkKey.Network
{
    public class NetPortal : NetworkBehaviour
    {
        private string passwordText;

        public event Action OnDisconnection;
        public event Action OnConnection;

        [Header("Debug Options")] [SerializeField]
        private bool debug;

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

            // EnableMenuItems
            OnDisconnection?.Invoke();
        }

        public void Host(string ipAddress, string password)
        {
            passwordText = password;

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

        private void ApprovalCheck(byte[] connectionData, ulong clientId,
            NetworkManager.ConnectionApprovedDelegate callback)
        {
            string password = Encoding.ASCII.GetString(connectionData);
            bool isApproved = password == passwordText;

            Log($"{isApproved}");

            // if (!isApproved)
            // {
            //     // TODO: Call UiStartMenu.ErrorLog;
            // }

            callback(true, null, isApproved, null, null);
        }

        private void HandleClientDisconnect(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                // EnableMenuItems
                OnDisconnection?.Invoke();
            }
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                // DisableMenuItems
                OnConnection?.Invoke();
            }
        }

        private void HandleServerStarted()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                // DisableMenuItems
                OnConnection?.Invoke();
            }
        }

        private void Log(string msg)
        {
            if (!debug) return;
            NetworkLog.LogInfoServer($"[GameNM]: {msg}");
        }
    }
}