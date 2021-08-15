using UnityEngine;

namespace DarkKey.DeveloperTools.Console.Commands
{
    public sealed class CommandQuit : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Format { get; protected set; }
        public override bool HasArguments { get; protected set; }

        public CommandQuit()
        {
            Name = "Quit";
            Command = "quit";
            Description = "Quits the application.";
            Format = "\"quit\" no_args";
            HasArguments = false;

            AddCommandToConsole();
        }

        public override void ExecuteCommand(string[] args)
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }
    }
}