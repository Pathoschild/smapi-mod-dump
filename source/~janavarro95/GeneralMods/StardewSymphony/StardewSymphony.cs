using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework.Audio;
using Omegasis.StardewSymphony.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Omegasis.StardewSymphony
{
    /*
    TODO:
    0. Add in event handling so that I don't mute a heart event or wedding music.
    6. add in Stardew songs again to music selection
    7. add in more tracks.
    11. Tutorial for adding more music into the game?
    15. add in blank templates for users to make their own wave/sound banks
    */
    /// <summary>The mod entry point.</summary>
    public class StardewSymphony : Mod
    {
        /*********
        ** Properties
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;

        /// <summary>All of the music/soundbanks and their locations.</summary>
        private IList<MusicManager> MasterList = new List<MusicManager>();

        /// <summary>All of the cue names that I ever add in.</summary>
        private IDictionary<string, MusicManager> SongWaveReference;

        /// <summary>The game locations.</summary>
        private IList<GameLocation> GameLocations;

        /// <summary>The number generator for randomisation.</summary>
        private Random Random;

        /// <summary>The game's original soundbank.</summary>
        private ISoundBank DefaultSoundbank;

        /// <summary>The game's original wavebank.</summary>
        private WaveBank DefaultWavebank;

        private MusicHexProcessor HexProcessor;

        /****
        ** Context
        ****/
        /// <summary>Whether no music pack was loaded for the current location.</summary>
        private bool HasNoMusic;

        /// <summary>The song that's currently playing.</summary>
        private Cue CurrentSong;

        /// <summary>The current sound info.</summary>
        private MusicManager CurrentSoundInfo;

        /// <summary>A timer used to create random pauses between songs.</summary>
        private Timer SongDelayTimer = new Timer();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            this.HexProcessor = new MusicHexProcessor(this.MasterList, this.Reset);

            SaveEvents.AfterLoad += this.SaveEvents_AfterLoad;
            TimeEvents.AfterDayStarted += this.TimeEvents_AfterDayStarted;
            GameEvents.UpdateTick += this.GameEvents_UpdateTick;
            LocationEvents.CurrentLocationChanged += this.LocationEvents_CurrentLocationChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the game updates (roughly 60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || !this.MasterList.Any())
                return; //basically if absolutly no music is loaded into the game for locations/festivals/seasons, don't override the game's default music player.

            if (this.CurrentSong == null)
            {
                this.HasNoMusic = true;
                return; //if there wasn't any music at loaded at all for the area, just play the default stardew soundtrack.
            }
            if (this.HasNoMusic && !this.CurrentSong.IsPlaying)
                this.CurrentSong = null; //if there was no music loaded for the area and the last song has finished playing, default to the Stardew Soundtrack.

            if (this.CurrentSong != null)
            {
                this.HasNoMusic = false;
                if (!this.CurrentSong.IsPlaying && !this.SongDelayTimer.Enabled)
                    this.StartMusicDelay();
            }

            if (Game1.isFestival())
                return; // replace with festival
            if (Game1.eventUp)
                return; // replace with event music
            if (Game1.isRaining && !this.Config.SilentRain)
                return; // play the rain ambience soundtrack

            Game1.currentSong.Stop(AudioStopOptions.Immediate); //stop the normal songs from playing over the new songs
            Game1.nextMusicTrack = "";  //same as above line
        }

        /// <summary>The method invoked after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            this.StopSound(); //if my music player is called and I forget to clean up sound before hand, kill the old sound.
            this.SelectMusic();
        }

        /// <summary>The method invoked after the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            // init context
            this.Random = new Random();
            this.MasterList = new List<MusicManager>();
            this.SongWaveReference = new Dictionary<string, MusicManager>();
            this.GameLocations = Game1.locations;
            this.HasNoMusic = true;

            // keep a copy of the original banks
            this.DefaultSoundbank = Game1.soundBank;
            this.DefaultWavebank = Game1.waveBank;

            // load music packs
            {
                string musicPacksPath = Directory.CreateDirectory(Path.Combine(Helper.DirectoryPath, "Music_Packs")).FullName;
                var musicPacks = new Dictionary<string, string>();
                ProcessDirectory(musicPacksPath, musicPacks);
                this.SongDelayTimer.Enabled = false;
                foreach (var pack in musicPacks)
                    this.LoadMusicInfo(pack.Key, pack.Value);
            }

            // init sound
            this.HexProcessor.ProcessHex();  //Get all of the songs from the music packs.
            this.SelectMusic();
        }

        /// <summary>The method invoked after the player warps to a new area.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void LocationEvents_CurrentLocationChanged(object sender, EventArgsCurrentLocationChanged e)
        {
            if (!Context.IsWorldReady)
                return;

           
            this.SelectMusic();
        }

        /// <summary>Create a random delay and then choose the next song.</summary>
        private void StartMusicDelay()
        {
            // reset timer
            this.SongDelayTimer.Dispose();
            this.SongDelayTimer = new Timer(this.Random.Next(this.Config.MinSongDelay, this.Config.MaxSongDelay));

            // start timer
            this.SongDelayTimer.Elapsed += (sender, args) =>
            {
                this.SelectMusic();
                this.SongDelayTimer.Enabled = false;
            };
            this.SongDelayTimer.Start();
        }

        /// <summary>Reads cue names from a text file and adds them to a specific list. Morphs with specific conditional name.</summary>
        /// <param name="rootDir">The root directory for music files.</param>
        /// <param name="configPath">The full path to the config file to read.</param>
        private void LoadMusicInfo(string rootDir, string configPath)
        {
            // make sure file exists
            if (!File.Exists(configPath))
            {
                this.Monitor.Log("StardewSymphony:This music pack lacks a Config.txt. Without one, I can't load in the music.");
                return;
            }

            // parse config file
            string[] text = File.ReadAllLines(configPath);
            string wave = Convert.ToString(text[3]);
            string sound = Convert.ToString(text[5]);

            // load all of the info files here. This is some deep magic I worked at 4 AM. I almost forgot how the heck this worked when I woke up.
            MusicManager manager = new MusicManager(this.Monitor, wave, sound, rootDir);
            manager.LoadSeasonalSongs("spring", this.SongWaveReference);
            manager.LoadSeasonalSongs("summer", this.SongWaveReference);
            manager.LoadSeasonalSongs("fall", this.SongWaveReference);
            manager.LoadSeasonalSongs("winter", this.SongWaveReference);
            manager.LoadSeasonalSongs("spring_night", this.SongWaveReference);
            manager.LoadSeasonalSongs("summer_night", this.SongWaveReference);
            manager.LoadSeasonalSongs("fall_night", this.SongWaveReference);
            manager.LoadSeasonalSongs("winter_night", this.SongWaveReference);
            manager.LoadSeasonalSongs("spring_rain", this.SongWaveReference);
            manager.LoadSeasonalSongs("summer_rain", this.SongWaveReference);
            manager.LoadSeasonalSongs("fall_rain", this.SongWaveReference);
            manager.LoadSeasonalSongs("winter_snow", this.SongWaveReference);
            manager.LoadSeasonalSongs("spring_rain_night", this.SongWaveReference);
            manager.LoadSeasonalSongs("summer_rain_night", this.SongWaveReference);
            manager.LoadSeasonalSongs("fall_rain_night", this.SongWaveReference);
            manager.LoadSeasonalSongs("winter_snow_night", this.SongWaveReference);

            // load location music
            foreach (GameLocation location in this.GameLocations)
            {
                manager.Music_Loader_Locations(location.Name, this.SongWaveReference);
                manager.Music_Loader_Locations_Night(location.Name + "_night", this.SongWaveReference);
                manager.Music_Loader_Locations_Rain(location.Name + "_rain", this.SongWaveReference);
                manager.Music_Loader_Locations_Rain_Night(location.Name + "_rain_night", this.SongWaveReference);
            }

            // add everything to master song list
            this.MasterList.Add(manager);
        }

        /// <summary>Recursively load music packs from the given directory.</summary>
        /// <param name="dirPath">The directory path to search for music packs.</param>
        /// <param name="musicPacks">The dictionary to update with music packs.</param>
        private void ProcessDirectory(string dirPath, IDictionary<string, string> musicPacks)
        {
            // load music files
            foreach (string filePath in Directory.GetFiles(dirPath))
            {
                string extension = Path.GetExtension(filePath);
                if (extension == ".xsb")
                {
                    this.Monitor.Log(filePath);
                    this.HexProcessor.AddSoundBank(filePath);
                }
                //if (extension == "xwb")
                //{
                //    Log.AsyncC(path);
                //    MusicHexProcessor.allWaveBanks.Add(path);
                //}
            }

            // read config file
            string configPath = Path.Combine(dirPath, "Config.txt");
            if (File.Exists(configPath))
                musicPacks.Add(dirPath, configPath);

            // check subdirectories
            foreach (string childDir in Directory.GetDirectories(dirPath))
                this.ProcessDirectory(childDir, musicPacks);
        }

        /// <summary>Get the current in-game season.</summary>
        private Season GetSeason()
        {
            if (Game1.IsSpring)
                return Season.Spring;
            if (Game1.IsSummer)
                return Season.Summer;
            if (Game1.IsFall)
                return Season.Fall;
            return Season.Winter;
        }

        /// <summary>Select music for the current location.</summary>
        private void SelectMusic()
        {
            if (!Context.IsWorldReady)
                return;

            //  no_music = false;
            //if at any time the music for an area can't be played for some unknown reason, the game should default to playing the Stardew Valley Soundtrack.
            bool isRaining = Game1.isRaining;

            if (Game1.player.currentLocation is Farm)
            {
                farm_music_selector();
                return;
            }
            if (Game1.isFestival())
            {
                this.StopSound();
                return; //replace with festival music if I decide to support it.
            }
            if (Game1.eventUp)
            {
                this.StopSound();
                return; //replace with event music if I decide to support it/people request it.
            }


            bool isNight = (Game1.timeOfDay < 600 || Game1.timeOfDay > Game1.getModeratelyDarkTime());
            if (isRaining)
            {
                if (isNight)
                {
                    this.PlayRainyNightMusic(); //some really awful heirarchy type thing I made up to help ensure that music plays all the time
                    if (this.HasNoMusic)
                    {
                        this.PlayRainSong();
                        if (this.HasNoMusic)
                        {
                            this.PlayNightSong();
                            if (this.HasNoMusic)
                                this.PlayDefaultSong();
                        }
                    }
                }
                else
                {
                    this.PlayRainSong();
                    if (this.HasNoMusic)
                    {
                        this.PlayNightSong();
                        if (this.HasNoMusic)
                            this.PlayDefaultSong();
                    }
                }
            }
            else
            {
                if (isNight)
                {
                    this.PlayNightSong();
                    if (this.HasNoMusic) //if there is no music playing right now play some music.
                        this.PlayDefaultSong();
                }
                else
                    this.PlayDefaultSong();
            }

            if (this.HasNoMusic) //if there is valid music playing
            {
                if (!this.Config.PlaySeasonalMusic)
                    return;

                if (this.CurrentSong != null && this.CurrentSong.IsPlaying)
                    return;

                this.Monitor.Log("Loading Default Seasonal Music");

                if (!this.MasterList.Any())
                {
                    this.Monitor.Log("The Wave Bank list is empty. Something went wrong, or you don't have any music packs installed, or you didn't have any songs in the list files.");
                    this.Reset();
                    return;
                }

                //add in seasonal stuff here
                if (this.HasNoMusic)
                {
                    Season season = this.GetSeason();
                    if (isRaining)
                        this.StartRainySong(season);
                    else
                        this.StartDefaultSong(season);
                }
            }
        }

        public void farm_music_selector()
        {
            if (!Context.IsWorldReady)
                return;

            //  no_music = false;
            //if at any time the music for an area can't be played for some unknown reason, the game should default to playing the Stardew Valley Soundtrack.
            bool rainy = Game1.isRaining;

            this.Monitor.Log("Loading farm music.");
            if (Game1.isFestival())
            {
                this.StopSound();
                return; //replace with festival music if I decide to support it.
            }
            if (Game1.eventUp)
            {
                this.StopSound();
                return; //replace with event music if I decide to support it/people request it.
            }

            this.Monitor.Log("Loading Default Seasonal Music");

            if (!this.MasterList.Any())
            {
                this.Monitor.Log("The Wave Bank list is empty. Something went wrong, or you don't have any music packs installed, or you didn't have any songs in the list files.");
                this.Reset();
                return;

            }

            //add in seasonal stuff here
            Season season = this.GetSeason();
            if (rainy)
                this.StartRainySong(season);
            else
                this.StartDefaultSong(season);

            //end seasonal songs
            if (this.CurrentSong != null && this.CurrentSong.IsPlaying)
                return;

            //start locational songs
            bool nightTime = Game1.timeOfDay < 600 || Game1.timeOfDay > Game1.getModeratelyDarkTime();
            if (rainy && nightTime)
            {
                this.PlayRainyNightMusic(); //some really awful heirarchy type thing I made up to help ensure that music plays all the time
                if (this.HasNoMusic)
                {
                    this.PlayRainSong();
                    if (this.HasNoMusic)
                    {
                        this.PlayNightSong();
                        if (this.HasNoMusic)
                            this.PlayDefaultSong();
                    }
                }

            }
            if (rainy && !nightTime)
            {
                this.PlayRainSong();
                if (this.HasNoMusic)
                {
                    this.PlayNightSong();
                    if (this.HasNoMusic)
                        this.PlayDefaultSong();
                }

            }
            if (!rainy && nightTime)
            {
                this.PlayNightSong();
                if (this.HasNoMusic)
                    this.PlayDefaultSong();

            }
            if (!rainy && !nightTime)
                this.PlayDefaultSong();
        }

        public void PlayDefaultSong()
        {
            if (!Context.IsWorldReady)
            {
                this.StartMusicDelay();
                return;
            }
            int randomNumber = this.Random.Next(0, this.MasterList.Count);

            if (!this.MasterList.Any())
            {
                this.Monitor.Log("The Wave Bank list is empty. Something went wrong, or you don't have any music packs installed, or you didn't have any songs in the list files.");
                this.Reset();
                return;

            }

            this.CurrentSoundInfo = this.MasterList.ElementAt(randomNumber); //grab a random wave bank/song bank/music pack/ from all available music packs.
            if (Game1.player.currentLocation != null)
            {
                int helper1 = 0;
                int masterHelper = 0;
                bool found = false;

                //this mess of a while loop iterates across all of my music packs looking for a valid music pack to play music from.
                while (true)
                {
                    if (this.CurrentSoundInfo.LocationSongs.Keys.Contains(Game1.player.currentLocation.Name))
                    {

                        foreach (var entry in this.CurrentSoundInfo.LocationSongs)
                        {
                            if (entry.Key == Game1.player.currentLocation.Name)
                            {
                                if (entry.Value.Count > 0)
                                {
                                    //Monitor.Log("FOUND THE RIGHT POSITIONING OF THE CLASS");
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    //this section tells me if it is valid and is less than or equal to 0
                                    masterHelper++; //iterate across the classes
                                    break;
                                }

                            }
                            else
                            {
                                this.Monitor.Log("Not there");
                                helper1++;
                            }
                        } //itterate through all of the valid locations that were stored in this class
                    }
                    else
                        this.Monitor.Log("No data could be loaded on this area. Swaping music packs");
                    if (found)
                        break;

                    masterHelper++;

                    if (masterHelper > this.MasterList.Count)
                    {
                        this.Monitor.Log("I've gone though every music pack with no success. There is no music to load for this area so it will be silent once this song finishes playing. Sorry!");
                        this.HasNoMusic = true;
                        return;

                    }
                    int randomIndex = (masterHelper + randomNumber) % this.MasterList.Count;
                    this.CurrentSoundInfo = this.MasterList.ElementAt(randomIndex); //grab a random wave bank/song bank/music pack/ from all available music packs.
                }

                List<Cue> cues = this.CurrentSoundInfo.LocationSongs.Values.ElementAt(helper1); //set a list of songs to a "random" list of songs from a music pack
                int pointer = 0;
                while (!cues.Any()) //yet another circular array
                {
                    pointer++;
                    int randomID = (pointer + randomNumber) % this.MasterList.Count;

                    this.CurrentSoundInfo = this.MasterList.ElementAt(randomID);
                    if (pointer > this.MasterList.Count)
                    {
                        this.Monitor.Log("No music packs have any valid music for this area. AKA all music packs are empty;");
                        this.HasNoMusic = true;
                        return;
                    }

                }

                this.Monitor.Log("loading music for this area");
                this.StopSound();
                int random3 = this.Random.Next(0, cues.Count);
                Game1.soundBank = this.CurrentSoundInfo.Soundbank; //change the game's soundbank temporarily
                Game1.waveBank = this.CurrentSoundInfo.Wavebank;//dito but wave bank

                this.CurrentSong = cues.ElementAt(random3); //grab a random song from the winter song list
                this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                if (this.CurrentSong != null)
                {
                    this.Monitor.Log("Now listening to: " + this.CurrentSong.Name + " from the music pack located at: " + this.CurrentSoundInfo.Directory + "for the location " + Game1.player.currentLocation);
                    this.HasNoMusic = false;
                    this.CurrentSong.Play(); //play some music
                    this.Reset();
                }
            }
            else
            {
                this.Monitor.Log("Location is null");
                this.HasNoMusic = true;
            }
        }

        public void PlayRainSong()
        {
            if (!Context.IsWorldReady)
            {
                this.StartMusicDelay();
                return;
            }
            int randomNumber = this.Random.Next(0, this.MasterList.Count);

            if (!this.MasterList.Any())
            {
                this.Monitor.Log("The Wave Bank list is empty. Something went wrong, or you don't have any music packs installed, or you didn't have any songs in the list files.");
                this.Reset();
                return;

            }

            this.CurrentSoundInfo = this.MasterList.ElementAt(randomNumber); //grab a random wave bank/song bank/music pack/ from all available music packs.            

            if (Game1.player.currentLocation != null)
            {
                int helper1 = 0;
                int masterHelper = 0;
                bool found = false;

                while (true)
                {
                    if (this.CurrentSoundInfo.LocationRainSongs.Keys.Contains(Game1.player.currentLocation.Name + "_rain"))
                    {

                        foreach (var entry in this.CurrentSoundInfo.LocationRainSongs)
                        {
                            if (entry.Key == Game1.player.currentLocation.Name + "_rain")
                            {
                                if (entry.Value.Count > 0)
                                {
                                    //Monitor.Log("FOUND THE RIGHT POSITIONING OF THE CLASS");
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    //this section tells me if it is valid and is less than or equal to 0
                                    masterHelper++; //iterate across the classes
                                    break;
                                }

                            }
                            else
                            {
                                this.Monitor.Log("Not there");
                                helper1++;
                            }
                        }
                    }
                    else
                        this.Monitor.Log("No data could be loaded on this area. Swaping music packs");

                    if (found)
                        break;
                    masterHelper++;

                    if (masterHelper > this.MasterList.Count)
                    {
                        this.Monitor.Log("I've gone though every music pack with no success. There is no music to load for this area so it will be silent once this song finishes playing. Sorry!");
                        this.HasNoMusic = true;
                        return;
                    }
                    int randomIndex = (masterHelper + randomNumber) % this.MasterList.Count;
                    this.CurrentSoundInfo = this.MasterList.ElementAt(randomIndex); //grab a random wave bank/song bank/music pack/ from all available music packs.            
                }

                List<Cue> cues = this.CurrentSoundInfo.LocationRainSongs.Values.ElementAt(helper1);


                int pointer = 0;
                while (!cues.Any())
                {
                    pointer++;
                    int randomID = (pointer + randomNumber) % this.MasterList.Count;

                    this.CurrentSoundInfo = this.MasterList.ElementAt(randomID);
                    if (pointer > this.MasterList.Count)
                    {
                        this.Monitor.Log("No music packs have any valid music for this area. AKA all music packs are empty;");
                        this.HasNoMusic = true;
                        return;
                    }
                }

                this.Monitor.Log("loading music for this area");
                this.StopSound();
                int random3 = this.Random.Next(0, cues.Count);
                Game1.soundBank = this.CurrentSoundInfo.Soundbank;
                Game1.waveBank = this.CurrentSoundInfo.Wavebank;

                this.CurrentSong = cues.ElementAt(random3); //grab a random song from the winter song list
                this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                if (this.CurrentSong != null)
                {
                    this.HasNoMusic = false;
                    this.Monitor.Log("Now listening to: " + this.CurrentSong.Name + " from the music pack located at: " + this.CurrentSoundInfo.Directory + "for the location " + Game1.player.currentLocation + " while it is raining");
                    this.CurrentSong.Play();
                    this.Reset();
                }
            }
            else
                this.Monitor.Log("Location is null");
        }

        public void PlayNightSong()
        {
            if (!Context.IsWorldReady)
            {
                this.StartMusicDelay();
                return;
            }
            int randomNumber = this.Random.Next(0, this.MasterList.Count);

            if (!this.MasterList.Any())
            {
                this.Monitor.Log("The Wave Bank list is empty. Something went wrong, or you don't have any music packs installed, or you didn't have any songs in the list files.");
                this.Reset();
                return;
            }

            this.CurrentSoundInfo = this.MasterList.ElementAt(randomNumber); //grab a random wave bank/song bank/music pack/ from all available music packs.            

            if (Game1.player.currentLocation != null)
            {
                int helper1 = 0;
                int masterHelper = 0;
                bool found = false;

                while (true)
                {
                    if (this.CurrentSoundInfo.LocationNightSongs.Keys.Contains(Game1.player.currentLocation.Name + "_night"))
                    {
                        foreach (var entry in this.CurrentSoundInfo.LocationNightSongs)
                        {
                            if (entry.Key == Game1.player.currentLocation.Name + "_night")
                            {
                                if (entry.Value.Count > 0)
                                {
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    //this section tells me if it is valid and is less than or equal to 0
                                    masterHelper++; //iterate across the classes
                                    break;
                                }
                            }
                            else
                            {
                                this.Monitor.Log("Not there");
                                helper1++;
                            }
                        }
                    }
                    else
                        this.Monitor.Log("No data could be loaded on this area. Swaping music packs");
                    if (found)
                        break;

                    masterHelper++;

                    if (masterHelper > this.MasterList.Count)
                    {
                        this.Monitor.Log("I've gone though every music pack with no success. There is no music to load for this area so it will be silent once this song finishes playing. Sorry!");
                        this.HasNoMusic = true;
                        return;
                    }
                    int randomIndex = (masterHelper + randomNumber) % this.MasterList.Count;
                    this.CurrentSoundInfo = this.MasterList.ElementAt(randomIndex); //grab a random wave bank/song bank/music pack/ from all available music packs.            
                }

                List<Cue> cues = this.CurrentSoundInfo.LocationNightSongs.Values.ElementAt(helper1);
                int pointer = 0;
                while (!cues.Any())
                {
                    pointer++;
                    int randomID = (pointer + randomNumber) % this.MasterList.Count;

                    this.CurrentSoundInfo = this.MasterList.ElementAt(randomID);
                    if (pointer > this.MasterList.Count)
                    {
                        this.Monitor.Log("No music packs have any valid music for this area. AKA all music packs are empty;");
                        this.HasNoMusic = true;
                        return;
                    }
                }

                this.Monitor.Log("loading music for this area");
                this.StopSound();
                int random3 = this.Random.Next(0, cues.Count);
                Game1.soundBank = this.CurrentSoundInfo.Soundbank;
                Game1.waveBank = this.CurrentSoundInfo.Wavebank;

                this.CurrentSong = cues.ElementAt(random3); //grab a random song from the winter song list
                this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                if (this.CurrentSong != null)
                {
                    this.HasNoMusic = false;
                    this.Monitor.Log("Now listening to: " + this.CurrentSong.Name + " from the music pack located at: " + this.CurrentSoundInfo.Directory + "for the location " + Game1.player.currentLocation + " while it is night time.");
                    this.CurrentSong.Play();
                    this.Reset();
                }
            }
            else
                this.Monitor.Log("Location is null");
        }

        public void PlayRainyNightMusic()
        {
            if (!Context.IsWorldReady)
            {
                this.StartMusicDelay();
                return;
            }
            int randomNumber = this.Random.Next(0, this.MasterList.Count);

            if (!this.MasterList.Any())
            {
                this.Monitor.Log("The Wave Bank list is empty. Something went wrong, or you don't have any music packs installed, or you didn't have any songs in the list files.");
                this.Reset();
                return;

            }

            this.CurrentSoundInfo = this.MasterList.ElementAt(randomNumber); //grab a random wave bank/song bank/music pack/ from all available music packs.            

            if (Game1.player.currentLocation != null)
            {

                int helper1 = 0;
                int masterHelper = 0;
                bool found = false;

                while (true)
                {
                    if (this.CurrentSoundInfo.LocationRainNightSongs.Keys.Contains(Game1.player.currentLocation.Name + "_rain_night"))
                    {

                        foreach (var entry in this.CurrentSoundInfo.LocationRainNightSongs)
                        {
                            if (entry.Key == Game1.player.currentLocation.Name + "_rain_night")
                            {
                                if (entry.Value.Count > 0)
                                {
                                    found = true;
                                    break;
                                }
                                else
                                {
                                    //this section tells me if it is valid and is less than or equal to 0
                                    masterHelper++; //iterate across the classes
                                    break;
                                }

                            }
                            else
                            {
                                this.Monitor.Log("Not there");
                                helper1++;
                            }
                        }
                    }
                    else
                    {
                        this.Monitor.Log("No data could be loaded on this area. Swaping music packs");
                    }

                    if (found)
                        break;

                    masterHelper++;

                    if (masterHelper > this.MasterList.Count)
                    {
                        this.Monitor.Log("I've gone though every music pack with no success. There is no music to load for this area so it will be silent once this song finishes playing. Sorry!");
                        this.HasNoMusic = true;
                        return;

                    }
                    int randomIndex = (masterHelper + randomNumber) % this.MasterList.Count;
                    this.CurrentSoundInfo = this.MasterList.ElementAt(randomIndex); //grab a random wave bank/song bank/music pack/ from all available music packs.            
                }

                List<Cue> cues = this.CurrentSoundInfo.LocationRainNightSongs.Values.ElementAt(helper1);

                int pointer = 0;
                while (!cues.Any())
                {
                    pointer++;
                    int randomID = (pointer + randomNumber) % this.MasterList.Count;

                    this.CurrentSoundInfo = this.MasterList.ElementAt(randomID);
                    if (pointer > this.MasterList.Count)
                    {
                        this.Monitor.Log("No music packs have any valid music for this area. AKA all music packs are empty;");
                        this.HasNoMusic = true;
                        return;
                    }

                }
                this.Monitor.Log("loading music for this area");
                this.StopSound();
                int random3 = this.Random.Next(0, cues.Count);
                Game1.soundBank = this.CurrentSoundInfo.Soundbank;
                Game1.waveBank = this.CurrentSoundInfo.Wavebank;

                this.CurrentSong = cues.ElementAt(random3); //grab a random song from the winter song list
                this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                if (this.CurrentSong != null)
                {
                    this.HasNoMusic = false;
                    this.Monitor.Log("Now listening to: " + this.CurrentSong.Name + " from the music pack located at: " + this.CurrentSoundInfo.Directory + "for the location " + Game1.player.currentLocation + " while it is raining at night.");
                    this.CurrentSong.Play();
                    this.Reset();
                }
            }
            else
                this.Monitor.Log("Location is null");
        }

        /// <summary>Start a default song for the given season.</summary>
        /// <param name="season">The season for which to play music.</param>
        private void StartDefaultSong(Season season)
        {
            // check exit conditions
            if (!Context.IsWorldReady)
            {
                this.StartMusicDelay();
                return;
            }
            if (!this.MasterList.Any())
            {
                this.Monitor.Log("The Wave Bank list is empty. Something went wrong, or you don't have any music packs installed, or you didn't have any songs in the list files.");
                this.Reset();
                return;
            }

            // get random song from available music packs
            this.CurrentSoundInfo = this.MasterList[this.Random.Next(0, this.MasterList.Count)];

            // pick night music
            if (Game1.timeOfDay < 600 || Game1.timeOfDay >= Game1.getModeratelyDarkTime())
            {
                int randomSongIndex = this.Random.Next(0, this.CurrentSoundInfo.NightSongs[season].Count);

                if (!this.CurrentSoundInfo.NightSongs[season].Any())
                {
                    this.Monitor.Log($"The {season} night song list is empty. Trying to look for more songs."); //or should I default where if there aren't any nightly songs to play a song from a different play list?

                    int minIndex = 0;
                    for (; minIndex < this.MasterList.Count; minIndex++)
                    {
                        if (this.CurrentSoundInfo.NightSongs[season].Count > 0)
                        {
                            this.StopSound();
                            Game1.soundBank = this.CurrentSoundInfo.Soundbank; //access my new sound table
                            Game1.waveBank = this.CurrentSoundInfo.Wavebank;
                            this.CurrentSong = this.CurrentSoundInfo.NightSongs[season].ElementAt(randomSongIndex); //grab a random song from the seasonal song list
                            this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                            break;
                        }

                        if (minIndex == this.MasterList.Count - 1)
                        {
                            this.Monitor.Log("I've gone though every music pack with no success for default music. There is no music to load for this area so it will be silent once this song finishes playing. Sorry!");
                            this.HasNoMusic = true;
                            return;
                        }
                        int randomIndex = (minIndex + randomSongIndex) % this.MasterList.Count;
                        this.CurrentSoundInfo = this.MasterList.ElementAt(randomIndex); //grab a random wave bank/song bank/music pack/ from all available music packs.
                    }
                }
                else
                {
                    this.StopSound();
                    this.CurrentSong = this.CurrentSoundInfo.NightSongs[season].ElementAt(randomSongIndex); //grab a random song from the seasonal song list
                    Game1.soundBank = this.CurrentSoundInfo.Soundbank; //access my new sound table
                    Game1.waveBank = this.CurrentSoundInfo.Wavebank;
                    this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                }

                // play selected song
                if (this.CurrentSong != null)
                {
                    this.HasNoMusic = false;
                    this.Monitor.Log($"Now listening to: {this.CurrentSong.Name} from the music pack located at: {this.CurrentSoundInfo.Directory} while it is a {season} Night. Check the seasons folder for more info");
                    this.CurrentSong.Play();
                    this.Reset();
                    return;
                }
            }

            // else pick default music
            {
                int randomSongIndex = this.Random.Next(0, this.CurrentSoundInfo.DefaultSongs[season].Count); //random number between 0 and n. 0 not includes
                if (!this.CurrentSoundInfo.DefaultSongs[season].Any())
                {
                    this.Monitor.Log($"The {season} night song list is empty. Trying to look for more songs."); //or should I default where if there aren't any nightly songs to play a song from a different play list?

                    int minIndex = 0;
                    for (; minIndex < this.MasterList.Count; minIndex++)
                    {
                        if (this.CurrentSoundInfo.NightSongs[season].Count > 0)
                        {
                            this.StopSound();
                            Game1.soundBank = this.CurrentSoundInfo.Soundbank; //access my new sound table
                            Game1.waveBank = this.CurrentSoundInfo.Wavebank;
                            this.CurrentSong = this.CurrentSoundInfo.DefaultSongs[season].ElementAt(randomSongIndex); //grab a random song from the seasonal song list
                            this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                            break;
                        }

                        if (minIndex == this.MasterList.Count - 1)
                        {
                            this.Monitor.Log("I've gone though every music pack with no success for default music. There is no music to load for this area so it will be silent once this song finishes playing. Sorry!");
                            this.HasNoMusic = true;
                            return;
                        }
                        int randomIndex = (minIndex + randomSongIndex) % this.MasterList.Count;
                        this.CurrentSoundInfo = this.MasterList.ElementAt(randomIndex); //grab a random wave bank/song bank/music pack/ from all available music packs.
                    }
                }
                else
                {
                    this.StopSound();
                    this.CurrentSong = this.CurrentSoundInfo.DefaultSongs[season].ElementAt(randomSongIndex); //grab a random song from the seasonal song list
                    Game1.soundBank = this.CurrentSoundInfo.Soundbank; //access my new sound table
                    Game1.waveBank = this.CurrentSoundInfo.Wavebank;
                    this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                }

                // play selected song
                if (this.CurrentSong != null)
                {
                    this.HasNoMusic = false;
                    this.Monitor.Log($"Now listening to: {this.CurrentSong.Name} from the music pack located at: {this.CurrentSoundInfo.Directory} while it is {season} Time. Check the seasons folder for more info");
                    this.CurrentSong.Play();
                    this.Reset();
                }
            }
        }

        /// <summary>Start a song for rainy days in the given season.</summary>
        /// <param name="season">The season for which to play music.</param>
        private void StartRainySong(Season season)
        {
            // check exit conditions
            if (!Context.IsWorldReady)
            {
                this.StartMusicDelay();
                return;
            }
            if (!this.MasterList.Any())
            {
                this.Monitor.Log("The Wave Bank list is empty. Something went wrong, or you don't have any music packs installed, or you didn't have any songs in the list files.");
                this.Reset();
                return;
            }

            // get random song from available music packs
            this.CurrentSoundInfo = this.MasterList[this.Random.Next(0, this.MasterList.Count)];

            // pick night song
            if (Game1.timeOfDay < 600 || Game1.timeOfDay >= Game1.getModeratelyDarkTime())
            {
                int randomNumber = this.Random.Next(0, this.CurrentSoundInfo.RainyNightSongs[season].Count);

                if (this.CurrentSoundInfo.RainyNightSongs[season].Count == 0)
                {
                    this.Monitor.Log($"The {season}_rain night song list is empty. Trying to look for more songs."); //or should I default where if there aren't any nightly songs to play a song from a different play list?

                    int minIndex = 0;
                    for (; minIndex < this.MasterList.Count; minIndex++)
                    {
                        if (this.CurrentSoundInfo.RainyNightSongs[season].Count > 0)
                        {
                            this.StopSound();
                            Game1.soundBank = this.CurrentSoundInfo.Soundbank; //access my new sound table
                            Game1.waveBank = this.CurrentSoundInfo.Wavebank;
                            this.CurrentSong = this.CurrentSoundInfo.RainyNightSongs[season].ElementAt(randomNumber); //grab a random song from the seasonal rain song list
                            this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                            break;
                        }

                        if (minIndex == this.MasterList.Count - 1)
                        {
                            this.Monitor.Log("I've gone though every music pack with no success for default music. There is no music to load for this area so it will be silent once this song finishes playing. Sorry!");
                            this.HasNoMusic = true;
                            return;
                        }
                        int randomIndex = (minIndex + randomNumber) % this.MasterList.Count;
                        this.CurrentSoundInfo = this.MasterList.ElementAt(randomIndex); //grab a random wave bank/song bank/music pack/ from all available music packs.
                    }
                }
                else
                {
                    this.StopSound();
                    this.CurrentSong = this.CurrentSoundInfo.RainyNightSongs[season].ElementAt(randomNumber); //grab a random song from the seasonal song list
                    Game1.soundBank = this.CurrentSoundInfo.Soundbank; //access my new sound table
                    Game1.waveBank = this.CurrentSoundInfo.Wavebank;
                    this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                }

                // play selected song
                if (this.CurrentSong != null)
                {
                    this.HasNoMusic = false;
                    this.Monitor.Log($"Now listening to: {this.CurrentSong.Name} from the music pack located at: {this.CurrentSoundInfo.Directory} while it is a rainy {season} night. Check the Seasons folder for more info");
                    this.CurrentSong.Play();
                    this.Reset();
                    return;
                }
            }

            // pick default song
            {
                int randomNumber = this.Random.Next(0, this.CurrentSoundInfo.RainySongs[season].Count);

                if (this.CurrentSoundInfo.RainySongs[season].Count == 0)
                {
                    this.Monitor.Log($"The {season}_rain night song list is empty. Trying to look for more songs."); //or should I default where if there aren't any nightly songs to play a song from a different play list?
                    int minIndex = 0;
                    for (; minIndex < this.MasterList.Count; minIndex++)
                    {
                        if (this.CurrentSoundInfo.RainySongs[Season.Spring].Count > 0)
                        {
                            this.StopSound();
                            Game1.soundBank = this.CurrentSoundInfo.Soundbank; //access my new sound table
                            Game1.waveBank = this.CurrentSoundInfo.Wavebank;
                            this.CurrentSong = this.CurrentSoundInfo.RainySongs[Season.Spring].ElementAt(randomNumber); //grab a random song from the spring_rain song list
                            this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                            break;
                        }

                        if (minIndex > this.MasterList.Count)
                        {
                            this.Monitor.Log("I've gone though every music pack with no success for default music. There is no music to load for this area so it will be silent once this song finishes playing. Sorry!");
                            this.HasNoMusic = true;
                            return;
                        }
                        int randomIndex = (minIndex + randomNumber) % this.MasterList.Count;
                        this.CurrentSoundInfo = this.MasterList.ElementAt(randomIndex); //grab a random wave bank/song bank/music pack/ from all available music packs.            
                    }
                }
                else
                {
                    this.StopSound();
                    this.CurrentSong = this.CurrentSoundInfo.RainySongs[Season.Spring].ElementAt(randomNumber); //grab a random song from the fall song list
                    Game1.soundBank = this.CurrentSoundInfo.Soundbank; //access my new sound table
                    Game1.waveBank = this.CurrentSoundInfo.Wavebank;
                    this.CurrentSong = Game1.soundBank.GetCue(this.CurrentSong.Name);
                }

                // play selected song
                if (this.CurrentSong != null)
                {
                    this.HasNoMusic = false;
                    this.Monitor.Log($"Now listening to: {this.CurrentSong.Name} from the music pack located at: {this.CurrentSoundInfo.Directory} while it is a rainy {season} Day. Check the seasons folder for more info");
                    this.CurrentSong.Play();
                    this.Reset();
                }
            }
        }

        /// <summary>Stop the currently playing sound, if any.</summary>
        public void StopSound()
        {
            if (this.CurrentSong == null)
            {
                //trying to stop a song that doesn't "exist" crashes the game. This prevents that.
                return;
            }

            if (this.CurrentSoundInfo == null)
            {
                //if my info class is null, return. Should only be null if the game starts. Pretty sure my code should prevent this.
                return;
            }
            Game1.soundBank = this.CurrentSoundInfo.Soundbank; //reset the wave/sound banks back to the music pack's
            Game1.waveBank = this.CurrentSoundInfo.Wavebank;
            this.CurrentSong.Stop(AudioStopOptions.Immediate); //redundant stopping code
            this.CurrentSong.Stop(AudioStopOptions.AsAuthored);
            Game1.soundBank = this.DefaultSoundbank; //reset the wave/sound to the game's original
            Game1.waveBank = this.DefaultWavebank;
            this.CurrentSong = null;
        }

        /// <summary>Reset the game audio to the original settings.</summary>
        private void Reset()
        {
            Game1.waveBank = this.DefaultWavebank;
            Game1.soundBank = this.DefaultSoundbank;
        }
    }
}
