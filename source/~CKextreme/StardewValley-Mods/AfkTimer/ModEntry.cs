using System;
using StardewValley;
using StardewModdingAPI;
using System.Collections.Generic;

namespace AfkTimer
{
    class ModEntry : Mod
    {
        private static Models.TimeConfig Config;
        private static DateTime LastInteraction { get; set; }
        private static bool MessageIsShown = false;
        private static readonly object _testlock = new object();
        private static HashSet<SButton> _list = new HashSet<SButton>();

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Models.TimeConfig>();
            if (Config.IdleTime > 0 && Config.IdleTime <= 15) { Config.IdleTime = 15; }
            if (!Config.IsActive || Config.IdleTime <= 0)
            {
                Monitor.Log("Mod is disabled caused by the config-file!", LogLevel.Warn);
                return;
            }

            helper.ConsoleCommands.Add("afktimer_reloadconfig", "Reloads changed config file without restarting game", ReloadConfig);
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            // spin the order in unsubscription
            Helper.Events.Input.ButtonPressed -= Input_ButtonPressed;
            Helper.Events.Input.ButtonReleased -= Input_ButtonReleased;
            Helper.Events.GameLoop.OneSecondUpdateTicked -= GameLoop_OneSecondUpdateTicked;
            Helper.Events.GameLoop.ReturnedToTitle -= GameLoop_ReturnedToTitle;
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            UpdateInteraction(false, null);
            Helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void UpdateInteraction(bool isclicked, SButton? tmp_btn = null)
        {
            lock (_testlock)
            {
                if (isclicked && tmp_btn != null)
                {
                    _list.Add(tmp_btn.Value);
                }
                else if (!isclicked && tmp_btn != null)
                {
                    _list.Remove(tmp_btn.Value);
                }
                LastInteraction = DateTime.Now;
                MessageIsShown = false;
            }
        }

        private void ReloadConfig(string command, string[] args)
        {
            try
            {
                Config = Helper.ReadConfig<Models.TimeConfig>();
                Monitor.Log(Helper.Translation.Get("config_reload_success"), LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log(Helper.Translation.Get("config_reload_error"), LogLevel.Warn);
                Monitor.Log("Exception-Message: " + ex.Message, LogLevel.Warn);
            }
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.CanPlayerMove)
            {
                UpdateInteraction(false);
                return;
            }

            lock (_testlock)
            {
                if (_list.Count == 0 && !MessageIsShown && LastInteraction.AddSeconds(Config.IdleTime) <= DateTime.Now)
                {
                    Game1.pauseThenMessage(250, Helper.Translation.Get("idle_text"), false);
                    MessageIsShown = true;
                }
            }
        }

        private void Input_ButtonReleased(object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e)
        {
            UpdateInteraction(false, e.Button);
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            UpdateInteraction(true, e.Button);
        }
    }
}
