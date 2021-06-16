using UnityEngine;

namespace DarkKey.Core
{
    [RequireComponent(typeof(InputHandler))]
    public class CamMovement : MonoBehaviour
    {
        private InputHandler _inputHandler;
        [SerializeField] private Transform cam;
        [SerializeField] private Vector3 camOffSet = Vector3.up;

        [Header("Debug")] 
        [SerializeField] private bool lockCursor;
        
#region Unity Methods

        private void Start() => _inputHandler = GetComponent<InputHandler>();

        private void Update()
        {
            var mouseInput = _inputHandler.MouseInput;
            
            PlayerRotation(mouseInput.x);
            CamRotation(mouseInput);
            
            LockCursor();
        }

#endregion

#region private Methods

        private void PlayerRotation(float mouseX) => transform.localRotation = Quaternion.Euler(0, mouseX, 0);

        private void CamRotation(Vector2 mouseInput)
        {
            cam.localRotation = Quaternion.Euler(-mouseInput.y, mouseInput.x, 0);
            cam.transform.position = transform.position + camOffSet;
        }

        // Debug Only
        private void LockCursor()
        {
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockCursor;
        }
        
#endregion
    }
}
