/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using BetterFarmAnimalVariety.Commands;
using StardewModdingAPI;

namespace BetterFarmAnimalVariety
{
    public class ModCommand
    {
        private readonly ModConfig Config;
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private readonly ConfigCommands ConfigCommands;
        private readonly FarmAnimalsCommands FarmAnimalsCommands;

        public ModCommand(ModConfig config, IModHelper helper, IMonitor monitor)
        {
            this.Config = config;
            this.Helper = helper;
            this.Monitor = monitor;

            this.ConfigCommands = new ConfigCommands(this.Config, this.Helper, this.Monitor);
            this.FarmAnimalsCommands = new FarmAnimalsCommands(this.Config, this.Helper, this.Monitor);
        }

        public void SetUp()
        {
            this.ConfigCommands.SetUp();
            this.FarmAnimalsCommands.SetUp();

            this.Helper.ConsoleCommands.Add("bfav", "List all BFAV commands with help.\nUsage: bfav", this.ListCommands);
        }

        /// <summary>List all farm animal commands when the 'bfav_fa' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ListCommands(string command, string[] args)
        {
            this.Helper.ConsoleCommands.Trigger($"help", new string[1] { "bfav" });
            this.Helper.ConsoleCommands.Trigger($"bfav_conf", new string[] { });
            this.Helper.ConsoleCommands.Trigger($"bfav_fa", new string[] { });
        }
    }
}
