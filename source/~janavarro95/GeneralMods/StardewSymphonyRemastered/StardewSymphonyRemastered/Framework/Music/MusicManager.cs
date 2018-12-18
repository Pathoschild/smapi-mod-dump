using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewSymphonyRemastered;
using StardewValley;
using System.IO;
using System.Timers;

namespace StardewSymphonyRemastered.Framework
{
    /// <summary>
    /// Manages all music for the mod.
    /// </summary>
    public class MusicManager
    {
        /// <summary>
        /// A dictionary containing all of the music packs loaded in.
        /// </summary>
        public Dictionary<string,MusicPack> musicPacks;

        public MusicPack currentMusicPack;

        Random packSelector;
        Random songSelector;

        Timer timer;

        bool lastSongWasLocationSpecific;
        /// <summary>
        /// Constructor.
        /// </summary>
        public MusicManager()
        {
            this.musicPacks = new Dictionary<string, MusicPack>();
            this.currentMusicPack = null;
            packSelector = new Random(Game1.random.Next(1,1000000));
            songSelector = new Random(Game1.player.deepestMineLevel + Game1.player.facingDirection + packSelector.Next(0,10000));
            lastSongWasLocationSpecific = false;
        }

        /// <summary>
        /// Swaps between referenced music packs and stops the last playing song.
        /// </summary>
        /// <param name="nameOfNewMusicPack"></param>
        public void swapMusicPacks(string nameOfNewMusicPack)
        {
            if (isMusicPackValid(nameOfNewMusicPack) == true)
            {
                if (this.currentMusicPack.isNull()==false)
                {
                    this.currentMusicPack.stopSong();
                }
                this.currentMusicPack = getMusicPack(nameOfNewMusicPack);
            }
            else
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("ERROR: Music Pack " + nameOfNewMusicPack + " isn't valid for some reason.", StardewModdingAPI.LogLevel.Alert);
            }
        }

        /// <summary>
        /// Updtes the timer every second to check if a song is playing or not.
        /// </summary>
        public void updateTimer()
        {
            
            if (this.currentMusicPack == null) return;
            if (StardewSymphonyRemastered.StardewSymphony.Config.disableStardewMusic==true)
            {
                if (this.currentMusicPack.isPlaying())
                {
                    return;
                }
            }
            else
            {
                try
                {
                    string songName = Game1.currentSong.Name.ToLower();

                    if (this.currentMusicPack.isPlaying() || (Game1.currentSong.IsPlaying && !songName.Contains("ambient")) )
                    {
                        return;
                    }
                }
                catch (Exception err)
                {
                    if (this.currentMusicPack.isPlaying())
                    {
                        return;
                    }
                }
            }
                if (timer == null)
                {
                    Random r = new Random(Game1.random.Next());
                    int val = r.Next(StardewSymphony.Config.MinimumDelayBetweenSongsInMilliseconds, StardewSymphony.Config.MaximumDelayBetweenSongsInMilliseconds + 1);
                    //StardewSymphony.ModMonitor.Log("Music Pack is not playing! Generate a new timer! Delay: "+val.ToString());
                    timer = new Timer(val);
                    timer.Elapsed += onTimerFinished;
                    timer.Enabled = true;
                }
                else
                {
                    timer.Enabled = true;
                    timer.Elapsed += onTimerFinished;
                }
            

        }

        /// <summary>
        /// Selects a new song when the timer delay runs out.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void onTimerFinished(System.Object source, ElapsedEventArgs e)
        {
            if (this.currentMusicPack.isPlaying())
            {
                timer.Enabled = false;
                timer = null;
                return;
            }
            //StardewSymphony.ModMonitor.Log("AHH THE TIMER FINISHED!");
            timer.Enabled = false;
            timer.Elapsed -= onTimerFinished;

            

            selectMusic(SongSpecifics.getCurrentConditionalString());
            timer = null;
        }

        /// <summary>
        /// Plays the song from the currently loaded music pack.
        /// </summary>
        /// <param name="songName"></param>
        public void playSongFromCurrentPack(string songName)
        {
            if (this.currentMusicPack.isNull() == false)
            {
                this.currentMusicPack.playSong(songName);
            }
        }

        /// <summary>
        /// Resumes the paused song from the current music pack.
        /// </summary>
        public void pauseSongFromCurrentPack() {
            if (this.currentMusicPack.isNull() == false)
            {
                this.currentMusicPack.pauseSong();
            }
        }

        /// <summary>
        /// Stops the song from the current music pack.
        /// </summary>
        public void stopSongFromCurrentMusicPack()
        {
            if (this.currentMusicPack.isNull() == false)
            {
                this.currentMusicPack.stopSong();
            }
        }

