/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/PollenSprites
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace PollenSprites
{
    /// <summary>Manages lists of object IDs for seeds dropped by <see cref="PollenSprite"/> monsters.</summary>
    public static class SeedManager
    {
        /// <summary>Mixed seeds' ID.</summary>
        public static string MixedSeeds { get; set; } = "770";

        /// <summary>Mixed flower seeds' ID.</summary>
        public static string FlowerSeeds { get; set; } = "MixedFlowerSeeds";

        private static List<string> allSeeds = null;
        /// <summary>A list of all available seed IDs.</summary>
        public static List<string> AllSeeds
        {
            get
            {
                if (allSeeds == null) //if the list is null
                {
                    allSeeds = new List<string>(); //create a new list

                    foreach (var entry in Game1.objectData) //for each loaded object ID
                    {
                        if (string.Equals(entry.Value.Type, "Seeds", StringComparison.Ordinal) && entry.Value.Category == -74) //if this is a crop or flower seed (note: tree saplings seem to use the "Basic" or "Crafting" type, etc)
                            allSeeds.Add(entry.Key); //add its ID to the list
                    }
                }

                return allSeeds; //return the list
            }

            set
            {
                allSeeds = value; //set the field
            }
        }

        /// <summary>Whenever the player exits a loaded game, clear the <see cref="AllSeeds"/> list.</summary>
        public static void GameLoop_ReturnedToTitle_ClearAllSeedsList(object sender, ReturnedToTitleEventArgs e)
        {
            AllSeeds = null; //dispose of the existing list, if any
        }
    }
}
