/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dem1se/SDVMods
**
*************************************************/

using Dem1se.CustomReminders.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dem1se.CustomReminders
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary> Object containing the read data from config file.</summary>
        private ModConfig Config;
        protected string NotificationSound;
        /// <summary> List of absolute file paths to reminders that have matured and are awaiting cleanup </summary>
        private Queue<string> DeleteQueue = new Queue<string>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load the config
            Config = Helper.ReadConfig<ModConfig>();
            Monitor.Log("Config loaded and read.");

            // Set up globals (utilities.cs)
            Utilities.Globals.Helper = Helper;
            Utilities.Globals.Monitor = Monitor;

            // Set the notification sound
            NotificationSound = Config.SubtlerReminderSound ? "crit" : "questcomplete";
            Monitor.Log($"Notification sound set to {NotificationSound}.");

            // Binds the event with method.
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.TimeChanged += ReminderNotifier;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.Multiplayer.ModMessageReceived += Multiplayer.Multiplayer.OnModMessageReceived;
            helper.Events.Multiplayer.PeerContextReceived += Multiplayer.Multiplayer.OnPeerConnected;
            helper.Events.GameLoop.GameLaunched += MobilePhoneModAPI.MobilePhoneMod.HookToMobilePhoneMod;
        }

        /// <summary> Loops through any mature reminders since last save and deletes their file </summary>
        //Json-x-ly Notes: Alternatively we could just parse the files again and cleanup any old entries if we'd like to avoid maintaining a collection
        private void OnSaved(object sender, SavedEventArgs ev)
        {
            for (var i = 0; i < DeleteQueue.Count; i++)
            {
                string AbsolutePath = DeleteQueue.Dequeue();
                try
                {
                    if (File.Exists(AbsolutePath))
                    {
                        File.Delete(AbsolutePath);
                    }
                }
                catch (Exception e)
                {
                    Monitor.Log(e.Message, LogLevel.Error);
                }
            }
        }

        ///<summary> Defines what happens when a save is loaded</summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // set the SaveFolderName field if multiplayer host or singleplayer
            if (Context.IsMainPlayer)
            {
                Utilities.Globals.SaveFolderName = Constants.SaveFolderName;
                Utilities.Globals.MenuButton = Utilities.Globals.GetMenuButton();
            }
            else
            {
                // SaveFolderName will be assigned on peerContextRecieved
                Utilities.Globals.MenuButton = Config.FarmhandInventoryButton;
            }

            /* Create the data subfolder for the save for first time users. 
             * Avoid DirectoryNotFound Exception in OnChangedBehaviour() when trying to save new reminder for first time */
            if (!Directory.Exists(Path.Combine(Helper.DirectoryPath, "data", Utilities.Globals.SaveFolderName)))
            {
                Monitor.Log("Reminders directory not found. Creating directory.", LogLevel.Info);
                Directory.CreateDirectory(Path.Combine(Helper.DirectoryPath, "data", Utilities.Globals.SaveFolderName));
                Monitor.Log("Reminders directory created successfully.", LogLevel.Info);
            }

            //Json-x-ly Notes: Wipes the Queue for the new save context
            DeleteQueue.Clear();

            //Json-x-ly Notes: Checks to see if there are any mature reminders at start of day on load
            ReminderNotifierLoop(Game1.timeOfDay);
        }

        /// <summary> Defines what happens when user press the config button </summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs ev)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) return;
            if (Game1.activeClickableMenu != null || (!Context.IsPlayerFree) || ev.Button != Config.CustomRemindersButton) return;

            ShowReminderMenu();
        }

        /// <summary>Create a new instance of the Reminder menus, and displays them. Called on Button press</summary>
        public static void ShowReminderMenu()
        {
            /* These are all the variables that hold the values of the reminder */
            string reminderMessage;
            int reminderDate;
            int reminderTime;

            ModConfig config = Utilities.Globals.Helper.ReadConfig<ModConfig>();

            // Do the MobilePhoneMod housekeeping
            var api = Utilities.Globals.Helper.ModRegistry.GetApi<MobilePhoneModAPI.IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (config.EnableMobilePhoneApp)
            {
                if (api != null)
                {
                    api.SetAppRunning(true);
                    api.SetPhoneOpened(false);
                }
            }

            Utilities.Globals.Monitor.Log("Opening ReminderMenu page 1");
            Game1.activeClickableMenu = new NewReminder_Page1((string message, string season, int day) =>
            {
                int seasonIndex = (int)Enum.Parse(typeof(Utilities.Season), season);
                Game1.exitActiveMenu();

                // Convert to DaysSinceStart - Contextually choose year.
                int year;

                if (SDate.Now().SeasonIndex == seasonIndex) // same seasons
                    year = (SDate.Now().Day > day) ? SDate.Now().Year + 1 : SDate.Now().Year;
                else if (SDate.Now().SeasonIndex > seasonIndex) // past season
                    year = SDate.Now().Year + 1;
                else // future season
                    year = SDate.Now().Year;

                reminderDate = Utilities.Converts.ConvertToDays(day, seasonIndex, year);
                reminderMessage = message;
                // open the second page
                Utilities.Globals.Monitor.Log("First page completed. Opening second page now.");
                Game1.activeClickableMenu = new NewReminder_Page2((int time) =>
                {
                    reminderTime = time;
                    // write the data to file
                    Utilities.Files.WriteToFile(reminderMessage, reminderDate, reminderTime);
                    Utilities.Globals.Monitor.Log($"Saved new reminder: {reminderMessage} for {season} {day} at {Utilities.Converts.ConvertToPrettyTime(reminderTime)}.", LogLevel.Info);
                });
            });

            // MobilePhoneMod exit housekeeping
            if (config.EnableMobilePhoneApp)
                if (api != null)
                    api.SetAppRunning(false);
        }

        /// <summary> Loop that checks if any reminders are mature.</summary>
        private void ReminderNotifier(object sender, TimeChangedEventArgs ev)
        {
            ReminderNotifierLoop(ev.NewTime);
        }

        // Json-x-ly Notes: Separated for OnSaveLoaded check since loading a new game does not send a TimeChanged event for the 600 hour 
        private void ReminderNotifierLoop(int newTime)
        {
            // returns function if game time isn't multiple of 30 in-game minutes.
            string timeString = Convert.ToString(newTime);
            if (!(timeString.EndsWith("30") || timeString.EndsWith("00"))) return;

            // Loops through all the reminder files and evaluates if they are current.

            #region ReminderNotifierloop
            SDate currentDate = SDate.Now();
            foreach (string filePathAbsolute in Directory.EnumerateFiles(Path.Combine(Helper.DirectoryPath, "data", Utilities.Globals.SaveFolderName)))
            {
                try
                {
                    // make relative path from absolute path
                    string filePathRelative = Utilities.Extras.MakeRelativePath(filePathAbsolute);

                    // make sure the file is a Json. Fix for Vortex generated files causing issues.
                    if (!filePathRelative.ToLower().EndsWith(".json"))
                        continue;

                    // Read the reminder and notify if mature
                    Monitor.Log($"Processing {newTime}");
                    ReminderModel Reminder = Helper.Data.ReadJsonFile<ReminderModel>(filePathRelative);
                    if (Reminder.DaysSinceStart == currentDate.DaysSinceStart)
                    {
                        if (Reminder.Time == newTime)
                        {
                            Game1.addHUDMessage(new HUDMessage(Reminder.ReminderMessage, 2));
                            Game1.playSound(NotificationSound);
                            Monitor.Log($"Reminder notified for {Reminder.DaysSinceStart}: {Reminder.ReminderMessage}", LogLevel.Info);
                            // Store the path for deletion later.
                            DeleteQueue.Enqueue(filePathAbsolute);
                        }
                        /* this is a very rare case (should be impossible) and won't happen normally, but I've still included it just in case,
                         * (to avoid sedimentary files hogging the performance unnecessarily) */
                        else if (Reminder.DaysSinceStart < SDate.Now().DaysSinceStart)
                        {
                            File.Delete(filePathAbsolute);
                            Monitor.Log("Deleted old, useless reminder", LogLevel.Info);
                        }
                    }
                }
                catch (Exception e)
                {
                    Monitor.Log(e.Message, LogLevel.Error);
                }
            }
            #endregion
        }
    }
}
