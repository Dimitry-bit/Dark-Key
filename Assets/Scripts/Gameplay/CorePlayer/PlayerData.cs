using Mirror;

namespace DarkKey.Gameplay.CorePlayer
{
    public struct PlayerData
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
    }

    public static class CustomReadWriteFunctions
    {
        public static void WriteMyType(this NetworkWriter writer, PlayerData value)
        {
            writer.WriteULong(value.ClientId);
            writer.WriteString(value.Name);
            writer.WriteString(value.Role);
        }

        public static PlayerData ReadMyType(this NetworkReader reader)
        {
            return new PlayerData(reader.ReadULong(), reader.ReadString(), reader.ReadString());
        }
    }
}