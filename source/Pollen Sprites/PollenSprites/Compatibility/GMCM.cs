using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace PollenSprites
{
    public static class GMCM
    {
        public static void EnableGMCM(object sender, GameLaunchedEventArgs e)
        {
            IManifest manifest = ModEntry.Instance.ModManifest; //get this mod's manifest

            GenericModConfigMenuAPI api = ModEntry.Instance.Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

            if (api == null) //if the API is not available
                return;

            api.RegisterModConfig(manifest, () => ModEntry.ModConfig = new ModConfig(), () => ModEntry.Instance.Helper.WriteConfig(ModEntry.ModConfig)); //register "revert to default" and "write" methods for this mod's config

            //register an option for each of this mod's config settings
            api.RegisterSimpleOption(
                manifest, 
                "Enable slow debuff", 
                "If this box is checked, Pollen Sprites will apply a slow effect when they touch you.\nIn multiplayer, this option only affects you.",
                () => ModEntry.ModConfig.EnableSlowDebuff,
                (bool val) => ModEntry.ModConfig.EnableSlowDebuff = val
            );

            api.RegisterSimpleOption(
                manifest, 
                "Enable energy drain", 
                "If this box is checked, Pollen Sprites will slowly drain your energy when they touch you (but never below 10 points).\nIn multiplayer, this option only affects you.", 
                () => ModEntry.ModConfig.EnableEnergyDrain, 
                (bool val) => ModEntry.ModConfig.EnableEnergyDrain = val
            );

            api.RegisterLabel(
                manifest, 
                "Seed drop chances", 
                "When Pollen Sprites are defeated, these options decide how often they drop seeds.\nUse 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc."
            );

            api.RegisterSimpleOption(
                manifest, 
                "Mixed seeds", 
                "The chance that Pollen Sprites will drop mixed seeds.\nUse 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc.", 
                () => ModEntry.ModConfig.SeedDropChances.MixedSeeds.ToString(), //read this setting as a string
                (string val) =>
                {
                    if (double.TryParse(val, out double result)) //if the string can be parsed to a double
                        ModEntry.ModConfig.SeedDropChances.MixedSeeds = result; //use the parsed value
                }
            );

            api.RegisterSimpleOption(manifest, 
                "Flower seeds", 
                "The chance that Pollen Sprites will drop random flower seeds.\nUse 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc.", 
                () => ModEntry.ModConfig.SeedDropChances.FlowerSeeds.ToString(), //read this setting as a string
                (string val) =>
                {
                    if (double.TryParse(val, out double result)) //if the string can be parsed to a double
                        ModEntry.ModConfig.SeedDropChances.FlowerSeeds = result; //use the parsed value
                }
            );

            api.RegisterSimpleOption(
                manifest, 
                "All seeds", 
                "The chance that Pollen Sprites will drop ANY random seeds, including from modded crops.\nUse 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc.",
                () => ModEntry.ModConfig.SeedDropChances.AllSeeds.ToString(), //read this setting as a string
                (string val) =>
                {
                    if (double.TryParse(val, out double result)) //if the string can be parsed to a double
                        ModEntry.ModConfig.SeedDropChances.AllSeeds = result; //use the parsed value
                }
            );
        }
    }

    /// <summary>Generic Mod Config Menu's API interface. Used to recognize & interact with the mod's API when available.</summary>
    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);
    }
}
