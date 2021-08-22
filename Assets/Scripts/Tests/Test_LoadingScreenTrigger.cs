using DarkKey.Core.Network;
using UnityEngine;

namespace DarkKey.Tests
{
    public class Test_LoadingScreenTrigger : MonoBehaviour
    {
        public NetworkSceneManagerDk NetworkSceneManagerDk;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                NetworkSceneManagerDk.SwitchToOnlineScene();
            }
        }
    }
}