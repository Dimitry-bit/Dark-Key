using DarkKey.Core.Debugger;
using UnityEngine;

namespace DarkKey.Ui.Utilities
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        #region Unity Functions

        private void Start()
        {
            if (mainCamera != null) return;

            mainCamera = Camera.main;
            CustomDebugger.LogWarning($"mainCamera variable is not assigned | Defaulted to Camera.Main",
                new[] {DebugLogLevel.UI});
        }

        private void LateUpdate() => transform.LookAt(transform.position + mainCamera.transform.forward);

        #endregion
    }
}