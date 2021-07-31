using DarkKey.Core.Debugger;
using MLAPI;
using UnityEngine;

namespace DarkKey.Gameplay.CorePlayer
{
    public class ScenePlayer : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Network, DebugLogLevel.Player};

        public PlayerData PlayerData { get; private set; }

        #region Public Methods

        public void InitializePlayerData(PlayerData playerData)
        {
            PlayerData = new PlayerData(playerData.ClientId, playerData.Name, playerData.Role);
            DisableUnownedAudioListeners();

            CustomDebugger.LogInfo("ScenePlayer", $"ScenePlayer Initialized", ScriptLogLevel);
        }

        #endregion

        #region Private Methods

        private void DisableUnownedAudioListeners()
        {
            var ownedAudioListener = GetComponentInChildren<AudioListener>();
            if (ownedAudioListener != null)
                ownedAudioListener.enabled = true;

            var sceneAudioListeners = FindObjectsOfType<AudioListener>();

            foreach (var audioListener in sceneAudioListeners)
            {
                if (audioListener == ownedAudioListener) continue;
                audioListener.enabled = false;
            }
        }

        #endregion
    }
}