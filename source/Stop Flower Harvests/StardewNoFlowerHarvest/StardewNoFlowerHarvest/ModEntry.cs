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

namespace StopFlowerHarvests
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            // example patch, you'll need to edit this for your patch
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Crop), nameof(StardewValley.Crop.GetHarvestMethod)),
               postfix: new HarmonyMethod(typeof(Patch), nameof(Patch.Postfix_GetHarvestMethod))
            );
            //helper.Events.World.DebrisListChanged += this.CheckObjectListChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
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
