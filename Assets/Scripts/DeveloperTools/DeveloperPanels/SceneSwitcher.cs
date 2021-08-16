using System.Collections.Generic;
using System.IO;
using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.DeveloperTools.DeveloperPanels
{
    public class SceneSwitcher : DeveloperPanel
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.UI};

        [Header("Scene Switcher Options")]
        [SerializeField] private GameObject buttonTemplate;
        [SerializeField] [Scene] private string[] switchableScenes;
        private Dictionary<string, Button> _registeredScenes;
        private NetworkIdentity _networkIdentity;

        #region Unity Methods

        protected override void Start()
        {
            base.Start();
            _registeredScenes = new Dictionary<string, Button>();
        }

        private void OnDestroy() => UnSubscribeAllButtons();

        #endregion

        #region Protected Methods

        protected override void InitializePanel()
        {
            if (NetworkManager.singleton == null || !NetworkManager.singleton.isNetworkActive)
            {
                DisablePanel();
                return;
            }

            if (!_networkIdentity.isServer)
            {
                DisablePanel();
                return;
            }

            if (_networkIdentity == null)
                _networkIdentity = GetComponentInChildren<NetworkIdentity>();

            ServiceLocator.Instance.GetDebugger().LogInfoToServer("Fetching levels data", ScriptLogLevel);

            foreach (var scenePath in switchableScenes)
                RegisterLevel(scenePath);
        }

        protected override void OnPanelEnable() => InitializePanel();

        #endregion

        #region Private Methods

        private void RegisterLevel(string scenePath)
        {
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            if (_registeredScenes == null || _registeredScenes.ContainsKey(sceneName)) return;

            Button button = CreateButton(sceneName);
            _registeredScenes.Add(sceneName, button);
            button.onClick.AddListener(() => ChangeLevel(button.name));

            ServiceLocator.Instance.GetDebugger().LogInfoToServer($"Added ({scenePath}) to scenes list.", ScriptLogLevel);
        }

        private void ChangeLevel(string levelName)
        {
            var clientConnectionId = _networkIdentity.connectionToServer.connectionId;
            if (NetworkServer.connections.ContainsKey(clientConnectionId))
            {
                NetworkIdentity client = NetworkServer.connections[clientConnectionId].identity;
                if (client == null || !client.isLocalPlayer) return;
            }

            ServiceLocator.Instance.GetNetworkSceneManager().SwitchScene(levelName);
        }

        private void UnSubscribeAllButtons()
        {
            foreach (var button in _registeredScenes.Values)
            {
                if (button != null)
                    button.onClick.RemoveAllListeners();
            }
        }

        private Button CreateButton(string buttonName)
        {
            GameObject buttonGameObject = Instantiate(buttonTemplate, Vector3.zero, Quaternion.identity, gridTransform);
            Button myButton = buttonGameObject.GetComponent<Button>();
            TMP_Text tmp = myButton.GetComponentInChildren<TMP_Text>();

            buttonGameObject.name = buttonName;
            tmp.text = buttonName;

            return myButton;
        }

        #endregion
    }
}