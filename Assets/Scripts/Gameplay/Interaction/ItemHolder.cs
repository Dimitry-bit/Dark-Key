using DarkKey.Core.Debugger;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    public class ItemHolder : Interactable
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [SerializeField] private Transform itemHolderTransform;
        private readonly NetworkVariable<GenericItem> _itemHeld = new NetworkVariable<GenericItem>(
            new NetworkVariableSettings
            {
                ReadPermission = NetworkVariablePermission.Everyone,
                WritePermission = NetworkVariablePermission.Everyone
            });

        #region Untiy Methods

        private void Start()
        {
            var item = GetComponentInChildren<GenericItem>();
            if (item == null) return;

            var holdPosition = itemHolderTransform == null ? transform : itemHolderTransform;
            HoldItem(item, holdPosition);
        }

        #endregion

        #region Public Methods

        public override void OnSelected(PlayerInteraction playerInteraction)
        {
            if (IsHoldingItem())
                InteractionDescription =
                    playerInteraction.IsHoldingItem() ? "" : $"Pick Up {_itemHeld.Value.ObjectName}";
            else
                InteractionDescription = playerInteraction.IsHoldingItem() ? "" : $"Put Down Object";
        }

        public override void Interact(PlayerInteraction playerInteraction)
        {
            if (IsHoldingItem())
            {
                AssignItemToPlayer(playerInteraction);
            }
            else
            {
                GetItemFromPlayer(playerInteraction);
            }
        }

        public bool IsHoldingItem() => _itemHeld.Value;

        #endregion

        #region Private Methods

        private void HoldItem(GenericItem item, Transform holdPositionTransform)
        {
            if (holdPositionTransform == null)
            {
                CustomDebugger.LogWarning("holdPositionTransform is null", ScriptLogLevel);
                return;
            }

            _itemHeld.Value = item;

            var position = holdPositionTransform.position + item.inHandOffset;
            var rotation = holdPositionTransform.rotation;
            var itemTransform = _itemHeld.Value.transform;

            itemTransform.SetParent(holdPositionTransform);
            itemTransform.SetPositionAndRotation(position, rotation);

            _itemHeld.Value.EnableItemForAllPlayersServerRpc();
        }


        protected void AssignItemToPlayer(PlayerInteraction playerInteraction)
        {
            if (playerInteraction.IsHoldingItem()) return;

            playerInteraction.HoldItem(_itemHeld.Value);
            _itemHeld.Value = null;
        }

        protected void GetItemFromPlayer(PlayerInteraction playerInteraction)
        {
            if (!playerInteraction.IsHoldingItem()) return;

            GenericItem itemToHold = playerInteraction.RemoveAndReturnItem();
            var holdPositionTransform = itemHolderTransform == null ? transform : itemHolderTransform;

            HoldItem(itemToHold, holdPositionTransform);
        }

        #endregion
    }
}