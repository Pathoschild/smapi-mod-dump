/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seichen/Stardew-Stop-Flower-Harvests
**
*************************************************/

using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Crops;
using GenericModConfigMenu;

namespace StopFlowerHarvests
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {

        public static ModConfig Config;
        private IGenericModConfigMenuApi? configMenu;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            Config = this.Helper.ReadConfig<ModConfig>();

            // example patch, you'll need to edit this for your patch
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Crop), nameof(StardewValley.Crop.GetHarvestMethod)),
               postfix: new HarmonyMethod(typeof(Patch), nameof(Patch.Postfix_GetHarvestMethod))
            );
            //helper.Events.World.DebrisListChanged += this.CheckObjectListChanged;

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            this.configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (this.configMenu is null)
                return;

            // register mod
            this.configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(Config)
            );

            // add some config options
            this.configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Fairy Rose",
                tooltip: () => "Checking this will turn on scythe only harvesting.",
                getValue: () => Config.FairyRose,
                setValue: value => Config.FairyRose = value
            );
            this.configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Poppy",
                tooltip: () => "Checking this will turn on scythe only harvesting.",
                getValue: () => Config.Poppy,
                setValue: value => Config.Poppy = value
            );
            this.configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Blue Jazz",
                tooltip: () => "Checking this will turn on scythe only harvesting.",
                getValue: () => Config.BlueJazz,
                setValue: value => Config.BlueJazz = value
            );
        }
        //private void CheckObjectListChanged(object? sender, DebrisListChangedEventArgs e)
        //{
        // ignore if player hasn't loaded a save yet
        //if (!Context.IsWorldReady)
        //return;

        // print button presses to the console window
        //foreach (StardewValley.Debris kvp in e.Removed)
        //{
        //this.Monitor.Log($"{Game1.player.Name} removed {kvp.itemId}.", LogLevel.Debug);
        //}
        //}
    }
}
