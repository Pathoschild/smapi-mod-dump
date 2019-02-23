using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace AudioDevices
{
    /// <summary>The entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        private static volatile bool doTheThing = true;
        private static volatile bool theThingIsInited = false;

        private static volatile List<RendererDetail> rendererDetails = new List<RendererDetail>();
        private static volatile object rendererDetailsLock = new object();

        public static ModEntry mod;

        public AudioDeviceSettings Settings { get; set; } = new AudioDeviceSettings();

        public void Log(string message, LogLevel level = LogLevel.Debug)
        {
            Monitor.Log(message, level);
        }

        public static IReflectionHelper Reflection
        {
            get
            {
                return mod.Helper.Reflection;
            }
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.mod = this;
            Settings = helper.ReadConfig<AudioDeviceSettings>() ?? new AudioDeviceSettings();
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }

        private void SwitchAudioDevice(RendererDetail rendererDetail)
        {
            AudioEngine newAudioEngine = new AudioEngine(Path.Combine(Game1.game1.Content.RootDirectory, "XACT", "FarmerSounds.xgs"), TimeSpan.Zero, rendererDetail.RendererId);
            WaveBank newWaveBank = new WaveBank(newAudioEngine, Path.Combine(Game1.game1.Content.RootDirectory, "XACT", "Wave Bank.xwb"));
            SoundBank newSoundBank = new SoundBank(newAudioEngine, Path.Combine(Game1.game1.Content.RootDirectory, "XACT", "Sound Bank.xsb"));
            newAudioEngine.Update();

            FixCues(newSoundBank);

            Game1.audioEngine.Dispose();
            Game1.waveBank.Dispose();
            Game1.soundBank.Dispose();

            Game1.audioEngine = newAudioEngine;
            Game1.waveBank = newWaveBank;
            Game1.soundBank = new SoundBankWrapper(newSoundBank);

            Game1.musicCategory = Game1.audioEngine.GetCategory("Music");
            Game1.soundCategory = Game1.audioEngine.GetCategory("Sound");
            Game1.ambientCategory = Game1.audioEngine.GetCategory("Ambient");
            Game1.footstepCategory = Game1.audioEngine.GetCategory("Footsteps");

            Game1.musicCategory.SetVolume(Game1.options.musicVolumeLevel);
            Game1.soundCategory.SetVolume(Game1.options.soundVolumeLevel);
            Game1.ambientCategory.SetVolume(Game1.options.ambientVolumeLevel);
            Game1.footstepCategory.SetVolume(Game1.options.footstepVolumeLevel);

            AmbientLocationSounds.InitShared();
            if (Game1.currentLocation != null)
                Game1.locationCues.Update(Game1.currentLocation);

            string prevAudioDevice = Settings.SelectedAudioDevice;
            Settings.SelectedAudioDevice = rendererDetail.FriendlyName;
            StoreSettings();

            curAudioOptionsDropDown?.UpdateDeviceList();

            Log("Switched Audio Device: " + Settings.SelectedAudioDevice + " (previous: " + prevAudioDevice + ")");
        }

        private Cue FixCue(SoundBank newSoundBank, Cue cue)
        {
            Cue fixedCue = null;
            if (cue != null)
            {
                fixedCue = newSoundBank.GetCue(cue.Name);
                if (cue.IsPaused)
                {
                    fixedCue.Play();
                    fixedCue.Pause();
                }
                if (cue.IsPlaying)
                {
                    fixedCue.Play();
                }
            }
            return fixedCue;
        }

        private void FixCues(SoundBank newSoundBank)
        {
            typeof(AmbientLocationSounds).GetField("babblingBrook", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);
            typeof(AmbientLocationSounds).GetField("cracklingFire", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);
            typeof(AmbientLocationSounds).GetField("engine", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);
            typeof(AmbientLocationSounds).GetField("cricket", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);

            Game1.currentSong = FixCue(newSoundBank, Game1.currentSong);
            Game1.chargeUpSound = FixCue(newSoundBank, Game1.chargeUpSound);
            Game1.wind = FixCue(newSoundBank, Game1.wind);

            Intro.roadNoise = FixCue(newSoundBank, Intro.roadNoise);
            Fly.buzz = FixCue(newSoundBank, Fly.buzz);
            MineShaft.bugLevelLoop = FixCue(newSoundBank, MineShaft.bugLevelLoop);
            Railroad.trainLoop = FixCue(newSoundBank, Railroad.trainLoop);
            BobberBar.reelSound = FixCue(newSoundBank, BobberBar.reelSound);
            BobberBar.unReelSound = FixCue(newSoundBank, BobberBar.unReelSound);
            FishingRod.reelSound = FixCue(newSoundBank, FishingRod.reelSound);
            FishingRod.chargeSound = FixCue(newSoundBank, FishingRod.chargeSound);
            Grass.grassSound = FixCue(newSoundBank, Grass.grassSound);

            Game1.locationCues.StopAll();

            // AbigailGame.overworldSong = FixCue(AbigailGame.overworldSong);
            // AbigailGame.outlawSong = FixCue(AbigailGame.outlawSong);
        }

        public static void DoTheThing()
        {
            while (doTheThing)
            {
                Thread.Sleep(Math.Max(10, mod.Settings.CheckAudioDevicesInterval));
                AudioEngine audioEngine = new AudioEngine(Path.Combine(Game1.game1.Content.RootDirectory, "XACT", "FarmerSounds.xgs"));
                lock (rendererDetailsLock)
                {
                    rendererDetails.Clear();
                    rendererDetails.AddRange(audioEngine.RendererDetails);
                }
                audioEngine.Dispose();
            }
        }

        private void InitTheThing()
        {
            if (theThingIsInited)
                return;

            theThingIsInited = true;

            rendererDetails.Clear();
            rendererDetails.AddRange(Game1.audioEngine.RendererDetails);

            if (Settings.SwitchToDefaultDeviceMode != AudioDeviceSettings.AudioSwitchToDefaultDeviceMode.Always
                && Settings.SelectedAudioDevice != ""
                && Settings.SelectedAudioDevice != rendererDetails[0].FriendlyName
                && rendererDetails.Exists(r => Settings.SelectedAudioDevice == r.FriendlyName))
            {
                SwitchAudioDevice(rendererDetails.Find(r => Settings.SelectedAudioDevice == r.FriendlyName));
                rendererDetails.Clear();
                rendererDetails.AddRange(Game1.audioEngine.RendererDetails);
            }
            else
            {
                Settings.SelectedAudioDevice = Game1.audioEngine.RendererDetails[0].FriendlyName;
                StoreSettings();
            }

            new Thread(new ThreadStart(DoTheThing))
            {
                Priority = ThreadPriority.Lowest
            }.Start();
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is GameMenu gameMenu)
            {
                InjectAudioOptionsIntoGameMenu(gameMenu);
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            InitTheThing();

            lock (rendererDetailsLock)
            {
                if ((Settings.SwitchToDefaultDeviceMode == AudioDeviceSettings.AudioSwitchToDefaultDeviceMode.Always
                    && rendererDetails[0].FriendlyName != Settings.SelectedAudioDevice)
                    || (Settings.SwitchToDefaultDeviceMode == AudioDeviceSettings.AudioSwitchToDefaultDeviceMode.WhenCurrentDeviceLost
                    && !rendererDetails.Exists(r => r.FriendlyName == Settings.SelectedAudioDevice)))
                {
                    SwitchAudioDevice(rendererDetails[0]);
                }
            }
        }

        public void StoreSettings()
        {
            mod.Helper.WriteConfig<AudioDeviceSettings>(Settings);
        }

        private void InjectAudioOptionsIntoGameMenu(GameMenu gameMenu)
        {
            List<IClickableMenu> pages = Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();
            InjectAudioOptionsIntoOptionsPage(pages.Find(i => i is OptionsPage) as OptionsPage);
        }

        private List<OptionsElement> curOptionElements = new List<OptionsElement>();
        private AudioOptionsDropDown curAudioOptionsDropDown = null;

        private void InjectAudioOptionsIntoOptionsPage(OptionsPage optionsPage)
        {
            List<OptionsElement> options = Helper.Reflection.GetField<List<OptionsElement>>(optionsPage, "options").GetValue();

            // First find the audio settings
            string soundLabel = Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsPage.cs.11241");
            int soundLabelIndex = options.FindIndex(el => el.label == soundLabel);

            options.InsertRange(soundLabelIndex + 1, curOptionElements = MakeAudioOptions());
        }

        private List<OptionsElement> MakeAudioOptions()
        {
            return new List<OptionsElement>() {
                new OptionsElement("Auto-switch to default device..."),
                new AudioOptionsCheckBox("Always", AudioDeviceSettings.AudioSwitchToDefaultDeviceMode.Always),
                new AudioOptionsCheckBox("When current device is lost", AudioDeviceSettings.AudioSwitchToDefaultDeviceMode.WhenCurrentDeviceLost),
                new AudioOptionsCheckBox("Never", AudioDeviceSettings.AudioSwitchToDefaultDeviceMode.Never),
                (curAudioOptionsDropDown = new AudioOptionsDropDown() {
                    greyedOut = Settings.SwitchToDefaultDeviceMode == AudioDeviceSettings.AudioSwitchToDefaultDeviceMode.Always
                }),
            };
        }

        public static List<string> GetAudioDeviceNames()
        {
            lock (rendererDetailsLock)
            {
                return rendererDetails.ConvertAll(r => r.FriendlyName);
            }
        }


        public static void UpdateAudioSetting(AudioDeviceSettings.AudioSwitchToDefaultDeviceMode which)
        {
            foreach (var e in mod.curOptionElements)
            {
                if (e is AudioOptionsCheckBox audioOptionsCheckBox)
                {
                    if (audioOptionsCheckBox.which != which)
                        audioOptionsCheckBox.isChecked = false;
                }
                else if (e is AudioOptionsDropDown optionsDropDown)
                {
                    optionsDropDown.greyedOut = which == AudioDeviceSettings.AudioSwitchToDefaultDeviceMode.Always;
                }
            }
            mod.Settings.SwitchToDefaultDeviceMode = which;
            mod.StoreSettings();
        }

        public static void UpdateAudioDevice(string name)
        {
            if (mod.Settings.SelectedAudioDevice != name)
                mod.SwitchAudioDevice(rendererDetails.Find(r => name == r.FriendlyName));
        }
    }
}
