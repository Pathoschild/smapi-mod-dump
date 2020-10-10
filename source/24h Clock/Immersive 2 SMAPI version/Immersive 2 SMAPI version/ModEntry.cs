/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LajnaLegenden/Stardew_Valley_Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using xTile;

//Map map = this.Helper.Content.Load<Map>(pathToTbin);
//GameLocation location = Game1.getLocationFromName("SomeLocation");
//location.map = map;


namespace Immersive_2_SMAPI_version
{
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            Map farm = this.Helper.Content.Load<Map>("map.tbin");
            GameLocation location = Game1.getFarm();
            location.map = farm;

            this.Monitor.Log("Replacing the farm with the Immersive one...", LogLevel.Info);
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            
        }
    }
}
