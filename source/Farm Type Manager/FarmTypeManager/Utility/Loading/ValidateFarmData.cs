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
            /// <summary>Validates a single instance of farm data, correcting obsolete/invalid settings automatically.</summary>
            /// <param name="config">The contents of a single config file to be validated.</param>
            /// <param name="pack">The content pack associated with this config data; null if the file was from this mod's own folders.</param>
            public static void ValidateFarmData(FarmConfig config, IContentPack pack)
            {
                if (pack != null)
                {
                    Monitor.Log($"Validating data from content pack: {pack.Manifest.Name}", LogLevel.Trace);
                }
                else
                {
                    Monitor.Log("Validating data from FarmTypeManager/data", LogLevel.Trace);
                }

                List<SpawnArea[]> allAreas = new List<SpawnArea[]>(); //a unified list of each "Areas" array in this config file
                allAreas.Add(config.Forage_Spawn_Settings.Areas);
                allAreas.Add(config.Large_Object_Spawn_Settings.Areas);
                allAreas.Add(config.Ore_Spawn_Settings.Areas);

                Monitor.Log("Checking for duplicate UniqueAreaIDs...", LogLevel.Trace);
                HashSet<string> IDs = new HashSet<string>(); //a record of all unique IDs encountered during this process

                //erase any duplicate IDs and record the others in the "IDs" hashset
                foreach (SpawnArea[] areas in allAreas) //for each "Areas" array in allAreas
                {
                    foreach (SpawnArea area in areas) //for each area in the current array
                    {
                        if (String.IsNullOrWhiteSpace(area.UniqueAreaID) || area.UniqueAreaID.ToLower() == "null") //if the area ID is null, blank, or the string "null" (to account for user confusion)
                        {
                            continue; //this name will be replaced later, so ignore it for now
                        }

                        if (IDs.Contains(area.UniqueAreaID)) //if this area's ID was already encountered
                        {
                            Monitor.Log($"Duplicate UniqueAreaID found: \"{area.UniqueAreaID}\" will be renamed.", LogLevel.Info);
                            if (pack != null) //if this config is from a content pack
                            {
                                Monitor.Log($"Content pack: {pack.Manifest.Name}", LogLevel.Info);
                                Monitor.Log($"If this happens after updating another mod, it might cause certain conditions (such as one-time-only spawns) to reset in that area.", LogLevel.Info);
                            }

                            area.UniqueAreaID = ""; //erase this area's ID, marking it for replacement
                        }
                        else //if this ID is unique so far
                        {
                            IDs.Add(area.UniqueAreaID); //add the area to the ID set
                        }
                    }
                }

                Monitor.Log("Assigning new UniqueAreaIDs to any blanks or duplicates...", LogLevel.Trace);
                string newName; //temp storage for a new ID while it's created/tested
                int newNumber; //temp storage for the numeric part of a new ID

                //create new IDs for any empty ones
                foreach (SpawnArea[] areas in allAreas) //for each "Areas" array in allAreas
                {
                    foreach (SpawnArea area in areas) //for each area in the current array
                    {
                        if (String.IsNullOrWhiteSpace(area.UniqueAreaID) || area.UniqueAreaID.ToLower() == "null") //if the area ID is null, blank, or the string "null" (to account for user confusion)
                        {
                            //create a new name, based on which type of area this is
                            newName = area.MapName;
                            if (area is ForageSpawnArea) { newName += " forage area "; }
                            else if (area is LargeObjectSpawnArea) { newName += " large object area "; }
                            else if (area is OreSpawnArea) { newName += " ore area "; }
                            else { newName += " area "; }

                            newNumber = 1;

                            while (IDs.Contains(newName + newNumber)) //if this ID wouldn't be unique
                            {
                                newNumber++; //increment and try again
                            }

                            area.UniqueAreaID = newName + newNumber; //apply the new unique ID
                            Monitor.Log($"New UniqueAreaID assigned: {area.UniqueAreaID}", LogLevel.Trace);
                        }

                        IDs.Add(area.UniqueAreaID); //the ID is finalized, so add it to the set of encountered IDs
                    }
                }

                if (pack != null)
                {
                    Monitor.Log($"Validation complete for content pack: {pack.Manifest.Name}", LogLevel.Trace);
                }
                else
                {
                    Monitor.Log("Validation complete for this file from FarmTypeManager/data", LogLevel.Trace);
                }
                return;
            }
        }
    }
}