using System;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    [RequireComponent(typeof(InputHandler))]
    public class PlayerInteraction : NetworkBehaviour
    {
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private float interactionMaxDistance = 5;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform rightHandSlot;
        [SerializeField] private float throwForceMultiplier;

        private InputHandler _inputHandler;
        private Interactable _selectedObject;

        private readonly NetworkVariable<GenericItem> _itemHeld = new NetworkVariable<GenericItem>(new NetworkVariableSettings
        {
            ReadPermission = NetworkVariablePermission.Everyone, 
            WritePermission = NetworkVariablePermission.OwnerOnly
        });

        public event Action<Interactable> OnInteractableSelected;
        public event Action OnInteractableDeselected;


        #region Unity Methods

        private void Start()
        {
            if (!IsLocalPlayer) return;
            TryGetComponent(out _inputHandler);

            _inputHandler.OnInteract += InteractWithSelectedObject;
            _inputHandler.OnDrop += DropItem;
        }

        private void OnDestroy()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnInteract -= InteractWithSelectedObject;
            _inputHandler.OnDrop -= DropItem;
        }

        private void FixedUpdate()
        {
            if (!IsLocalPlayer) return;
            SearchForInteractableObjects();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var camTransform = playerCamera.transform;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(camTransform.position, camTransform.forward * interactionMaxDistance);
        }
#endif

        #endregion

        #region Public Methods

        public void HoldItem(GenericItem item)
        {
            _itemHeld.Value = item;

            var position = rightHandSlot.position + item.inHandOffset;
            var rotation = rightHandSlot.rotation;
            var itemTransform = _itemHeld.Value.transform;

            itemTransform.SetParent(rightHandSlot);
            itemTransform.SetPositionAndRotation(position, rotation);

            _itemHeld.Value.DisableItemForOtherPlayersServerRpc(OwnerClientId);
        }

        public GenericItem GetItemType() => IsHoldingItem() ? _itemHeld.Value : null;

        public GenericItem RemoveAndReturnItem()
        {
            if (!IsHoldingItem()) return null;

            var item = _itemHeld.Value;
            _itemHeld.Value.transform.parent = null;
            _itemHeld.Value = null;

            return item;
        }

        public bool IsHoldingItem() => _itemHeld.Value;

        #endregion

        #region Private Methods

        private void SearchForInteractableObjects()
        {
            var camTransform = playerCamera.transform;
            var ray = new Ray(camTransform.position, camTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionMaxDistance, interactionMask))
            {
                hitInfo.transform.TryGetComponent(out Interactable interactable);

                if (_selectedObject != null && _selectedObject == interactable) return;

                _selectedObject = interactable;
                _selectedObject.OnSelected(this);
                OnInteractableSelected?.Invoke(_selectedObject);
            }
            else
            {
                if (_selectedObject == null) return;

                _selectedObject = null;
                OnInteractableDeselected?.Invoke();
            }
        }

        private void InteractWithSelectedObject()
        {
            if (_selectedObject != null)
                _selectedObject.Interact(this);
        }

        private void DropItem()
        {
            if (!IsHoldingItem()) return;

            _itemHeld.Value.transform.parent = null;

            var throwForce = transform.forward * throwForceMultiplier;
            _itemHeld.Value.ThrowObject(throwForce);

            _itemHeld.Value = null;
        }

        #endregion
    }
}