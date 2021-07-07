using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace DarkKey.Gameplay
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Transform rightHandSlot;
        [SerializeField] private float itemLaunchForce = 500f;
        private InputHandler _inputHandler;

        public NetworkVariable<Item> ItemInHand { get; }

        #region Unity Methods

        private void Start()
        {
            if (!IsLocalPlayer) return;
            TryGetComponent(out _inputHandler);

            _inputHandler.OnDrop += DropItem;
        }

        private void OnDestroy()
        {
            if (_inputHandler == null) return;
            _inputHandler.OnDrop -= DropItem;
        }

        #endregion

        #region Public Methods

        public void AssignItemToHand(Item item)
        {
            ItemInHand.Value = item;
            ItemInHand.Value.DisablePhysics();
            TransferItemToHandPosition();
        }

        public void RemoveItemFromHand()
        {
            ItemInHand.Value.transform.parent = null;
            ItemInHand.Value = null;
        }

        public bool IsHoldingItem() => ItemInHand.Value;

        #endregion

        #region Private Methods

        private void DropItem()
        {
            if (!IsHoldingItem()) return;

            ItemInHand.Value.LaunchIntoAir(itemLaunchForce);
            RemoveItemFromHand();
        }

        private void TransferItemToHandPosition()
        {
            var position = rightHandSlot.transform.position + ItemInHand.Value.inHandOffset;
            var rotation = transform.rotation;

            ItemInHand.Value.transform.SetParent(rightHandSlot);
            ItemInHand.Value.transform.SetPositionAndRotation(position, rotation);
        }

        #endregion
    }
}