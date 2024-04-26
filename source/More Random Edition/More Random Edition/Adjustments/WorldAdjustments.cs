/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using RandomizerItem = Randomizer.Item;

namespace Randomizer
{
    /// <summary>
    /// Adjustments that are for the state of the game world in general
    /// </summary>
    public class WorldAdjustments
    {
        /// <summary>
        /// Fixes the foragables on day 1 - the save file is created too quickly for it to be
        /// randomized right away, so we'll change them on the spot on the first day
        /// </summary>
        public static void ChangeDayOneForagables()
        {
            if (!Globals.Config.RandomizeForagables)
            {
                return;
            }

            SDate currentDate = SDate.Now();
            if (currentDate.DaysSinceStart < 2)
            {
                List<GameLocation> locations = Game1.locations
                    .Concat(
                        from location in Game1.locations
                        from building in location.buildings
                        where building.indoors.Value != null
                        select building.indoors.Value
                    ).ToList();

                List<RandomizerItem> newForagables =
                    ItemList.GetForagables(Seasons.Spring)
                        .Where(x => x.ShouldBeForagable) // Removes the 1/1000 items
                        .Cast<RandomizerItem>().ToList();

                var rng = RNG.GetFarmRNG(nameof(WorldAdjustments));
                foreach (GameLocation location in locations)
                {
                    List<string> foragableIds = ItemList.GetForagables().Select(x => x.QualifiedId).ToList();
                    List<Vector2> tiles = location.Objects.Pairs
                        .Where(x => foragableIds.Contains(x.Value.QualifiedItemId))
                        .Select(x => x.Key)
                        .ToList();

                    foreach (Vector2 oldForagableKey in tiles)
                    {
                        RandomizerItem newForagable = rng.GetRandomValueFromList(newForagables);
                        location.Objects[oldForagableKey] = (Object)newForagable.GetSaliableObject();
                        location.Objects[oldForagableKey].IsSpawnedObject = true;
                    }
                }
            }
        }
    }
}
