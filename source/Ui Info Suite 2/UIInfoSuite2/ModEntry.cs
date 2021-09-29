/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UIInfoSuite.AdditionalFeatures;
using UIInfoSuite.Infrastructure;
using UIInfoSuite.Options;

namespace UIInfoSuite
{
    public class ModEntry : Mod
    {

        #region Properties
        public static IMonitor MonitorObject { get; private set; }

        private SkipIntro _skipIntro; // Needed so GC won't throw away object with subscriptions
        private ModConfig _options;

        private ModOptionsPageHandler _modOptionsPageHandler;
        #endregion


        #region Entry
        public override void Entry(IModHelper helper)
        {
            MonitorObject = Monitor;
            _skipIntro = new SkipIntro(helper.Events);

            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += OnSaved;

            helper.Events.Display.Rendering += IconHandler.Handler.Reset;
            helper.Events.Input.ButtonsChanged += HandleKeyBindings;

            // for initializing the config.json
            _options = this.Helper.ReadConfig<ModConfig>();
        }
        #endregion


        #region Event subscriptions
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            // Unload if the main player quits.
            if (Context.ScreenId != 0) return;
            
            _modOptionsPageHandler?.Dispose();
            _modOptionsPageHandler = null;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // Only load once for split screen.
            if (Context.ScreenId != 0) return;

            ModOptions options = this.Helper.Data.ReadJsonFile<ModOptions>($"data/{Constants.SaveFolderName}.json") ?? _options;

            _modOptionsPageHandler = new ModOptionsPageHandler(Helper, options, _options.ShowOptionsTabInMenu);
        }

        private void OnSaved(object sender, EventArgs e)
        {
            // Only save for the main player.
            if (Context.ScreenId != 0) return;

            // Only save if the options differ from the default config or there is already a file for that character
            var defaultOptions = this.Helper.ReadConfig<ModOptions>();
            var savedUserOptions = this.Helper.Data.ReadJsonFile<ModOptions>($"data/{Constants.SaveFolderName}.json");
            if (!_options.Equals(defaultOptions) || savedUserOptions != null) {   
                this.Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", _options);
            }

        }

        private void HandleKeyBindings(object sender, ButtonsChangedEventArgs e)
        {
            if (_options != null)
            {
                if(Context.IsPlayerFree && _options.OpenCalendarKeybind.JustPressed())
                {
                    Helper.Input.SuppressActiveKeybinds(_options.OpenCalendarKeybind);
                    Game1.activeClickableMenu = new Billboard(false);
                }
                else if (Context.IsPlayerFree && _options.OpenQuestBoardKeybind.JustPressed())
                {
                    Helper.Input.SuppressActiveKeybinds(_options.OpenQuestBoardKeybind);
                    Game1.RefreshQuestOfTheDay();
                    Game1.activeClickableMenu = new Billboard(true);
                }
            }
        }
        #endregion
    }
}
