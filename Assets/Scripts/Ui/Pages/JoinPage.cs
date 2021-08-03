using System.Collections;
using System.Text.RegularExpressions;
using DarkKey.Core.Managers;
using DarkKey.Core.Network;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui.Pages
{
    public class JoinPage : Page
    {
        [SerializeField] private TMP_InputField ipInputField;
        [SerializeField] private TMP_InputField passwordInputField;
        [SerializeField] private TMP_Text errorText;

        private Coroutine _errorMessageCoroutine;

        #region Public Methods

        public void JoinGame()
        {
            if (string.IsNullOrEmpty(ipInputField.text)) ipInputField.text = "127.0.0.1";
            else if (!IsValidIp()) return;

            NetPortal.Instance.Join(ipInputField.text, passwordInputField.text);
        }

        public void Back() => ServiceLocator.Instance.pageController.TurnOffPage(PageType, PageType.MainPage);

        #endregion

        #region Private Methods

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