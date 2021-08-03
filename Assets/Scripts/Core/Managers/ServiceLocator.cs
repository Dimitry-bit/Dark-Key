using DarkKey.Core.Debugger;
using DarkKey.Core.Network;
using DarkKey.Ui.Pages;
using UnityEngine;

namespace DarkKey.Core.Managers
{
    public class ServiceLocator : MonoBehaviour
    {
        public static ServiceLocator Instance { get; private set; }

        public NetworkSceneManagerDk networkSceneManager;
        public CustomDebugger customDebugger;
        public PageController pageController;
        public GameManager gameManager;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);
        }
    }
}