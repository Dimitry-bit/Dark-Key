using System.Collections;
using System.Text.RegularExpressions;
using DarkKey.Network;
using MLAPI;
using MLAPI.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Ui
{
    public class UiStartMenu : NetworkBehaviour
    {
        private NetPortal _netPortal;

        [SerializeField] private GameObject mainPanel;
        [SerializeField] private InputField ipInputField;
        [SerializeField] private InputField passwordInputField;
        [SerializeField] private Button leaveButton;
        [SerializeField] private Text errorText;
        
        [Header("Optional")]
        [SerializeField] [Tooltip("If left empty it will grab Camera.Main")] private Camera lobbyCam;

        private Coroutine _errorMessageCoroutine;

        private void Start()
        {
            _netPortal = FindObjectOfType<NetPortal>();
            if (_netPortal == null) Debug.LogError("[UiStartMenu]: NetPortal not found. Please place NetPortal script on an object.");
            if (lobbyCam == null) lobbyCam = Camera.main;
            
            _netPortal.OnConnection += () => ToggleMenuItems(false);
            _netPortal.OnDisconnection += () => ToggleMenuItems(true);
        }

        private void OnDestroy()
        {
            if (_netPortal == null) return;
            
            _netPortal.OnConnection -= () => ToggleMenuItems(false);
            _netPortal.OnDisconnection -= () => ToggleMenuItems(true);
        }

        public void HostButton()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            _netPortal.Host(ipInputField.text, passwordInputField.text);
        }

        public void JoinButton()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            _netPortal.Join(ipInputField.text, passwordInputField.text);
        }

        public void LeaveButton() => _netPortal.Leave();

        private void ToggleMenuItems(bool state)
        {
            lobbyCam.gameObject.SetActive(state);
            mainPanel.SetActive(state);
            leaveButton.gameObject.SetActive(!state);
            NetworkLog.LogInfoServer("ToggleMenuItems is called");
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
    }
}