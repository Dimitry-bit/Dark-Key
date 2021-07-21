using MLAPI.Serialization;

namespace DarkKey.Gameplay
{
    public struct PlayerData : INetworkSerializable
    {
        public ulong ClientId;
        public string Name;
        public string Role;

        public PlayerData(ulong clientId, string name, string role)
        {
            ClientId = clientId;
            Name = name;
            Role = role;
        }

        public void NetworkSerialize(NetworkSerializer serializer)
        {
            serializer.Serialize(ref ClientId);
            serializer.Serialize(ref Name);
            serializer.Serialize(ref Role);
        }
    }
}