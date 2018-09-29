using Microsoft.Xna.Framework.Audio;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewSymphonyRemastered.Framework
{
    /// <summary>
    /// TODO: Make this work and add in overrided functions.
    /// </summary>
   public class XACTMusicPack: MusicPack
    {
        public WaveBank WaveBank;
        public ISoundBank SoundBank;

        public Cue currentCue;

        public string WaveBankPath;
        public string SoundBankPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="directoryToXwb"></param>
        /// <param name="pathToWaveBank"></param>
        /// <param name="pathToSoundBank"></param>
        public XACTMusicPack(string directoryToXwb,string pathToWaveBank,string pathToSoundBank)
        {
            this.directory = directoryToXwb;
            this.WaveBankPath = pathToWaveBank;
            this.SoundBankPath = pathToSoundBank;
            this.setModDirectoryFromFullDirectory();
            this.songInformation = new SongSpecifics();
            this.currentCue = null;
            this.musicPackInformation = MusicPackMetaData.readFromJson(directoryToXwb);
            if (this.musicPackInformation == null)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("Error: MusicPackInformation.json not found at: " + directoryToXwb + ". Blank information will be put in place.",StardewModdingAPI.LogLevel.Warn);
                this.musicPackInformation = new MusicPackMetaData("???","???","","0.0.0","");
            }

            this.WaveBank = new WaveBank(Game1.audioEngine, this.WaveBankPath);
            this.SoundBank = (ISoundBank)new SoundBankWrapper(new SoundBank(Game1.audioEngine, this.SoundBankPath));
            this.loadMusicFiles();
        }

        /// <summary>
        /// Load all of the generic music file names into the music pack's information.
        /// </summary>
        public override void loadMusicFiles()
        {

            var listOfSongStrings = StardewSymphonyRemastered.Framework.MusicHexProcessor.ProcessSongNamesFromHex(this, StardewSymphony.Reset, this.SoundBankPath);

            List<Song> listofSongs = new List<Song>();
            foreach(var songname in listOfSongStrings)
            {
                Song song = new Song(this.WaveBankPath, songname);
                listofSongs.Add(song);
            }

            this.songInformation.listOfSongsWithoutTriggers = listofSongs;            
               
        }

        public override void playRandomSong()
        {
            Random r = new Random();
            int value = r.Next(0,this.songInformation.listOfSongsWithoutTriggers.Count);
            Song s = this.songInformation.listOfSongsWithoutTriggers.ElementAt(value);
            this.swapSong(s.name);
        }

        /// <summary>
        /// Get the cue from the list of songs.
        /// </summary>
        /// <param name="name">The name of the song to get.</param>
        /// <returns></returns>
        private Cue getCue(string name) {
            if (this.songInformation.isSongInList(name) == false)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log("Error! The song " + name + " could not be found in music pack " + this.musicPackInformation.name+". Please ensure that this song is part of this music pack located at: "+ this.WaveBankPath+ " or contact the music pack author: "+this.musicPackInformation.author,StardewModdingAPI.LogLevel.Error);
                return null;
            }
            else
            {
                return this.SoundBank.GetCue(name);
            }
        }

        /// <summary>
        /// Play a song.
        /// </summary>
        /// <param name="name">The name of the song to play.</param>
        public override void playSong(string name)
        {
            this.currentCue = this.getCue(name);
            if (this.currentCue == null)
            {
                return; //getCue will throw the error message.
            }
            Game1.waveBank = this.WaveBank;
            Game1.soundBank = this.SoundBank;
            this.currentCue.Play();
            StardewSymphony.Reset();
        }

        /// <summary>
        /// Pause the currently playing song.
        /// </summary>
        /// <param name="name">Pause the current song that is playing.</param>
        public override void pauseSong()
        {
            if (this.currentCue == null) return;
            else
            {
                Game1.waveBank = this.WaveBank;
                Game1.soundBank = this.SoundBank;
                this.currentCue.Pause();
                StardewSymphony.Reset();
            }
        }

        /// <summary>
        /// Resume playing the current set cue.
        /// </summary>
        /// <param name="name"></param>
        public override void resumeSong()
        {
            if (this.currentCue == null) return;
            else
            {
                Game1.waveBank = this.WaveBank;
                Game1.soundBank = this.SoundBank;
                this.currentCue.Resume();
                StardewSymphony.Reset();
            }
        }

        /// <summary>
        /// Stops the currently playing song and nulls the current song.
        /// </summary>
        public override void stopSong()
        {
            if(Game1.currentSong!=null) Game1.currentSong.Stop(AudioStopOptions.Immediate);
            if (this.currentCue == null) return;
            else
            {
                Game1.waveBank = this.WaveBank;
                Game1.soundBank = this.SoundBank;
                this.currentCue.Stop(AudioStopOptions.Immediate);
                StardewSymphony.Reset();
                this.currentCue = null;
            }
        }

        /// <summary>
        /// Stops the currently playing song and starts playing a new song.
        /// </summary>
        /// <param name="newSongName">The name of the new song to play.</param>
        public override void swapSong(string newSongName)
        {
            this.stopSong();
            this.playSong(newSongName);
        }

        /// <summary>
        /// Returns the name of the currently playing song.
        /// </summary>
        /// <returns></returns>
        public override string getNameOfCurrentSong()
        {
            if (this.currentCue == null) return "";
            return this.currentCue.Name;
        }

        /// <summary>
        /// Returns a shortened directory name for display purposes.
        /// </summary>
        /// <returns></returns>
        public override void setModDirectoryFromFullDirectory()
        {
            string[] spliter = this.WaveBankPath.Split(Path.DirectorySeparatorChar);
            string directoryLocation="";
            for (int i = spliter.Length - 5; i < spliter.Length; i++)
            {
                directoryLocation += spliter[i];

                if (i != spliter.Length - 1)
                {
                    directoryLocation += Path.DirectorySeparatorChar;
                }
            }
            this.shortenedDirectory = directoryLocation;
        }

        public override bool isPlaying()
        {
            if (this.currentCue == null) return false;
            return this.currentCue.IsPlaying;
        }

    }
}
