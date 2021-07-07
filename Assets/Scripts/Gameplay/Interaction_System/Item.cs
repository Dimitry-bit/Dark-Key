using DarkKey.Gameplay.Interfaces;
using MLAPI;
using Unity.Collections;
using UnityEngine;

namespace DarkKey.Gameplay
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Item : NetworkBehaviour, IInteractable
    {
        [ReadOnly] public Vector3 inHandOffset;
        [ReadOnly] public Vector3 inItemHolderOffset;
        private Collider _collider;
        private Rigidbody _rigidbody;

        #region Public Methods

        public void DisablePhysics()
        {
            if (_rigidbody == null)
            {
                TryGetComponent(out _collider);
                TryGetComponent(out _rigidbody);
            }

            _collider.enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
        }

        public void LaunchIntoAir(float force)
        {
            EnablePhysics();
            _rigidbody.AddForce(transform.forward * force);
        }

        public virtual void Interact(Player player)
        {
            player.AssignItemToHand(this);
        }

        #endregion

        #region Private Methods

        private void EnablePhysics()
        {
            if (_rigidbody == null)
            {
                TryGetComponent(out _collider);
                TryGetComponent(out _rigidbody);
            }

            _collider.enabled = true;
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
        }

        #endregion
    }
}