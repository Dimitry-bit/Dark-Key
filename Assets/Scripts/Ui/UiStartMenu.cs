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
        [SerializeField] private NetPortal netPortal;

        [SerializeField] private InputField ipInputField;
        [SerializeField] private InputField passwordInputField;

        [SerializeField] private GameObject startPanel;
        [SerializeField] private Text errorText;
        [SerializeField] private Button leaveBtn;
        [SerializeField] private Camera lobbyCam;

        private Coroutine _errorMessageCoroutine;

        private void Start()
        {
            netPortal.OnConnection += () => ToggleMenuItems(false);
            netPortal.OnDisconnection += () => ToggleMenuItems(true);
        }

        private void OnDestroy()
        {
            if (netPortal == null) return;
            
            netPortal.OnConnection -= () => ToggleMenuItems(false);
            netPortal.OnDisconnection -= () => ToggleMenuItems(true);
        }

        public void HostButton()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            netPortal.Host(ipInputField.text, passwordInputField.text);
        }

        public void JoinButton()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            netPortal.Join(ipInputField.text, passwordInputField.text);
        }

        public void LeaveButton() => netPortal.Leave();

        private void ToggleMenuItems(bool state)
        {
            lobbyCam.gameObject.SetActive(state);
            startPanel.SetActive(state);
            leaveBtn.gameObject.SetActive(!state);
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