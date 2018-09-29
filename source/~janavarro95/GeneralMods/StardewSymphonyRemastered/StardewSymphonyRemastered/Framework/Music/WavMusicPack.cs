using Microsoft.Xna.Framework;
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
    /// TODO: Make this class
    /// </summary>
    public class WavMusicPack : MusicPack
    {
        public Song currentSong;
        public string songsDirectory;

        /// <summary>
        /// Used to actually play the song.
        /// </summary>
        DynamicSoundEffectInstance dynamicSound;
        /// <summary>
        /// Used to keep track of where in the song we are.
        /// </summary>
        int position;
        /// <summary>
        /// ???
        /// </summary>
        int count;
        /// <summary>
        /// Used to store the info for the song.
        /// </summary>
        byte[] byteArray;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="directoryToMusicPack"></param>
        public WavMusicPack(string directoryToMusicPack)
        {
            this.directory = directoryToMusicPack;
            this.setModDirectoryFromFullDirectory();
            this.songsDirectory = Path.Combine(this.directory, "Songs");
            this.songInformation = new SongSpecifics();
            this.musicPackInformation = MusicPackMetaData.readFromJson(directoryToMusicPack);

            if (this.musicPackInformation == null)
            {
                //StardewSymphony.ModMonitor.Log("Error: MusicPackInformation.json not found at: " + directoryToMusicPack + ". Blank information will be put in place.", StardewModdingAPI.LogLevel.Warn);
                //this.musicPackInformation = new MusicPackMetaData("???", "???", "", "0.0.0","");
            }
            StardewSymphony.ModMonitor.Log(this.musicPackInformation.name.ToString());
            this.loadMusicFiles();
        }

        /// <summary>
        /// A shortened directory name for display purposes.
        /// </summary>
        /// <returns></returns>
        public override void setModDirectoryFromFullDirectory()
        {
            string[] spliter = this.directory.Split(Path.DirectorySeparatorChar);
            string directoryLocation = "";
            for (int i = spliter.Length - 6; i < spliter.Length; i++)
            {
                directoryLocation += spliter[i];

                if (i != spliter.Length - 1)
                {
                    directoryLocation += Path.DirectorySeparatorChar;
                }
            }
            this.shortenedDirectory = directoryLocation;
        }

        /// <summary>
        /// Load a wav file into the stream to be played.
        /// </summary>
        public void LoadWavFromFileToStream(string p)
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            string file =p;
            System.IO.Stream waveFileStream = File.OpenRead(file); //TitleContainer.OpenStream(file);
          
            BinaryReader reader = new BinaryReader(waveFileStream);

            int chunkID = reader.ReadInt32();
            int fileSize = reader.ReadInt32();
            int riffType = reader.ReadInt32();
            int fmtID = reader.ReadInt32();
            int fmtSize = reader.ReadInt32();
            int fmtCode = reader.ReadInt16();
            int channels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            int fmtAvgBPS = reader.ReadInt32();
            int fmtBlockAlign = reader.ReadInt16();
            int bitDepth = reader.ReadInt16();

            if (fmtSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            int dataID = reader.ReadInt32();
            int dataSize = reader.ReadInt32();

            byteArray = reader.ReadBytes(dataSize);


            dynamicSound = new DynamicSoundEffectInstance(sampleRate, (AudioChannels)channels);
            count = dynamicSound.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(10000));

            dynamicSound.BufferNeeded += new EventHandler<EventArgs>(DynamicSound_BufferNeeded);
            this.currentSong = new Song(p);
        }

        void DynamicSound_BufferNeeded(object sender, EventArgs e)
        {
            //StardewSymphony.ModMonitor.Log(byteArray.Length.ToString());
            //StardewSymphony.ModMonitor.Log(position.ToString());
            //StardewSymphony.ModMonitor.Log(count.ToString());
            try
            {
                dynamicSound.SubmitBuffer(byteArray, position, count);
            }
            catch(Exception err)
            {
                StardewSymphony.ModMonitor.Log(err.ToString(), StardewModdingAPI.LogLevel.Error);
            }
            
            //dynamicSound.SubmitBuffer(byteArray);
            //dynamicSound.SubmitBuffer(byteArray, position + count / 2, count / 2);

            position += count;
            if (position + count > byteArray.Length)
            {
                position = 0;
            }
        }


        /// <summary>
        /// Returns the name of the currently playing song.
        /// </summary>
        /// <returns></returns>
        public override string getNameOfCurrentSong()
        {
            if (this.currentSong == null) return "";
            return this.currentSong.name;
        }

        /// <summary>
        /// Load in the music files from the pack's respective Directory/Songs folder. Typically Content/Music/Wav/FolderName/Songs
        /// </summary>
        public override void loadMusicFiles()
        {
            string[] wavFiles = Directory.GetFiles(this.songsDirectory, "*.wav");
            List<Song> listOfSongs = new List<Song>();
            foreach(var wav in wavFiles)
            {
                Song song = new Song(wav);
                listOfSongs.Add(song);
            }
            this.songInformation.listOfSongsWithoutTriggers = listOfSongs;
        }

        /// <summary>
        /// Used to pause the current song.
        /// </summary>
        public override void pauseSong()
        {
            if (dynamicSound != null) dynamicSound.Pause();
        }

        /// <summary>
        /// Used to play a song.
        /// </summary>
        /// <param name="name"></param>
        public override void playSong(string name)
        {
            string pathToSong = getSongPathFromName(name);
            LoadWavFromFileToStream(pathToSong);
            dynamicSound.Play();
        }

        public override void playRandomSong()
        {
            Random r = Game1.random;
            int value=r.Next(0, this.songInformation.listOfSongsWithoutTriggers.Count);
            Song s = this.songInformation.listOfSongsWithoutTriggers.ElementAt(value);
            this.swapSong(s.name);
        }

        /// <summary>
        /// Used to resume the currently playing song.
        /// </summary>
        public override void resumeSong()
        {
            if (dynamicSound == null) return;
            dynamicSound.Resume();
        }

        /// <summary>
        /// Used to stop the currently playing song.
        /// </summary>
        public override void stopSong()
        {
            if (Game1.currentSong != null) Game1.currentSong.Stop(AudioStopOptions.Immediate);
            if (this.currentSong == null) return;
            if (dynamicSound != null)
            {
                dynamicSound.Stop(true);
                dynamicSound.BufferNeeded -= new EventHandler<EventArgs>(DynamicSound_BufferNeeded);
                dynamicSound = null;
                this.currentSong = null;
                position = 0;
                count = 0;
                byteArray = new byte[0];
            }
        }

        /// <summary>
        /// Used to change from one playing song to another;
        /// </summary>
        /// <param name="songName"></param>
        public override void swapSong(string songName)
        {
            this.stopSong();
            this.playSong(songName);
        }

        /// <summary>
        /// Get the son's name from the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string getSongNameFromPath(string path)
        {
            foreach(var song in this.songInformation.listOfSongsWithoutTriggers)
            {
                if (song.getPathToSong()== path) return song.name;
            }
            return "";
        }

        /// <summary>
        /// Gets the song's path that shares the same name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string getSongPathFromName(string name)
        {
            foreach (var song in this.songInformation.listOfSongsWithoutTriggers)
            {
                if (song.name == name) return song.getPathToSong();
            }
            return "";
        }

        public override bool isPlaying()
        {
            if (this.dynamicSound == null) return false;
            if (this.dynamicSound.State == SoundState.Playing) return true;
            else return false;
        }
    }
}
