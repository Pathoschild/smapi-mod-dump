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
        /// <summary>True if SaveAnywhere's save process is currently being handled by this mod. Used to avoid redundant save data handling.</summary>
        public static bool SaveAnywhereIsSaving { get; set; } = false;

        /// <summary>Saves and removes objects/data that cannot be handled by Stardew Valley's save process.</summary>
        private void SaveAnywhere_BeforeSave()
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything

            SaveAnywhereIsSaving = true; //set the "saving" flag

            BeforeMidDaySave();
        }

        /// <summary>Restores saved objects/data that could not be handled by Stardew Valley's save process.</summary>
        private void SaveAnywhere_AfterSave()
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything

            AfterMidDaySave();

            SaveAnywhereIsSaving = false; //clear the "saving" flag
        }
    }
}
