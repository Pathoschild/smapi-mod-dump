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

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<TimeConfig>();
            PrevDate = DateTime.Now;

            helper.Events.GameLoop.OneSecondUpdateTicked += GameEvents_OneSecondTick;
            helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;

            Helper.ConsoleCommands.Add("SetReminder","This sets a one time reminder", SetReminder);
            Helper.ConsoleCommands.Add("ClearAllReminders", "This clears all reminders", ClearAllReminder);
            Helper.ConsoleCommands.Add("SetRReminder", "This sets a recurring reminder", SetRecurringReminder);
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
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

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
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
