using DarkKey.Ui.Utilities;
using MLAPI;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Ui.Debug_Panels
{
    [RequireComponent(typeof(DraggableUi))]
    public abstract class BaseDebugPanel : NetworkBehaviour
    {
        [Header("Base Options (Required)")]
        [SerializeField] private Button openPanelButton;
        [SerializeField] private GameObject debugPanelGameObject;
        [SerializeField] protected Transform gridTransform;

        private Vector2 _offset;
        private bool _isPanelEnabled;

        #region Unity Methods

        private void Start()
        {
            if (debugPanelGameObject == null) debugPanelGameObject = gameObject;

            if (gridTransform == null)
            {
                var gridGameObject = GetComponentInChildren<GridLayoutGroup>();
                if (gridGameObject != null)
                    gridTransform = gridGameObject.transform;
            }

            if (openPanelButton != null)
                openPanelButton.onClick.AddListener(ShowPanel);

            InitializePanel();
        }

        private void OnDestroy()
        {
            if (openPanelButton != null)
                openPanelButton.onClick.RemoveListener(ShowPanel);
        }

        #endregion

        #region Protected Methods

        protected abstract void InitializePanel();

        protected virtual void EnablePanel()
        {
            debugPanelGameObject.SetActive(true);
            _isPanelEnabled = true;
        }

        protected virtual void DisablePanel()
        {
            debugPanelGameObject.SetActive(false);
            _isPanelEnabled = false;
        }

        #endregion

        #region Private Methods

        private void ShowPanel()
        {
            if (_isPanelEnabled)
                DisablePanel();
            else
                EnablePanel();
        }

        #endregion
    }
}