using System.Collections;
using DarkKey.Core.Debugger;
using DarkKey.Core.Managers;
using DarkKey.Core.Network;
using DarkKey.Gameplay;
using Mirror;
using UnityEngine;

namespace DarkKey.Ui
{
    public class UiPausePanel : NetworkBehaviour
    {
        private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.Core, DebugLogLevel.Network};

        [SerializeField] private GameObject mainPanel;
        private InputHandler _inputHandler;
        private bool _isHidden;

        #region Unity Methods

        public override void OnStartClient()
        {
            mainPanel.SetActive(false);
            StartCoroutine(FetchPlayerInputHandler());
        }

        private void OnDestroy()
        {
            if (_inputHandler != null)
                _inputHandler.OnEscape -= Menu;
        }

        #endregion

        #region Public Methods

        public void Resume() => DisableMenu();

        public void Disconnect()
        {
            DisableMenu();
            NetPortal.Instance.Disconnect();
        }

        public void Quit()
        {
            DisableMenu();
            NetPortal.Instance.Disconnect();
            ServiceLocator.Instance.GetGameManager().QuitGame();
        }

        #endregion

        #region Private Methods

        private void Menu()
        {
            if (_isHidden)
                EnableMenu();
            else
                DisableMenu();
        }

        private void EnableMenu()
        {
            mainPanel.SetActive(true);
            _isHidden = false;

            CursorManager.ShowCursor();
            _inputHandler.actionMap = InputHandler.InputActionMap.Ui;

            ServiceLocator.Instance.GetDebugger().LogInfoToServer("EnableMenu Executed", ScriptLogLevel);
        }

        private void DisableMenu()
        {
            mainPanel.SetActive(false);
            _isHidden = true;

            CursorManager.HideCursor();
            _inputHandler.actionMap = InputHandler.InputActionMap.Gameplay;

            ServiceLocator.Instance.GetDebugger().LogInfoToServer("Disable Executed", ScriptLogLevel);
        }

        private IEnumerator FetchPlayerInputHandler()
        {
            var timePassed = 0f;

            while (_inputHandler == null)
            {
                var localPlayer = NetworkClient.localPlayer;

                if (localPlayer == null) yield return null;

                if (localPlayer.TryGetComponent(out InputHandler inputHandler))
                {
                    _inputHandler = inputHandler;
                    _inputHandler.OnEscape += Menu;
                    yield break;
                }

                timePassed += Time.deltaTime;
                if (timePassed >= 60f)
                {
                    ServiceLocator.Instance.GetDebugger().LogErrorToServer("Unable to fetch inputHandler.", ScriptLogLevel);
                    yield break;
                }

                yield return new WaitForSeconds(0.3f);
            }

            yield return null;
        }

        #endregion
    }
}