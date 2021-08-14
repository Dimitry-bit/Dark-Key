using DarkKey.Core.Managers;

namespace DarkKey.Ui.Pages
{
    public class SettingsPage : Page
    {
        public void Back() => ServiceLocator.Instance.GetPageController().TurnOffPage(PageType, PageType.MainPage);
    }
}