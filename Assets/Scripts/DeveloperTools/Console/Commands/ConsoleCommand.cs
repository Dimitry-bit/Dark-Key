namespace DarkKey.DeveloperTools.Console.Commands
{
    public abstract class ConsoleCommand
    {
        public abstract string Name { get; protected set; }
        public abstract string Command { get; protected set; }
        public abstract string Description { get; protected set; }
        public abstract string Format { get; protected set; }
        public abstract bool HasArguments { get; protected set; }

        protected void AddCommandToConsole()
        {
            DeveloperConsole.AddCommandToConsole(Command, this);
            DeveloperConsole.Instance.AddMessageToConsole($"\"{Command}\" Command has been added to console.", "green");
        }

        public abstract void ExecuteCommand(string[] args);
    }
}