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
using System;
using System.Collections.Generic;
using System.Reflection;

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
                            string farmTypeID = Helper.Reflection.GetMethod(typeof(Game1), "GetFarmTypeID", false)?.Invoke<string>(); //get the farm type ID string (null in SDV 1.5.4 or older) 

                            if (name.Equals("All", StringComparison.OrdinalIgnoreCase) || name.Equals("Any", StringComparison.OrdinalIgnoreCase)) //if this is "all" or "any"
                            {
                                validType = true;
                                break; //skip checking the rest of the farm types
                            }
                            else if (name.Equals(farmTypeID, StringComparison.OrdinalIgnoreCase)) //if this is the name of the player's SDV-supported custom farm type (SDV 1.5.5 or newer)
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
                        else if (Game1.whichFarm == 200) //if this may be a MTN farm type, handle compatibility (based on MTN 2.1.0-beta8)
                        {
                            //if MTN is installed, use reflection to access its "whichFarm" equivalent

                            try
                            {
                                IModInfo modInfo = Helper.ModRegistry.Get("SgtPickles.MTN"); // get MTN info (null if it's not installed)
                                if (modInfo == null) //if MTN isn't installed
                                    continue; //skip to the next farm type check

                                object mod = modInfo.GetType().GetProperty("Mod", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(modInfo); //get MTN's Mod instance
                                if (mod == null) //if it couldn't be accessed
                                    continue; //skip to the next farm type check

                                object customManager = mod.GetType().GetProperty("CustomManager", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(mod); //get CustomManager instance
                                if (customManager == null) //if it couldn't be accessed
                                    continue; //skip to the next farm type check

                                object loadedFarm = customManager.GetType().GetProperty("LoadedFarm", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(customManager); //get LoadedFarm instance
                                if (loadedFarm == null) //if it couldn't be accessed
                                    continue; //skip to the next farm type check

                                int farmID = (int)loadedFarm.GetType().GetProperty("ID", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(loadedFarm); //get the loaded farm's ID

                                if (type == farmID) //if MTN's custom farm ID matches the parsed type
                                {
                                    Monitor.VerboseLog($"Farm type matches the loaded MTN farm type's ID: {type}");
                                    validType = true;
                                    break; //skip checking the rest of the farm types
                                }
                            }
                            catch (Exception) //if any exception is thrown while accessing MTN
                            {
                                Monitor.Log($"Error encountered while trying to check MTN farm type. This check may be obsolete or require updates.", LogLevel.Trace);
                                continue; //skip to the next farm type check
                            }
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