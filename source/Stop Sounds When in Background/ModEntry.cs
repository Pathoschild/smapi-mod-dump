/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mustafa-git/StopSoundsWhenAltTabbed
**
*************************************************/


using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StopSoundsWhenAltTabbed
{
    public class ModConfig
    {
        // Config option.
        public bool OnlyWhenPaused { get; set; } = true;
    }
    public class ModEntry : Mod
    {
        // Store current settings to restore later.
        private float soundVol;
        private float ambientVol;
        private float footstepVol;
        private float musicVol;
        private bool volumeSaved = false;
        private ModConfig Config = null!;
        private ModConfig LoadConfig()
        {
            var Config = this.Helper.ReadConfig<ModConfig>();
            return Config;
        }
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
            this.Config = this.LoadConfig();
        }
        void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsGameLaunched || Game1.game1 is null){
                return;
            }
            if (Game1.currentSong is null) {
                modLoopMinigame();
            } else {
                modLoop();
            }
        }

        void modLoop(){
            if (!Game1.game1.IsActive)
            {
                if (Config.OnlyWhenPaused)
                {
                    if (!volumeSaved && Game1.options.pauseWhenOutOfFocus == true)
                    {
                        soundVol = Game1.options.soundVolumeLevel;
                        ambientVol = Game1.options.ambientVolumeLevel;
                        footstepVol = Game1.options.footstepVolumeLevel;
                        musicVol = Game1.options.musicVolumeLevel;
                        Game1.currentSong.Pause();
                        Game1.musicCategory.SetVolume(0.0f);
			            Game1.musicPlayerVolume = 0.0f;
                        Game1.soundCategory.SetVolume(0.0f);
                        Game1.ambientCategory.SetVolume(0.0f);
                        Game1.ambientPlayerVolume = 0.0f;
                        Game1.footstepCategory.SetVolume(0.0f);
                        volumeSaved = true;
                    }
                }
                else
                {
                    if (!volumeSaved)
                    {
                        soundVol = Game1.options.soundVolumeLevel;
                        ambientVol = Game1.options.ambientVolumeLevel;
                        footstepVol = Game1.options.footstepVolumeLevel;
                        musicVol = Game1.options.musicVolumeLevel;
                        Game1.currentSong.Pause();
                        Game1.musicCategory.SetVolume(0.0f);
			            Game1.musicPlayerVolume = 0.0f;
                        Game1.soundCategory.SetVolume(0.0f);
                        Game1.ambientCategory.SetVolume(0.0f);
                        Game1.ambientPlayerVolume = 0.0f;
                        Game1.footstepCategory.SetVolume(0.0f);
                        volumeSaved = true;
                    }
                }
            }
            else
            {
                if (volumeSaved)
                {
                    Game1.currentSong.Resume();
                    Game1.soundCategory.SetVolume(soundVol);
                    Game1.musicCategory.SetVolume(musicVol);
                    Game1.musicPlayerVolume = musicVol;
                    Game1.ambientCategory.SetVolume(ambientVol);
                    Game1.ambientPlayerVolume = ambientVol;
                    Game1.footstepCategory.SetVolume(footstepVol);
                    volumeSaved = false;
                }
            }
        }

        void modLoopMinigame(){
            if (!Game1.game1.IsActive)
            {
                if (Config.OnlyWhenPaused)
                {
                    if (!volumeSaved && Game1.options.pauseWhenOutOfFocus == true)
                    {
                        soundVol = Game1.options.soundVolumeLevel;
                        ambientVol = Game1.options.ambientVolumeLevel;
                        footstepVol = Game1.options.footstepVolumeLevel;
                        musicVol = Game1.options.musicVolumeLevel;
                        Game1.musicCategory.SetVolume(0.0f);
			            Game1.musicPlayerVolume = 0.0f;
                        Game1.soundCategory.SetVolume(0.0f);
                        Game1.ambientCategory.SetVolume(0.0f);
                        Game1.ambientPlayerVolume = 0.0f;
                        Game1.footstepCategory.SetVolume(0.0f);
                        volumeSaved = true;
                    }
                }
                else
                {
                    if (!volumeSaved)
                    {
                        soundVol = Game1.options.soundVolumeLevel;
                        ambientVol = Game1.options.ambientVolumeLevel;
                        footstepVol = Game1.options.footstepVolumeLevel;
                        musicVol = Game1.options.musicVolumeLevel;
                        Game1.musicCategory.SetVolume(0.0f);
			            Game1.musicPlayerVolume = 0.0f;
                        Game1.soundCategory.SetVolume(0.0f);
                        Game1.ambientCategory.SetVolume(0.0f);
                        Game1.ambientPlayerVolume = 0.0f;
                        Game1.footstepCategory.SetVolume(0.0f);
                        volumeSaved = true;
                    }
                }
            }
            else
            {
                if (volumeSaved)
                {
                    Game1.soundCategory.SetVolume(soundVol);
                    Game1.musicCategory.SetVolume(musicVol);
                    Game1.musicPlayerVolume = musicVol;
                    Game1.ambientCategory.SetVolume(ambientVol);
                    Game1.ambientPlayerVolume = ambientVol;
                    Game1.footstepCategory.SetVolume(footstepVol);
                    volumeSaved = false;
                }
            }
        }

    }
}
