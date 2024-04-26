/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/PollenSprites
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace PollenSprites
{
    public static class GMCM
    {
        public static void Initialize(object sender, GameLaunchedEventArgs e)
        {
            IManifest manifest = ModEntry.Instance.ModManifest; //get this mod's manifest

            var api = ModEntry.Instance.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api == null)
            {
                if (ModEntry.Instance.Monitor.IsVerbose)
                    ModEntry.Instance.Monitor.Log($"GMCM does not seem to be available. Skipping menu setup.", LogLevel.Trace);
                return;
            }

            api.Register(manifest,
                () => ModEntry.ModConfig = new ModConfig(),
                () => ModEntry.Instance.Helper.WriteConfig(ModEntry.ModConfig)
            );

            //register an option for each of this mod's config settings
            api.AddBoolOption(
                manifest,
                () => ModEntry.ModConfig.EnableSlowDebuff,
                (bool val) => ModEntry.ModConfig.EnableSlowDebuff = val,
                () => "Enable slow debuff",
                () => "If this box is checked, Pollen Sprites will apply a slow effect when they touch you.\nIn multiplayer, this option only affects you."

            );

            api.AddBoolOption(
                manifest,
                () => ModEntry.ModConfig.EnableEnergyDrain,
                (bool val) => ModEntry.ModConfig.EnableEnergyDrain = val,
                () => "Enable energy drain",
                () => "If this box is checked, Pollen Sprites will slowly drain your energy when they touch you (but never below 10 points).\nIn multiplayer, this option only affects you."
            );

            api.AddSectionTitle(
                manifest,
                () => "Seed drop chances",
                () => "When Pollen Sprites are defeated, these options decide how often they drop seeds.\nUse 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc."
            );

            api.AddNumberOption(
                manifest,
                () => ModEntry.ModConfig.SeedDropChances.MixedSeeds,
                (float val) => ModEntry.ModConfig.SeedDropChances.MixedSeeds = val,
                () => "Mixed seeds",
                () => "The chance that Pollen Sprites will drop mixed seeds.\nUse 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc."
            );

            api.AddNumberOption(
                manifest,
                () => ModEntry.ModConfig.SeedDropChances.FlowerSeeds,
                (float val) => ModEntry.ModConfig.SeedDropChances.FlowerSeeds = val,
                () => "Mixed flower seeds",
                () => "The chance that Pollen Sprites will drop mixed flower seeds.\nUse 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc."
            );

            api.AddNumberOption(
                manifest,
                () => ModEntry.ModConfig.SeedDropChances.AllSeeds,
                (float val) => ModEntry.ModConfig.SeedDropChances.AllSeeds = val,
                () => "All seeds",
                () => "The chance that Pollen Sprites will drop random seeds, including from modded crops.\nUse 0 for a 0% chance, 0.45 for 45%, 1 for 100%, etc."
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
