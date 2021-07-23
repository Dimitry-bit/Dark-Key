using DarkKey.Gameplay.Interfaces;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace DarkKey.Gameplay
{
    public class ItemHolder : NetworkBehaviour, IInteractable
    {
        [SerializeField] private NetworkVariable<Item> itemHeld;
        private Transform _itemHolderTransform;

        #region Unity Methods

        private void Start() => _itemHolderTransform = transform;

        #endregion

        #region Public Methods

        public void Interact(Player player)
        {
            if (itemHeld.Value == null)
            {
                if (!player.IsHoldingItem()) return;
                HoldItem(player);
            }
            else
            {
                if (player.IsHoldingItem()) return;
                GiveItemToPlayer(player);
            }
        }

        #endregion

        #region Private Methods

        public void GiveItemToPlayer(Player player)
        {
            player.AssignItemToHand(itemHeld.Value);
            itemHeld.Value = null;
        }

        public void HoldItem(Player player)
        {
            itemHeld.Value = player.ItemInHand.Value;

            player.RemoveItemFromHand();
            itemHeld.Value.DisablePhysics();
            TransferItemToItemHolderPosition();
        }

        private void TransferItemToItemHolderPosition()
        {
            var position = _itemHolderTransform.position + itemHeld.Value.inItemHolderOffset;
            var rotation = _itemHolderTransform.rotation;
            
            itemHeld.Value.transform.SetParent(_itemHolderTransform);
            itemHeld.Value.transform.SetPositionAndRotation(position, rotation);
        }

        #endregion
    }
}