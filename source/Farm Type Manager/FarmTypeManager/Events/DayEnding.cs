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
using System.IO;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed before a day ends, i.e. right before saving. This is also called when a new farm is created, *before* DayStarted.</summary>
        private void DayEnding(object sender, EventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything

            Utility.Monitor.Log($"Day is ending. Processing save data and object expiration settings.", LogLevel.Trace);

            Utility.DayIsEnding = true; //set the "day is ending" flag

            Utility.TimedSpawns.Clear(); //clear any remaining spawns for the day (avoiding a known issue with mid-save/overnight time changes)
            Utility.MonsterTracker.Clear(); //clear any tracked monster data (preventing loot drops when monsters are removed by this process)

            if (Utility.FarmDataList == null) { return; } //if the farm data list is blank, do nothing

            foreach (FarmData data in Utility.FarmDataList) //for each set of farm data
            {
                if (data.Pack != null) //if this data is from a content pack
                {
                    Utility.Monitor.VerboseLog($"Processing save data for content pack: {data.Pack.Manifest.Name}");
                }
                else //this data is from this mod's own folders
                {
                    Utility.Monitor.VerboseLog($"Processing save data for FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save");
                }

                Utility.ProcessObjectExpiration(save: data.Save, endOfDay: true); //remove custom object classes, but do not process expiration settings

                data.Save.WeatherForYesterday = Game1.netWorldState.Value.GetWeatherForLocation("Default").weather.Value; //update saved weather info

                if (data.Pack != null) //if this data is from a content pack
                {
                    data.Pack.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), data.Save); //update the save file for that content pack
                }
                else //this data is from this mod's own folders
                {
                    Helper.Data.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), data.Save); //update the save file in this mod's own folders
                }
            }
        }
    }
}
