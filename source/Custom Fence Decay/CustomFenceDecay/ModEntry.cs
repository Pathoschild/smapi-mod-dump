/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JeanSebGwak/CustomFenceDecay
**
*************************************************/

using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomFenceDecay.Configuration;
using CustomFenceDecay.GMCM;
using HarmonyLib;
using StardewValley.Characters;

namespace CustomFenceDecay
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration from the player.</summary>
        public static ModConfig Config = null!;
        public static ITranslationHelper I18n = null!;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Monitor.Log("Starting mod - Custom Fence Decay", LogLevel.Trace);

            Config = Helper.ReadConfig<ModConfig>();
            I18n = Helper.Translation;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            var harmony = new Harmony(ModManifest.UniqueID);

            HarmonyPatch_CustomFenceDecay.ApplyPatch(harmony, Monitor);
        }

        /*********
        ** Private methods
        *********/
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => I18n.Get("IdenticalValue.Name"),
                tooltip: () => I18n.Get("IdenticalValue.Tooltip"),
                getValue: () => Config.IdenticalValue,
                setValue: value => Config.IdenticalValue = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.Get("FenceDecaySpeedInPercent.Name"),
                tooltip: () => I18n.Get("FenceDecaySpeedInPercent.Tooltip"),
                getValue: () => Config.FenceDecaySpeedInPercent,
                setValue: value => Config.FenceDecaySpeedInPercent = value,
                min: 0, 
                max: 100
            );

            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => I18n.Get("IndependentValueSection.Text")
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => I18n.Get("IndependentValueParagraph.Text")
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.Get("WoodFenceDecaySpeedInPercent.Name"),
                tooltip: () => I18n.Get("WoodFenceDecaySpeedInPercent.Tooltip"),
                getValue: () => Config.WoodFenceDecaySpeedInPercent,
                setValue: value => Config.WoodFenceDecaySpeedInPercent = value,
                min: 0,
                max: 100
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.Get("StoneFenceDecaySpeedInPercent.Name"),
                tooltip: () => I18n.Get("StoneFenceDecaySpeedInPercent.Tooltip"),
                getValue: () => Config.StoneFenceDecaySpeedInPercent,
                setValue: value => Config.StoneFenceDecaySpeedInPercent = value,
                min: 0,
                max: 100
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.Get("IronFenceDecaySpeedInPercent.Name"),
                tooltip: () => I18n.Get("IronFenceDecaySpeedInPercent.Tooltip"),
                getValue: () => Config.IronFenceDecaySpeedInPercent,
                setValue: value => Config.IronFenceDecaySpeedInPercent = value,
                min: 0,
                max: 100
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => I18n.Get("HardwoodFenceDecaySpeedInPercent.Name"),
                tooltip: () => I18n.Get("HardwoodFenceDecaySpeedInPercent.Tooltip"),
                getValue: () => Config.HardwoodFenceDecaySpeedInPercent,
                setValue: value => Config.HardwoodFenceDecaySpeedInPercent = value,
                min: 0,
                max: 100
            );
        }

        private void OnSaveLoaded(object sender, EventArgs ex)
        {
            try
            {
                Config = Helper.ReadConfig<ModConfig>();
            }
            catch (Exception e)
            {
                Monitor.Log($"CustomFenceDecay: Failed to load config settings. Will use default settings instead. Error details:\n{e}", LogLevel.Debug);
                Config = new ModConfig();
            }
        }
    }
}
