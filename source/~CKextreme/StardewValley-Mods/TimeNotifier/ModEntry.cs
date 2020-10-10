/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CKextreme/StardewValley-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace TimeNotifier
{
    class ModEntry : Mod
    {
        private Models.TimeConfig Config { get; set; }
        private bool istriggered = false;
        private int lock_minute;
        private DateTime LastNotification { get; set; }

        private int OneTimeReminder;
        private HashSet<short> RepeatingReminder;

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<Models.TimeConfig>();
            SetDateTime();
            RepeatingReminder = new HashSet<short>();
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;

            // Übersetzen
            helper.ConsoleCommands.Add("reload_config", "Reloads changed config file without restarting game", ReloadConfig);
            helper.ConsoleCommands.Add("clear_all_reminders", "Clears both one-time and repeated-time in-game time reminders", ClearAllReminders);
            helper.ConsoleCommands.Add("set_reminder", "Usage: set_reminder [true=repeat|false=once] [timevalue]", SetReminder);
            helper.ConsoleCommands.Add("clear_reminder", "Usage: clear_reminder [true=repeat|false=once] [timevalue]", ClearReminder);
            helper.ConsoleCommands.Add("list_reminders","list all reminders",ListAllReminders);
        }

        private void SetReminder(string command, string[] args)
        {
            if(args.Length!=2) { Monitor.Log(Helper.Translation.Get("false_args"), LogLevel.Info); return; }

            // Check if should repeat is a valid bool and if time ist valid number
            if (bool.TryParse(args[0], out bool repeat) && short.TryParse(args[1], out short time))
            {
                // Check if time between 6 a.m and 2 p.m.
                if (time <= 2600 || time >= 600)
                {
                    time = time.RoundUp();
                    // check if should repeat in-game time
                    if(repeat)
                    {
                        if(!RepeatingReminder.Contains(time))
                        {
                            RepeatingReminder.Add(time);
                        }
                        else
                        {
                            Monitor.Log(Helper.Translation.Get("already_exists"), LogLevel.Info);
                            return;
                        }
                    }
                    else
                    {
                        OneTimeReminder = time;
                    }
                    Monitor.Log(Helper.Translation.Get("new_ingame") + $" ({time})", LogLevel.Info);
                }
                else
                {
                    Monitor.Log(Helper.Translation.Get("false_args"), LogLevel.Info);
                }
            }
            else
            {
                Monitor.Log(Helper.Translation.Get("false_value"), LogLevel.Info);
            }
        }

        private void ClearReminder(string command, string[] args)
        {
            if (args.Length != 2) { Monitor.Log(Helper.Translation.Get("false_args"), LogLevel.Info); return; }

            // Check if should repeat is a valid bool and if time ist valid number
            if (bool.TryParse(args[0], out bool repeat) && short.TryParse(args[1], out short time))
            {
                // Check if time between 6 a.m and 2 p.m.
                if (time <= 2600 || time >= 600)
                {
                    // check if repeat bool set
                    if (repeat)
                    {
                        RepeatingReminder.Remove(time);
                    }
                    else
                    {
                        OneTimeReminder = 0;
                    }
                    Monitor.Log(Helper.Translation.Get("removed_value"), LogLevel.Info);
                }
                else
                {
                    Monitor.Log(Helper.Translation.Get("false_args"), LogLevel.Info);
                }
            }
            else
            {
                Monitor.Log(Helper.Translation.Get("false_args"), LogLevel.Info);
            }
        }

        private void ClearAllReminders(string command, string[] args)
        {
            OneTimeReminder = 0;
            RepeatingReminder.Clear();
            Monitor.Log(Helper.Translation.Get("clear_success"), LogLevel.Info);
        }

        private void ListAllReminders(string command, string[] args)
        {
            var once = OneTimeReminder !=0 ? Helper.Translation.Get("once") + ": " + OneTimeReminder : "";
            var repeat = RepeatingReminder.Count != 0 ? Helper.Translation.Get("repeatedly") + ": " + string.Join(", ", RepeatingReminder) : "";

            if(once.Length == 0 && repeat.Length == 0)
            {
                Monitor.Log(Helper.Translation.Get("no_reminder"), LogLevel.Info);
            }
            if(!string.IsNullOrWhiteSpace(once))
            {
                Monitor.Log(once, LogLevel.Info);
            }
            if (!string.IsNullOrWhiteSpace(repeat))
            {
                Monitor.Log(repeat, LogLevel.Info);
            }
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            var tmp = (short)e.NewTime;
            if(tmp != OneTimeReminder && !RepeatingReminder.Contains(tmp)) { return; }
            Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("in_game_reminder") + ": " + tmp));
            OneTimeReminder = 0;
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            var now = DateTime.Now;

            // Save check if new hour and reset without check if seconds==0 because of process change it could be already 1
            // Event comes every ~ 1 second
            if (now.Minute==0 && lock_minute==0 || lock_minute <= now.Minute)
            {
                istriggered = false;
                lock_minute = 0;
            }

            if (this.istriggered == false && Config.AlertOnFullHour && now.Minute == 0)
            {
                Notify(nameof(Models.TimeConfig.AlertOnFullHour));
            }
            else if (this.istriggered == false && Config.AlertSpecificMinute != 0 && DateTime.Now.Minute == Config.AlertSpecificMinute)
            {
                Notify(nameof(Models.TimeConfig.AlertSpecificMinute));
            }
            else if (this.istriggered == false && Config.AlertEveryXMinute != 0 && LastNotification.Add(new TimeSpan(0, Config.AlertEveryXMinute, 0)) < now)
            {
                Notify(nameof(Models.TimeConfig.AlertEveryXMinute));
            }
        }

        private void Notify(string caller)
        {
            var now_str = DateTime.Now.ToString("t", System.Globalization.CultureInfo.CurrentCulture);
            caller = Config.showCallerName ? caller + ": " : "";
            Game1.addHUDMessage(new HUDMessage(caller + Helper.Translation.Get("notify_text") + " " + now_str + " " + Helper.Translation.Get("clock_suffix")));
            istriggered =true;
            lock_minute = DateTime.Now.Minute+1;
            SetDateTime();
        }

        private void SetDateTime()
        {
            var now = DateTime.Now;
            LastNotification = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        }

        private void ReloadConfig(string command, string[] args)
        {
            try
            {
                this.Config = Helper.ReadConfig<Models.TimeConfig>();
                SetDateTime();
                this.istriggered = false;
                Monitor.Log(Helper.Translation.Get("config_reload_success"), LogLevel.Debug);
            }
            catch (Exception)
            {
                Monitor.Log(Helper.Translation.Get("config_reload_error"), LogLevel.Debug);
            }
        }
    }
}