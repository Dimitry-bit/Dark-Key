using DarkKey.Core.Managers;
using UnityEngine;

namespace DarkKey.Tests
{
    public class Test_NetworkSceneManagerDk : MonoBehaviour
    {
#if UNITY_EDITOR

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                ServiceLocator.Instance.networkSceneManager.SwitchToOfflineScene();
            }
        }

#endif
    }
}