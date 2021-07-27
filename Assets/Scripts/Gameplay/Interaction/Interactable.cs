using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay.Interaction
{
    public abstract class Interactable : NetworkBehaviour
    {
        [SerializeField] private string objectName;
        public string ObjectName => objectName;
        public string InteractionDescription { get; protected set; }

        public virtual void OnSelected(PlayerInteraction playerInteraction) =>
            InteractionDescription = $"Interact With {objectName}";

        public abstract void Interact(PlayerInteraction playerInteraction);
    }
}