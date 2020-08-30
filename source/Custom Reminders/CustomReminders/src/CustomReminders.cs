using Dem1se.CustomReminders.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Dem1se.CustomReminders
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary> Object containing the read data from config file.</summary>
        private ModConfig Config;
        protected string NotificationSound;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load the Config
            Config = Helper.ReadConfig<ModConfig>();
            Monitor.Log("Config loaded and read.");

            // set up statics (utilities.cs)
            Utilities.Data.Helper = Helper;
            Utilities.Data.Monitor = Monitor;

            // Set the notification sound
            NotificationSound = Config.SubtlerReminderSound ? "crit" : "questcomplete";
            Monitor.Log($"Notification sound set to {NotificationSound}.");

            // Binds the event with method.
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.TimeChanged += ReminderNotifier;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Multiplayer.ModMessageReceived += Multiplayer.Multiplayer.OnModMessageReceived;
            helper.Events.Multiplayer.PeerContextReceived += Multiplayer.Multiplayer.OnPeerConnected;
            helper.Events.GameLoop.GameLaunched += MobilePhoneModAPI.MobilePhoneMod.HookToMobilePhoneMod;
        }

        ///<summary> Defines what happens when a save is loaded</summary>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // set the SaveFolderName field if multiplayer host or singleplayer
            if (Context.IsMainPlayer)
            {
                Utilities.Data.SaveFolderName = Constants.SaveFolderName;
                Utilities.Data.MenuButton = Utilities.Data.GetMenuButton();
            }
            else
            {
                // SaveFolderName will be assigned on peerContextRecieved
                Utilities.Data.MenuButton = Config.FarmhandInventoryButton;
            }

            /* Create the data subfolder for the save for first time users. 
             * Avoid DirectoryNotFound Exception in OnChangedBehaviour() when trying to save new reminder for first time */
            if (!Directory.Exists(Path.Combine(Helper.DirectoryPath, "data", Utilities.Data.SaveFolderName)))
            {
                Monitor.Log("Reminders directory not found. Creating directory.", LogLevel.Info);
                Directory.CreateDirectory(Path.Combine(Helper.DirectoryPath, "data", Utilities.Data.SaveFolderName));
                Monitor.Log("Reminders directory created successfully.", LogLevel.Info);
            }
        }

        /// <summary> Defines what happens when user press the config button </summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs ev)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady) { return; }
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

            ModConfig config = Utilities.Data.Helper.ReadConfig<ModConfig>();

            // Do the MobilePhoneMod housekeeping
            var api = Utilities.Data.Helper.ModRegistry.GetApi<MobilePhoneModAPI.IMobilePhoneApi>("aedenthorn.MobilePhone");
            if (config.EnableMobilePhoneApp)
            {
                if (api != null)
                {
                    api.SetAppRunning(true);
                    api.SetPhoneOpened(false);
                }
            }

            Utilities.Data.Monitor.Log("Opening ReminderMenu page 1");
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
                Utilities.Data.Monitor.Log("First page completed. Opening second page now.");
                Game1.activeClickableMenu = new NewReminder_Page2((int time) =>
                {
                    reminderTime = time;
                    // write the data to file
                    Utilities.Files.WriteToFile(reminderMessage, reminderDate, reminderTime);
                    Utilities.Data.Monitor.Log($"Saved new reminder: {reminderMessage} for {season} {day} at {Utilities.Converts.ConvertToPrettyTime(reminderTime)}.", LogLevel.Info);
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
            // returns function if game time isn't multiple of 30 in-game minutes.
            string timeString = Convert.ToString(ev.NewTime);
            if (!(timeString.EndsWith("30") || timeString.EndsWith("00"))) return;

            // Loops through all the reminder files and evaluates if they are current.
            #region ReminderNotifierloop
            SDate currentDate = SDate.Now();
            foreach (string filePathAbsolute in Directory.EnumerateFiles(Path.Combine(Helper.DirectoryPath, "data", Utilities.Data.SaveFolderName)))
            {
                try
                {
                    // make relative path from absolute path
                    string filePathRelative = Utilities.Extras.MakeRelativePath(filePathAbsolute);

                    // Read the reminder and notify if mature
                    Monitor.Log($"Processing {ev.NewTime}");
                    ReminderModel Reminder = Helper.Data.ReadJsonFile<ReminderModel>(filePathRelative);
                    if (Reminder.DaysSinceStart == currentDate.DaysSinceStart)
                    {
                        if (Reminder.Time == ev.NewTime)
                        {
                            Game1.addHUDMessage(new HUDMessage(Reminder.ReminderMessage, 2));
                            Game1.playSound(NotificationSound);
                            Monitor.Log($"Reminder notified for {Reminder.DaysSinceStart}: {Reminder.ReminderMessage}", LogLevel.Info);
                            File.Delete(filePathAbsolute);
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
