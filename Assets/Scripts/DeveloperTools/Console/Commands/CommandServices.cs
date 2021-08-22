using DarkKey.Core.Managers;

namespace DarkKey.DeveloperTools.Console.Commands
{
    public class CommandServices : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Format { get; protected set; }
        public override bool HasArguments { get; protected set; }

        public CommandServices()
        {
            Name = "Services";
            Command = "services";
            Description = "Lists all initialized services.";
            Format = "\"services\" no_args";
            HasArguments = false;
            
            AddCommandToConsole();
        }

        public override void ExecuteCommand(string[] args)
        {
            var services = ServiceLocator.Instance.Services;
            foreach (var service in services)
            {
                DeveloperConsole.Instance.AddMessageToConsole($"Service: {service.Key}, GameObject: {service.Value}.");
            }
        }
    }
}