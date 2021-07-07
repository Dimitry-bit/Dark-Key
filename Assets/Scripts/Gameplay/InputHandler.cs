using System;
using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay
{
    public class InputHandler : NetworkBehaviour
    {
        [SerializeField] [Range(1, 5)] private float mouseSensitivity;
        private float _horizontalMouseClamp;
        public InputActionMap actionMap = InputActionMap.Gameplay;

        public enum InputActionMap
        {
            Gameplay,
            Ui,
            None,
        }

        public event Action OnInteract;
        public event Action OnJump;
        public event Action OnEscape;
        public event Action OnConsole;
        public event Action OnDrop;

        public Vector2 MovementInput { get; private set; }
        public Vector2 MouseInput { get; private set; }

        #region Unity Methods

        private void Update()
        {
            if (!IsLocalPlayer) return;

            CheckInputMap();
        }

        #endregion

        #region Public Methods

        public void SetMouseClamp(float clampValue) => _horizontalMouseClamp = clampValue;

        #endregion

        #region Private Methods

        private void CheckInputMap()
        {
            switch (actionMap)
            {
                case InputActionMap.Gameplay:
                {
                    GetMovementInput();
                    GetMouseInput();
                    GetInteractionInput();
                    GetUiInput();
                    break;
                }
                case InputActionMap.Ui:
                {
                    MovementInput = Vector2.zero;
                    GetUiInput();
                    break;
                }
                case InputActionMap.None:
                {
                    MovementInput = Vector2.zero;
                    break;
                }
            }
        }

        private void GetMovementInput()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");

            MovementInput = new Vector2(x, y);

            if (Input.GetKeyDown(KeyCode.Space)) OnJump?.Invoke();
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
            if (Input.GetKeyDown(KeyCode.G)) OnDrop?.Invoke();
        }

        private void GetUiInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OnEscape?.Invoke();
            if (Input.GetKeyDown(KeyCode.BackQuote)) OnConsole?.Invoke();
        }

        #endregion
    }
}