using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using StardewValley;

namespace StardewSymphonyRemastered.Framework
{
    /// <summary>Manages all music for the mod.</summary>
    public class MusicManager
    {
        /*********
        ** Fields
        *********/
        /// <summary>The RNG used to select music packs and songs.</summary>
        private readonly Random Random = new Random();

        /// <summary>The delay timer between songs.</summary>
        private readonly Timer Timer = new Timer();

        private bool lastSongWasLocationSpecific;


        /*********
        ** Accessors
        *********/
        /// <summary>The loaded music packs.</summary>
        public IDictionary<string, MusicPack> MusicPacks { get; } = new Dictionary<string, MusicPack>();

        /// <summary>The current music pack playing music, if any.</summary>
        public MusicPack CurrentMusicPack { get; private set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public MusicManager()
        {
            this.Timer.Elapsed += this.OnTimerFinished;
        }

        /// <summary>Swap between referenced music packs and stop the current song.</summary>
        /// <param name="nameOfNewMusicPack">The name of the new music pack to select.</param>
        public void SwapMusicPacks(string nameOfNewMusicPack)
        {
            if (!this.MusicPacks.TryGetValue(nameOfNewMusicPack, out MusicPack musicPack))
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log($"ERROR: Music Pack '{nameOfNewMusicPack}' isn't valid for some reason.", StardewModdingAPI.LogLevel.Alert);
                return;
            }

            this.CurrentMusicPack?.StopSong();
            this.CurrentMusicPack = musicPack;
        }

        /// <summary>Updates the timer every second to check whether a song is playing.</summary>
        public void UpdateTimer()
        {
            if (this.CurrentMusicPack == null)
                return;

            if (StardewSymphony.Config.DisableStardewMusic)
            {
                if (this.CurrentMusicPack.IsPlaying())
                    return;
            }
            else if (this.CurrentMusicPack.IsPlaying() || (Game1.currentSong?.IsPlaying == true && !Game1.currentSong.Name.ToLower().Contains("ambient")))
                return;

            if (!this.Timer.Enabled)
            {
                this.Timer.Interval = this.Random.Next(StardewSymphony.Config.MinimumDelayBetweenSongsInMilliseconds, StardewSymphony.Config.MaximumDelayBetweenSongsInMilliseconds + 1);
                this.Timer.Enabled = true;
            }
        }

        /// <summary>Choose a new song when a delay runs out.</summary>
        private void OnTimerFinished(object source, ElapsedEventArgs e)
        {
            this.Timer.Enabled = false;
            if (!this.CurrentMusicPack.IsPlaying())
                this.selectMusic(SongSpecifics.getCurrentConditionalString());
        }

        /// <summary>Play a song from the current music pack.</summary>
        /// <param name="songName">The song to play.</param>
        public void PlaySongFromCurrentPack(string songName)
        {
            this.CurrentMusicPack?.PlaySong(songName);
        }

        /// <summary>Stop the current song being played.</summary>
        public void stopSongFromCurrentMusicPack()
        {
            this.CurrentMusicPack?.StopSong();
        }

        /// <summary>Get all music packs which contain songs that can be played right now.</summary>
        public Dictionary<MusicPack, List<string>> GetApplicableMusicPacks(string songListKey)
        {
            Dictionary<MusicPack, List<string>> listOfValidDictionaries = new Dictionary<MusicPack, List<string>>();
            foreach (var v in this.MusicPacks)
            {
                try
                {
                    var songList = v.Value.SongInformation.getSongList(songListKey).Value;
                    if (songList.Count > 0)
                        listOfValidDictionaries.Add(v.Value, songList);
                }
                catch { }
            }
            return listOfValidDictionaries;
        }

        public Dictionary<MusicPack, List<string>> GetListOfApplicableMusicPacksForFestivals()
        {
            Dictionary<MusicPack, List<string>> listOfValidDictionaries = new Dictionary<MusicPack, List<string>>();
            foreach (var v in this.MusicPacks)
            {
                try
                {
                    var songList = v.Value.SongInformation.getFestivalMusic();
                    if (songList.Count > 0)
                        listOfValidDictionaries.Add(v.Value, songList);
                }
                catch { }
            }
            return listOfValidDictionaries;
        }

