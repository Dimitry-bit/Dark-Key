using TMPro;

namespace DarkKey.Ui.Debug_Panels.InfoPlugins
{
    public interface IInfoPlugin
    {
        public void InitializePlugin(TMP_Text textMeshPro);

        public void UpdateUi();
    }
}