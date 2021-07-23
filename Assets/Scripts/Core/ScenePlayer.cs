using MLAPI;
using DarkKey.Gameplay;
using DarkKey.Core.Debugger;

namespace DarkKey.Core
{
    public class ScenePlayer : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Network, DebugLogLevel.Player};

        public PlayerData PlayerData { get; private set; }

        #region Unity Methods

        #endregion

        #region Public Methods

        public void InitializePlayerData(PlayerData playerData)
        {
            PlayerData = new PlayerData(playerData.ClientId, playerData.Name, playerData.Role);
            CustomDebugger.LogInfo("ScenePlayer", $"{PlayerData.ClientId}, {PlayerData.Name}, {PlayerData.Role}",
                ScriptLogLevel);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}