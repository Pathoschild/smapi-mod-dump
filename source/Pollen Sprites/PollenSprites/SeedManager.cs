using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace PollenSprites
{
    /// <summary>Manages lists of object IDs for seeds dropped by <see cref="PollenSprite"/> monsters.</summary>
    public static class SeedManager
    {
        /// <summary>Mixed seeds' ID.</summary>
        public static int MixedSeeds { get; set; } = 770;

        /// <summary>A list of flower seed IDs.</summary>
        public static List<int> FlowerSeeds { get; set; } = new List<int>()
        {
            429, //jazz seeds
            427, //tulip bulb
            453, //poppy seeds
            455, //spangle seeds
            431, //sunflower seeds
            425  //fairy seeds
        };

        private static List<int> allSeeds = null;
        /// <summary>A list of all available seed IDs.</summary>
        public static List<int> AllSeeds
        {
            get
            {
                if (allSeeds == null) //if the list is null
                {
                    allSeeds = new List<int>(); //create a new list

                    foreach (int key in (Game1.objectInformation.Keys)) //for each loaded object ID
                    {
                        string[] fields = Game1.objectInformation[key]?.Split('/'); //if this object has a data string, split it into fields

                        if (fields != null && fields.Length >= 4) //if fields[3] exists (i.e. the "category" field)
                        {
                            if (fields[3].Trim().EndsWith("-74") && fields[0].IndexOf("Sapling", StringComparison.OrdinalIgnoreCase) < 0) //if this object is in category -74 (seeds) and its name does NOT contain "Sapling"
                                allSeeds.Add(key); //add this object's ID to the seed list
                        }
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
        /// <remarks>Most mod seem to limit object data changes to when a save is loaded.
        /// Clearing this data at the title screen should account for those changes.</remarks>
        public static void GameLoop_ReturnedToTitle_ClearAllSeedsList(object sender, ReturnedToTitleEventArgs e)
        {
            AllSeeds = null; //dispose of the existing list, if any
        }
    }
}
