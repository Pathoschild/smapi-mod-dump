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
using StardewModdingAPI.Events;
using System;

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
        /// <summary>True if DayStarted events should be skipped, e.g. due to being caused by a mid-day save with SaveAnywhere.</summary>
        /// <remarks>
        /// As of SaveAnywhere v3.2.7, saving mid-day causes a DayStarted event after saving is complete.
        /// This should be skipped by FTM to avoid errors (additional spawns, saved spawn issues, untracked monsters leading to serialization failure).
        /// </remarks>
        public static bool SkipDayStartedEvents { get; set; } = false;

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).</summary>
        public void EnableSaveAnywhere(object sender, GameLaunchedEventArgs e)
        {
            //Save Anywhere: pass compatibility events for handling this mod's custom classes
            ISaveAnywhereAPI saveAnywhere = Utility.Helper.ModRegistry.GetApi<ISaveAnywhereAPI>("Omegasis.SaveAnywhere");
            if (saveAnywhere != null) //if the API was accessed successfully
            {
                Utility.Monitor.Log("Save Anywhere API loaded. Sending compatibility events.", LogLevel.Trace);
                saveAnywhere.addBeforeSaveEvent(ModManifest.UniqueID, SaveAnywhere_BeforeSave);
                saveAnywhere.addAfterSaveEvent(ModManifest.UniqueID, SaveAnywhere_AfterSave);

                Helper.Events.GameLoop.DayEnding += SaveAnywhere_DayEnding;
                Helper.Events.GameLoop.ReturnedToTitle += SaveAnywhere_ReturnedToTitle;
            }
        }

        /// <summary>Saves and removes objects/data that cannot be handled by Stardew Valley's save process.</summary>
        private void SaveAnywhere_BeforeSave()
        {
            Utility.GameIsSaving = true;

            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, do nothing

            SaveAnywhereIsSaving = true;
            SkipDayStartedEvents = true;

            BeforeMidDaySave();
        }

        /// <summary>Restores saved objects/data that could not be handled by Stardew Valley's save process.</summary>
        private void SaveAnywhere_AfterSave()
        {
            if (SaveAnywhereIsSaving == false) { return; } //if SaveAnywhere isn't actually saving, skip this event (NOTE: As of SaveAnywhere v3.2.7, saving mid-day causes this event to occur during normal end-of-day saves as well)

            Utility.GameIsSaving = false;

            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, do nothing

            AfterMidDaySave();

            SaveAnywhereIsSaving = false;
        }

        /// <summary>Stops skipping DayStarted events (if applicable) at the end of each day.</summary>
        private void SaveAnywhere_DayEnding(object sender, DayEndingEventArgs e)
        {
            SkipDayStartedEvents = false;
        }

        /// <summary>Stops skipping DayStarted events (if applicable) when the player exits a loaded game.</summary>
        private void SaveAnywhere_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            SkipDayStartedEvents = false;
        }
    }
}
