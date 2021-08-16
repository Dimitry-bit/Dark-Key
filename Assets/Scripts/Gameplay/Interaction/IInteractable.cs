namespace DarkKey.Gameplay.Interaction
{
    public interface IInteractable
    {
        public string ItemName { get; }
        public string InteractionDescription { get; }

        public void OnHover(PlayerInteraction playerInteraction);
        public void Interact(PlayerInteraction playerInteraction);
    }
}