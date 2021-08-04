using DarkKey.Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Ui.Pages
{
    public class MainPage : Page
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void Start() => SubscribeButtons();

        private void SubscribeButtons()
        {
            hostButton.onClick.AddListener(() =>
                ServiceLocator.Instance.pageController.TurnOffPage(PageType.MainPage, PageType.HostPage));

            joinButton.onClick.AddListener(() =>
                ServiceLocator.Instance.pageController.TurnOffPage(PageType.MainPage, PageType.JoinPage));

            settingsButton.onClick.AddListener(() =>
                ServiceLocator.Instance.pageController.TurnOffPage(PageType.MainPage, PageType.SettingsPage));

            quitButton.onClick.AddListener(ServiceLocator.Instance.gameManager.QuitGame);
        }
    }
}