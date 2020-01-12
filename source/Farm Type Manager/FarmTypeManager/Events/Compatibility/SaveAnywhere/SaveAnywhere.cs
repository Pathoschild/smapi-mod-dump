using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    /// <summary>An interface describing the necessary content of Save Anywhere's API.</summary>
    public interface ISaveAnywhereAPI
    {
        void addBeforeSaveEvent(string ID, Action BeforeSave);
        void addAfterSaveEvent(string ID, Action BeforeSave);
    }

    public partial class ModEntry : Mod
    {
        /// <summary>Saves and removes objects/data that cannot be handled by Stardew Valley's save process.</summary>
        private void SaveAnywhere_BeforeSave()
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything

            Utility.Monitor.Log($"Modded save event started. Saving and removing custom objects/data.", LogLevel.Trace);

            Utility.MonsterTracker.Clear(); //clear any tracked monster data (note: this should happen *before* handling monster expiration/removal)

            if (Utility.FarmDataList == null) { return; } //if the farm data list is blank, do nothing

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

        /// <summary>Restores saved objects/data that could not be handled by Stardew Valley's save process.</summary>
        private void SaveAnywhere_AfterSave()
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything

            Monitor.Log($"Modded save event ended. Restoring custom objects/data.", LogLevel.Trace);

            //note: do not clear Utility.TimedSpawns here; that should only happen when in the DayStarted event

            Utility.MonsterTracker.Clear(); //clear stored monster information before they are respawned (note: all of this mod's monsters should be removed before saving)

            foreach (FarmData data in Utility.FarmDataList) //for each loaded set of data
            {
                if (data.Pack != null) //if this data is from a content pack
                {
                    Monitor.VerboseLog($"Checking objects from content pack: {data.Pack.Manifest.Name}");
                }
                else //this data is from this mod's own folders
                {
                    Monitor.VerboseLog($"Checking objects from FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save");
                }

                Utility.ReplaceProtectedSpawns(data.Save); //protect unexpired spawns listed in the save data
            }
        }
    }
}
