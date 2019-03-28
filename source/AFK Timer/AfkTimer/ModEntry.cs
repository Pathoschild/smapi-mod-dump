using System;
using StardewValley;
using StardewModdingAPI;
using System.Timers;

namespace AfkTimer
{
    class ModEntry : Mod
    {
        private Models.TimeConfig Config;
        private static DateTime LastInteraction { get; set; }
        private static bool MessageIsShown = false;
        private static Timer loopTimer;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Models.TimeConfig>();
            if(!Config.IsActive) { return; }
            helper.ConsoleCommands.Add("afktimer_reloadconfig", "Reloads changed config file without restarting game", ReloadConfig);
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Input.ButtonReleased += Input_ButtonReleased;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
        }

        private void LoopTimerEvent(object sender, ElapsedEventArgs e)
        {
            LastInteraction = DateTime.Now;
            MessageIsShown = false;
        }

        private void Input_ButtonReleased(object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }

            loopTimer.Enabled = false;
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }

            loopTimer.Enabled = true;
            LastInteraction = DateTime.Now;
            MessageIsShown = false;
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            LastInteraction = DateTime.Now;
            //loop timer
            loopTimer = new Timer
            {
                Interval = 500,
                Enabled = false
            };
            loopTimer.Elapsed += LoopTimerEvent;
            loopTimer.AutoReset = true;
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }
            if(Config.IdleTime <= 0) { return; }

            if (!MessageIsShown && LastInteraction.AddSeconds(Config.IdleTime) <= DateTime.Now)
            {
                Game1.pauseThenMessage(250, Helper.Translation.Get("idle_text"), false);
                MessageIsShown = true;
            }
        }

        private void ReloadConfig(string command, string[] args)
        {
            try
            {
                this.Config = Helper.ReadConfig<Models.TimeConfig>();
                Monitor.Log(Helper.Translation.Get("config_reload_success"), LogLevel.Debug);
            }
            catch (Exception)
            {
                Monitor.Log(Helper.Translation.Get("config_reload_error"), LogLevel.Debug);
            }
        }
    }
}
