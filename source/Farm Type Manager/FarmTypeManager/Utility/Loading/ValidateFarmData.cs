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
            /// <param name="pack">The content pack associated with this config data; null if the file was from this mod's own data folder.</param>
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
                //add each group of spawn areas to the list (unless its config section is null)
                if (config.Forage_Spawn_Settings != null)
                {
                    allAreas.Add(config.Forage_Spawn_Settings.Areas);
                }
                if (config.Large_Object_Spawn_Settings != null)
                {
                    allAreas.Add(config.Large_Object_Spawn_Settings.Areas);
                }
                if (config.Ore_Spawn_Settings != null)
                {
                    allAreas.Add(config.Ore_Spawn_Settings.Areas);
                }
                if (config.Monster_Spawn_Settings != null)
                {
                    allAreas.Add(config.Monster_Spawn_Settings.Areas);
                }

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
                            Monitor.Log($"Duplicate UniqueAreaID found: \"{area.UniqueAreaID}\" will be renamed.", LogLevel.Debug);
                            if (pack != null) //if this config is from a content pack
                            {
                                Monitor.Log($"Content pack: {pack.Manifest.Name}", LogLevel.Info);
                                Monitor.Log($"If this happened after updating another mod, it might cause certain conditions (such as one-time-only spawns) to reset in that area.", LogLevel.Debug);
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
                            else if (area is MonsterSpawnArea) { newName += " monster area "; }
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

                //confirm that any paired min/max settings are in the correct order
                foreach (SpawnArea[] areas in allAreas) //for each "Areas" array in allAreas
                {
                    foreach (SpawnArea area in areas) //for each area in the current array
                    {

                        if (area.MinimumSpawnsPerDay > area.MaximumSpawnsPerDay) //if the min and max are in the wrong order
                        {
                            //swap min and max
                            int temp = area.MinimumSpawnsPerDay;
                            area.MinimumSpawnsPerDay = area.MaximumSpawnsPerDay;
                            area.MaximumSpawnsPerDay = temp;
                            Monitor.Log($"Swapping minimum and maximum spawns per day for this area: {area.UniqueAreaID}", LogLevel.Trace);
                        }

                        if (area.SpawnTiming.StartTime > area.SpawnTiming.EndTime) //if start and end are in the wrong order
                        {
                            //swap start and end
                            StardewTime temp = area.SpawnTiming.StartTime;
                            area.SpawnTiming.StartTime = area.SpawnTiming.EndTime;
                            area.SpawnTiming.EndTime = temp;
                            Monitor.Log($"Swapping StartTime and EndTime in the SpawnTiming settings for this area: {area.UniqueAreaID}", LogLevel.Trace);
                        }
                    }
                }

                //detect invalid sound names and warn the user
                //NOTE: this will not remove the invalid name, in case the problem is related to custom sound loading
                foreach (SpawnArea[] areas in allAreas) //for each "Areas" array in allAreas
                {
                    foreach (SpawnArea area in areas) //for each area in the current array
                    {
                        if (area.SpawnTiming.SpawnSound != null && area.SpawnTiming.SpawnSound.Trim() != "") //if a SpawnSound has been provided for this area
                        {
                            try
                            {
                                Game1.soundBank.GetCue(area.SpawnTiming.SpawnSound); //test whether this sound exists by retrieving it from the game's soundbank
                            }
                            catch //if an exception is thrown while retrieving the sound
                            {
                                Monitor.Log($"This spawn sound could not be found: {area.SpawnTiming.SpawnSound}", LogLevel.Debug);
                                Monitor.Log($"Please make sure the sound's name is spelled and capitalized correctly. Sound names are case-sensitive.", LogLevel.Debug);
                                Monitor.Log($"Area: {area.UniqueAreaID}", LogLevel.Debug);
                                if (pack != null) //if this file is from a content pack
                                {
                                    Monitor.Log($"Content pack: {pack.Manifest.Name}", LogLevel.Debug);
                                }
                                else //if this file is from FarmTypeManager/data
                                {
                                    Monitor.Log($"File: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Debug);
                                }
                            }
                        }
                    }
                }

                if (pack != null)
                {
                    Monitor.Log($"Validation complete for content pack: {pack.Manifest.Name}", LogLevel.Trace);
                }
                else
                {
                    Monitor.Log("Validation complete for data from FarmTypeManager/data", LogLevel.Trace);
                }
                return;
            }
        }
    }
}