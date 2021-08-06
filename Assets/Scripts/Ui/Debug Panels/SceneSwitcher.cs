using System.Collections.Generic;
using System.IO;
using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.Ui.Debug_Panels
{
    public class SceneSwitcher : BaseDebugPanel
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.UI};

        [Header("Options")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] [Scene] private string[] switchableScenes;

        private NetworkIdentity _networkIdentity;
        private readonly List<Button> _buttons = new List<Button>();

        #region Unity Methods

        protected override void Start()
        {
            _networkIdentity = GetComponentInParent<NetworkIdentity>();
            base.Start();
        }

        private void OnDestroy() => UnSubscribeAllButtons();

        #endregion

        #region Private Methods

        protected override void EnablePanel()
        {
            if (NetworkManager.singleton == null || !NetworkManager.singleton.isNetworkActive) return;

            if (_networkIdentity.isServer)
            {
                base.EnablePanel();
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (_buttons.Count != 0) return;

            ServiceLocator.Instance.customDebugger.LogInfo("Try to fetch level data", ScriptLogLevel);
            InitializePanel();
        }

        protected override void InitializePanel()
        {
            if (NetworkManager.singleton == null || !NetworkManager.singleton.isNetworkActive) return;

            foreach (var scenePath in switchableScenes)
            {
                ServiceLocator.Instance.customDebugger.LogInfo($"Initializing => ({scenePath})", ScriptLogLevel);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                CreateButton(sceneName);
            }

            SubscribeAllButtons();
        }

        private void CreateButton(string buttonName)
        {
            GameObject buttonGameObject = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, gridTransform);
            Button buttonScript = buttonGameObject.GetComponent<Button>();
            TMP_Text tmp = buttonScript.GetComponentInChildren<TMP_Text>();

            buttonGameObject.name = buttonName;
            tmp.text = buttonName;

            _buttons.Add(buttonScript);
        }

        private void SubscribeAllButtons()
        {
            foreach (var button in _buttons) SubscribeToEvent(button);
        }

        private void UnSubscribeAllButtons()
        {
            foreach (var button in _buttons) UnSubscribeToEvent(button);
        }

        private void SubscribeToEvent(Button button)
        {
            if (button != null)
                button.onClick.AddListener(() => ChangeLevel(button.name));
        }

        private void UnSubscribeToEvent(Button button)
        {
            if (button != null)
                button.onClick.RemoveAllListeners();
        }

        private void ChangeLevel(string levelName)
        {
            ChangeLevel(_networkIdentity.connectionToServer.connectionId, levelName);
        }

        private void ChangeLevel(int clientConnectionId, string levelName)
        {
            if (NetworkServer.connections.ContainsKey(clientConnectionId))
            {
                NetworkIdentity client = NetworkServer.connections[clientConnectionId].identity;
                if (client == null || !client.isLocalPlayer) return;
            }

            ServiceLocator.Instance.networkSceneManager.SwitchScene(levelName);
        }

        #endregion
    }
}