using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Locomotion
{
    [RequireComponent(typeof(InputHandler))]
    public class CamMovement : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Player};

        [SerializeField] private float camMaxRotationAngleY;
        [SerializeField] private Vector3 camOffSet = Vector3.up * 0.5f;

        private Camera _cam;
        private InputHandler _inputHandler;

        #region Unity Methods

        public override void OnStartClient()
        {
            if (!isLocalPlayer) return;

            _cam = GetComponentInChildren<Camera>();
            if (_cam == null)
                ServiceLocator.Instance.GetDebugger().LogError("No camera was found.", ScriptLogLevel);

            SetVerticalMouseClamp();

            CursorManager.HideCursor();
        }


        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            if (_cam == null) return;

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