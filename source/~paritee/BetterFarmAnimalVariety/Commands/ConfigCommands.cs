/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/paritee/Paritee.StardewValley.Frameworks
**
*************************************************/

using BetterFarmAnimalVariety.Models;
using Newtonsoft.Json;
using Paritee.StardewValleyAPI.FarmAnimals.Variations;
using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace BetterFarmAnimalVariety.Commands
{
    class ConfigCommands : BaseCommands
    {
        public ConfigCommands(ModConfig config, IModHelper helper, IMonitor monitor) : base(config, helper, monitor)
        {
            this.Commands = new List<Command>()
            {
                new Command("bfav_conf", "List all config commands.\nUsage: bfav_conf", this.ListCommands),
                new Command("bfav_conf_list", $"List the config.json settings.\nUsage: bfav_conf_list", this.ShowConfig),
                new Command("bfav_conf_voidshop", $"Set presence of void animals in shop.\nUsage: bfav_conf_voidshop <flag>\n- flag: {VoidConfig.InShop.Never}, {VoidConfig.InShop.QuestOnly}, {VoidConfig.InShop.Always}", this.VoidShop),
                new Command("bfav_conf_randfromcategory", $"Set newbown and hatchling randomization settings.\nUsage: bfav_conf_randfromcategory <newborn> <hatchling> <ignoreparentproduce>\n- newborn: true or false\n- hatchling: true or false\n- ignoreparentproduce: true or false", this.RandFromCategory),
            };
        }

        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ShowConfig(string command, string[] args)
        {
            string output = JsonConvert.SerializeObject(this.Config);

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void VoidShop(string command, string[] args)
        {
            if (args.Length < 1)
            {
                this.Monitor.Log($"flag is required", LogLevel.Error);
                return;
            }

            if (!Enum.TryParse(args[0], true, out VoidConfig.InShop flag))
            {
                this.Monitor.Log($"{args[0]} is not a valid flag", LogLevel.Error);
                return;
            }

            this.Config.VoidFarmAnimalsInShop = flag;

            this.HandleUpdatedConfig();

            this.Monitor.Log($"Config successfully updated.", LogLevel.Info);
        }

        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void RandFromCategory(string command, string[] args)
        {
            if (args.Length < 3)
            {
                this.Monitor.Log($"newbown, hatchling and ignoreparentproduce are required", LogLevel.Error);
                return;
            }

            if (!bool.TryParse(args[0], out bool newbown))
            {
                this.Monitor.Log($"newbown must be true or false", LogLevel.Error);
                return;
            }

            if (!bool.TryParse(args[1], out bool hatchling))
            {
                this.Monitor.Log($"hatchling must be true or false", LogLevel.Error);
                return;
            }

            if (!bool.TryParse(args[2], out bool ignoreparentproduce))
            {
                this.Monitor.Log($"ignoreparentproduce must be true or false", LogLevel.Error);
                return;
            }

            this.Config.RandomizeNewbornFromCategory = newbown;
            this.Config.RandomizeHatchlingFromCategory = hatchling;
            this.Config.IgnoreParentProduceCheck = ignoreparentproduce;

            this.HandleUpdatedConfig();

            this.Monitor.Log($"Config successfully updated.", LogLevel.Info);
        }
    }
}
