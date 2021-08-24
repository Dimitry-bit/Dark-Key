using DarkKey.Core.Managers;
using UnityEngine;

namespace DarkKey.Tests
{
    public class Test_LoadingScreenTrigger : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                var serviceLocator = ServiceLocator.Instance;
                if (serviceLocator != null && serviceLocator.GetNetworkSceneManager() != null)
                {
                    serviceLocator.GetNetworkSceneManager().SwitchToOnlineScene();
                }
            }
        }
    }
}