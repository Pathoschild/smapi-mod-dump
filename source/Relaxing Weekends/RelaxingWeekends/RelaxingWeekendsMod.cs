/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-ModCollection
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoWeekendWatering
{
    /// <summary>
    /// Mod for watering all your crops on the weekend
    /// </summary>
    public class RelaxingWeekendsMod : Mod
    {
        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        /// <summary>
        /// Automatically waters all tilled dirt
        /// </summary>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            Game1.addMailForTomorrow($"This mail was sent via a string", noLetter: false, sendToEveryone: false);
            // Only run on the weekends
            if (Game1.Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                Stopwatch sw = Stopwatch.StartNew();

                foreach (GameLocation location in Game1.locations)
                {
                    // Each location has terrain features, and some may be able to grow plants
                    foreach (TerrainFeature terrainFeature in location.terrainFeatures.Values)
                    {
                        if (terrainFeature is HoeDirt hoedirt)
                        {
                            // Simple setting its value to 1 marks it as watered
                            hoedirt.state.Value = 1;
                        }
                    }

                    // Pots can be indside buildings as well
                    foreach (IndoorPot indoorPot in location.objects.Values.OfType<IndoorPot>())
                    {
                        indoorPot.showNextIndex.Value = true;
                    }
                }
                Monitor.Log($"Time to water: {sw.Elapsed}");
            }
        }
    }
}
