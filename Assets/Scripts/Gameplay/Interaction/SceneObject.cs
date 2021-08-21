using System.Collections;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    public class SceneObject : NetworkBehaviour, IInteractable
    {
        [SerializeField] private string itemName;
        [SerializeField] private bool isDestructAble;
        [SerializeField] private Transform holderTransform;
        [SyncVar(hook = nameof(OnChangeEquipment))]
        public ItemTypes equippedItem;

        public string ItemName => itemName;
        public string InteractionDescription { get; }
        public bool IsDestructAble => isDestructAble;

        #region Unity Methods

        private void Start()
        {
            InitializeHolderTransform();

            if (!isDestructAble)
            {
                holderTransform.GetChild(0).GetComponent<GenericItem>().DisablePhysics();
            }
        }

        #endregion

        #region Public Methods

        public void OnHover(PlayerInteraction playerInteraction) { }

        public void Interact(PlayerInteraction playerInteraction)
        {
            if (equippedItem == ItemTypes.Nothing)
            {
                equippedItem = playerInteraction.equippedItem;
                playerInteraction.equippedItem = ItemTypes.Nothing;
            }
            else
            {
                if (playerInteraction.equippedItem != ItemTypes.Nothing) return;

                playerInteraction.CmdHoldItem(this);
                equippedItem = ItemTypes.Nothing;
            }
        }

        private void InitializeHolderTransform()
        {
            if (holderTransform == null)
                holderTransform = transform;
        }

        public void SetItem(ItemTypes newItem)
        {
            InitializeHolderTransform();

            var newItemPrefab = ItemUtility.GetPrefabByType(newItem);
            GameObject itemInstance = Instantiate(newItemPrefab, holderTransform);

            if (isDestructAble) return;
            
            if (itemInstance.TryGetComponent(out GenericItem genericItem))
            {
                genericItem.DisablePhysics();
            }
        }

        #endregion

        #region Private Methods

        private void OnChangeEquipment(ItemTypes oldEquippedItem, ItemTypes newEquippedItem) =>
            StartCoroutine(ChangeEquipment(newEquippedItem));

        private IEnumerator ChangeEquipment(ItemTypes newEquippedItem)
        {
            InitializeHolderTransform();

            while (holderTransform.childCount > 0)
            {
                Destroy(holderTransform.GetChild(0).gameObject);
                yield return null;
            }

            SetItem(newEquippedItem);
        }

        #endregion
    }
}