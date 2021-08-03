using DarkKey.Core.Managers;
using DarkKey.Core.Network;
using Mirror;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui.Pages
{
    public class HostPage : Page
    {
        [SerializeField] private TMP_InputField passwordInputField;

        public void HostGame() => NetPortal.Instance.Host(passwordInputField.text);

        public void Back() => ServiceLocator.Instance.pageController.TurnOffPage(PageType, PageType.MainPage);
    }
}