// using System.Collections.Generic;
// using MLAPI;
// using MLAPI.SceneManagement;
// using DarkKey.Core.Debugger;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace DarkKey.Ui.Debug_Panels
// {
//     public class LevelSelector : BaseDebugPanel
//     {
//         private static readonly DebugLogLevel[] ScriptLogLevel = {DebugLogLevel.UI};
//
//         [Header("LevelSelector Options")]
//         [SerializeField] private GameObject buttonPrefab;
//         private int _levelCount;
//         private readonly List<Button> _buttons = new List<Button>();
//
//         #region Unity Methods
//
//         private void OnDestroy() => UnSubscribeAllButtons();
//
//         #endregion
//
//         #region Private Methods
//
//         protected override void EnablePanel()
//         {
//             base.EnablePanel();
//
//             if (_buttons.Count != 0) return;
//
//             // CustomDebugger.LogInfo("LevelSelector", "Try to fetch level data", ScriptLogLevel);
//             Debug.Log("Try to fetch level data");
//             InitializePanel();
//         }
//
//         protected override void InitializePanel()
//         {
//             if (NetworkManager.Singleton == null) return;
//
//             List<string> registeredLevelsNames = NetworkManager.Singleton.NetworkConfig.RegisteredScenes;
//             _levelCount = registeredLevelsNames.Count;
//
//             foreach (var levelName in registeredLevelsNames)
//             {
//                 CustomDebugger.LogInfo($"Initializing => ({levelName})", ScriptLogLevel);
//                 CreateButton(levelName);
//             }
//
//             SubscribeAllButtons();
//         }
//
//         private void CreateButton(string buttonName)
//         {
//             GameObject buttonGameObject = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, gridTransform);
//             Button buttonScript = buttonGameObject.GetComponent<Button>();
//             TMP_Text text = buttonScript.GetComponentInChildren<TMP_Text>();
//
//             buttonGameObject.name = buttonName;
//             text.text = buttonName;
//
//             _buttons.Add(buttonScript);
//         }
//
//         private void SubscribeAllButtons()
//         {
//             foreach (var button in _buttons) SubscribeToEvent(button);
//         }
//
//         private void UnSubscribeAllButtons()
//         {
//             foreach (var button in _buttons) UnSubscribeToEvent(button);
//         }
//
//         private void SubscribeToEvent(Button button)
//         {
//             if (button != null)
//                 button.onClick.AddListener(() => ChangeLevel(button.name));
//         }
//
//         private void UnSubscribeToEvent(Button button)
//         {
//             if (button != null)
//                 button.onClick.RemoveAllListeners();
//         }
//
//         private void ChangeLevel(string levelName)
//         {
//             if (!IsServer) return;
//
//             // // TODO : Check if the scene is in build settings
//             //     CustomDebugger.LogWarning($"LevelSelector", $"Level: {levelName} hasn't been added to build settings",
//             //         ScriptLogLevel);
//
//             NetworkSceneManager.SwitchScene(levelName);
//         }
//
//         #endregion
//     }
// }