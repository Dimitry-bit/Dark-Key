using DarkKey.Core;
using DarkKey.Network;
using MLAPI;
using MLAPI.Connection;
using MLAPI.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkKey.Ui
{
    public class UiDebugMenu : NetworkBehaviour
    {
        [SerializeField] private TMP_InputField debugInputField;
        [SerializeField] private GameObject debugPanel;
        [SerializeField] private GameObject mainPanel;

        [Header("Scene Settings")] [SerializeField]
        private TMP_Dropdown sceneDropdown;
        private string _newSceneName;
        private bool _hasNewSelectedScene;
        
        private NetworkClient _localClient;
        private NetPortal _netPortal;
        private InputHandler _inputHandler;

        public override void NetworkStart()
        {
            _netPortal = FindObjectOfType<NetPortal>();
            if (_netPortal == null)
                Debug.LogError("[UiPauseMenu]: NetPortal not found. Please place NetPortal script on an object.");

            // Get LocalClient GameObject
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.LocalClientId,
                out NetworkClient client))
            {
                _localClient = client;
                if (client.PlayerObject == null)
                {
                    // Waits 5sec To make sure player has spawned.
                    Invoke(nameof(GetInputHandler), 5);
                }
                else
                {
                    GetInputHandler();
                }
            }
        }
        
        private void OnDestroy()
        {
            if (sceneDropdown != null)
            {
                sceneDropdown.onValueChanged.RemoveListener(delegate { DropdownValueChanged(sceneDropdown); });
            }

            if (_netPortal == null) return;
            if (_inputHandler == null) return;
            _inputHandler.OnConsole -= ToggleConsole;
        }
        private void Start()
        {
            sceneDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(sceneDropdown); });
        }

        private void DropdownValueChanged(TMP_Dropdown change)
        {
            _newSceneName = sceneDropdown.options[change.value].text;
            _hasNewSelectedScene = true;
        }

        public void DebugCheck()
        {
            if (!(debugInputField.text == "Debug" || debugInputField.text == "debug")) return;
            EnableMenu();
        }

        private void ToggleConsole()
        {
            if (debugPanel.activeSelf)
            {
                debugPanel.SetActive(false);
                DisableMenu();
            }
            else
            {
                debugPanel.SetActive(true);
                CursorManager.ShowCursor();
                _inputHandler.disabledInput = DisableInput.All;
            }
        }

        private void EnableMenu()
        {
            debugPanel.SetActive(false);
            mainPanel.SetActive(true);
            CursorManager.ShowCursor();
            _inputHandler.disabledInput = DisableInput.All;
            SetSceneSettingsData();
        }

        public void DisableMenu()
        {
            mainPanel.SetActive(false);
            _hasNewSelectedScene = false;
            CursorManager.HideCursor();
            _inputHandler.disabledInput = DisableInput.None;
        }

        public void ApplyChanges()
        {
            if (_hasNewSelectedScene)
            {
                DisableMenu();
                NetworkSceneManager.SwitchScene(_newSceneName);
            }
        }

        private void SetSceneSettingsData()
        {
            Debug.Log(NetworkManager.Singleton.NetworkConfig.RegisteredScenes);
            sceneDropdown.ClearOptions();
            sceneDropdown.AddOptions(NetworkManager.Singleton.NetworkConfig.RegisteredScenes);
            sceneDropdown.value = SceneManager.GetActiveScene().buildIndex;
        }
        
        private void GetInputHandler()
        {
            if (_localClient == null) return;
            if (_localClient.PlayerObject == null) return;
            if (!_localClient.PlayerObject.IsSpawned) return;

            if (_localClient.PlayerObject.gameObject.TryGetComponent(out InputHandler inputHandler))
                _inputHandler = inputHandler;

            _inputHandler.OnConsole += ToggleConsole;
        }
    }
}