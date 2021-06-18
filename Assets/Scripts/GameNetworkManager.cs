using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey
{
    public class GameNetworkManager : NetworkBehaviour
    {
        [SerializeField] private InputField ipInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private GameObject startPanel;
        [SerializeField] private Text errorText;
        
        private Coroutine _errorMessageCoroutine;

        [Header("Debug Options")]
        [SerializeField] private bool debug;

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

        private void Leave()
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
            
            startPanel.SetActive(true);
        }

        public void StartHost()
        {
            if (ipInputField.text == String.Empty) ipInputField.text = "127.0.0.1";
            
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ipInputField.text;
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            // TODO: Refactor regex.
            Regex ipRegex = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");

            if (ipInputField.text == String.Empty) ipInputField.text = "127.0.0.1";
            if (!ipRegex.IsMatch(ipInputField.text))
            {
                if (_errorMessageCoroutine != null) StopCoroutine(_errorMessageCoroutine);
                errorText.text = "Error : Invalid Ip Address.";
                _errorMessageCoroutine = StartCoroutine(TimedErrorMessage(5f));

                return;
            }

            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ipInputField.text;
            NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(passwordInputField.text);
            NetworkManager.Singleton.StartClient();
        }
        
        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            string password = Encoding.ASCII.GetString(connectionData);
            bool isApproved = password == passwordInputField.text;

            Log($"{isApproved}");

            // BUG: Text wont register. No Idea why...
            if (!isApproved)
            {
                if (_errorMessageCoroutine != null) StopCoroutine(_errorMessageCoroutine);
                errorText.text = "Wrong Password.";
                _errorMessageCoroutine = StartCoroutine(TimedErrorMessage(5f));
            }
            
            callback(true, null, isApproved, null, null);
        }
        
        private void HandleClientDisconnect(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId) startPanel.SetActive(true);
        }

        private void HandleClientConnected(ulong clientId)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId) startPanel.SetActive(false);
        }

        private void HandleServerStarted()
        {
            if (NetworkManager.Singleton.IsHost) startPanel.SetActive(false);
        }
        
        private void Log(string msg)
        {
            if (!debug) return;
            Debug.Log($"[GameNM]: {msg}");
        }
        
        private IEnumerator TimedErrorMessage(float waitTime)
        {
            errorText.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
            errorText.gameObject.SetActive(false);
        }
    }
}
