using System.Collections;
using System.Text.RegularExpressions;
using DarkKey.Network;
using MLAPI;
using MLAPI.Logging;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui
{
    public class UiStartMenu : NetworkBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private TMP_InputField ipInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TextMeshProUGUI errorText;

        [Header("Optional")] [SerializeField] [Tooltip("If left empty it will grab Camera.Main")]
        private Camera lobbyCam;

        [Header("Debug")] [SerializeField] private bool debug;
        
        private Coroutine _errorMessageCoroutine;
        private NetPortal _netPortal;

        private void Start()
        {
            _netPortal = FindObjectOfType<NetPortal>();
            if (_netPortal == null)
                Log("NetPortal not found. Please place NetPortal script on an object.");
            if (lobbyCam == null) lobbyCam = Camera.main;

            _netPortal.OnConnection += DisableMenu;
            _netPortal.OnDisconnection += EnableMenu;
        }

        private void OnDestroy()
        {
            if (_netPortal == null) return;

            _netPortal.OnConnection -= DisableMenu;
            _netPortal.OnDisconnection -= EnableMenu;
        }

        public void Host()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            _netPortal.Host(ipInputField.text, passwordInputField.text);
        }

        public void Join()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            _netPortal.Join(ipInputField.text, passwordInputField.text);
        }

        public void Quit()
        {
            DisableMenu();
            _netPortal.Disconnect();
            GameManager.QuitGame();
        }

        private void EnableMenu()
        {
            lobbyCam.gameObject.SetActive(true);
            mainPanel.SetActive(true);
            CursorManager.ShowCursor();
            Log("EnableMenu Executed");
        }

        private void DisableMenu()
        {
            lobbyCam.gameObject.SetActive(false);
            mainPanel.SetActive(false);
            CursorManager.HideCursor();
            Log("DisableMenu Executed");
        }

        private bool IsValidIp()
        {
            // TODO: Refactor regex.
            var ipRegex = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");

            if (ipRegex.IsMatch(ipInputField.text)) return true;

            if (_errorMessageCoroutine != null) StopCoroutine(_errorMessageCoroutine);
            errorText.text = "Error : Invalid Ip Address.";
            _errorMessageCoroutine = StartCoroutine(TimedErrorMessage(5f));
            return false;
        }

        private IEnumerator TimedErrorMessage(float waitTime)
        {
            errorText.gameObject.SetActive(true);
            yield return new WaitForSeconds(waitTime);
            errorText.gameObject.SetActive(false);
        }

        private void Log(string msg)
        {
           if (!debug) return;
           if (NetworkManager.IsConnectedClient)
            NetworkLog.LogInfoServer($"[UiStartMenu]: {msg}");
           else
               Debug.Log($"[UiPauseMenu]: {msg}");
        }
    }
}