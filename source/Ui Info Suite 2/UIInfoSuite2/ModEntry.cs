/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using UIInfoSuite.AdditionalFeatures;
using UIInfoSuite.Compatibility;
using UIInfoSuite.Infrastructure;
using UIInfoSuite.Options;

namespace UIInfoSuite
{
    public class ModEntry : Mod
    {

        #region Properties
        public static IMonitor MonitorObject { get; private set; }

        private static SkipIntro _skipIntro; // Needed so GC won't throw away object with subscriptions
        private static ModConfig _modConfig;

        private ModOptions _modOptions;
        private ModOptionsPageHandler _modOptionsPageHandler;

        private static EventHandler<ButtonsChangedEventArgs> _calendarAndQuestKeyBindingsHandler;
        #endregion


        #region Entry
        public override void Entry(IModHelper helper)
        {
            MonitorObject = Monitor;

            _skipIntro = new SkipIntro(helper.Events);
            _modConfig = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Display.Rendering += IconHandler.Handler.Reset;
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

            _modOptions = Helper.Data.ReadJsonFile<ModOptions>($"data/{Constants.SaveFolderName}.json")
                ?? Helper.Data.ReadJsonFile<ModOptions>($"data/{_modConfig.ApplyDefaultSettingsFromThisSave}.json")
                ?? new ModOptions();

            _modOptionsPageHandler = new ModOptionsPageHandler(Helper, _modOptions, _modConfig.ShowOptionsTabInMenu);
        }

        private void OnSaved(object sender, EventArgs e)
        {
            // Only save for the main player.
            if (Context.ScreenId != 0) return;

            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", _modOptions);
        }

        public static void RegisterCalendarAndQuestKeyBindings(IModHelper helper, bool subscribe)
        {
            if (_calendarAndQuestKeyBindingsHandler == null)
                _calendarAndQuestKeyBindingsHandler = (object sender, ButtonsChangedEventArgs e) => HandleCalendarAndQuestKeyBindings(helper);

            helper.Events.Input.ButtonsChanged -= _calendarAndQuestKeyBindingsHandler;

            if (subscribe)
            {
                helper.Events.Input.ButtonsChanged += _calendarAndQuestKeyBindingsHandler;
            }
        }

        private static void HandleCalendarAndQuestKeyBindings(IModHelper helper)
        {
            if (_modConfig != null)
            {
                if (Context.IsPlayerFree && _modConfig.OpenCalendarKeybind.JustPressed())
                {
                    helper.Input.SuppressActiveKeybinds(_modConfig.OpenCalendarKeybind);
                    Game1.activeClickableMenu = new Billboard(false);
                }
                else if (Context.IsPlayerFree && _modConfig.OpenQuestBoardKeybind.JustPressed())
                {
                    helper.Input.SuppressActiveKeybinds(_modConfig.OpenQuestBoardKeybind);
                    Game1.RefreshQuestOfTheDay();
                    Game1.activeClickableMenu = new Billboard(true);
                }
            }
        }
        #endregion

        #region Generic mod config menu
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => _modConfig = new ModConfig(),
                save: () => Helper.WriteConfig(_modConfig)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Show options in in-game menu",
                tooltip: () => "Enables an extra tab in the in-game menu where you can configure every options for this mod.",
                getValue: () => _modConfig.ShowOptionsTabInMenu,
                setValue: value => _modConfig.ShowOptionsTabInMenu = value
            );
            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Apply default settings from this save",
                tooltip: () => "New characters will inherit the settings for the mod from this save file.",
                getValue: () => _modConfig.ApplyDefaultSettingsFromThisSave,
                setValue: value => _modConfig.ApplyDefaultSettingsFromThisSave = value
            );
            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Open calendar keybind",
                tooltip: () => "Opens the calendar tab.",
                getValue: () => _modConfig.OpenCalendarKeybind,
                setValue: value => _modConfig.OpenCalendarKeybind = value
            );
            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Open quest board keybind",
                tooltip: () => "Opens the quest board.",
                getValue: () => _modConfig.OpenQuestBoardKeybind,
                setValue: value => _modConfig.OpenQuestBoardKeybind = value
            );
        }
        #endregion
    }
}
