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

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed before the game saves.</summary>
        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            Utility.GameIsSaving = true;

            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything
            if (Utility.DayIsEnding || SaveAnywhereIsSaving) { return; } //if a specialized save process is already handling this, don't do anything

            BeforeMidDaySave();
        }

        /// <summary>Saves and removes custom entities before a mid-day save event.</summary>
        private void BeforeMidDaySave()
        {
            Utility.Monitor.Log($"Mid-day save event started. Saving and removing custom objects/data.", LogLevel.Trace);

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

                Utility.ProcessObjectExpiration(save: data.Save, endOfDay: false); //remove custom object classes, but do not process expiration settings

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
