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
using System;
using System.Collections.Generic;
using System.IO;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Loads all available content packs' data into a list. This does NOT include farm-specific personal data from the folder "FarmTypeManager/data".</summary>
            /// <returns>A list of all available content packs' data.</returns>
            public static List<FarmData> LoadFarmDataForApi(IManifest manifest)
            {
                Monitor.Log($"The mod \"{manifest?.Name ?? "null"}\" ({manifest?.UniqueID ?? "null ID"}) used to API to request information about all content packs. Loading...", LogLevel.Trace);

                List<FarmData> list = new List<FarmData>();

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

                        list.Add(new FarmData(config, save, pack)); //add the config, save, and content pack to the farm data list
                        Monitor.Log("Content pack loaded successfully.", LogLevel.Trace);
                    }

                    Monitor.Log("All available content packs checked for this API call.", LogLevel.Trace);
                }
                else
                {
                    Monitor.Log("Content packs are disabled in config.json. No information to load for this API call.", LogLevel.Trace);
                }

                return list;
            }
        }
    }
}