/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace LiveProgressBar
{
    public class ModEntry : Mod
    {
        internal ModConfig? Config { get; private set; } = null;

        internal bool MenuVisible { get; private set; } = true;
        private float LastProgress;

        private ProgressHUD? ProgressHUD { get; set; } = null;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

            helper.Events.Input.ButtonPressed += OnButtonPressed;

            helper.ConsoleCommands.Add("progress", "Sets a fake progress percentage for testing.", SetProgressCmd);
        }


        private void SetProgressCmd(string command, string[] args)
        {
            ProgressHUD?.SetProgress(float.Parse(args[0]));
        }


        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();
            LastProgress = 0f;
            ProgressHUD = new(LastProgress);
            ProgressHUD.SetVisible(MenuVisible);

            Game1.onScreenMenus.Add(ProgressHUD);
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Config is not null && Config.ToggleKey.JustPressed() && ProgressHUD is not null)
            {
                MenuVisible = !MenuVisible;
                ProgressHUD.SetVisible(MenuVisible);
            }
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !MenuVisible || ProgressHUD is null)
                return;

            float latestProgress = Utility.percentGameComplete();
            if (latestProgress == LastProgress)
                return;

            Monitor.Log($"Progress Changed: {latestProgress:P2}.", LogLevel.Debug);
            LastProgress = latestProgress;

            ProgressHUD.SetProgress(latestProgress);
        }
    }
}
