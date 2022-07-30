/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/notevenamouse/StardewMods
**
*************************************************/

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Objects;

namespace FixTVsOnCustomFarmTypes
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        
        // all vanilla TVs
        private int[] possibleTVs = new[] {1466, 1468, 1680, 2326};
        
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.World.BuildingListChanged += this.OnBuildingListChanged;
        }

        

        /*********
        ** Private methods
        *********/

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //only scan once, and not at all if there's no reason for the TV to be broken
            if (Context.IsMainPlayer && Game1.getFarm().getMapProperty("FarmHouseFurniture") != "" && Game1.Date.TotalDays < 1)
            {
                var fixedTVs = 0;
                foreach (Building building in Game1.getFarm().buildings)
                {
                    if (building.isCabin)
                    {
                        fixedTVs += FixTVs(Game1.getLocationFromName(building.nameOfIndoors));
                    }
                }

                if (Game1.currentLocation is FarmHouse farmhouse)
                {
                    fixedTVs += FixTVs(Game1.currentLocation);
                }
                if (fixedTVs > 0)
                {
                    this.Monitor.Log($"{fixedTVs} TV{(fixedTVs > 1 ? "s were" : " was")} fixed.", LogLevel.Trace);
                }
            }
        }
    
        private void OnBuildingListChanged(object sender, BuildingListChangedEventArgs e)
        {
            // if something was built, check if it was a cabin and fix if using custom starting furniture
            if (Context.IsMainPlayer && Game1.getFarm().getMapProperty("FarmHouseFurniture") != "" && e.Added.Any())
            {
                var fixedTVs = 0;
                foreach (Building building in e.Added)
                {
                    if (building.isCabin)
                    {
                        fixedTVs += FixTVs(Game1.getLocationFromName(building.nameOfIndoors));
                    }
                }
                if (fixedTVs > 0)
                {
                    this.Monitor.Log($"{fixedTVs} TV{(fixedTVs > 1 ? "'s were" : " was")} fixed.", LogLevel.Trace);
                }
            }
        }

        private int FixTVs(GameLocation loc)
        {
            var fixedTVs = 0;
            foreach (Furniture furniture in loc.furniture.ToArray())
            {
                if (possibleTVs.Contains(furniture.ParentSheetIndex) && furniture is not TV)
                {
                    loc.furniture.Remove(furniture);
                    loc.furniture.Add(new TV(furniture.ParentSheetIndex, furniture.TileLocation));
                    fixedTVs++;
                }
            }
            return fixedTVs;
        }
    }
}