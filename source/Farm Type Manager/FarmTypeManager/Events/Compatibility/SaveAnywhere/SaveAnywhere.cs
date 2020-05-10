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

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).</summary>
        public void EnableSaveAnywhere(object sender, GameLaunchedEventArgs e)
        {
            //Save Anywhere: pass compatibility events for handling this mod's custom classes
            ISaveAnywhereAPI saveAnywhere = Utility.Helper.ModRegistry.GetApi<ISaveAnywhereAPI>("Omegasis.SaveAnywhere");
            if (saveAnywhere != null) //if the API was accessed successfully
            {
                Utility.Monitor.Log("Save Anywhere API loaded. Sending compatibility events.", LogLevel.Trace);
                saveAnywhere.addBeforeSaveEvent(ModManifest.UniqueID, SaveAnywhere_BeforeSave);
                /*
                 * disable "aftersave" due to the current version of SaveAnywhere not executing it; a workaround has been added below
                 * 
                saveAnywhere.addAfterSaveEvent(ModManifest.UniqueID, SaveAnywhere_AfterSave);
                */

                Utility.Helper.Events.Display.MenuChanged += SaveAnywhere_MenuChanged;
            }
        }

        private void SaveAnywhere_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything

            if (e.OldMenu != null && e.OldMenu.GetType().FullName.Contains("SaveAnywhere") && e.NewMenu == null) //if the old menu was a SaveAnywhere menu & no new menu is being displayed
            {
                SaveAnywhere_AfterSave(); //call the AfterSave method directly
            }
        }

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
