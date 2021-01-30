/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Wellbott/StardewValleyMods
**
*************************************************/

using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;

namespace SlimHutch
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
        }


        /*********
        ** Private methods
        *********/
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Context.IsMainPlayer)
            {
                foreach (Building building in Game1.getFarm().buildings)
                {
                    //this.Monitor.Log($"{building.buildingType.Value}, {building.tileX}, {building.tileY}, {building.tilesWide}, {building.tilesHigh}.", LogLevel.Debug);
                    if (building.buildingType.Value == "Slime Hutch" && (building.tilesWide != 7 || building.tilesHigh != 4))
                    {
                        this.Monitor.Log($"Resizing existing Slime Hutch at {building.tileX}, {building.tileY}.", LogLevel.Debug);
                        building.tilesHigh.Value = 4;
                        building.tilesWide.Value = 7;
                    }
                }
            }
        }
    }
}