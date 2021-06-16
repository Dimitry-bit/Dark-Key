using UnityEngine;

namespace DarkKey.Core
{
    public class InputHandler : MonoBehaviour
    {
        public Vector2 InputVector { get; private set; }
        
        public Vector2 MouseInput { get; private set; } 
        [SerializeField] [Range(1,5)] private float mouseSensitivity;

#region Unity Methods
        
        private void Update()
        {
            GetMovementInput();
            GetMouseInput();
        }
        
#endregion

#region Private Methods

        private void GetMovementInput()
        {
            var x = Input.GetAxisRaw("Horizontal");
            var y = Input.GetAxisRaw("Vertical");

            InputVector = new Vector3(x,y);
        }

        private void GetMouseInput()
        {
            var input = MouseInput;
            input.x += Input.GetAxis("Mouse X") * mouseSensitivity;
            input.y += Input.GetAxis("Mouse Y") * mouseSensitivity;

            MouseInput = input;
        }
        
#endregion
    }
}