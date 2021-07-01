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

        #region Unity Methods

        private void Start()
        {
            if (!IsLocalPlayer) return;
            TryGetComponent(out _inputHandler);

            _inputHandler.OnInteract += SearchForInteractableObject;
        }

        private void OnDestroy()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnInteract -= SearchForInteractableObject;
        }

        #endregion

        #region Private Methods

        private void SearchForInteractableObject()
        {
            var camTransform = playerCamera.transform;
            var ray = new Ray(camTransform.position, camTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionMaxDistance, interactionMask))
            {
                hitInfo.transform.TryGetComponent(out IInteractable interactable);
                interactable.Interact();
            }
        }

        #endregion
    }
}