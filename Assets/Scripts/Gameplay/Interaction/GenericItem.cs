using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class GenericItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string itemName;
        [SerializeField] private bool isPhysicsControlled;
        [Space]
        [SerializeField] private Vector3 inHandOffset;
        [SerializeField] private Vector3 inFrameOffset;
        private Rigidbody _rigidbody;
        public string ItemName => itemName;
        public string InteractionDescription { get; private set; }
        public Vector3 InHandOffset => inHandOffset;
        public Vector3 InFrameOffset => inFrameOffset;

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

        public void OnHover(PlayerInteraction playerInteraction) { }

        public void Interact(PlayerInteraction playerInteraction) { }

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

        #endregion
    }
}