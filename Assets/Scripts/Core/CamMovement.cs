using UnityEngine;

namespace DarkKey.Core
{
    [RequireComponent(typeof(InputHandler))]
    public class CamMovement : MonoBehaviour
    {
        private InputHandler _inputHandler;
        
        [SerializeField] private Transform cam;
        [SerializeField] private float camMaxRotationAngleY;
        [SerializeField] private Vector3 camOffSet = Vector3.up * 0.5f;

        [Header("Debug")] 
        [SerializeField] private bool lockCursor;
        
#region Unity Methods

        private void Start()
        {
            _inputHandler = GetComponent<InputHandler>();
            _inputHandler.SetMouseClamp(camMaxRotationAngleY);

            if (cam == null) cam = Camera.main.transform;
        }

        private void Update()
        {
            LockCursor();
        }

        private void FixedUpdate()
        {
            CamRotation(_inputHandler.MouseInput);
            PlayerRotation(_inputHandler.MouseInput.x);
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
