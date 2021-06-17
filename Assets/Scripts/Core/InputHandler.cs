using UnityEngine;

namespace DarkKey.Core
{
    public class InputHandler : MonoBehaviour
    {
        public Vector2 InputVector { get; private set; }
        
        public Vector2 MouseInput { get; private set; }
        private float _mouseClampY;
        [SerializeField] [Range(1,5)] private float mouseSensitivity;

#region Unity Methods
        
        private void Update()
        {
            GetMovementInput();
            GetMouseInput();
        }
        
#endregion

#region Public Methods

        public void SetMouseClamp(float clampValue) => _mouseClampY = clampValue;

#endregion

#region Private Methods

        private void GetMovementInput()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");

            InputVector = new Vector2(x,y);
        }

        private void GetMouseInput()
        {
            var input = MouseInput;
            input.x += Input.GetAxis("Mouse X") * mouseSensitivity;
            input.y += Input.GetAxis("Mouse Y") * mouseSensitivity;
            input.y = Mathf.Clamp(input.y, -_mouseClampY, _mouseClampY);
            
            MouseInput = input;
        }
        
#endregion
    }
}