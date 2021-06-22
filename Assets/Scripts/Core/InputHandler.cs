using System;
using MLAPI;
using UnityEngine;

namespace DarkKey.Core
{
    public class InputHandler : NetworkBehaviour
    {
        [SerializeField] [Range(1, 5)] private float mouseSensitivity;
        private float _horizontalMouseClamp;

        public Vector2 MovementInput { get; private set; }
        public Vector2 MouseInput { get; private set; }

        public event Action OnInteract;
        public event Action OnJump;
        public event Action OnEscape;

        #region Unity Methods

        private void Update()
        {
            if (!IsLocalPlayer) return;
            GetMovementInput();
            GetMouseInput();
            GetInteractionInput();
        }

        #endregion

        #region Public Methods

        public void SetMouseClamp(float clampValue) => _horizontalMouseClamp = clampValue;

        #endregion

        #region Private Methods

        private void GetMovementInput()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");

            MovementInput = new Vector2(x, y);
        }

        private void GetMouseInput()
        {
            var input = MouseInput;
            input.x += Input.GetAxis("Mouse X") * mouseSensitivity;
            input.y += Input.GetAxis("Mouse Y") * mouseSensitivity;
            input.y = Mathf.Clamp(input.y, -_horizontalMouseClamp, _horizontalMouseClamp);

            MouseInput = input;
        }

        private void GetInteractionInput()
        {
            if (Input.GetKeyDown(KeyCode.E)) OnInteract?.Invoke();
            if (Input.GetKeyDown(KeyCode.Space)) OnJump?.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape)) OnEscape?.Invoke();
        }

        #endregion
    }
}