using System;
using System.Collections;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    public class PlayerInteraction : NetworkBehaviour
    {
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private float interactionMaxDistance = 5;
        [SerializeField] private float throwForceMultiplier;
        [SerializeField] private Transform rightHandTransform;
        private Camera _playerCamera;
        private InputHandler _inputHandler;
        private IInteractable _selectedObject;

        [SyncVar(hook = nameof(OnChangeEquipment))]
        public ItemTypes equippedItem;


        public event Action<IInteractable> OnInteractableSelected;
        public event Action OnInteractableDeselected;

        #region Unity Methods

        private void Start()
        {
            if (!isLocalPlayer) return;

            TryGetComponent(out _inputHandler);
            _playerCamera = GetComponentInChildren<Camera>();

            _inputHandler.OnInteract += InteractWithSelectedObject;
            // _inputHandler.OnDrop += CmdDropItem;
        }

        private void OnDestroy()
        {
            if (_inputHandler == null) return;

            _inputHandler.OnInteract -= InteractWithSelectedObject;
            // _inputHandler.OnDrop -= CmdDropItem;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            SearchForInteractableObjects();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var camTransform = _playerCamera.transform;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(camTransform.position, camTransform.forward * interactionMaxDistance);
        }
#endif

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private void SearchForInteractableObjects()
        {
            var camTransform = _playerCamera.transform;
            var ray = new Ray(camTransform.position, camTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, interactionMaxDistance, interactionMask))
            {
                hitInfo.transform.TryGetComponent(out IInteractable interactable);

                if (_selectedObject != null && _selectedObject == interactable) return;

                _selectedObject = interactable;
                _selectedObject.OnHover(this);
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

        private void OnChangeEquipment(ItemTypes oldEquippedItem, ItemTypes newEquippedItem) =>
            StartCoroutine(ChangeEquipment(newEquippedItem));

        private IEnumerator ChangeEquipment(ItemTypes newEquippedItem)
        {
            while (rightHandTransform.transform.childCount > 0)
            {
                Destroy(rightHandTransform.transform.GetChild(0).gameObject);
                yield return null;
            }

            var newItem = ItemUtility.GetPrefabByType(newEquippedItem);
            Instantiate(newItem, rightHandTransform);
        }

        #endregion
    }
}