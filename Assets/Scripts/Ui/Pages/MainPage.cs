using System;
using DarkKey.Core;
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

        private void Start () => SubscribeButtons();

        private void SubscribeButtons()
        {
            hostButton.onClick.AddListener(() =>
                PageController.Instance.TurnOffPage(PageType.MainPage, PageType.HostPage));

            joinButton.onClick.AddListener(() =>
                PageController.Instance.TurnOffPage(PageType.MainPage, PageType.JoinPage));

            settingsButton.onClick.AddListener(() =>
                PageController.Instance.TurnOffPage(PageType.MainPage, PageType.SettingsPage));

            quitButton.onClick.AddListener(GameManager.QuitGame);
        }
    }
}