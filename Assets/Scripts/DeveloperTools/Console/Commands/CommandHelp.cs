using System.Linq;

namespace DarkKey.DeveloperTools.Console.Commands
{
    public sealed class CommandHelp : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Format { get; protected set; }
        public override bool HasArguments { get; protected set; }

        public CommandHelp()
        {
            Name = "Help";
            Command = "help";
            Description = "Display all available commands.";
            Format = "\"help\" no_args";
            HasArguments = false;

            AddCommandToConsole();
        }

        public override void ExecuteCommand(string[] args)
        {
            ConsoleCommand[] availableCommands = DeveloperConsole.Commands.Values.ToArray();
            var helpMessage = "";

            foreach (var consoleCommand in availableCommands)
                helpMessage += $"\"{consoleCommand.Command}\" {consoleCommand.Description}\n";

            if (string.IsNullOrWhiteSpace(helpMessage)) return;
            DeveloperConsole.Instance.AddFormattedOutputToConsole("<style=H3>Available Commands:</style>\n"+ helpMessage, '#', "black");
        }
    }
}