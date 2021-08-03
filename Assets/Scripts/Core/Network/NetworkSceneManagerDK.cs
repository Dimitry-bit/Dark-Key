using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Core.Network
{
    public class NetworkSceneManagerDk : NetworkBehaviour
    {
        [Header("Scene")]
        [SerializeField] private string offlineScene = "OfflineScene";
        [SerializeField] private string onlineScene = "Multiplayer_Test";

        #region Public Methods

        public void SwitchScene(Scene scene)
        {
        }

        public void SwitchToOfflineScene()
        {
            
        }

        public void SwitchToOnlineScene()
        {
                 
        }

        #endregion
    }
}