using DarkKey.Ui.Debug_Panels.InfoPlugins;
using TMPro;
using UnityEngine;

namespace DarkKey.Ui.Debug_Panels
{
    public class DebugInfoPanel : BaseDebugPanel
    {
        [Header("InfoDrawer")]
        [SerializeField] private GameObject textTemplate;
        [SerializeField] private float updateInterval;

        private IInfoPlugin[] _plugins;
        private float _timePassed;

        #region Unity Methods

        private void Update()
        {
            _timePassed += Time.deltaTime;
            if (_timePassed >= updateInterval)
            {
                UpdatePlugins();
                _timePassed = 0;
            }
        }

        #endregion

        #region Protected Methods

        protected override void InitializePanel()
        {
            _plugins = GetComponents<IInfoPlugin>();
            InitializePlugins();
        }

        #endregion

        #region Private Methods

        private TMP_Text CreateText(string textName)
        {
            GameObject tmpGameObject = Instantiate(textTemplate, Vector3.zero, Quaternion.identity, gridTransform);
            tmpGameObject.name = textName;
            return tmpGameObject.GetComponent<TMP_Text>();
        }

        private void InitializePlugins()
        {
            if (_plugins == null) return;

            for (int index = 0; index < _plugins.Length; index++)
            {
                _plugins[index].InitializePlugin(CreateText($"Plugin-{index}_TMP"));
            }
        }

        private void UpdatePlugins()
        {
            if (_plugins == null) return;

            foreach (var plugin in _plugins)
            {
                plugin.UpdateUi();
            }
        }

        #endregion
    }
}