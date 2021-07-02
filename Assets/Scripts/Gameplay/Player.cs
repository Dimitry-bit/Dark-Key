using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Transform rightHand;
        private Item _itemInHand;

        #region Public Methods

        public void AssignItemToHand(Item item)
        {
            _itemInHand = item;

            var position = rightHand.transform.position + _itemInHand.InHandOffset;
            var rotation = transform.rotation;

            _itemInHand.transform.parent = rightHand;
            _itemInHand.transform.SetPositionAndRotation(position, rotation);
        }

        public Item RemoveItemFromHand()
        {
            var itemInHand = _itemInHand;
            _itemInHand.transform.parent = null;
            _itemInHand = null;

            return itemInHand;
        }

        public bool IsHoldingItem() => _itemInHand;

        #endregion
    }
}