        public Dictionary<MusicPack, List<string>> GetListOfApplicableMusicPacksForEvents()
        {
            Dictionary<MusicPack, List<string>> listOfValidDictionaries = new Dictionary<MusicPack, List<string>>();
            foreach (var v in this.MusicPacks)
            {
                try
                {
                    var songList = v.Value.SongInformation.getEventMusic();
                    if (songList.Count > 0)
                        listOfValidDictionaries.Add(v.Value, songList);
                }
                catch { }
            }
            return listOfValidDictionaries;
        }

        public void SelectMenuMusic(string songListKey)
        {
            // stop timer when new music is selected
            this.Timer.Enabled = false;

            // get applicable music packs
            var listOfValidMusicPacks = this.GetApplicableMusicPacks(songListKey);
            if (listOfValidMusicPacks.Count == 0)
                return;

            // swap to new music pack
            var pair = listOfValidMusicPacks.ElementAt(this.Random.Next(0, listOfValidMusicPacks.Count - 1));
            this.SwapMusicPacks(pair.Key.Name);
            string songName = pair.Value.ElementAt(this.Random.Next(0, pair.Value.Count));
            this.CurrentMusicPack.PlaySong(songName);

            StardewSymphony.menuChangedMusic = true;

        }
        /// <summary>Select the actual song to be played right now based on the selector key. The selector key should be called when the player's location changes.</summary>
        public void selectMusic(string songListKey)
        {
            // stop timer timer when music is selected
            this.Timer.Enabled = false;

            // get applicable music packs
            var listOfValidMusicPacks = this.GetApplicableMusicPacks(songListKey);
            string subKey = songListKey;

            //This chunk is to determine song specifics for location.
            while (listOfValidMusicPacks.Count == 0)
            {
                if (subKey.Length == 0)
                    break;

                string[] subList = subKey.Split(SongSpecifics.seperator);
                if (subList.Length == 0)
                    break; //Because things would go bad otherwise.

                subKey = "";
                for (int i = 0; i < subList.Length - 1; i++)
                {
                    subKey += subList[i];
                    if (i != subList.Length - 2)
                        subKey += SongSpecifics.seperator;
                }
                if (subKey == "")
                    break;

                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log(subKey, StardewModdingAPI.LogLevel.Alert);
                listOfValidMusicPacks = this.GetApplicableMusicPacks(subKey);
                if (listOfValidMusicPacks.Count == 0)
                {
                    //No valid songs to play at this time.
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + subKey + ". 1 Are you sure you did this properly?");
                    //return;
                }
            }

