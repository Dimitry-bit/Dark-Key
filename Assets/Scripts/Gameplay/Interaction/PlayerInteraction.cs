using System.Collections;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    public class PlayerInteraction : NetworkBehaviour
    {
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private float interactionMaxDistance = 5;
        [SerializeField] private Transform rightHandTransform;

        private Camera _playerCamera;
        private InputHandler _inputHandler;
        private IInteractable _selectedObject;

        [SyncVar(hook = nameof(OnChangeItem))]
        public ItemTypes equippedItem;

        #region Unity Methods

        public override void OnStartClient()
        {
            if (!isLocalPlayer) return;

            TryGetComponent(out _inputHandler);
            _playerCamera = GetComponentInChildren<Camera>();

            _inputHandler.OnInteract += InteractWithSelectedObject;
            _inputHandler.OnDrop += CmdDropItem;
        }

        private void OnDestroy()
        {
            if (_inputHandler == null) return;

            _inputHandler.OnInteract -= InteractWithSelectedObject;
            _inputHandler.OnDrop -= CmdDropItem;
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

        [Command(requiresAuthority = false)]
        public void CmdHoldItem(SceneObject sceneObject)
        {
            if (sceneObject.equippedItem == ItemTypes.Nothing) return;
            if (equippedItem != ItemTypes.Nothing) return;

            equippedItem = sceneObject.equippedItem;
            NetworkServer.Destroy(sceneObject.gameObject);
        }

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
            }
            else
            {
                if (_selectedObject == null) return;
                _selectedObject = null;
            }
        }

        private void InteractWithSelectedObject()
        {
            if (_selectedObject != null)
                _selectedObject.Interact(this);
        }

        private void OnChangeItem(ItemTypes oldEquippedItem, ItemTypes newEquippedItem) =>
            StartCoroutine(ChangeItem(newEquippedItem));

        private IEnumerator ChangeItem(ItemTypes newItem)
        {
            while (rightHandTransform.childCount > 0)
            {
                Destroy(rightHandTransform.GetChild(0).gameObject);
                yield return null;
            }

            if (newItem != ItemTypes.Nothing)
            {
                var newItemPrefab = ItemUtility.GetPrefabByType(newItem);
                var newItemGameObject = Instantiate(newItemPrefab, rightHandTransform.position,
                    rightHandTransform.rotation,
                    rightHandTransform);

                newItemGameObject.GetComponent<GenericItem>().DisablePhysics();
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdDropItem()
        {
            var position = rightHandTransform.position;
            var rotation = rightHandTransform.rotation;

            var sceneObjectPrefab = ItemUtility.GetPrefabByType(ItemTypes.SceneObject);
            var sceneGameObject = Instantiate(sceneObjectPrefab, position, rotation);

            var sceneScript = sceneGameObject.GetComponent<SceneObject>();

            sceneScript.SetItem(equippedItem);
            sceneScript.equippedItem = equippedItem;

            equippedItem = ItemTypes.Nothing;

            NetworkServer.Spawn(sceneGameObject);
        }

        #endregion
    }
}