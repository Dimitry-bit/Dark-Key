namespace DarkKey.Ui.Pages
{
    public class SettingsPage : Page
    {
        public void Back() => PageController.Instance.TurnOffPage(PageType, PageType.MainPage);
    }
}