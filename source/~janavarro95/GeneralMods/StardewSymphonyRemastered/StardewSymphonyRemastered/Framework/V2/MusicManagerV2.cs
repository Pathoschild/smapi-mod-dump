/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using StardewValley;

namespace StardewSymphonyRemastered.Framework.V2
{
    /// <summary>Manages all music for the mod.</summary>
    public class MusicManagerV2
    {
        /*********
        ** Fields
        *********/
        /// <summary>The RNG used to select music packs and songs.</summary>
        private readonly Random Random = new Random();

        /// <summary>The delay timer between songs.</summary>
        private readonly Timer Timer = new Timer();

        /*********
        ** Accessors
        *********/
        /// <summary>The loaded music packs.</summary>
        public IDictionary<string, MusicPackV2> MusicPacks { get; } = new Dictionary<string, MusicPackV2>();

        /// <summary>The current music pack playing music, if any.</summary>
        public MusicPackV2 CurrentMusicPack { get; private set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public MusicManagerV2()
        {
            this.Timer.Elapsed += this.OnTimerFinished;
        }

        /// <summary>Swap between referenced music packs and stop the current song.</summary>
        /// <param name="nameOfNewMusicPack">The name of the new music pack to select.</param>
        public void SwapMusicPacks(string nameOfNewMusicPack)
        {
            if (!this.MusicPacks.TryGetValue(nameOfNewMusicPack, out MusicPackV2 musicPack))
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
                this.selectMusic(SongSpecificsV2.getCurrentConditionalString());
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
        public Dictionary<MusicPackV2, List<string>> GetApplicableMusicPacks(string songListKey)
        {
            Dictionary<MusicPackV2, List<string>> listOfValidDictionaries = new Dictionary<MusicPackV2, List<string>>();
            foreach (var v in this.MusicPacks)
            {
                try
                {
                    var songList = v.Value.SongInformation.getSongList(songListKey);
                    if (songList.Count > 0)
                        listOfValidDictionaries.Add(v.Value, songList);
                }
                catch { }
            }
            return listOfValidDictionaries;
        }
        /*
        public Dictionary<MusicPackV2, List<string>> GetListOfApplicableMusicPacksForFestivals()
        {
            Dictionary<MusicPackV2, List<string>> listOfValidDictionaries = new Dictionary<MusicPackV2, List<string>>();
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

        public Dictionary<MusicPackV2, List<string>> GetListOfApplicableMusicPacksForEvents()
        {
            Dictionary<MusicPackV2, List<string>> listOfValidDictionaries = new Dictionary<MusicPackV2, List<string>>();
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
        */
        /// <summary>
        /// Get a list of applicable songs to play in the given menu and find one to play.
        /// </summary>
        /// <param name="songListKey"></param>
        public bool SelectMenuMusic(string songListKey)
        {

            if (this.selectMusic(songListKey, false))
            {
                StardewSymphony.menuChangedMusic = true;
                return true;
            }
            StardewSymphony.menuChangedMusic = false;
            return false;

        }
        /// <summary>Select the actual song to be played right now based on the selector key. The selector key should be called when the player's location changes.</summary>
        public bool selectMusic(string songListKey, bool warpCheck = false)
        {

            if (this.CurrentMusicPack != null)
            {
                if (this.CurrentMusicPack.IsPlaying() && StardewSymphony.Config.WaitForSongToFinishBeforeMusicSwap)
                {
                    StardewSymphony.ModMonitor.Log("Waiting for music to finish before playing another song.");
                    return false;
                }
            }
            if (warpCheck == true)
            {
                if (StardewSymphony.Config.LocationsToIgnoreWarpMusicChange.ContainsKey(Game1.player.currentLocation.Name))
                {
                    if (StardewSymphony.Config.LocationsToIgnoreWarpMusicChange[Game1.player.currentLocation.Name] == true)
                    {
                        return false;
                    }
                }
                return false;
            }


            //Prevent generic song changes when running about.

            SongConditionals conditional = new SongConditionals(songListKey);

            // stop timer timer when music is selected
            this.Timer.Enabled = false;

            // get applicable music packs
            var listOfValidMusicPacks = this.GetApplicableMusicPacks(songListKey);

            //If the list of valid packs are 0, check if I'm currently at an event or festival or get some location specific music and try to play a generalized song from there.
            if (listOfValidMusicPacks.Count == 0)
            {
                //No valid songs to play at this time.
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("Error: There are no songs to play across any music pack for the song key: " + songListKey + ".7 Are you sure you did this properly?");
                StardewSymphony.menuChangedMusic = false;
                return false;


            }
            //All music packs that have songs have been taken into acount.
            //Check all songs in all music packs to see if it can be played here.

            List<KeyValuePair<MusicPackV2, string>> packSongNames = new List<KeyValuePair<MusicPackV2, string>>();
            List<KeyValuePair<MusicPackV2, string>> locpackSongNames = new List<KeyValuePair<MusicPackV2, string>>();
            foreach (var ok in listOfValidMusicPacks)
            {
                foreach (SongInformation song in ok.Key.SongInformation.songs.Values)
                {
                    if (song.canBePlayed(songListKey))
                    {
                        foreach (SongConditionals con in song.songConditionals.Values)
                        {
                            if (con.isLocationSpecific() == false)
                            {
                                //If the key is generic skip over it.
                                if (con.isKeyGeneric())
                                {
                                    if (this.CurrentMusicPack != null)
                                    {
                                        if (this.CurrentMusicPack.IsPlaying()) continue;
                                    }
                                }
                                //If I have warped and the key only is to be played when time changes prevent a new song from playing.
                                //If the key is more specific (I.E has a location associated with it) then music will change.
                                if (warpCheck == true && con.isTimeSpecific())
                                {
                                    if (this.CurrentMusicPack != null)
                                    {
                                        if (this.CurrentMusicPack.IsPlaying()) continue;
                                    }
                                }
                            }
                            else
                            {
                                locpackSongNames.Add(new KeyValuePair<MusicPackV2, string>(ok.Key, song.name));
                            }
                        }
                        packSongNames.Add(new KeyValuePair<MusicPackV2, string>(ok.Key, song.name));
                    }
                }
            }

            //If there is a valid key for the place/time/event/festival I am at, play it!

            int randInt = this.Random.Next(0, packSongNames.Count - 1);

            var musicPackPair = packSongNames.ElementAt(randInt);

            /*
            //Check to see if the only location song was the one I was just playing. If so ignore it and chose more generic music.
            if (locpackSongNames.Count == 1)
            {
                if (musicPackPair.Key == locpackSongNames[0].Key)
                {
                    if (musicPackPair.Value == locpackSongNames[0].Value)
                    {
                        //Don't care
                    }
                }
            }
            //If not the try to chose another location specific song.
            else
            {
                packSongNames = locpackSongNames;
                randInt = this.Random.Next(0, packSongNames.Count - 1);
                musicPackPair = packSongNames.ElementAt(randInt);
            }*/

            while (true)
            {
                if (this.CurrentMusicPack != null)
                {
                    if (packSongNames.Count == 0) return false;
                    if (musicPackPair.Key.Name == this.CurrentMusicPack.Name)
                    {
                        if (musicPackPair.Value == this.CurrentMusicPack.CurrentSongName)
                        {
                            if (packSongNames.Count == 1) return false;
                            else
                            {
                                //Used to eliminate chosing the same song from the same pack.
                                packSongNames.Remove(musicPackPair);
                                randInt = this.Random.Next(0, packSongNames.Count - 1);
                                musicPackPair = packSongNames.ElementAt(randInt);
                            }
                        }
                        else
                        {
                            //Different song from same pack
                            break;
                        }
                    }
                    else
                    {
                        //Different pack so probably different song.
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            //used to swap the music packs and stop the last playing song.
            this.SwapMusicPacks(musicPackPair.Key.Name);
            string songName = musicPackPair.Value;
            this.CurrentMusicPack.PlaySong(songName);
            return true;
        }


        public Dictionary<MusicPackV2, List<string>> getLocationSpecificMusic()
        {
            Dictionary<MusicPackV2, List<string>> listOfValidDictionaries = new Dictionary<MusicPackV2, List<string>>();
            //StardewSymphony.ModMonitor.Log(SongSpecificsV2.getCurrentConditionalString(true));

            foreach (var v in this.MusicPacks)
            {
                try
                {
                    var songList = v.Value.SongInformation.getSongList(SongSpecificsV2.getCurrentConditionalString(true));
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

        /// <summary>Adds a valid xwb music pack to the list of music packs available.</summary>
        /// <param name="musicPack">The music pack to add.</param>
        /// <param name="displayLogInformation">Whether or not to display the process to the console. Will include information from the pack's metadata. Default:False</param>
        /// <param name="displaySongs">If displayLogInformation is also true this will display the name of all of the songs in the music pack when it is added in.</param>
        public void addMusicPack(MusicPackV2 musicPack, bool displayLogInformation = false, bool displaySongs = false)
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
                    foreach (string song in musicPack.SongInformation.songs.Keys)
                        StardewSymphony.ModMonitor.Log($"        {song}");
                }
            }

            this.MusicPacks.Add(musicPack.Name, musicPack);
        }
    }
}
