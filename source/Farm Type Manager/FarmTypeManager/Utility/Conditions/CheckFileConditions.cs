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
            /// <summary>Checks whether a config file should be used with the currently loaded farm.</summary>
            /// <param name="config">The FarmConfig to be checked.</param>
            /// <param name="pack">The content pack associated with this file, if any.</param>
            /// <returns>True if the file should be used with the current farm; false otherwise.</returns>
            public static bool CheckFileConditions(FarmConfig config, IContentPack pack)
            {
                Monitor.Log("Checking file conditions...", LogLevel.Trace);

                //check "reset main data folder" flag
                //NOTE: it's preferable to do this as the first step; it's intended to be a one-off cleaning process, rather than a conditional effect
                if (config.File_Conditions.ResetMainDataFolder && MConfig.EnableContentPackFileChanges) //if "reset" is true and file changes are enabled
                {
                    if (pack != null) //if this is part of a content pack
                    {
                        //attempt to load the content pack's global save data
                        ContentPackSaveData packSave = null;
                        try
                        {
                            packSave = pack.ReadJsonFile<ContentPackSaveData>(Path.Combine("data", "ContentPackSaveData.save")); //load the content pack's global save data (null if it doesn't exist)
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Warning: This content pack's save data could not be parsed correctly: {pack.Manifest.Name}", LogLevel.Warn);
                            Monitor.Log($"Affected file: data/ContentPackSaveData.save", LogLevel.Warn);
                            Monitor.Log($"Please delete the file and/or contact the mod's developer.", LogLevel.Warn);
                            Monitor.Log($"The content pack will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                            Monitor.Log($"----------", LogLevel.Warn);
                            Monitor.Log($"{ex.Message}", LogLevel.Warn);
                            return false; //disable this content pack's config, since it may require this process to function
                        }

                        if (packSave == null) //no global save data exists for this content pack
                        {
                            packSave = new ContentPackSaveData();
                        }

                        if (!packSave.MainDataFolderReset) //if this content pack has NOT reset the main data folder yet
                        {
                            Monitor.Log($"ResetMainDataFolder requested by content pack: {pack.Manifest.Name}", LogLevel.Debug);
                            string dataPath = Path.Combine(Helper.DirectoryPath, "data"); //the path to this mod's data folder
                            DirectoryInfo dataFolder = new DirectoryInfo(dataPath); //an object representing this mod's data directory

                            if (dataFolder.Exists) //the data folder exists
                            {
                                Monitor.Log("Attempting to archive data folder...", LogLevel.Trace);
                                try
                                {
                                    string archivePath = Path.Combine(Helper.DirectoryPath, "data", "archive", DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                                    DirectoryInfo archiveFolder = Directory.CreateDirectory(archivePath); //create a timestamped archive folder
                                    foreach (FileInfo file in dataFolder.GetFiles()) //for each file in dataFolder
                                    {
                                        file.MoveTo(Path.Combine(archiveFolder.FullName, file.Name)); //move each file to archiveFolder
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Monitor.Log($"Warning: This content pack attempted to archive Farm Type Manager's data folder but failed: {pack.Manifest.Name}", LogLevel.Warn);
                                    Monitor.Log($"Please report this issue to Farm Type Manager's developer. This might also be fixed by manually removing your FarmTypeManager/data/ files.", LogLevel.Warn);
                                    Monitor.Log($"The content pack will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                                    Monitor.Log($"----------", LogLevel.Warn);
                                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                                    return false; //disable this content pack's config, since it may require this process to function
                                }
                            }
                            else //the data folder doesn't exist
                            {
                                Monitor.Log("Data folder not found; assuming it was deleted or not yet generated.", LogLevel.Trace);
                            }

                            packSave.MainDataFolderReset = true; //update save data
                        }

                        pack.WriteJsonFile(Path.Combine("data", "ContentPackSaveData.save"), packSave); //update the content pack's global save data file
                        Monitor.Log("Data folder archive successful.", LogLevel.Trace);
                    }
                    else //if this is NOT part of a content pack
                    {
                        Monitor.Log("This farm's config file has ResetMainDataFolder = true, but this setting only works for content packs.", LogLevel.Info);
                    }
                }

                //check farm type
                if (config.File_Conditions.FarmTypes != null && config.File_Conditions.FarmTypes.Length > 0)
                {
                    Monitor.Log("Farm type condition(s) found. Checking...", LogLevel.Trace);

                    bool validType = false;

                    foreach (object obj in config.File_Conditions.FarmTypes) //for each listed farm type
                    {
                        int type = -1;

                        //parse the farm type object into an integer (int type)
                        if (obj is long || obj is int) //if the object is a readable integer
                        {
                            type = Convert.ToInt32(obj); //convert it to a 32-bit integer and use it
                        }
                        else if (obj is string name) //if the object is a string, cast it as one
                        {
                            if (name.Equals("All", StringComparison.OrdinalIgnoreCase) || name.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if this is "all" or "any"
                            {
                                validType = true;
                                break; //skip checking the rest of the farm types
                            }
                            else if (Enum.TryParse(name, true, out FarmTypes farmType)) //if this name can be parsed into a FarmTypes enum
                            {
                                type = (int)farmType; //use it as an integer
                            }
                            else //if this is a string, but not a recognized value
                            {
                                if (int.TryParse(name, out int parsed)) //if this string can be parsed as an integer
                                {
                                    type = parsed; //use the parsed value
                                }
                                else //if this string cannot be parsed
                                {
                                    Monitor.Log($"This setting in the Farm Types list could not be parsed: {name}", LogLevel.Debug);
                                    Monitor.Log($"The setting will be ignored. If it's intended to be a custom farm type, please use its ID number instead of its name.", LogLevel.Debug);
                                    continue; //skip to the next farm type condition
                                }
                            }
                        }

                        if (type == Game1.whichFarm) //if the parsed type matches the current farm type
                        {
                            validType = true;
                            break; //skip checking the rest of the farm types
                        }
                    }

                    if (validType) //if a valid farm type was listed
                    {
                        Monitor.Log("Farm type matched a setting. File allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Farm type did NOT match any settings. File disabled.", LogLevel.Trace);
                        return false; //prevent config use
                    }
                }

                //check farmer name
                if (config.File_Conditions.FarmerNames != null && config.File_Conditions.FarmerNames.Length > 0)
                {
                    Monitor.Log("Farmer name condition(s) found. Checking...", LogLevel.Trace);

                    bool validName = false;

                    foreach (string name in config.File_Conditions.FarmerNames) //for each listed name
                    {
                        if (name.Equals(Game1.player.Name, StringComparison.OrdinalIgnoreCase)) //if the name matches the current player's
                        {
                            validName = true;
                            break; //skip the rest of these checks
                        }
                    }

                    if (validName) //if a valid farmer name was listed
                    {
                        Monitor.Log("Farmer name matched a setting. File allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Farmer name did NOT match any settings. File disabled.", LogLevel.Trace);
                        return false; //prevent config use
                    }
                }

                //check save file names (technically the save folder name)
                if (config.File_Conditions.SaveFileNames != null && config.File_Conditions.SaveFileNames.Length > 0)
                {
                    Monitor.Log("Save file name condition(s) found. Checking...", LogLevel.Trace);

                    bool validSave = false;

                    foreach (string saveName in config.File_Conditions.SaveFileNames) //for each listed save name
                    {
                        if (saveName.Equals(Constants.SaveFolderName, StringComparison.OrdinalIgnoreCase)) //if the name matches the current player's save folder name
                        {
                            validSave = true;
                            break; //skip the rest of these checks
                        }
                    }

                    if (validSave) //if a valid save name was listed
                    {
                        Monitor.Log("Save file name matched a setting. File allowed.", LogLevel.Trace);
                    }
                    else
                    {
                        Monitor.Log("Save file name did NOT match any settings. File disabled.", LogLevel.Trace);
                        return false; //prevent config use
                    }
                }

                //check whether other mods exist
                if (config.File_Conditions.OtherMods != null && config.File_Conditions.OtherMods.Count > 0)
                {
                    Monitor.Log("Other mod condition(s) found. Checking...", LogLevel.Trace);

                    bool validMods = true; //whether all entries are accurate (true by default, unlike most other settings)

                    foreach (KeyValuePair<string, bool> entry in config.File_Conditions.OtherMods) //for each mod entry in OtherMods
                    {
                        bool validEntry = !(entry.Value); //whether the current entry is accurate (starts false if the mod should exist; starts true if the mod should NOT exist)

                        foreach (IModInfo mod in Helper.ModRegistry.GetAll()) //for each mod currently loaded by SMAPI
                        {
                            if (entry.Value == true) //if the mod should exist
                            {
                                if (entry.Key.Equals(mod.Manifest.UniqueID, StringComparison.OrdinalIgnoreCase)) //if this mod's UniqueID matches the OtherMods entry
                                {
                                    validEntry = true; //this entry is valid
                                    break; //skip the rest of these checks
                                }
                            }
                            else //if the mod should NOT exist
                            {
                                if (entry.Key.Equals(mod.Manifest.UniqueID, StringComparison.OrdinalIgnoreCase)) //if this mod's UniqueID matches the OtherMods entry
                                {
                                    validEntry = false; //this entry is invalid
                                    break; //skip the rest of these checks
                                }
                            }
                        }

                        if (validEntry) //if the current mod entry is valid
                        {
                            Monitor.Log($"Mod check successful: \"{entry.Key}\" {(entry.Value ? "does exist" : "does not exist")}.", LogLevel.Trace);
                        }
                        else //if the current mod entry is NOT valid
                        {
                            Monitor.Log($"Mod check failed: \"{entry.Key}\" {(entry.Value ? "does not exist" : "does exist")}.", LogLevel.Trace);
                            validMods = false;
                            break; //skip the rest of these checks
                        }
                    }

                    if (validMods) //if all mod entries in the list are valid
                    {
                        Monitor.Log("The OtherMods list matches the player's mods. File allowed.", LogLevel.Trace);
                    }
                    else //if any entries were NOT valid
                    {
                        Monitor.Log("The OtherMods list does NOT match the player's mods. File disabled.", LogLevel.Trace);
                        return false; //prevent config use
                    }
                }

                return true; //all checks were successful; config should be used
            }
        }
    }
}