/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/DestroyableBushes
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Linq;

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
                    for (int x = Data.DestroyedBushes.Count - 1; x >= 0; x--) //for each destroyed bush (looping backward to allow removal)
                    {
                        var bushData = Data.DestroyedBushes[x];

                        if (BushShouldRegrowToday(bushData.DateDestroyed)) //if this bush should regrow today
                        {
                            GameLocation location = Game1.getLocationFromName(bushData.LocationName);

                            if (location != null && !bushData.GetCollisionTiles().Any(tile => !location.CanItemBePlacedHere(tile) || location.terrainFeatures.ContainsKey(tile))) //if this bush's tiles are NOT obstructed by anything (including passable features)
                            {
                                try
                                {
                                    //create bush and apply its saved data
                                    Bush newBush = new Bush(bushData.Tile, bushData.Size, location);
                                    newBush.townBush.Value = bushData.TownBush;
                                    if (bushData.TilesheetOffset.HasValue)
                                    {
                                        newBush.tileSheetOffset.Value = bushData.TilesheetOffset.Value; //set tilesheet offset
                                        newBush.setUpSourceRect(); //update sprite
                                    }

                                    location.largeTerrainFeatures.Add(newBush); //add the bush to its location
                                    Data.DestroyedBushes.RemoveAt(x); //remove this bush from the list
                                }
                                catch (Exception ex)
                                {
                                    Instance.Monitor.Log($"Error respawning a bush at {bushData.LocationName} ({bushData.Tile.X},{bushData.Tile.Y}): \n{ex.ToString()}", LogLevel.Error);
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
