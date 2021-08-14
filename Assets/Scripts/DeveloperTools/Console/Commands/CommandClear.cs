namespace DarkKey.DeveloperTools.Console.Commands
{
    public sealed class CommandClear : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Format { get; protected set; }
        public override bool HasArguments { get; protected set; }

        public CommandClear()
        {
            Name = "Clear";
            Command = "clear";
            Description = "Clears console.";
            Format = "\"clear\" no_args";
            HasArguments = false;

            AddCommandToConsole();
        }

        public override void ExecuteCommand(string[] args) => DeveloperConsole.Instance.ClearConsole();
    }
}