using System.Collections;
using System.Text.RegularExpressions;
using DarkKey.Core.Network;
using Mirror;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui.Pages
{
    public class JoinPage : Page
    {
        [SerializeField] private GameObject joinPanel;
        [SerializeField] private GameObject connectingPanel;
        [SerializeField] private TMP_InputField ipInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_Text errorText;

        private Coroutine _errorMessageCoroutine;

        #region Unity Methods

        private void LateUpdate()
        {
            if (NetworkClient.isConnecting)
            {
                if (joinPanel.activeSelf)
                {
                    joinPanel.SetActive(false);
                }

                if (!connectingPanel.activeSelf)
                {
                    connectingPanel.SetActive(true);
                }
            }
            else if (NetworkClient.isConnected)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (!joinPanel.activeSelf)
                {
                    joinPanel.SetActive(true);
                }

                if (connectingPanel.activeSelf)
                {
                    connectingPanel.SetActive(false);
                }
            }
        }

        #endregion

        #region Public Methods

        public void JoinGame()
        {
            if (string.IsNullOrEmpty(ipInputField.text))
            {
                ipInputField.text = "127.0.0.1";
            }
            else if (!IsValidIpAddress())
            {
                if (_errorMessageCoroutine != null)
                {
                    StopCoroutine(_errorMessageCoroutine);
                }

                errorText.text = "Error : Invalid Ip Address.";
                _errorMessageCoroutine = StartCoroutine(TimedErrorMessage(5f));

                return;
            }

            NetPortal.Instance.Join(ipInputField.text, passwordInputField.text);
        }

        public void StopTryingToConnect()
        {
            if (!NetworkClient.active) return;
            NetworkManager.singleton.StopClient();
        }

        public void Back() => PageController.Instance.TurnOffPage(PageType, PageType.MainPage);

        #endregion

        #region Private Methods

        private bool IsValidIpAddress()
        {
            var ipRegex = new Regex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
            return ipRegex.IsMatch(ipInputField.text);
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