using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods involved in spawning objects into the game.</summary> 
        private static partial class Generation
        {
            /// <summary>Generates forageable items in the game based on the current player's config settings.</summary>
            public static void ForageGeneration()
            {
                foreach (FarmData data in Utility.FarmDataList)
                {
                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"Starting forage generation for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"Starting forage generation for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }

                    if (data.Config.Forage_Spawn_Settings != null) //if this config contains forage settings
                    {
                        if (data.Config.ForageSpawnEnabled)
                        {
                            Utility.Monitor.Log("Forage generation is enabled. Starting generation process...", LogLevel.Trace);
                        }
                        else
                        {
                            Utility.Monitor.Log($"Forage generation is disabled for this {(data.Pack == null ? "file" : "content pack")}.", LogLevel.Trace);
                            continue;
                        }
                    }
                    else //if this config's forage settings are null
                    {
                        Utility.Monitor.Log($"This {(data.Pack == null ? "file" : "content pack")}'s forage spawn settings are blank.", LogLevel.Trace);
                        continue;
                    }

                    foreach (ForageSpawnArea area in data.Config.Forage_Spawn_Settings.Areas)
                    {
                        Utility.Monitor.Log($"Checking forage settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);

                        //validate the map name for the area
                        List<GameLocation> locations = Utility.GetAllLocationsFromName(area.MapName); //get all locations for this map name
                        if (locations.Count == 0) //if no locations were found
                        {
                            Utility.Monitor.Log($"No map named \"{area.MapName}\" could be found. Forage won't be spawned there.", LogLevel.Trace);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log("All extra conditions met. Retrieving list of forage types...", LogLevel.Trace);

                        List<SavedObject> forageObjects = null; //the list of possible forage objects to spawn in this area today (parsed into SavedObject format)

                        switch (Game1.currentSeason)
                        {
                            case "spring":
                                if (area.SpringItemIndex != null) //if there's an "override" list set for this area
                                {
                                    if (area.SpringItemIndex.Length > 0) //if the override includes any items
                                    {
                                        forageObjects = Utility.ParseSavedObjectsFromItemList(area.SpringItemIndex, area.UniqueAreaID); //parse the override index list for this area
                                    }
                                    //if an area index exists but is empty, *do not* use the main index; users may want to disable spawns in this season
                                }
                                else if (data.Config.Forage_Spawn_Settings.SpringItemIndex.Length > 0) //if no "override" list exists and the main index list includes any items
                                {
                                    forageObjects = Utility.ParseSavedObjectsFromItemList(data.Config.Forage_Spawn_Settings.SpringItemIndex, area.UniqueAreaID); //parse the global index list
                                }
                                break;
                            case "summer":
                                if (area.SummerItemIndex != null)
                                {
                                    if (area.SummerItemIndex.Length > 0)
                                    {
                                        forageObjects = Utility.ParseSavedObjectsFromItemList(area.SummerItemIndex, area.UniqueAreaID);
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.SummerItemIndex.Length > 0)
                                {
                                    forageObjects = Utility.ParseSavedObjectsFromItemList(data.Config.Forage_Spawn_Settings.SummerItemIndex, area.UniqueAreaID);
                                }
                                break;
                            case "fall":
                                if (area.FallItemIndex != null)
                                {
                                    if (area.FallItemIndex.Length > 0)
                                    {
                                        forageObjects = Utility.ParseSavedObjectsFromItemList(area.FallItemIndex, area.UniqueAreaID);
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.FallItemIndex.Length > 0)
                                {
                                    forageObjects = Utility.ParseSavedObjectsFromItemList(data.Config.Forage_Spawn_Settings.FallItemIndex, area.UniqueAreaID);
                                }
                                break;
                            case "winter":
                                if (area.WinterItemIndex != null)
                                {
                                    if (area.WinterItemIndex.Length > 0)
                                    {
                                        forageObjects = Utility.ParseSavedObjectsFromItemList(area.WinterItemIndex, area.UniqueAreaID);
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.WinterItemIndex.Length > 0)
                                {
                                    forageObjects = Utility.ParseSavedObjectsFromItemList(data.Config.Forage_Spawn_Settings.WinterItemIndex, area.UniqueAreaID);
                                }
                                break;
                        }

                        if (forageObjects == null || forageObjects.Count <= 0) 
                        {
                            Utility.Monitor.Log($"This area's forage list is null or empty. This generally means the {Game1.currentSeason}IndexList contains no valid items. Skipping to the next forage area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log($"Forage types found: {forageObjects.Count}. Beginning generation process...", LogLevel.Trace);

                        for (int x = 0; x < locations.Count; x++) //for each location matching this area's map name
                        {
                            //calculate how much forage to spawn today
                            int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, data.Config.Forage_Spawn_Settings.PercentExtraSpawnsPerForagingLevel, Utility.Skills.Foraging);

                            List<SavedObject> spawns = new List<SavedObject>(); //the list of objects to be spawned
                            int skippedSpawns = 0; //the number of objects skipped due to their spawn chances

                            //begin to generate forage
                            while (spawnCount > 0) //while more forage should be spawned
                            {
                                spawnCount--;
                                SavedObject randomForage = forageObjects[Utility.RNG.Next(forageObjects.Count)]; //select a random object from the forage list

                                double? spawnChance = randomForage.ConfigItem?.PercentChanceToSpawn; //get this object's spawn chance, if provided
                                if (spawnChance.HasValue && spawnChance.Value < Utility.RNG.Next(100)) //if this object "fails" its chance to spawn
                                {
                                    skippedSpawns++; //increment skip counter
                                    continue; //skip to the next spawn
                                }

                                //create a new saved object based on the randomly selected forage (still using a "blank" tile location)
                                SavedObject forage = new SavedObject()
                                {
                                    MapName = locations[x].uniqueName.Value ?? locations[x].Name,
                                    Type = randomForage.Type,
                                    Name = randomForage.Name,
                                    ID = randomForage.ID,
                                    DaysUntilExpire = area.DaysUntilSpawnsExpire,
                                    ConfigItem = Utility.Clone(randomForage.ConfigItem) //use a separate copy of this (TODO: make a more efficient clone method for this class)
                                };

                                if (forage.DaysUntilExpire == null && forage.Type != SavedObject.ObjectType.Object) //if this is an item or container without an expiration setting
                                {
                                    forage.DaysUntilExpire = 1; //default to overnight expiration
                                }

                                //if this object has contents with spawn chances, process them
                                if (forage.ConfigItem?.Contents != null) //if this forage item has contents
                                {
                                    for (int content = forage.ConfigItem.Contents.Count - 1; content >= 0; content--) //for each of the contents
                                    {
                                        List<SavedObject> contentSave = Utility.ParseSavedObjectsFromItemList(new object[] { forage.ConfigItem.Contents[content] }, area.UniqueAreaID); //parse this into a saved object

                                        double? contentSpawnChance = contentSave[0].ConfigItem?.PercentChanceToSpawn; //get this item's spawn chance, if provided
                                        if (contentSpawnChance.HasValue && contentSpawnChance.Value < Utility.RNG.Next(100)) //if this item "fails" its chance to spawn
                                        {
                                            forage.ConfigItem.Contents.RemoveAt(content); //remove this content from the forage object
                                        }
                                    }
                                }

                                spawns.Add(forage); //add it to the list
                            }

                            if (locations.Count > 1) //if this area targets multiple locations
                            {
                                Utility.Monitor.Log($"Potential spawns at {locations[x].Name} #{x + 1}: {spawns.Count}", LogLevel.Trace);
                            }
                            else //if this area only targets one location
                            {
                                Utility.Monitor.Log($"Potential spawns at {locations[x].Name}: {spawns.Count}", LogLevel.Trace);
                            }

                            if (skippedSpawns > 0) //if any spawns were skipped due to their spawn chances
                            {
                                Utility.Monitor.Log($"Spawns skipped due to spawn chance settings: {skippedSpawns}", LogLevel.Trace);
                            }

                            Utility.PopulateTimedSpawnList(spawns, data, area); //process the listed spawns and add them to Utility.TimedSpawns
                        }

                        Utility.Monitor.Log($"Forage generation complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                    }

                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Forage generation complete for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Forage generation complete for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }
                }

                Utility.Monitor.Log("All files and content packs checked. Forage generation process complete.", LogLevel.Trace);
            }
        }
    }
}
