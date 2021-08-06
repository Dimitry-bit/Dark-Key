namespace DarkKey.Gameplay.CorePlayer
{
    public struct PlayerData
    {
        public int ClientId;
        public string Name;
        public string Role;

        public PlayerData(int clientId, string name, string role)
        {
            ClientId = clientId;
            Name = name;
            Role = role;
        }
    }
}