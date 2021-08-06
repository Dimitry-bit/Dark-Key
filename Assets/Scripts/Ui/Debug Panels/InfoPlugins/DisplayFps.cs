using TMPro;
using UnityEngine;

namespace DarkKey.Ui.Debug_Panels.InfoPlugins
{
    public class DisplayFps : MonoBehaviour, IInfoPlugin
    {
        private TMP_Text _fpsTMP;

        public void InitializePlugin(TMP_Text textMeshPro)
        {
            _fpsTMP = textMeshPro;
            DisplayFpsUi();
        }

        public void UpdateUi() => DisplayFpsUi();

        private void DisplayFpsUi()
        {
            if (_fpsTMP == null) return;

            var fps = $"FPS: {CalculateFPS()}";
            _fpsTMP.text = fps;
        }

        private float CalculateFPS() => 1 / Time.smoothDeltaTime;
    }
}