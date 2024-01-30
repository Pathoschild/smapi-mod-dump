/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hikari-BS/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace MuteMusic
{
    /// <summary>The mod entry point.</summary>
    internal class ModEntry : Mod
    {
        private ModConfig config;
        private float oldMusicVolume;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
        }

        private void Input_ButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (config.MuteHotkey.JustPressed())
            {
                bool isMuted = ToggleMuteMusic();
                if (isMuted) Monitor.Log("Music muted.", LogLevel.Debug);
            }
        }

        /// <summary>
        /// return true if music becomes muted
        /// </summary>
        /// <returns></returns>
        public bool ToggleMuteMusic()
        {
            if (Game1.soundBank != null)
            {
                if (Game1.options.musicVolumeLevel != 0f)
                {
                    DisableMusic();
                    return true;
                }
                EnableMusic();
            }
            return false;
        }

        private void DisableMusic()
        {
            if (Game1.soundBank != null)
            {
                oldMusicVolume = Game1.options.musicVolumeLevel;

                Game1.options.musicVolumeLevel = 0f;
                Game1.musicCategory.SetVolume(0f);
                Game1.musicPlayerVolume = 0f;
                // Game1.options.ambientVolumeLevel = 0f;
                // Game1.ambientCategory.SetVolume(0f);
                // Game1.ambientPlayerVolume = 0f;
            }
        }

        private void EnableMusic()
        {
            if (Game1.soundBank != null)
            {
                Game1.options.musicVolumeLevel = oldMusicVolume;
                Game1.musicCategory.SetVolume(oldMusicVolume);
                Game1.musicPlayerVolume = oldMusicVolume;
                // Game1.options.ambientVolumeLevel = 0.75f;
                // Game1.ambientCategory.SetVolume(0.75f);
                // Game1.ambientPlayerVolume = 0.75f;
            }
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            InitializeConfigMenu();
        }

        private void InitializeConfigMenu()
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => config = new ModConfig(),
                save: () => Helper.WriteConfig(config));

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Mute Hotkey",
                getValue: () => config.MuteHotkey,
                setValue: value => config.MuteHotkey = value);
        }
    }

    internal class ModConfig
    {
        public KeybindList MuteHotkey { get; set; } = KeybindList.Parse("M");
    }
}
