using System;
using UnityEngine;

namespace DarkKey.Core
{
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

        private void FixedUpdate()
        {
            Move(_inputHandler.InputVector);
        }

        #endregion
        private void Move(Vector3 inputDirection)
        {
            var inputDir = new Vector3(inputDirection.x, 0, inputDirection.y);
            _rb.MovePosition(transform.position + inputDir * (moveSpeed * Time.deltaTime));
        }
    }
}
