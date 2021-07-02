using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay
{
    public class ItemHolder : NetworkBehaviour, IInteractable
    {
        [SerializeField] private Item itemHeld;
        private Transform _itemHolderTransform;

        #region Unity Methods

        private void Start() => _itemHolderTransform = transform;

        #endregion

        #region Public Methods

        public void Interact(Player player)
        {
            if (itemHeld == null)
            {
                if (!player.IsHoldingItem()) return;
                itemHeld = player.RemoveItemFromHand();

                var position = _itemHolderTransform.position + itemHeld.InItemHolderOffset;
                var rotation = _itemHolderTransform.rotation;
                
                itemHeld.transform.parent = _itemHolderTransform;
                itemHeld.transform.SetPositionAndRotation(position, rotation);
            }
            else
            {
                if (player.IsHoldingItem()) return;

                itemHeld.transform.parent = null;
                player.AssignItemToHand(itemHeld);
                itemHeld = null;
            }
        }

        #endregion
    }
}