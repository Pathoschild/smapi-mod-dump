/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using GenericModConfigMenu;
using StardewValley.Monsters;

namespace MoreSettings
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig? Config;

        private bool muted = false;

        private float musicVol;
        private float ambientVol;
        private float footstepVol;
        private float soundVol;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if(e.NewMenu is GameMenu menu)
            {
                OptionsPage page = (OptionsPage)menu.pages[GameMenu.optionsTab];
                
                // Adjust UI depending on if muted
                string buttonLabel = "Mute";
                if(muted)
                {
                    buttonLabel = "Unmute";
                    foreach(var option in page.options)
                    {
                        if(option.label.Contains("Volume")) option.greyedOut = true;
                    }
                }
                
                // Add mute/unmute button to the options submenu
                if(Config != null) page.options.Insert(Config.MuteButtonIndex, new OptionsButton(buttonLabel, () => ToggleMute()));
                else page.options.Insert(16, new OptionsButton(buttonLabel, () => ToggleMute()));
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Get MCM API(if installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            // Exit if no config
            if(configMenu == null) return;
            if(Config == null) return;

            // Register mod for MCM
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // Add UI for MCM
            configMenu.AddKeybind(
                mod: ModManifest,
                getValue: () => Config.MuteKey,
                setValue: value => Config.MuteKey = value,
                name: () => "Toggle Mute Key",
                tooltip: () => "The key you press to toggle between muted and unmuted."
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.MuteButtonIndex,
                setValue: value => Config.MuteButtonIndex = (int)value,
                name: () => "Mute Button Index",
                tooltip: () => "Determines where the mute button is placed in the options menu. The default places it just below the audio header.",
                min: 0
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                getValue: () => Config.FullscreenKey,
                setValue: value => Config.FullscreenKey = value,
                name: () => "Toggle Fullscreen Key",
                tooltip: () => "The key you press to toggle between fullscreen and windowed."
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => Config.UseWindowedBorderless,
                setValue: value => Config.UseWindowedBorderless = value,
                name: () => "Use Windowed Borderless",
                tooltip: () => "Use windowed borderless mode instead of fullscreen mode. This will make the delay to switch less, and doesn't look different, but windowed modes are less performant."
            );
        }

        private void ToggleMute()
        {
            if(!muted)
            {
                // Store existing volume levels
                musicVol = Game1.options.musicVolumeLevel;
                soundVol = Game1.options.soundVolumeLevel;
                ambientVol = Game1.options.ambientVolumeLevel;
                footstepVol = Game1.options.footstepVolumeLevel;

                // Set volume levels to zero
                Game1.options.changeSliderOption(1, 0);
                Game1.options.changeSliderOption(2, 0);
                Game1.options.changeSliderOption(20, 0);
                Game1.options.changeSliderOption(21, 0);
            }
            else
            {
                // Set to stored volume levels (this function uses an int * 100 to represent float)
                Game1.options.changeSliderOption(1, (int)(musicVol * 100));
                Game1.options.changeSliderOption(2, (int)(soundVol * 100));
                Game1.options.changeSliderOption(20, (int)(ambientVol * 100));
                Game1.options.changeSliderOption(21, (int)(footstepVol * 100));
            }

            // Refresh GameMenu if it's currently open
            if(Game1.activeClickableMenu is GameMenu gameMenu)
            {
                OptionsPage optionsPage = (OptionsPage)gameMenu.pages[GameMenu.optionsTab];
                RefreshGameMenu(gameMenu.currentTab, optionsPage.currentItemIndex, false);
            }

            muted = !muted;
        }

        private void RefreshGameMenu(int startingTab, int startingIndex, bool playOpeningSound)
        {
            Game1.activeClickableMenu = null;
            Game1.activeClickableMenu = new GameMenu(startingTab, startingIndex, playOpeningSound);
        }

        public void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if(Config == null) return;

            if(e.Button == Config.MuteKey) ToggleMute();

            if(e.Button == Config.FullscreenKey) ToggleFullscreen();
        }

        public void ToggleFullscreen()
        {
            if(Game1.options.isCurrentlyFullscreen() || Game1.options.isCurrentlyWindowedBorderless()) Game1.options.setWindowedOption("Windowed");
            else
            {
                if(Config != null && !Config.UseWindowedBorderless) Game1.options.setWindowedOption("Fullscreen");
                else Game1.options.setWindowedOption("Windowed Borderless");
            }
        }

        public void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            if(muted)
            {
                // Store existing volume levels
                musicVol = Game1.options.musicVolumeLevel;
                soundVol = Game1.options.soundVolumeLevel;
                ambientVol = Game1.options.ambientVolumeLevel;
                footstepVol = Game1.options.footstepVolumeLevel;

                // Set volume levels to zero
                Game1.options.changeSliderOption(1, 0);
                Game1.options.changeSliderOption(2, 0);
                Game1.options.changeSliderOption(20, 0);
                Game1.options.changeSliderOption(21, 0);
            }
        }
    }
}
