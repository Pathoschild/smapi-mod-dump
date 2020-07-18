using Dem1se.CustomReminders.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.IO;

namespace Dem1se.CustomReminders
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /// <summary> Object containing the read data from config file.</summary>
        private ModConfig Config;

        /* These are all the fields that hold the values of the reminder */
        protected string ReminderMessage;
        protected int ReminderDate;
        protected int ReminderTime;

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

            // Create the data subfolder for the save for first time users. ( Avoid DirectoryNotFound Exception in OnChangedBehaviour() )
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
            if (Game1.activeClickableMenu != null || (!Context.IsPlayerFree) || ev.Button != Config.CustomRemindersButton) { return; }

            Monitor.Log("Opening ReminderMenu page 1");
            Game1.activeClickableMenu = new NewReminder_Page1((string message, string season, int day) =>
            {
                // have to capitalize the season due to the enum members being Pascal case and season being all lower case.
                int seasonIndex = (int)Enum.Parse(typeof(Utilities.Season), season.Replace(season[0], char.ToUpper(season[0])));
                int year;
                Game1.exitActiveMenu();
                // Convert to DaysSinceStart - calculate year fix.
                if (SDate.Now().SeasonIndex == seasonIndex) // same seasons
                {
                    if (SDate.Now().Day > day) // same season , past date
                    {
                        year = SDate.Now().Year + 1;
                        ReminderDate = Utilities.Converts.ConvertToDays(day, seasonIndex, year);
                    }
                    else if (SDate.Now().Day == day) // same season, same date
                    {
                        year = SDate.Now().Year;
                        ReminderDate = Utilities.Converts.ConvertToDays(day, seasonIndex, year);
                    }
                    else // same season, Future Date
                    {
                        year = SDate.Now().Year;
                        ReminderDate = Utilities.Converts.ConvertToDays(day, seasonIndex, year);
                    }
                }
                else if (SDate.Now().SeasonIndex > seasonIndex) // past season
                {
                    year = SDate.Now().Year + 1;
                    ReminderDate = Utilities.Converts.ConvertToDays(day, seasonIndex, year);
                }
                else // future season
                {
                    year = SDate.Now().Year;
                    ReminderDate = Utilities.Converts.ConvertToDays(day, seasonIndex, year);
                }
                ReminderMessage = message;
                // open the second page
                Monitor.Log("First page completed. Opening second page now.");
                Game1.activeClickableMenu = new NewReminder_Page2((int time) =>
                {
                    ReminderTime = time;
                    // write the data to file
                    Utilities.Files.WriteToFile(ReminderMessage, ReminderDate, ReminderTime);
                    Monitor.Log($"Saved new reminder: {ReminderMessage} for {season} {day} at {Utilities.Converts.ConvertToPrettyTime(ReminderTime)}.", LogLevel.Info);
                });

            });
        }

        /// <summary> Loop that checks if any reminders are mature.</summary>
        private void ReminderNotifier(object sender, TimeChangedEventArgs ev)
        {
            // returns function if game time isn't multiple of 30 in-game minutes.
            string timeString = Convert.ToString(ev.NewTime);
            if (!(timeString.EndsWith("30") || timeString.EndsWith("00"))) { return; }

            // Loops through all the reminder files and evaluates if they are current.
            #region CoreReminderNotiferLoop
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
