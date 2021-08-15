using TMPro;

namespace DarkKey.Ui.DeveloperPanels.InfoPlugins
{
    public interface IInfoPlugin
    {
        public void InitializePlugin(TMP_Text textMeshPro);

        public void UpdateUi();
    }
}