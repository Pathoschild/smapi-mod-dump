using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed before a day ends, i.e. right before saving. This is also called when a new farm is created, *before* DayStarted.</summary>
        private void DayEnding(object sender, EventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            if (Utility.FarmDataList == null || Utility.FarmDataList.Count < 1) { return; } //if the farm data list is blank, do nothing (e.g. when called by a newly created farm)

            foreach (FarmData data in Utility.FarmDataList) //for each set of farm data
            {
                if (data.Pack != null) //if this data is from a content pack
                {
                    Monitor.VerboseLog($"Processing save data for content pack: {data.Pack.Manifest.Name}");
                }
                else //this data is from this mod's own folders
                {
                    Monitor.VerboseLog($"Processing save data for FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save");
                }

                Utility.ProcessObjectExpiration(data.Save); //remove expired objects & update saved expiration data

                data.Save.WeatherForYesterday = Utility.WeatherForToday(); //update saved weather info

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
