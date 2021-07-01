using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay
{
    [RequireComponent(typeof(InputHandler))]
    public class CamMovement : NetworkBehaviour
    {
        private InputHandler _inputHandler;

        private Camera _cam;
        [SerializeField] private float camMaxRotationAngleY;
        [SerializeField] private Vector3 camOffSet = Vector3.up * 0.5f;

        [Header("Debug")] [SerializeField] private bool lockCursor;

        #region Unity Methods

        private void Start()
        {
            _cam = GetComponentInChildren<Camera>();
            if (_cam == null)
            {
                Debug.LogError("[CamMovement]: No camera was found.");
                return;
            }

            if (IsLocalPlayer)
            {
                _inputHandler = GetComponent<InputHandler>();
                _inputHandler.SetMouseClamp(camMaxRotationAngleY);
            }
            else
            {
                if (_cam.TryGetComponent(out AudioListener audioListener)) audioListener.enabled = false;
                _cam.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            if (!IsLocalPlayer) return;
            CamRotation(_inputHandler.MouseInput.y);
            PlayerRotation(_inputHandler.MouseInput.x);
        }

        #endregion

        #region Private Methods

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