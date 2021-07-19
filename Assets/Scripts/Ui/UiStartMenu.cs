using System.Collections;
using System.Text.RegularExpressions;
using DarkKey.Core;
using DarkKey.Core.Network;
using MLAPI;
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

        [Header("Optional")]
        [Tooltip("If left empty it will grab Camera.Main")]
        [SerializeField] private Camera lobbyCam;
        private Coroutine _errorMessageCoroutine;

        #region Unity Methods

        private void Start()
        {
            if (lobbyCam == null) lobbyCam = Camera.main;

            NetPortal.Instance.OnLocalConnection += DisableMenu;
            // NetPortal.Instance.OnLocalDisconnection += EnableMenu;
        }

        private void OnDestroy()
        {
            if (NetPortal.Instance == null) return;

            NetPortal.Instance.OnLocalConnection -= DisableMenu;
            // NetPortal.Instance.OnLocalDisconnection -= EnableMenu;
        }

        #endregion

        #region Public Methods

        public void Host()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            NetPortal.Instance.Host(ipInputField.text, passwordInputField.text);
        }

        public void Join()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            NetPortal.Instance.Join(ipInputField.text, passwordInputField.text);
        }

        public void Quit()
        {
            DisableMenu();
            // NetPortal.Instance.Disconnect();
            GameManager.QuitGame();
        }

        #endregion

        #region Private Methods

        private void EnableMenu()
        {
            lobbyCam.gameObject.SetActive(true);
            mainPanel.SetActive(true);
            CursorManager.ShowCursor();
            CustomDebugger.Instance.LogError("UiStartMenu", "EnableMenu Executed");
        }

        private void DisableMenu()
        {
            // lobbyCam.gameObject.SetActive(false);
            mainPanel.SetActive(false);
            // CursorManager.HideCursor();
            CustomDebugger.Instance.LogInfo("UiStartMenu", "DisableMenu Executed");
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

        #endregion
    }
}