/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace TimeReminder
{
    public class TimeConfig
    {
        public int NumOfMinutes = 6;
        public bool AlertOnTheHour = true;
    }

    public class TimeReminder : Mod
    {
        public TimeConfig Config { get; set; }
        private DateTime PrevDate { get; set; }
        private int OneTimeReminder = 0;
        private int RecurringReminder = 0;

        private bool NotTriggered = true;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<TimeConfig>();
            PrevDate = DateTime.Now;

            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondTicked;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;

            Helper.ConsoleCommands.Add("SetReminder","This sets a one time reminder", SetReminder);
            Helper.ConsoleCommands.Add("ClearAllReminders", "This clears all reminders", ClearAllReminder);
            Helper.ConsoleCommands.Add("SetRReminder", "This sets a recurring reminder", SetRecurringReminder);
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<Integrations.GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.RegisterModConfig(ModManifest, () => Config = new TimeConfig(), () => Helper.WriteConfig(Config));
                api.RegisterSimpleOption(ModManifest, "Alerts On The Hour", "This option toggles alerts on the real world hour", () => Config.AlertOnTheHour, (bool val) => Config.AlertOnTheHour = val);
                api.RegisterClampedOption(ModManifest, "NumOfMinutes", "This controls how many real world minutes it takes between alerts. Constrained to be 1 minute to 720 minutes (12 hours)", () => Config.NumOfMinutes, (int val) => Config.NumOfMinutes = val, 1, 720);
            }
        }

        /// <summary>Raised after the in-game clock time changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (e.NewTime == OneTimeReminder || e.NewTime == RecurringReminder)
            {
                Game1.addHUDMessage(new HUDMessage($"Reminder: it is {e.NewTime}", Color.OrangeRed, 8450f));
                OneTimeReminder = 0;
            }
        }

        private void ClearAllReminder(string arg1, string[] arg2)
        {
            OneTimeReminder = 0;
            RecurringReminder= 0;
        }

        private void SetRecurringReminder(string arg1, string[] arg2)
        {
            //this sets a onetime.
            if (Int32.TryParse(arg2[0], out int val))
            {
                if (val < 2600 || val > 0600)
                {
                    RecurringReminder = val;
                    return;
                }
                Game1.addHUDMessage(new HUDMessage("You attempted to set an invalid time - 0600 to 2600 only. Please remember that only 10 minute intervals are checked."));
            }
            Game1.addHUDMessage(new HUDMessage("Invalid input detected! You need to use 0600-2600 to set the time. Please remember that only 10 minute intervals are checked"));
        }

        private void SetReminder(string arg1, string[] arg2)
        {
            //this sets a onetime.
            if (Int32.TryParse(arg2[0], out int val))
            {
                if (val < 2600 || val > 0600) { 
                    OneTimeReminder = val;
                    return;
                }
                Game1.addHUDMessage(new HUDMessage("You attempted to set an invalid time - 0600 to 2600 only. Please remember that only 10 minute intervals are checked."));
            }
            Game1.addHUDMessage(new HUDMessage("Invalid input detected! You need to use 0600-2600 to set the time. Please remember that only 10 minute intervals are checked"));
        }

        /// <summary>Raised once per second after the game state is updated.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnOneSecondTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (PrevDate.Add(new TimeSpan(0,Config.NumOfMinutes,0)) < DateTime.Now){
                Game1.hudMessages.Add(new HUDMessage("The current system time is " + DateTime.Now.ToString("h:mm tt"),Color.OrangeRed, 8450f));
                PrevDate = DateTime.Now;
            }

            if (Config.AlertOnTheHour && DateTime.Now.Minute == 0 && NotTriggered)
            {
                Game1.hudMessages.Add(new HUDMessage("The current system time is " + DateTime.Now.ToString("h:mm tt"), Color.OrangeRed, 8450f));
                NotTriggered = false;
            }

            if (DateTime.Now.Minute != 0)
                NotTriggered = true;
        }
    }
}
