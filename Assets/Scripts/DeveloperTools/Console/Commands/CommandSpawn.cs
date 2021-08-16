using DarkKey.Gameplay.Interaction;
using UnityEngine;

namespace DarkKey.DeveloperTools.Console.Commands
{
    public sealed class CommandSpawn : ConsoleCommand
    {
        public override string Name { get; protected set; }
        public override string Command { get; protected set; }
        public override string Description { get; protected set; }
        public override string Format { get; protected set; }
        public override bool HasArguments { get; protected set; }

        public CommandSpawn()
        {
            Name = "Spawn";
            Command = "spawn";
            Description = "Spawns a gameObject";
            Format = "\"spawn <GameObject_Name>\" Takes 1_arg -> GameObject_Name";
            HasArguments = true;

            AddCommandToConsole();
        }

        public override void ExecuteCommand(string[] args)
        {
            if (args.Length > 1) return;

            var prefab = ItemUtility.GetPrefabByName(args[0]);
            GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }
}