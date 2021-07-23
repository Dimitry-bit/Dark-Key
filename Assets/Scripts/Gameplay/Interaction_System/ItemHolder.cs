using DarkKey.Core.Debugger;
using DarkKey.Gameplay.Interfaces;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace DarkKey.Gameplay
{
    public class ItemHolder : Interactable
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [SerializeField] private Transform itemHolderTransform;
        private readonly NetworkVariable<GenericItem> _itemHeld = new NetworkVariable<GenericItem>(new NetworkVariableSettings
        {
            ReadPermission = NetworkVariablePermission.Everyone,
            WritePermission = NetworkVariablePermission.Everyone
        });

        #region Untiy Methods

        private void Start()
        {
            var item = GetComponentInChildren<GenericItem>();
            if (item != null)
                HoldItem(item);
        }

        #endregion

        #region Public Methods

        public override void OnSelected(PlayerInteraction playerInteraction)
        {
            if (IsHoldingItem())
                InteractionDescription = playerInteraction.IsHoldingItem() ? "" : $"Pick Up {_itemHeld.Value.ObjectName}";
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
                GetItemFormPlayer(playerInteraction);
            }
        }

        public bool IsHoldingItem() => _itemHeld.Value;

        #endregion

        #region Private Methods

        private void HoldItem(GenericItem item)
        {
            _itemHeld.Value = item;

            var position = itemHolderTransform.position + item.inHandOffset;
            var rotation = itemHolderTransform.rotation;
            var itemTransform = _itemHeld.Value.transform;

            itemTransform.SetParent(itemHolderTransform);
            itemTransform.SetPositionAndRotation(position, rotation);

            _itemHeld.Value.EnableItemForAllPlayersServerRpc();
        }


        private void AssignItemToPlayer(PlayerInteraction playerInteraction)
        {
            if (playerInteraction.IsHoldingItem()) return;

            playerInteraction.HoldItem(_itemHeld.Value);
            _itemHeld.Value = null;
        }

        private void GetItemFormPlayer(PlayerInteraction playerInteraction)
        {
            if (!playerInteraction.IsHoldingItem()) return;

            HoldItem(playerInteraction.RemoveAndReturnItem());
        }

        #endregion
    }
}