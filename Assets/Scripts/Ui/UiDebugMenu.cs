using DarkKey.Core;
using DarkKey.Core.Network;
using DarkKey.Gameplay;
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
        private InputHandler _inputHandler;

        #region Unity Methods

        private void Start()
        {
            sceneDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(sceneDropdown); });
            NetPortal.Instance.OnLocalConnection += GetInputHandlerAndAssignEvent;
        }

        private void OnDestroy()
        {
            if (sceneDropdown != null)
                sceneDropdown.onValueChanged.RemoveListener(delegate { DropdownValueChanged(sceneDropdown); });

            if (NetPortal.Instance != null)
                NetPortal.Instance.OnLocalConnection -= GetInputHandlerAndAssignEvent;

            if (_inputHandler != null)
                _inputHandler.OnConsole -= ToggleConsole;
        }

        #endregion

        #region Public Methods

        public void DebugCheck()
        {
            if (!(debugInputField.text == "Debug" || debugInputField.text == "debug")) return;
            EnableMenu();
        }

        public void DisableMenu()
        {
            mainPanel.SetActive(false);
            _hasNewSelectedScene = false;
            CursorManager.HideCursor();
            _inputHandler.actionMap = InputHandler.InputActionMap.Gameplay;
        }

        public void ApplyChanges()
        {
            if (!_hasNewSelectedScene) return;

            DisableMenu();
            NetworkSceneManager.SwitchScene(_newSceneName);
        }

        #endregion

        #region Private Methods

        private void DropdownValueChanged(TMP_Dropdown change)
        {
            _newSceneName = sceneDropdown.options[change.value].text;
            _hasNewSelectedScene = true;
        }

        private void ToggleConsole()
        {
            if (!IsServer) return;

            if (debugPanel.activeSelf)
            {
                debugPanel.SetActive(false);
                DisableMenu();
            }
            else
            {
                debugPanel.SetActive(true);
                CursorManager.ShowCursor();
                _inputHandler.actionMap = InputHandler.InputActionMap.Ui;
            }
        }

        private void EnableMenu()
        {
            debugPanel.SetActive(false);
            mainPanel.SetActive(true);
            CursorManager.ShowCursor();
            _inputHandler.actionMap = InputHandler.InputActionMap.Ui;
            SetSceneSettingsData();
        }

        private void SetSceneSettingsData()
        {
            sceneDropdown.ClearOptions();
            sceneDropdown.AddOptions(NetworkManager.Singleton.NetworkConfig.RegisteredScenes);
            sceneDropdown.value = SceneManager.GetActiveScene().buildIndex;
        }

        private void GetInputHandlerAndAssignEvent()
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.LocalClientId,
                out NetworkClient client)) return;

            if (client.PlayerObject == null) return;
            if (!client.PlayerObject.IsSpawned) return;

            if (client.PlayerObject.gameObject.TryGetComponent(out InputHandler inputHandler))
                _inputHandler = inputHandler;

            _inputHandler.OnConsole += ToggleConsole;
        }

        #endregion
    }
}