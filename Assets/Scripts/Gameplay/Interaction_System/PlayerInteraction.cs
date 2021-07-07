using DarkKey.Gameplay.Interfaces;
using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerInteraction : NetworkBehaviour
    {
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private float interactionMaxDistance = 5;
        [SerializeField] private Camera playerCamera;
        private InputHandler _inputHandler;
        private Player _playerScript;

        #region Unity Methods

        private void Start()
        {
            if (!IsLocalPlayer) return;
            TryGetComponent(out _inputHandler);
            TryGetComponent(out _playerScript);

            _inputHandler.OnInteract += SearchForInteractableObjectAndInteract;
        }

        private void OnDestroy()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnInteract -= SearchForInteractableObjectAndInteract;
        }

        #endregion

        #region Private Methods

        private void SearchForInteractableObjectAndInteract()
        {
            var camTransform = playerCamera.transform;
            var ray = new Ray(camTransform.position, camTransform.forward);

            if (!Physics.Raycast(ray, out RaycastHit hitInfo, interactionMaxDistance, interactionMask)) return;

            hitInfo.transform.TryGetComponent(out IInteractable interactable);
            interactable.Interact(_playerScript);
        }

        #endregion
    }
}