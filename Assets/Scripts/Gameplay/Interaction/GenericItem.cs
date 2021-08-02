using Mirror;
using Unity.Collections;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class GenericItem : Interactable
    {
        [ReadOnly] public Vector3 inHandOffset;
        [ReadOnly] public Vector3 inFrameOffset;
        [SerializeField] private bool isPhysicsControlled = false;

        private Rigidbody _rigidbody;

        #region Untiy Methods

        private void Start()
        {
            if (isPhysicsControlled)
            {
                isPhysicsControlled = false;
                EnablePhysics();
            }
            else
            {
                isPhysicsControlled = true;
                DisablePhysics();
            }
        }

        #endregion

        #region Public Methods

        public void ThrowObject(Vector3 force)
        {
            EnablePhysics();
            _rigidbody.AddForce(force);

            EnableItemForAllPlayersServerRpc();
        }

        public override void Interact(PlayerInteraction playerInteraction)
        {
            if (playerInteraction.IsHoldingItem()) return;

            DisablePhysics();
            playerInteraction.HoldItem(this);

            CmdDisableItemForOtherPlayers(playerInteraction.OwnerClientId);
        }

        #endregion

        #region Private Methods

        private void EnablePhysics()
        {
            if (isPhysicsControlled) return;

            FetchComponents(out var objectCollider);

            objectCollider.enabled = true;
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;

            isPhysicsControlled = true;
        }

        private void DisablePhysics()
        {
            if (!isPhysicsControlled) return;

            FetchComponents(out var objectCollider);

            objectCollider.enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;

            isPhysicsControlled = false;
        }

        private void FetchComponents(out Collider objectCollider)
        {
            objectCollider = GetComponent<Collider>();

            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
        }

        [Command(requiresAuthority= false)]
        public void CmdDisableItemForOtherPlayers(ulong interactedClientId) =>
            DisableItemForOtherPlayersItemClientRpc(interactedClientId);

        [ClientRpc]
        private void DisableItemForOtherPlayersItemClientRpc(ulong interactedClientId)
        {
            if (NetworkSpawnManager.GetLocalPlayerObject().OwnerClientId == interactedClientId) return;
            gameObject.SetActive(false);
        }

        [ServerRpc(RequireOwnership = false)]
        public void EnableItemForAllPlayersServerRpc() => EnableItemForAllPlayersClientRpc();

        [ClientRpc]
        private void EnableItemForAllPlayersClientRpc() => gameObject.SetActive(true);

        #endregion
    }
}