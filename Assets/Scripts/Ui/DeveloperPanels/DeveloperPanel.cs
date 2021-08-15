using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Ui.DeveloperPanels
{
    public abstract class DeveloperPanel : MonoBehaviour
    {
        [Header("Required Options (Auto Filled)")]
        [Tooltip("If \"null\" will populate with first child object.")]
        [SerializeField] private GameObject developerPanelGameObject;
        [SerializeField] protected Transform gridTransform;

        #region Unity Methods

        protected virtual void Start()
        {
            if (developerPanelGameObject == null)
            {
                Transform firstChild = transform.GetChild(0);
                if (firstChild != null)
                    developerPanelGameObject = transform.GetChild(0).gameObject;
            }

            if (gridTransform == null)
            {
                // Bug: Can't find GridLayoutGroup in children.
                var gridGameObject = GetComponentInChildren<GridLayoutGroup>();
                if (gridGameObject != null)
                    gridTransform = gridGameObject.transform;
            }

            InitializePanel();
        }

        #endregion

        #region Protected Methods

        protected void EnablePanel() => developerPanelGameObject.SetActive(true);
        protected void DisablePanel() => developerPanelGameObject.SetActive(false);
        protected abstract void InitializePanel();
        protected virtual void OnPanelEnable() { }
        protected virtual void OnPanelDisable() { }

        #endregion

        #region Private Methods

        // Method called from a UnityEvent.
        public void TogglePanel()
        {
            if (developerPanelGameObject.activeSelf)
            {
                DisablePanel();
                OnPanelDisable();
            }
            else
            {
                EnablePanel();
                OnPanelEnable();
            }
        }

        #endregion
    }
}