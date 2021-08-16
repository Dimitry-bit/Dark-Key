using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using Mirror;
using UnityEngine;

namespace DarkKey.Gameplay.CorePlayer
{
    public class ScenePlayer : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Network, DebugLogLevel.Player};

        public PlayerData PlayerData { get; private set; }

        #region Unity Methods

        public override void OnStartClient()
        {
            if (!isLocalPlayer) return;

            DisableUnownedCameras();
        }

        #endregion

        #region Public Methods

        public void InitializePlayerData(PlayerData playerData)
        {
            PlayerData = new PlayerData(playerData.ClientId, playerData.Name, playerData.Role);
            // DisableUnownedCameras();

            ServiceLocator.Instance.GetDebugger().LogInfoToServer("ScenePlayer Initialized", ScriptLogLevel);
        }

        #endregion

        #region Private Methods

        private void DisableUnownedCameras()
        {
            var ownedCamera = GetComponentInChildren<Camera>();
            if (ownedCamera != null)
            {
                ownedCamera.enabled = true;
                if (ownedCamera.TryGetComponent(out AudioListener ownedAudioListener))
                    ownedAudioListener.enabled = true;
            }

            var sceneCameras = FindObjectsOfType<Camera>();
            foreach (var sceneCamera in sceneCameras)
            {
                if (sceneCamera != ownedCamera)
                    sceneCamera.gameObject.SetActive(false);
            }
        }

        #endregion
    }
}