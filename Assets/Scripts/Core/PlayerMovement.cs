using UnityEngine;

namespace DarkKey.Core
{
    [RequireComponent(typeof(Rigidbody), typeof(InputHandler))]
    public class PlayerMovement : MonoBehaviour
    {
        private Rigidbody _rb;
        private InputHandler _inputHandler;
        [SerializeField] private float moveSpeed;

#region Unity Methods
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<InputHandler>();
        }

        private void FixedUpdate() => Move(_inputHandler.InputVector);

#endregion

#region Private Methods

        private void Move(Vector3 inputDirection)
        {
            var moveDirection = new Vector3();
            

            // Horizontal Input
            if (inputDirection.y > 0) moveDirection += transform.forward;
            if (inputDirection.y < 0) moveDirection -= transform.forward;

            // Vertical Input
            if (inputDirection.x > 0) moveDirection += transform.right;
            if (inputDirection.x < 0) moveDirection -= transform.right;

            moveDirection = moveDirection.normalized;
            
            _rb.MovePosition(transform.position + moveDirection * (moveSpeed * Time.deltaTime));
        }

#endregion
    }
}
