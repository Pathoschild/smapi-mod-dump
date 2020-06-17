using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework
{
    class Commands
    {
        public static void addCommands()
        {
            ModEntry.modHelper.ConsoleCommands.Add(
                "seed_machines_setsetting",
                "Dynamically change value in the settings and save it in your settings.json file in mod directory.\n\n"
                + "Usage: seed_machines_setsetting <parameterName> <value>\n"
                + "- parameterName: Name of the parameter in settings.json.\n"
                + "- value: Value of the parameter.",
                setSetting
            );
        }

        public static void setSetting(string command, string[] args)
        {
            String attributeName = args[0];
            switch (attributeName)
            {
                case "seedMachinePrice":
                    ModEntry.settings.seedMachinePrice = int.Parse(args[1]);
                    ModEntry.writeSettings();
                    break;
                case "seedMachineIngredients":
                    ModEntry.settings.seedMachineIngredients = args[1];
                    ModEntry.writeSettings();
                    ModEntry.monitor.Log("Ingridients for Seed Machine was changed, but you need to rerun the game for apply changes.", LogLevel.Warn);
                    break;
                case "seedMachinePriceForNonSalableSeeds":
                    ModEntry.settings.seedMachinePriceForNonSalableSeeds = int.Parse(args[1]);
                    ModEntry.writeSettings();
                    break;
                case "seedMachinePriceMultiplier":
                    ModEntry.settings.seedMachinePriceMultiplier = double.Parse(args[1]);
                    ModEntry.writeSettings();
                    break;
                case "seedBanditPrice":
                    ModEntry.settings.seedBanditPrice = int.Parse(args[1]);
                    ModEntry.writeSettings();
                    break;
                case "seedBanditIngredients":
                    ModEntry.settings.seedBanditIngredients = args[1];
                    ModEntry.writeSettings();
                    ModEntry.monitor.Log("Ingridients for Seed Bandit was changed, but you need to rerun the game for apply changes.", LogLevel.Warn);
                    break;
                case "seedBanditOneGamePrice":
                    ModEntry.settings.seedBanditOneGamePrice = int.Parse(args[1]);
                    ModEntry.writeSettings();
                    break;
                case "themeName":
                    ModEntry.settings.themeName = args[1];
                    ModEntry.writeSettings();
                    ModEntry.monitor.Log("Theme was changed, but you need to rerun the game for apply changes.", LogLevel.Warn);
                    break;
                default:
                    ModEntry.monitor.Log("Setting name \"" + args[0] + "\" is invalid.", LogLevel.Error);
                    break;
            }
        }
    }
}
