using UnityEngine;

namespace DarkKey.Core
{
    public class CamMovement : MonoBehaviour
    {
        [SerializeField] private Transform cam; 
        
        private Vector2 _mouseInput;
        [SerializeField] [Range(1,5)] private float mouseSensitivity;

        private void Update()
        {
            _mouseInput.x += Input.GetAxis("Mouse X") * mouseSensitivity;
            _mouseInput.y += Input.GetAxis("Mouse Y") * mouseSensitivity;

            cam.localRotation = Quaternion.Euler(-_mouseInput.y, _mouseInput.x, 0);
            transform.localRotation = Quaternion.Euler(0, _mouseInput.x, 0);
        }
    }
}
