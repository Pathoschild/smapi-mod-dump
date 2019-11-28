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

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generate an array of index numbers for large objects (a.k.a. resource clumps) based on an array of names. Duplicates are allowed; invalid entries are discarded.</summary>
            /// <param name="names">A list of names representing large objects (e.g. "Stump", "boulders").</param>
            /// <returns>An array of index numbers for large object spawning purposes.</returns>
            public static List<int> GetLargeObjectIDs(string[] names)
            {
                List<int> IDs = new List<int>(); //a list of index numbers to be returned

                foreach (string name in names)
                {
                    //for each valid name, add the game's internal ID for that large object (a.k.a. resource clump)
                    switch (name.ToLower())
                    {
                        case "cauliflower":
                        case "giantcauliflower":
                        case "giant cauliflower":
                            IDs.Add(190);
                            break;
                        case "melon":
                        case "giantmelon":
                        case "giant melon":
                            IDs.Add(254);
                            break;
                        case "pumpkin":
                        case "giantpumpkin":
                        case "giant pumpkin":
                            IDs.Add(276);
                            break;
                        case "stump":
                        case "stumps":
                            IDs.Add(600);
                            break;
                        case "log":
                        case "logs":
                            IDs.Add(602);
                            break;
                        case "boulder":
                        case "boulders":
                            IDs.Add(672);
                            break;
                        case "meteor":
                        case "meteors":
                        case "meteorite":
                        case "meteorites":
                            IDs.Add(622);
                            break;
                        case "minerock1":
                        case "mine rock 1":
                            IDs.Add(752);
                            break;
                        case "minerock2":
                        case "mine rock 2":
                            IDs.Add(754);
                            break;
                        case "minerock3":
                        case "mine rock 3":
                            IDs.Add(756);
                            break;
                        case "minerock4":
                        case "mine rock 4":
                            IDs.Add(758);
                            break;
                        default: //"name" isn't recognized as any existing object names
                            int parsed;
                            if (int.TryParse(name, out parsed)) //if the string seems to be a valid integer, save it to "parsed" and add it to the list
                            {
                                IDs.Add(parsed);
                            }
                            break;
                    }
                }

                return IDs;
            }
        }
    }
}