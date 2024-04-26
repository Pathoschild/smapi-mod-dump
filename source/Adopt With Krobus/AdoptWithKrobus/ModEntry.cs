/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aeremyns/AdoptWithKrobus
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AdoptWithKrobus
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
            //helper.Events.GameLoop.DayStarted += this.OnDayStarted;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            // example patch, you'll need to edit this for your patch
            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.canGetPregnant)),
                   postfix: new HarmonyMethod(typeof(Patches.CanGetPregnantPatch), nameof(Patches.CanGetPregnantPatch.Postfix))
                );
        }

        /*private void OnDayStarted(object? sender, DayStartedEventArgs e) 
        {
            IEnumerable<Farmer> farmers = Game1.getAllFarmers();
            foreach (var farmer in farmers)
            {
                this.Monitor.Log($"{farmer.GetDaysMarried()}", LogLevel.Debug);

            }
        }*/

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>

    }
}