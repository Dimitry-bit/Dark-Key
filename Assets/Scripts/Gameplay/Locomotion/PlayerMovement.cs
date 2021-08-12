using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Locomotion
{
    [RequireComponent(typeof(Rigidbody), typeof(InputHandler))]
    public class PlayerMovement : NetworkBehaviour
    {
        [Header("Movement Settings")]
        private Rigidbody _rigidbody;
        private InputHandler _inputHandler;
        [SerializeField] private float moveSpeed;
        [SerializeField] [Range(0, 1)] private float velocityLerpSpeed = 0.07f;

        [Header("Animation Settings")]
        [SerializeField] [Range(0, 1)] private float animationLerpSpeed = 0.25f;
        private Animator _animator;
        private float _currentVelocity;

        private static readonly int VelocityY = Animator.StringToHash("VelocityY");
        private static readonly int VelocityX = Animator.StringToHash("VelocityX");

        #region Unity Methods

        private void Start()
        {
            if (!isLocalPlayer) return;
            TryGetComponent(out _rigidbody);
            TryGetComponent(out _inputHandler);
            TryGetComponent(out _animator);
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            Move(_inputHandler.MovementInput);
            HandleAnimation(_inputHandler.MovementInput);
        }

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

            if (moveDirection.magnitude == 0)
                _currentVelocity = 0;

            _currentVelocity = Mathf.Lerp(_currentVelocity, moveSpeed, velocityLerpSpeed);
            _rigidbody.MovePosition(transform.position + moveDirection * (_currentVelocity * Time.deltaTime));
        }

        private void HandleAnimation(Vector3 inputDirection)
        {
            var currentAnimVelocityX = _animator.GetFloat(VelocityX);
            var currentAnimVelocityY = _animator.GetFloat(VelocityY);

            var smoothedAnimVelocityX = Mathf.Lerp(currentAnimVelocityX, inputDirection.y, animationLerpSpeed);
            var smoothedAnimVelocityY = Mathf.Lerp(currentAnimVelocityY, inputDirection.x, animationLerpSpeed);

            _animator.SetFloat(VelocityX, smoothedAnimVelocityX);
            _animator.SetFloat(VelocityY, smoothedAnimVelocityY);
        }

        #endregion
    }
}