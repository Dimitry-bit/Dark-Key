using DarkKey.Core;
using DarkKey.Core.Debugger;
using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay.Locomotion
{
    [RequireComponent(typeof(InputHandler))]
    public class CamMovement : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Player};

        [SerializeField] private float camMaxRotationAngleY;
        [SerializeField] private Vector3 camOffSet = Vector3.up * 0.5f;
        
        public Camera _cam;
        private InputHandler _inputHandler;

        #region Unity Methods

        private void Start()
        {
            if (!IsLocalPlayer) return;

            // _cam = GetComponentInChildren<Camera>();
            if (_cam == null)
            {
                CustomDebugger.LogError("CamMovement", "No camera was found.", ScriptLogLevel);
                return;
            }
            else
            {
            }

            SetVerticalMouseClamp();
            DisableUnusedCameras();
            
            _cam.gameObject.SetActive(true);


            CursorManager.HideCursor();
        }


        private void FixedUpdate()
        {
            if (!IsLocalPlayer) return;
            CamRotation(_inputHandler.MouseInput.y);
            PlayerRotation(_inputHandler.MouseInput.x);
        }

        #endregion

        #region Private Methods

        private void SetVerticalMouseClamp()
        {
            TryGetComponent(out _inputHandler);
            _inputHandler.SetMouseClamp(camMaxRotationAngleY);
        }

        private void DisableUnusedCameras()
        {
            var cameras = FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                if (cam == _cam) continue;

                cam.gameObject.SetActive(false);
            }
        }

        private void PlayerRotation(float mouseX) => transform.localRotation = Quaternion.Euler(0, mouseX, 0);

        private void CamRotation(float mouseY)
        {
            var camTransform = _cam.transform;
            camTransform.localRotation = Quaternion.Euler(-mouseY, 0, 0);
            camTransform.position = transform.position + camOffSet;
        }

        #endregion
    }
}