            if (listOfValidMusicPacks.Count == 0)
            {
                //This chunk is used to determine more general seasonal specifics if song specifics couldn't be found.
                subKey = songListKey;
                string[] season = subKey.Split(SongSpecifics.seperator);
                subKey = "";
                for (int i = 1; i < season.Length; i++)
                {
                    subKey += season[i];
                    if (i != season.Length - 1)
                        subKey += SongSpecifics.seperator;
                }
                if (string.IsNullOrEmpty(subKey))
                {
                    if (!this.checkGenericMusic(songListKey))
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + songListKey + ".2 Are you sure you did this properly?");
                        StardewSymphony.menuChangedMusic = false;
                        return;
                    }
                }
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log(subKey, StardewModdingAPI.LogLevel.Alert);
                listOfValidMusicPacks = this.GetApplicableMusicPacks(subKey);
                if (listOfValidMusicPacks.Count == 0)
                {
                    //No valid songs to play at this time.
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + subKey + ".3 Are you sure you did this properly?");
                    //return;
                }
                //Try to get more specific.


                //????? What does this part even do anymore???
                while (listOfValidMusicPacks.Count == 0)
                {
                    if (subKey.Length == 0) break;
                    string[] subList = subKey.Split(SongSpecifics.seperator);
                    if (subList.Length == 0) break; //Because things would go bad otherwise.
                    subKey = "";
                    for (int i = 0; i < subList.Length - 1; i++)
                    {
                        subKey += subList[i];
                        if (i != subList.Length - 2)
                            subKey += SongSpecifics.seperator;
                    }
                    if (subKey == "")
                        break;

                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log(subKey, StardewModdingAPI.LogLevel.Alert);
                    listOfValidMusicPacks = this.GetApplicableMusicPacks(subKey);
                    if (listOfValidMusicPacks.Count == 0)
                    {
                        //No valid songs to play at this time.
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + subKey + ".4 Are you sure you did this properly?");
                        //return;
                    }
                }
            }

            //If the list of valid packs are 0, check if I'm currently at an event or festival or get some location specific music and try to play a generalized song from there.
            if (listOfValidMusicPacks.Count == 0)
            {
                if (!this.checkGenericMusic(songListKey))
                {
                    //No valid songs to play at this time.
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + songListKey + ".7 Are you sure you did this properly?");
                    StardewSymphony.menuChangedMusic = false;
                    return;
                }
            }

            string[] sizeList = subKey.Split(SongSpecifics.seperator);

            if (this.CurrentMusicPack != null)
            {
                //If I am trying to play a generic song and a generic song is playing don't change the music.
                //If I am trying to play a generic song and a non-generic song is playing, then play my generic song since I don't want to play the specific music anymore.
                if (sizeList.Length < 3 && (this.CurrentMusicPack.IsPlaying() && !this.lastSongWasLocationSpecific))
                {
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Non specific music change detected. Not going to change the music this time");
                    return;
                }
            }

            this.lastSongWasLocationSpecific = sizeList.Length >= 3;

            //If there is a valid key for the place/time/event/festival I am at, play it!

            int randInt = this.Random.Next(0, listOfValidMusicPacks.Count - 1);

            var musicPackPair = listOfValidMusicPacks.ElementAt(randInt);


            //used to swap the music packs and stop the last playing song.
            this.SwapMusicPacks(musicPackPair.Key.Name);
            string songName = musicPackPair.Value.ElementAt(this.Random.Next(0, musicPackPair.Value.Count));
            this.CurrentMusicPack.PlaySong(songName);
        }


        public Dictionary<MusicPack, List<string>> getLocationSpecificMusic()
        {
            Dictionary<MusicPack, List<string>> listOfValidDictionaries = new Dictionary<MusicPack, List<string>>();
            //StardewSymphony.ModMonitor.Log(SongSpecifics.getCurrentConditionalString(true));

            foreach (var v in this.MusicPacks)
            {
                try
                {
                    var songList = v.Value.SongInformation.getSongList(SongSpecifics.getCurrentConditionalString(true)).Value;
                    if (songList == null) return null;
                    if (songList.Count > 0)
                    {
                        listOfValidDictionaries.Add(v.Value, songList);
                    }
                }
                catch { }
            }
            return listOfValidDictionaries;
        }

        public bool checkGenericMusic(string songListKey)
        {
            if (Game1.CurrentEvent != null)
            {
                if (Game1.CurrentEvent.isFestival)
                {
                    //Try to play a generalized festival song.
                    var listOfFestivalPacks = this.GetListOfApplicableMusicPacksForFestivals();
                    if (listOfFestivalPacks.Count > 0)
                    {
                        int randFestivalPack = this.Random.Next(0, listOfFestivalPacks.Count - 1);

                        var festivalMusicPackPair = listOfFestivalPacks.ElementAt(randFestivalPack);

                        //used to swap the music packs and stop the last playing song.
                        this.SwapMusicPacks(festivalMusicPackPair.Key.Name);
                        string festivalSongName = festivalMusicPackPair.Value.ElementAt(this.Random.Next(0, festivalMusicPackPair.Value.Count));
                        this.CurrentMusicPack.PlaySong(festivalSongName);
                        StardewSymphony.menuChangedMusic = false;
                        return true;
                    }
                    else
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + songListKey + ".5 Are you sure you did this properly?");
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("Also failed playing a festival event song.");
                        StardewSymphony.menuChangedMusic = false;
                        return false;
                    }
                }

                else
                {
                    //Try to play a generalized event song.
                    var listOfEventPacks = this.GetListOfApplicableMusicPacksForEvents();
                    if (listOfEventPacks.Count > 0)
                    {
                        int randEventPack = this.Random.Next(0, listOfEventPacks.Count - 1);

                        var eventMusicPackPair = listOfEventPacks.ElementAt(randEventPack);

                        //used to swap the music packs and stop the last playing song.
                        this.SwapMusicPacks(eventMusicPackPair.Key.Name);
                        string eventSongName = eventMusicPackPair.Value.ElementAt(this.Random.Next(0, eventMusicPackPair.Value.Count));
                        this.CurrentMusicPack.PlaySong(eventSongName);
                        StardewSymphony.menuChangedMusic = false;
                        return true;
                    }
                    else
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + songListKey + ".6 Are you sure you did this properly?");
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("Also failed playing a generalized event song.");
                        StardewSymphony.menuChangedMusic = false;
                        return false;
                    }
                }
            }
            else
            {
                //StardewSymphony.ModMonitor.Log("HELLO??? LOCATION????");
                //Try to play a generalized festival song.
                var listOfLocationPacks = this.getLocationSpecificMusic();
                if (listOfLocationPacks.Count > 0)
                {
                    int randFestivalPack = this.Random.Next(0, listOfLocationPacks.Count - 1);

                    var locationMusicPackPair = listOfLocationPacks.ElementAt(randFestivalPack);

                    //used to swap the music packs and stop the last playing song.
                    this.SwapMusicPacks(locationMusicPackPair.Key.Name);
                    string songName = locationMusicPackPair.Value.ElementAt(this.Random.Next(0, locationMusicPackPair.Value.Count));
                    this.CurrentMusicPack.PlaySong(songName);
                    StardewSymphony.menuChangedMusic = false;
                    return true;
                }
                else
                {
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + songListKey + ".5 Are you sure you did this properly?");
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Also failed playing a festival event song.");
                    StardewSymphony.menuChangedMusic = false;
                    return false;
                }
            }
        }

        /// <summary>Adds a valid xwb music pack to the list of music packs available.</summary>
        /// <param name="musicPack">The music pack to add.</param>
        /// <param name="displayLogInformation">Whether or not to display the process to the console. Will include information from the pack's metadata. Default:False</param>
        /// <param name="displaySongs">If displayLogInformation is also true this will display the name of all of the songs in the music pack when it is added in.</param>
        public void addMusicPack(MusicPack musicPack, bool displayLogInformation = false, bool displaySongs = false)
        {
            if (displayLogInformation)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                {
                    StardewSymphony.ModMonitor.Log("Adding music pack:");
                    StardewSymphony.ModMonitor.Log($"   Name: {musicPack.Name}");
                    StardewSymphony.ModMonitor.Log($"   Author: {musicPack.Manifest.Author}");
                    StardewSymphony.ModMonitor.Log($"   Description: {musicPack.Manifest.Description}");
                    StardewSymphony.ModMonitor.Log($"   Version Info: {musicPack.Manifest.Version}");
                }
                if (displaySongs && StardewSymphony.Config.EnableDebugLog)
                {
                    StardewSymphony.ModMonitor.Log("    Song List:");
                    foreach (string song in musicPack.SongInformation.listOfSongsWithoutTriggers)
                        StardewSymphony.ModMonitor.Log($"        {song}");
                }
            }

            this.MusicPacks.Add(musicPack.Name, musicPack);
        }

        /// <summary>Initializes all of the potential key triggers for playing songs.</summary>
        public void initializeSeasonalMusic()
        {
            foreach (var pack in this.MusicPacks)
                pack.Value.SongInformation.initializeSeasonalMusic();
        }

        /// <summary>Initializes all of the potential key triggers for playing songs.</summary>
        public void initializeMenuMusic()
        {
            foreach (var pack in this.MusicPacks)
                pack.Value.SongInformation.initializeMenuMusic();
        }

        /// <summary>Initializes all of the potential key triggers for playing songs.</summary>
        public void initializeFestivalMusic()
        {
            foreach (var pack in this.MusicPacks)
                pack.Value.SongInformation.initializeFestivalMusic();
        }

        /// <summary>Initializes all of the potential key triggers for playing songs.</summary>
        public void initializeEventMusic()
        {
            foreach (var pack in this.MusicPacks)
                pack.Value.SongInformation.initializeEventMusic();
        }
    }
}
