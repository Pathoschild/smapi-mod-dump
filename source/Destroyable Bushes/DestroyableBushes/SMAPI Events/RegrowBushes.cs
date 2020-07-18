using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using Harmony;

namespace DestroyableBushes
{
    public partial class ModEntry : Mod
    {
        private void RegrowBushes(object sender, DayStartedEventArgs e)
        {
            if (Context.IsMainPlayer) //if this is the main player
            {
                if (Data?.DestroyedBushes != null) //if the list of destroyed bushes exists
                {
                    for(int x = Data.DestroyedBushes.Count - 1; x >= 0; x--) //for each destroyed bush (looping backward to allow removal)
                    {
                        var bush = Data.DestroyedBushes[x];

                        if (BushShouldRegrowToday(bush.DateDestroyed)) //if this bush should regrow today
                        {
                            GameLocation location = Game1.getLocationFromName(bush.LocationName);

                            if (location?.isTileOccupiedForPlacement(bush.Tile) == false) //if this bush's tile is NOT obstructed by anything
                            {
                                try
                                {
                                    location.largeTerrainFeatures.Add(new Bush(bush.Tile, bush.Size, location)); //respawn this bush
                                    Data.DestroyedBushes.RemoveAt(x); //remove this bush from the list
                                }
                                catch (Exception ex)
                                {
                                    Instance.Monitor.Log($"Error respawning a bush at {bush.LocationName} ({bush.Tile.X},{bush.Tile.Y}): \n{ex.ToString()}", LogLevel.Error);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Determines whether a bush should regrow today.</summary>
        /// <param name="dateDestroyed">The <see cref="SDate"/> on which the bush was destroyed.</param>
        /// <returns>True if the bush should regrow; otherwise false.</returns>
        private bool BushShouldRegrowToday(SDate dateDestroyed)
        {
            if (dateDestroyed == null || Config.regrowNumber == null || Config.regrowUnit == null) //if any required information is null
            {
                return false; //this bush should NOT regrow
            }
            else
            {
                SDate regrowDate = null; //the date when this bush should regrow

                switch (Config.regrowUnit.Value)
                {
                    case RegrowUnit.Days:
                        regrowDate = dateDestroyed.AddDays(Config.regrowNumber.Value); //regrow after (regrowNumber) seasons
                        break;
                    case RegrowUnit.Seasons:
                        regrowDate = dateDestroyed.AddDays(28 * Config.regrowNumber.Value); //get a date after (regrowNumber) seasons
                        regrowDate = new SDate(1, regrowDate.Season, regrowDate.Year); //regrow on the first day of that season
                        break;
                    case RegrowUnit.Years:
                        regrowDate = new SDate(1, "spring", dateDestroyed.Year + Config.regrowNumber.Value); //regrow after (regrowNumber) years on the first day of spring
                        break;
                    default:
                        return false; //this bush should NOT regrow
                }

                if (SDate.Now() >= regrowDate) //if the regrow date is today or has already passed
                    return true; //this bush should regrow
                else
                    return false; //this bush should NOT regrow
            }
        }
    }
}
