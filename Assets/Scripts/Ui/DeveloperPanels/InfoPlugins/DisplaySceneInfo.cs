using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Ui.DeveloperPanels.InfoPlugins
{
    public class DisplaySceneInfo : MonoBehaviour, IInfoPlugin
    {
        private TMP_Text _currentSceneTMP;

        public void InitializePlugin(TMP_Text textMeshPro)
        {
            _currentSceneTMP = textMeshPro;
            DisplayCurrentScene();
        }

        public void UpdateUi() { }

        private void DisplayCurrentScene()
        {
            if (_currentSceneTMP == null) return;

            _currentSceneTMP.text = $"CurrentScene: {SceneManager.GetActiveScene().name}";
        }
    }
}