/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Adradis/StardewMods
**
*************************************************/

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.TerrainFeatures;

namespace LimitedHarvests
{
    internal sealed class ModEntry : Mod
    {
        private Harmony Harmony;

        public static ModEntry Instance;
        public IModHelper ModHelper;
        public ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Harmony = new Harmony(this.ModManifest.UniqueID);

            Instance = this;
            ModHelper = helper;
            Config = this.Helper.ReadConfig<ModConfig>();

            ModHelper.Events.GameLoop.GameLaunched += this.LoadModConfig;

            CropPatches.Patch(Harmony, this.Monitor);
        }

        public void LoadModConfig(object? sender, GameLaunchedEventArgs e)
        {
            var Config = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (Config == null)
            {
                Monitor.Log("Unable to register with Generic Mod Config Menu. If it is not installed, disregard this message.", LogLevel.Warn);
                return;
            }

            Config.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            Config.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Gentle Junimos",
                tooltip: () => "If enabled, Junimo Huts will not cause damage to the crop when harvesting.",
                getValue: () => this.Config.GentleJunimos,
                setValue: value => this.Config.GentleJunimos = value
            );

            Config.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Allow Harvest Override",
                tooltip: () => "If enabled, custom crops & Content Patcher patches can set their own harvest limits for crops",
                getValue: () => this.Config.AllowOverride,
                setValue: value => this.Config.AllowOverride = value
                );

            Config.AddTextOption(
                mod: this.ModManifest,
                name: () => "Harvest Limit Method",
                tooltip: () => "How the maximum harvests for a crop should be determined.",
                getValue: () => this.Config.HarvestCountMethod,
                setValue: value => this.Config.HarvestCountMethod = value,
                allowedValues: new string[] { "Fixed", "Harvests per Season", "(Hard) Harvests Per Season" }
                );

            Config.AddTextOption(
                mod: this.ModManifest,
                name: () => "Crop Randomization",
                getValue: () => this.Config.RandomizationSetting,
                setValue: value => this.Config.RandomizationSetting = value,
                allowedValues: new string[] { "None", "Per Crop", "Total" }
                );

            Config.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Harvest Values",
                tooltip: () =>"Sets harvest values for crop limits. Crop Random means all of a crop planted that day will share a harvest limit. Full Random means each individual plant can vary."
                );

            Config.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Fixed Number of Harvests",
                tooltip: () => "If Harvest Limit Method is set to fixed, the base number of times a crop will regrow.",
                getValue: () => this.Config.BaseHarvests,
                setValue: value =>
                    {
                        if (value < 0)
                        {
                            this.Config.BaseHarvests = 0;
                            return;
                        }

                        this.Config.BaseHarvests = value;
                    }
                );

            Config.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Lower Randomization Limit",
                tooltip: () => "The maximum number of harvests (inclusive) below the expected harvest that randomization can produce. Will never push a crop below 1 harvest.",
                getValue: () => this.Config.LowerRandModifier,
                setValue: value =>
                    {
                        if (value < 0 )
                        {
                            this.Config.LowerRandModifier = 0;
                            return;
                        }

                        this.Config.LowerRandModifier = value;
                    }
                );

            Config.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Upper Randomization Limit",
                tooltip: () => "The maximum number of harvests (inclusive) above the expected harvest that randomization can produce.",
                getValue: () => this.Config.UpperRandModifier,
                setValue: value =>
                    {
                        if (value < 0)
                        {
                            this.Config.UpperRandModifier = 0;
                            return;
                        }

                        this.Config.UpperRandModifier = value;
                    }
                );
        }
    }
}