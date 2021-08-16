using DarkKey.Core.Debugger;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    public class ItemHolder : NetworkBehaviour, IInteractable
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core};

        [SerializeField] private Transform itemHolderTransform;
        [SerializeField] private string objectName;
        public string ItemName { get; }
        public string InteractionDescription { get; private set; }

        #region Untiy Methods

        private void Start()
        {
            var item = GetComponentInChildren<GenericItem>();
            if (item == null) return;

            var holdPosition = itemHolderTransform == null ? transform : itemHolderTransform;
            // CmdHoldItem(item, holdPosition);
        }

        #endregion

        #region Public Methods

        public void OnHover(PlayerInteraction playerInteraction) { }

        public void Interact(PlayerInteraction playerInteraction) { }

        #endregion

        #region Protected Methods

        protected void GetItemFromPlayer(PlayerInteraction playerInteraction) { }

        #endregion

        #region Private Methods

        [Command]
        private void CmdHoldItem() { }

        [Command]
        protected void CmdAssignItemToPlayer() { }

        #endregion
    }
}