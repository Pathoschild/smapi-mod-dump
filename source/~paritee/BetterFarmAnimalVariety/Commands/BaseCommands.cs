using BetterFarmAnimalVariety.Models;
using StardewModdingAPI;
using System.Collections.Generic;

namespace BetterFarmAnimalVariety.Commands
{
    class BaseCommands
    {
        public List<Command> Commands = new List<Command>();

        protected readonly ModConfig Config;
        protected readonly IModHelper Helper;
        protected readonly IMonitor Monitor;

        public BaseCommands(ModConfig config, IModHelper helper, IMonitor monitor)
        {
            this.Config = config;
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public void SetUp()
        {
            foreach (Command command in this.Commands)
            {
                this.Helper.ConsoleCommands.Add(command.Name, command.Documentation, command.Callback);
            }
        }

        protected void HandleUpdatedConfig()
        {
            this.Helper.WriteConfig<ModConfig>(this.Config);
        }

        /// <summary>List all farm animal commands when the 'bfav_fa' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        protected void ListCommands(string command, string[] args)
        {
            foreach (Command cmd in this.Commands)
            {
                this.Helper.ConsoleCommands.Trigger($"help", new string[1] { cmd.Name });
            }
        }
    }
}
