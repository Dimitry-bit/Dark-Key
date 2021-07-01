using System;
using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay
{
    public class InputHandler : NetworkBehaviour
    {
        [SerializeField] [Range(1, 5)] private float mouseSensitivity;
        private float _horizontalMouseClamp;

        public Vector2 MovementInput { get; private set; }
        public Vector2 MouseInput { get; private set; }

        public DisableInput disabledInput = DisableInput.None;
        
        public event Action OnInteract;
        public event Action OnJump;
        public event Action OnEscape;
        public event Action OnConsole;
        
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
            if (disabledInput == DisableInput.Movement || disabledInput == DisableInput.All)
            {
                MovementInput = Vector2.zero;
                return;
            }

            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");

            MovementInput = new Vector2(x, y);
        }

        private void GetMouseInput()
        {
            if (disabledInput == DisableInput.Camera || disabledInput == DisableInput.All)
            {
                return;
            }

            var input = MouseInput;
            input.x += Input.GetAxis("Mouse X") * mouseSensitivity;
            input.y += Input.GetAxis("Mouse Y") * mouseSensitivity;
            input.y = Mathf.Clamp(input.y, -_horizontalMouseClamp, _horizontalMouseClamp);

            MouseInput = input;
        }

        private void GetInteractionInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) OnEscape?.Invoke();
            if (Input.GetKeyDown(KeyCode.BackQuote)) OnConsole?.Invoke();
            
            if (disabledInput != DisableInput.None) return;
            
            if (Input.GetKeyDown(KeyCode.E)) OnInteract?.Invoke();
            if (Input.GetKeyDown(KeyCode.Space)) OnJump?.Invoke();
        }

        #endregion
    }
    
    public enum DisableInput
    {
       None,
       Movement,
       Camera,
       All,
    }
}