        /// <summary>
        /// Resumes the song from the current music pack.
        /// </summary>
        public void resumeSongFromCurrentMusicPack()
        {
            if (this.currentMusicPack.isNull() == false)
            {
                this.currentMusicPack.resumeSong();
            }
        }

        /// <summary>
        /// Returns the name of the currently playing song.
        /// </summary>
        /// <returns></returns>
        public string getNameOfCurrentlyPlayingSong()
        {
            if (this.currentMusicPack.isNull() == false)
            {
                return this.currentMusicPack.getNameOfCurrentSong();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Get the information associated with the current music pack.
        /// </summary>
        /// <returns></returns>
        public MusicPackMetaData getMusicPackInformation()
        {
            if (this.currentMusicPack.isNull() == false)
            {
                return this.currentMusicPack.musicPackInformation;
            }
            else return null;
        }

        /// <summary>
        /// Checks to see if the music pack has been loaded into the Music Manager.
        /// </summary>
        /// <param name="nameOfMusicPack"></param>
        /// <returns></returns>
        public bool isMusicPackValid(string nameOfMusicPack)
        {
                return musicPacks.ContainsKey(nameOfMusicPack);
        }

        /// <summary>
        /// Gets the music pack from the
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MusicPack getMusicPack(string name)
        {
            if (isMusicPackValid(name) == false)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("Error, the music pack: " + name + " is not found. Please make sure it is loaded in and try again.");
                return null;
            }
            else
            {

                foreach (var pair in this.musicPacks)
                {
                    if (name == pair.Key) return pair.Value;
                }
                return null; //Needed I suppose to ensure this function compiles.
            }
        }

        /// <summary>
        /// Iterates across all music packs and determines which music packs contain songs that can be played right now.
        /// </summary>
        /// <param name="songListKey"></param>
        /// <returns></returns>
        public Dictionary<MusicPack,List<Song>> getListOfApplicableMusicPacks(string songListKey)
        {
            Dictionary<MusicPack, List<Song>> listOfValidDictionaries = new Dictionary<MusicPack, List<Song>>();
            foreach(var v in this.musicPacks)
            {
                try
                {
                    var songList = v.Value.songInformation.getSongList(songListKey).Value;
                    if (songList.Count > 0)
                    {
                        listOfValidDictionaries.Add(v.Value, songList);
                    }
                }
                catch(Exception err)
                {
                    err.ToString();
                }
            }
            return listOfValidDictionaries;
        }

        public Dictionary<MusicPack, List<Song>> getListOfApplicableMusicPacksForFestivals()
        {
            Dictionary<MusicPack, List<Song>> listOfValidDictionaries = new Dictionary<MusicPack, List<Song>>();
            foreach (var v in this.musicPacks)
            {
                try
                {
                    var songList = v.Value.songInformation.getFestivalMusic();
                    if (songList.Count > 0)
                    {
                        listOfValidDictionaries.Add(v.Value, songList);
                    }
                }
                catch (Exception err)
                {
                    err.ToString();
                }
            }
            return listOfValidDictionaries;
        }


        public Dictionary<MusicPack, List<Song>> getListOfApplicableMusicPacksForEvents()
        {
            Dictionary<MusicPack, List<Song>> listOfValidDictionaries = new Dictionary<MusicPack, List<Song>>();
            foreach (var v in this.musicPacks)
            {
                try
                {
                    var songList = v.Value.songInformation.getEventMusic();
                    if (songList.Count > 0)
                    {
                        listOfValidDictionaries.Add(v.Value, songList);
                    }
                }
                catch (Exception err)
                {
                    err.ToString();
                }
            }
            return listOfValidDictionaries;
        }



        public void selectMenuMusic(string songListKey)
        {
            //Nullify the timer when new music is selected.
            if (this.timer != null)
            {
                this.timer = null;
            }

            var listOfValidMusicPacks = getListOfApplicableMusicPacks(songListKey);

            if (listOfValidMusicPacks.Count == 0) return;


            int randInt = packSelector.Next(0, listOfValidMusicPacks.Count - 1);

            var musicPackPair = listOfValidMusicPacks.ElementAt(randInt);


            //used to swap the music packs and stop the last playing song.
            this.swapMusicPacks(musicPackPair.Key.musicPackInformation.name);

            int randInt2 = songSelector.Next(0, musicPackPair.Value.Count);


            var songName = musicPackPair.Value.ElementAt(randInt2);

            this.currentMusicPack.playSong(songName.name);

            StardewSymphony.menuChangedMusic = true;

        }
        /// <summary>
        /// Selects the actual song to be played right now based off of the selector key. The selector key should be called when the player's location changes.
        /// </summary>
        /// <param name="songListKey"></param>
        public void selectMusic(string songListKey)
        {
            //Nullify the timer when new music is selected.
            if (this.timer != null)
            {
                this.timer = null;
            }
           
            var listOfValidMusicPacks = getListOfApplicableMusicPacks(songListKey);
          
            string subKey = songListKey;
            //Try to get more specific.

           

            //This chunk is to determine song specifics for location.
            while (listOfValidMusicPacks.Count == 0)
            {
                if (subKey.Length == 0) break;
                string[] subList=  subKey.Split(SongSpecifics.seperator);
                if (subList.Length == 0) break; //Because things would go bad otherwise.
                subKey = "";
                for(int i = 0; i < subList.Length-1; i++)
                {
                    subKey += subList[i];
                    if (i != subList.Length - 2)
                    {
                        subKey += SongSpecifics.seperator;
                    }
                }
                if (subKey == "") break;
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log(subKey,StardewModdingAPI.LogLevel.Alert);
                listOfValidMusicPacks = getListOfApplicableMusicPacks(subKey);
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
                    {
                        subKey += SongSpecifics.seperator;
                    }
                }
                if (string.IsNullOrEmpty(subKey))
                {

                    bool f = checkGenericMusic(songListKey);

                    if (f == false)
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + songListKey + ".2 Are you sure you did this properly?");
                        StardewSymphony.menuChangedMusic = false;
                        return;
                    }
                }
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log(subKey, StardewModdingAPI.LogLevel.Alert);
                listOfValidMusicPacks = getListOfApplicableMusicPacks(subKey);
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
                        {
                            subKey += SongSpecifics.seperator;
                        }
                    }
                    if (subKey == "") break;
                    if(StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log(subKey, StardewModdingAPI.LogLevel.Alert);
                    listOfValidMusicPacks = getListOfApplicableMusicPacks(subKey);
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

                bool f= checkGenericMusic(songListKey);

                if (f == false)
                {
                    //No valid songs to play at this time.
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + songListKey + ".7 Are you sure you did this properly?");
                    StardewSymphony.menuChangedMusic = false;
                    return;
                }
            }


            string[] sizeList = subKey.Split(SongSpecifics.seperator);

            if (this.currentMusicPack != null) {
                //If I am trying to play a generic song and a generic song is playing don't change the music.
                //If I am trying to play a generic song and a non-generic song is playing, then play my generic song since I don't want to play the specific music anymore.
                if (sizeList.Length < 3 && (this.currentMusicPack.isPlaying() && this.lastSongWasLocationSpecific==false))
                {
                    if (StardewSymphony.Config.EnableDebugLog)
                        StardewSymphony.ModMonitor.Log("Non specific music change detected. Not going to change the music this time");
                    return;
                }
            }

            if (sizeList.Length < 3) this.lastSongWasLocationSpecific = false;
            else this.lastSongWasLocationSpecific = true;

            //If there is a valid key for the place/time/event/festival I am at, play it!

            int randInt = packSelector.Next(0, listOfValidMusicPacks.Count-1);

            var musicPackPair = listOfValidMusicPacks.ElementAt(randInt);


            //used to swap the music packs and stop the last playing song.
            this.swapMusicPacks(musicPackPair.Key.musicPackInformation.name);

            int randInt2 = songSelector.Next(0, musicPackPair.Value.Count);


            var songName = musicPackPair.Value.ElementAt(randInt2);

            this.currentMusicPack.playSong(songName.name);
        }


        public Dictionary<MusicPack,List<Song>> getLocationSpecificMusic()
        {
            Dictionary<MusicPack, List<Song>> listOfValidDictionaries = new Dictionary<MusicPack, List<Song>>();
            //StardewSymphony.ModMonitor.Log(SongSpecifics.getCurrentConditionalString(true));

            foreach (var v in this.musicPacks)
            {
                try
                {
                    var songList = v.Value.songInformation.getSongList(SongSpecifics.getCurrentConditionalString(true)).Value;
                    if (songList == null) return null;
                    if (songList.Count > 0)
                    {
                        listOfValidDictionaries.Add(v.Value, songList);
                    }
                }
                catch (Exception err)
                {
                    err.ToString();
                }
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
                    var listOfFestivalPacks = this.getListOfApplicableMusicPacksForFestivals();
                    if (listOfFestivalPacks.Count > 0)
                    {
                        int randFestivalPack = packSelector.Next(0, listOfFestivalPacks.Count - 1);

                        var festivalMusicPackPair = listOfFestivalPacks.ElementAt(randFestivalPack);

                        //used to swap the music packs and stop the last playing song.
                        this.swapMusicPacks(festivalMusicPackPair.Key.musicPackInformation.name);

                        int randFestivalPack2 = songSelector.Next(0, festivalMusicPackPair.Value.Count);

                        var festivalSongName = festivalMusicPackPair.Value.ElementAt(randFestivalPack2);

                        this.currentMusicPack.playSong(festivalSongName.name);
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
                    var listOfEventPacks = this.getListOfApplicableMusicPacksForEvents();
                    if (listOfEventPacks.Count > 0)
                    {
                        int randEventPack = packSelector.Next(0, listOfEventPacks.Count - 1);

                        var eventMusicPackPair = listOfEventPacks.ElementAt(randEventPack);

                        //used to swap the music packs and stop the last playing song.
                        this.swapMusicPacks(eventMusicPackPair.Key.musicPackInformation.name);

                        int randEventPack2 = songSelector.Next(0, eventMusicPackPair.Value.Count);

                        var eventSongName = eventMusicPackPair.Value.ElementAt(randEventPack2);

                        this.currentMusicPack.playSong(eventSongName.name);
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
                    int randFestivalPack = packSelector.Next(0, listOfLocationPacks.Count - 1);

                    var locationMusicPackPair = listOfLocationPacks.ElementAt(randFestivalPack);

                    //used to swap the music packs and stop the last playing song.
                    this.swapMusicPacks(locationMusicPackPair.Key.musicPackInformation.name);

                    int randLocPack2 = songSelector.Next(0, locationMusicPackPair.Value.Count);

                    var songName = locationMusicPackPair.Value.ElementAt(randLocPack2);

                    this.currentMusicPack.playSong(songName.name);
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
            return false;
        }

        /// <summary>
        /// Adds a valid xwb music pack to the list of music packs available.
        /// </summary>
        /// <param name="musicPack"></param>
        /// <param name="displayLogInformation">Whether or not to display the process to the console. Will include information from the pack's metadata. Default:False</param>
        /// <param name="xwbMusicPack">If displayLogInformation is also true this will display the name of all of the songs in the music pack when it is added in.</param>
        public void addMusicPack(MusicPack musicPack,bool displayLogInformation=false,bool displaySongs=false)
        {
            if (displayLogInformation == true)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                {
                    StardewSymphony.ModMonitor.Log("Adding a new music pack!");


                    //StardewSymphony.ModMonitor.Log("    Location:" + musicPack.shortenedDirectory);
                    StardewSymphony.ModMonitor.Log("    Name:" + musicPack.musicPackInformation.name);
                    StardewSymphony.ModMonitor.Log("    Author:" + musicPack.musicPackInformation.author);
                    StardewSymphony.ModMonitor.Log("    Description:" + musicPack.musicPackInformation.description);
                    StardewSymphony.ModMonitor.Log("    Version Info:" + musicPack.musicPackInformation.versionInfo);
                    StardewSymphony.ModMonitor.Log("    Song List:");
                }

                if (displaySongs == true)
                {
                    foreach(var song in musicPack.songInformation.listOfSongsWithoutTriggers)
                    {
                        if (StardewSymphony.Config.EnableDebugLog)
                            StardewSymphony.ModMonitor.Log("        " + song.name);
                    }
                }
            }

            this.musicPacks.Add(musicPack.musicPackInformation.name,musicPack);
        }

        /// <summary>
        /// Initializes all of the potential key triggers for playing songs.
        /// </summary>
        public void initializeSeasonalMusic()
        {
            foreach(var pack in this.musicPacks)
            {
                pack.Value.songInformation.initializeSeasonalMusic();
            }
        }

        /// <summary>
        /// Initializes all of the potential key triggers for playing songs.
        /// </summary>
        public void initializeMenuMusic()
        {
            foreach (var pack in this.musicPacks)
            {
                pack.Value.songInformation.initializeMenuMusic();
            }
        }

        /// <summary>
        /// Initializes all of the potential key triggers for playing songs.
        /// </summary>
        public void initializeFestivalMusic()
        {
            foreach (var pack in this.musicPacks)
            {
                pack.Value.songInformation.initializeFestivalMusic();
            }
        }

        /// <summary>
        /// Initializes all of the potential key triggers for playing songs.
        /// </summary>
        public void initializeEventMusic()
        {
            foreach (var pack in this.musicPacks)
            {
                pack.Value.songInformation.initializeEventMusic();
            }
        }

        /// <summary>
        /// Play a random song from a given music pack.
        /// </summary>
        /// <param name="musicPackName"></param>
        public void playRandomSongFromPack(string musicPackName)
        {
            this.musicPacks.TryGetValue(musicPackName, out MusicPack musicPack);
            if (this.currentMusicPack != null) this.currentMusicPack.stopSong();
            musicPack.playRandomSong();
            this.currentMusicPack = musicPack;
        }
    }
}
