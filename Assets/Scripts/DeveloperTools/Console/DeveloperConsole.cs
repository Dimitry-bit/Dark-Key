using System.Collections.Generic;
using System.Linq;
using DarkKey.DeveloperTools.Console.Commands;
using DarkKey.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkKey.DeveloperTools.Console
{
    public class DeveloperConsole : MonoBehaviour
    {
        [Header("Ui Components")]
        [SerializeField] private GameObject consolePanel;
        [SerializeField] private TMP_Text consoleText;
        [SerializeField] private TMP_InputField consoleInputField;
        [SerializeField] private ScrollRect scrollRect;

        public static DeveloperConsole Instance { get; private set; }
        public static Dictionary<string, ConsoleCommand> Commands { get; private set; }

        private bool _isPanelEnabled;
        private InputHandler _inputHandler;

        #region Unity Methods

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            Commands = new Dictionary<string, ConsoleCommand>();

            Application.logMessageReceived += HandleLogReceived;
            DontDestroyOnLoad(gameObject);

            consoleText.text = "";
            scrollRect.verticalNormalizedPosition = 0f;
            AddMessageToConsole("Developer Console Initialized.");
        }

        private void Start()
        {
            consolePanel.SetActive(false);
            CreateCommands();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                consolePanel.SetActive(!consolePanel.activeSelf);

            if (Input.GetKeyDown(KeyCode.Return))
            {
                ProcessInput(consoleInputField.text);
                consoleInputField.text = "";
            }
        }

        private void OnDestroy() => Application.logMessageReceived -= HandleLogReceived;

        #endregion

        #region Public Methods

        public static void AddCommandToConsole(string name, ConsoleCommand command)
        {
            if (!Commands.ContainsKey(name))
                Commands.Add(name, command);
        }

        public void AddMessageToConsole(string message, string textColor = "white")
        {
            consoleText.text += $"<color={textColor}>{message}</color>" + "\n";
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public void AddFormattedOutputToConsole(string output, char delimiter, string textColor = "white")
        {
            var delimiterRepeated = new string(delimiter, 30);

            output = output.TrimEnd('\n');

            var formattedMessage =
                $"{delimiterRepeated}\n" +
                $"{output}\n" +
                $"{delimiterRepeated}";

            AddMessageToConsole(formattedMessage, textColor);
        }

        public void ClearConsole() => consoleText.text = "";

        #endregion

        #region Private Methods

        private void CreateCommands()
        {
            var commandHelp = new CommandHelp();
            var commandClear = new CommandClear();
            var commandQuit = new CommandQuit();
            var commandSpawn = new CommandSpawn();
        }

        private void ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            string[] formattedInput = input.Split(' ');

            if (formattedInput.Length == 0 || !Commands.ContainsKey(formattedInput[0]))
            {
                AddMessageToConsole($"\"{input}\" Command not recognized, \"help\" to list all available commands.");
                return;
            }

            AddMessageToConsole(input);

            ConsoleCommand command = Commands[formattedInput[0]];
            var commandArgs = formattedInput.Skip(1).ToArray();

            if (commandArgs.Contains("-help"))
            {
                DisplayCommandHelpMessage(command);
                return;
            }

            switch (command.HasArguments)
            {
                // Invalid command no arguments provided.
                case true when commandArgs.Length == 0:
                    AddMessageToConsole($"Command not recognized. \"{formattedInput[0]} -help\" for help");
                    return;
                // Invalid command doesn't take any arguments. 
                case false when commandArgs.Length != 0:
                    AddMessageToConsole($"Command not recognized. \"{formattedInput[0]} -help\" for help");
                    return;
                default:
                    command.ExecuteCommand(commandArgs);
                    break;
            }
        }

        private void DisplayCommandHelpMessage(ConsoleCommand command)
        {
            var delimiter = new string('-', 30);

            var helpMessage =
                "<style=H3>Description:</style>\n" +
                $"{command.Description}\n" +
                $"{delimiter}\n" +
                "<style=H3>Format:</style>\n" +
                $"{command.Format}\n";

            AddFormattedOutputToConsole(helpMessage, '=', "black");
        }

        private void HandleLogReceived(string logMessage, string stacktrace, LogType type)
        {
            var message = $"[{type.ToString()}]: {logMessage}";

            string textColor = type switch
            {
                LogType.Log => "white",
                LogType.Warning => "yellow",
                LogType.Error => "red",
                LogType.Assert => "red",
                LogType.Exception => "red",
                _ => "white"
            };

            AddMessageToConsole(message, textColor);
        }

        #endregion
    }
}