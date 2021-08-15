using TMPro;

namespace DarkKey.DeveloperTools.DeveloperPanels.InfoPlugins
{
    public interface IInfoPlugin
    {
        public void InitializePlugin(TMP_Text textMeshPro);

        public void UpdateUi();
    }
}