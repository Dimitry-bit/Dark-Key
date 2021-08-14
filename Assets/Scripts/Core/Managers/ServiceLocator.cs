using System;
using System.Collections.Generic;
using DarkKey.Core.Debugger;
using DarkKey.Core.Network;
using DarkKey.Ui.Pages;
using UnityEngine;

namespace DarkKey.Core.Managers
{
    public class ServiceLocator : MonoBehaviour
    {
        public static ServiceLocator Instance { get; private set; }
        private Dictionary<Type, MonoBehaviour> _services;

        public NetworkSceneManagerDk networkSceneManager;
        public CustomDebugger customDebugger;
        public PageController pageController;
        public GameManager gameManager;

        #region Unity Methods

        private void Awake()
        {
            SingletonInitialize();
            ServicesInitialize();
        }

        private void ServicesInitialize()
        {
            _services = new Dictionary<Type, MonoBehaviour>();

            var customDebuggerService = GetDebugger();
            var networkSceneManagerService = GetNetworkSceneManager();
            var gameManagerService = GetGameManager();
            var pageControllerService = GetPageController();

            _services.Add(typeof(CustomDebugger), customDebuggerService);
            _services.Add(typeof(NetworkSceneManagerDk), networkSceneManagerService);
            _services.Add(typeof(GameManager), gameManagerService);
            _services.Add(typeof(PageController), pageControllerService);

            foreach (var service in _services.Values)
            {
                if (service == null) return;
                DontDestroyOnLoad(service.transform.root);
            }
        }

        private void SingletonInitialize()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);

            DontDestroyOnLoad(transform.root);
        }

        #endregion

        #region Public Methods

        public CustomDebugger GetDebugger()
        {
            if (_services.ContainsKey(typeof(CustomDebugger)))
                return (CustomDebugger) _services[typeof(CustomDebugger)];

            if (TryFindService(typeof(CustomDebugger), out MonoBehaviour service))
                return (CustomDebugger) service;

            return null;
        }

        public GameManager GetGameManager()
        {
            if (_services.ContainsKey(typeof(GameManager)))
                return (GameManager) _services[typeof(GameManager)];

            if (TryFindService(typeof(GameManager), out MonoBehaviour service))
                return (GameManager) service;

            return null;
        }

        public NetworkSceneManagerDk GetNetworkSceneManager()
        {
            if (_services.ContainsKey(typeof(NetworkSceneManagerDk)))
                return (NetworkSceneManagerDk) _services[typeof(NetworkSceneManagerDk)];

            if (TryFindService(typeof(NetworkSceneManagerDk), out MonoBehaviour service))
                return (NetworkSceneManagerDk) service;

            return null;
        }

        public PageController GetPageController()
        {
            if (_services.ContainsKey(typeof(PageController)))
                return (PageController) _services[typeof(PageController)];

            if (TryFindService(typeof(PageController), out MonoBehaviour service))
                return (PageController) service;

            return null;
        }

        #endregion

        #region Private Methods

        private bool TryFindService(Type serviceType, out MonoBehaviour service)
        {
            MonoBehaviour foundService = (MonoBehaviour) FindObjectOfType(serviceType);
            Debug.Log(foundService.name);

            service = null;

            if (foundService != null)
            {
                service = foundService;
                return true;
            }

            Debug.LogError("Couldn't find service.");
            return false;
        }

        #endregion
    }
}