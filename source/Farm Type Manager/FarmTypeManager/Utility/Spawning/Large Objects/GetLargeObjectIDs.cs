/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates a list of IDs for large objects (a.k.a. resource clumps) from a list of IDs and/or nicknames. Duplicates are kept; invalid entries are removed.</summary>
            /// <param name="names">A list of names representing large objects (e.g. "Stump", "boulders", and/or specific IDs).</param>
            /// /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A list of valid large object IDs parsed from the provided list.</returns>
            public static List<string> GetLargeObjectIDs(string[] names, string areaID = "")
            {
                List<string> IDs = new List<string>(); //a list of index numbers to be returned

                foreach (string name in names)
                {
                    //for each valid name, add the game's internal ID for that large object (a.k.a. resource clump)
                    switch (name.ToLower())
                    {
                        //known resource clumps that are NOT giant crops
                        case "44":
                        case "weed1":
                        case "weed 1":
                        case "giantweed1":
                        case "giant weed 1":
                            IDs.Add("44");
                            break;
                        case "46":
                        case "weed2":
                        case "weed 2":
                        case "giantweed2":
                        case "giant weed 2":
                            IDs.Add("46");
                            break;
                        case "148":
                        case "quarry":
                        case "quarryboulder":
                        case "quarryboulders":
                        case "quarry boulder":
                        case "quarry boulders":
                            IDs.Add("148");
                            break;
                        case "600":
                        case "stump":
                        case "stumps":
                            IDs.Add("600");
                            break;
                        case "602":
                        case "log":
                        case "logs":
                            IDs.Add("602");
                            break;
                        case "622":
                        case "meteor":
                        case "meteors":
                        case "meteorite":
                        case "meteorites":
                            IDs.Add("622");
                            break;
                        case "672":
                        case "boulder":
                        case "boulders":
                            IDs.Add("672");
                            break;
                        case "752":
                        case "minerock1":
                        case "mine rock 1":
                            IDs.Add("752");
                            break;
                        case "754":
                        case "minerock2":
                        case "mine rock 2":
                            IDs.Add("754");
                            break;
                        case "756":
                        case "minerock3":
                        case "mine rock 3":
                            IDs.Add("756");
                            break;
                        case "758":
                        case "minerock4":
                        case "mine rock 4":
                            IDs.Add("758");
                            break;

                        //giant crops that existed prior to Data/GiantCrops
                        case "190":
                        case "cauliflower":
                        case "giantcauliflower":
                        case "giant cauliflower":
                            IDs.Add("Cauliflower");
                            break;
                        case "254":
                        case "melon":
                        case "giantmelon":
                        case "giant melon":
                            IDs.Add("Melon");
                            break;
                        case "276":
                        case "pumpkin":
                        case "giantpumpkin":
                        case "giant pumpkin":
                            IDs.Add("Pumpkin");
                            break;
                        default: //if "name" isn't a known object name or ID
                            //NOTE: checks below may be case-sensitive, unlike the aliases above; do not use "name.ToLower()" or similar
                            if (Utility.ItemExtensionsAPI != null && Utility.ItemExtensionsAPI.IsClump(name)) //if this is an IE clump
                            {
                                IDs.Add(name);
                            }
                            else if (DataLoader.GiantCrops(Game1.content).ContainsKey(name)) //if there is a giant crop with this ID
                                IDs.Add(name);
                            else
                            {
                                Monitor.Log($"An area's large object list contains a name that did not match any objects.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                                Monitor.Log($"Object name: \"{name}\"", LogLevel.Info);
                            }
                            break;
                    }
                }

                return IDs;
            }
        }
    }
}