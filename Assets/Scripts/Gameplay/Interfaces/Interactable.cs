using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay.Interfaces
{
    public abstract class Interactable : NetworkBehaviour
    {
        [SerializeField] private string objectName;
        
        public abstract void OnSelected(PlayerInteraction playerInteraction);
        public abstract void Interact(PlayerInteraction playerInteraction);
    }
}