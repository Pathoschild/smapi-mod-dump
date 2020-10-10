/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

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
            /// <summary>Loads all available data files for the current farm into FarmDataList. Checks the mod's data folder and any relevant content packs.</summary>
            /// <returns>True if the files loaded successfully; false otherwise.</returns>
            public static void LoadFarmData()
            {
                Monitor.Log("Beginning file loading process...", LogLevel.Trace);

                //clear any existing farm data
                FarmDataList = new List<FarmData>();

                FarmConfig config; //temp for the current config as it's loaded
                InternalSaveData save; //temp for the current save as it's loaded

                if (MConfig.EnableContentPacks) //if content packs are enabled
                {
                    //load data from each relevant content pack
                    foreach (IContentPack pack in Helper.ContentPacks.GetOwned())
                    {
                        Monitor.Log($"Loading files from content pack: {pack.Manifest.Name}", LogLevel.Trace);

                        //clear each temp object
                        config = null;
                        save = null;

                        //attempt to load the farm config from this pack
                        try
                        {
                            config = pack.ReadJsonFile<FarmConfig>($"content.json"); //load the content pack's farm config (null if it doesn't exist)
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Warning: This content pack could not be parsed correctly: {pack.Manifest.Name}", LogLevel.Warn);
                            Monitor.Log($"Please edit the content.json file or reinstall the content pack. The auto-generated error message is displayed below:", LogLevel.Warn);
                            Monitor.Log($"----------", LogLevel.Warn);
                            Monitor.Log($"{ex.Message}", LogLevel.Warn);
                            continue; //skip to the next content pack
                        }

                        if (config == null) //no config file found for this farm
                        {
                            Monitor.Log($"Warning: The content.json file for this content pack could not be found: {pack.Manifest.Name}", LogLevel.Warn);
                            Monitor.Log($"Please reinstall the content pack. If you are its author, please create a config file named content.json in the pack's main folder (not the /data/ folder).", LogLevel.Warn);
                            continue; //skip to the next content pack
                        }

                        //attempt to load the save data for this pack and specific farm
                        try
                        {
                            save = pack.ReadJsonFile<InternalSaveData>($"data/{Constants.SaveFolderName}_SaveData.save"); //load the content pack's save data for this farm (null if it doesn't exist)
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Warning: Your farm's save data for this content pack could not be parsed correctly: {pack.Manifest.Name}", LogLevel.Warn);
                            Monitor.Log($"This file will need to be edited or deleted: data/{Constants.SaveFolderName}_SaveData.save", LogLevel.Warn);
                            Monitor.Log($"The content pack will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                            Monitor.Log($"----------", LogLevel.Warn);
                            Monitor.Log($"{ex.Message}", LogLevel.Warn);
                            continue; //skip to the next content pack
                        }

                        if (save == null) //no save file found for this farm
                        {
                            save = new InternalSaveData(); //load the (built-in) default save settings
                        }

                        ValidateFarmData(config, pack); //validate certain data in the current file before using it

                        pack.WriteJsonFile($"content.json", config); //update the content pack's config file
                        pack.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), save); //create or update the content pack's save file for the current farm

                        if (CheckFileConditions(config, pack)) //check file conditions; only use the current data if this returns true
                        {
                            FarmDataList.Add(new FarmData(config, save, pack)); //add the config, save, and content pack to the farm data list
                            Monitor.Log("Content pack loaded successfully.", LogLevel.Trace);
                        }
                    }

                    Monitor.Log("All available content packs checked.", LogLevel.Trace);
                }
                else
                {
                    Monitor.Log("Content packs disabled in config.json. Skipping to local files...", LogLevel.Trace);
                }

                //clear each temp object
                config = null;
                save = null;

                Monitor.Log("Loading files from FarmTypeManager/data", LogLevel.Trace);

                //attempt to load the farm config from this mod's data folder
                //NOTE: this should always be done *after* content packs, because it will end the loading process if an error occurs
                try
                {
                    config = Helper.Data.ReadJsonFile<FarmConfig>(Path.Combine("data", $"{Constants.SaveFolderName}.json")); //load the current save's config file (null if it doesn't exist)
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Warning: This file could not be parsed correctly: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Warn);
                    Monitor.Log($"Please edit the file, or delete it and reload your farm to generate a new config file.", LogLevel.Warn);
                    Monitor.Log($"Your config file will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                    Monitor.Log($"----------", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return; //end this process without adding this set of farm data
                }

                if (config == null) //no config file found for this farm
                {
                    //attempt to load the default.json config file
                    try
                    {
                        config = Helper.Data.ReadJsonFile<FarmConfig>(Path.Combine("data", "default.json")); //load the default.json config file (null if it doesn't exist)
                    }
                    catch (Exception ex)
                    {
                        Monitor.Log($"Warning: This file could not be parsed correctly: FarmTypeManager/data/default.json", LogLevel.Warn);
                        Monitor.Log($"Please edit the file, or delete it and reload your farm to generate a new default config file.", LogLevel.Warn);
                        Monitor.Log($"Your config file will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                        Monitor.Log($"----------", LogLevel.Warn);
                        Monitor.Log($"{ex.Message}", LogLevel.Warn);
                        return; //end this process without adding this set of farm data
                    }

                    if (config == null) //no default.json config file
                    {
                        config = new FarmConfig(); //load the (built-in) default config settings
                    }

                    ValidateFarmData(config, null); //validate certain data in the current file before using it

                    Helper.Data.WriteJsonFile(Path.Combine("data", "default.json"), config); //create or update the default.json config file
                }

                //attempt to load the save data for this farm
                try
                {
                    save = Helper.Data.ReadJsonFile<InternalSaveData>(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save")); //load the mod's save data for this farm (null if it doesn't exist)
                }
                catch (Exception ex)
                {
                    Monitor.Log($"Warning: This file could not be parsed correctly: FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save", LogLevel.Warn);
                    Monitor.Log($"Please edit the file, or delete it and reload your farm to generate a new savedata file.", LogLevel.Warn);
                    Monitor.Log($"Your config file will be skipped until this issue is fixed. The auto-generated error message is displayed below:", LogLevel.Warn);
                    Monitor.Log($"----------", LogLevel.Warn);
                    Monitor.Log($"{ex.Message}", LogLevel.Warn);
                    return; //end this process without adding this set of farm data
                }

                if (save == null) //no save file found for this farm
                {
                    save = new InternalSaveData(); //load the (built-in) default save settings
                }

                ValidateFarmData(config, null); //validate certain data in the current file before using it

                Helper.Data.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}.json"), config); //create or update the config file for the current farm
                Helper.Data.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), save); //create or update this config's save file for the current farm

                if (CheckFileConditions(config, null)) //check file conditions; only use the current data if this returns true
                {
                    FarmDataList.Add(new FarmData(config, save, null)); //add the config, save, and a *null* content pack to the farm data list
                    Monitor.Log("FarmTypeManager/data farm data loaded successfully.", LogLevel.Trace);
                }
            }
        }
    }
}