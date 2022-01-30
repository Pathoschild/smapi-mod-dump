/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jorgamun/EventMusicVolume
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;
using StardewModdingAPI;

namespace EventMusicVolume
{
    public class EventMusicVolume : Mod
    {
        public float musicOutsideEvent;
        public float musicInsideEvent;

        public bool eventTriggered = false;

        public ModConfig config;

        public override void Entry(IModHelper helper)
        {
            //initialize and validate volume levels
            config = helper.ReadConfig<ModConfig>();
            musicOutsideEvent = validateVolume((float)config.MusicOutsideEvent);
            musicInsideEvent = validateVolume((float)config.MusicInsideEvent);

            helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

        }

        private void GameLoop_GameLaunched(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            //GMCM support
            var configMenu = this.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu == null) return;

            configMenu.Register(mod: this.ModManifest, reset: () => this.config = new ModConfig(), save: updateConfig);

            configMenu.AddSectionTitle(
                mod: this.ModManifest,
                text: () => "Adjust music volume levels"
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Music volume outside events",
                getValue: () => this.config.MusicOutsideEvent,
                setValue: value => this.config.MusicOutsideEvent = value,
                min: 0,
                max: 100,
                interval: 5
            );

            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Music volume during events",
                getValue: () => this.config.MusicInsideEvent,
                setValue: value => this.config.MusicInsideEvent = value,
                min: 0,
                max: 100,
                interval: 5
            );
        }

        private float validateVolume(float volume)
        {
            volume = volume / 100f;
            volume = Math.Min(1, volume);
            volume = Math.Max(0, volume);
            return volume;
        }

        private void updateConfig()
        {
            musicOutsideEvent = validateVolume((float)config.MusicOutsideEvent);
            musicInsideEvent = validateVolume((float)config.MusicInsideEvent);

            this.Helper.WriteConfig<ModConfig>(this.config);

            if (Game1.eventUp)
            {
                if (!eventTriggered)
                {
                    Game1.options.musicVolumeLevel = musicInsideEvent;
                    Game1.musicCategory.SetVolume(musicInsideEvent);
                    Game1.musicPlayerVolume = musicInsideEvent;

                    eventTriggered = true;
                }
            }

            else if (eventTriggered)
            {
                Game1.options.musicVolumeLevel = musicOutsideEvent;
                Game1.musicCategory.SetVolume(musicOutsideEvent);
                Game1.musicPlayerVolume = musicOutsideEvent;

                eventTriggered = false;
            }


        }

        private void GameLoop_DayStarted(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            Game1.options.musicVolumeLevel = musicOutsideEvent;
            Game1.musicCategory.SetVolume(musicOutsideEvent);
            Game1.musicPlayerVolume = musicOutsideEvent;

            eventTriggered = false;
        }

        private void GameLoop_UpdateTicking(object? sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {

           if(Game1.eventUp)
            {
                if(!eventTriggered)
                {
                    Game1.options.musicVolumeLevel = musicInsideEvent;
                    Game1.musicCategory.SetVolume(musicInsideEvent);
                    Game1.musicPlayerVolume = musicInsideEvent;

                    eventTriggered = true;
                }
            }

           else if(eventTriggered)
            {
                Game1.options.musicVolumeLevel = musicOutsideEvent;
                Game1.musicCategory.SetVolume(musicOutsideEvent);
                Game1.musicPlayerVolume = musicOutsideEvent;

                eventTriggered = false;
            }
        }
    }

    public class ModConfig
    {
        public int MusicOutsideEvent { get; set; } = 0;
        public int MusicInsideEvent { get; set; } = 75;

    }
}
