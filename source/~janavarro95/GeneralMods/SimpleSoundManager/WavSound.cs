using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSoundManager
{
    class WavSound : Sound
    {

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

        public List<string> sounds;

        public string path;



        /// <summary>
        /// Get a raw disk path to the wav file.
        /// </summary>
        /// <param name="pathToWavFile"></param>
        public WavSound(string pathToWavFile)
        {
            this.path = pathToWavFile;
            LoadWavFromFileToStream();
        }

        /// <summary>
        /// A constructor that takes a mod helper and a relative path to a wav file.
        /// </summary>
        /// <param name="modHelper"></param>
        /// <param name="pathInModDirectory"></param>
        public WavSound(IModHelper modHelper, string pathInModDirectory)
        {
            string path = Path.Combine(modHelper.DirectoryPath, pathInModDirectory);
            this.path = path;
        }

        /// <summary>
        /// Constructor that is more flexible than typing an absolute path.
        /// </summary>
        /// <param name="modHelper">The mod helper for the mod you wish to use to load the music files from.</param>
        /// <param name="pathPieces">The list of folders and files that make up a complete path.</param>
        public WavSound(IModHelper modHelper, List<string> pathPieces)
        {
            string s = modHelper.DirectoryPath;
            foreach(var str in pathPieces)
            {
                s = Path.Combine(s, str);
            }
            this.path = s;
        }

        /// <summary>
        /// Loads the .wav file from disk and plays it.
        /// </summary>
        public void LoadWavFromFileToStream()
        {
            // Create a new SpriteBatch, which can be used to draw textures.

            string file = this.path;
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

        }

        void DynamicSound_BufferNeeded(object sender, EventArgs e)
        {
            try
            {
                dynamicSound.SubmitBuffer(byteArray, position, count);
            }
            catch (Exception err)
            {

            }

            position += count;
            if (position + count > byteArray.Length)
            {
                position = 0;
            }
        }

        /// <summary>
        /// Used to pause the current song.
        /// </summary>
        public void pause()
        {
            if (dynamicSound != null) dynamicSound.Pause();
        }

        /// <summary>
        /// Used to play a song.
        /// </summary>
        /// <param name="name"></param>
        public void play()
        {
            if (this.isPlaying() == true) return;
            dynamicSound.BufferNeeded += new EventHandler<EventArgs>(DynamicSound_BufferNeeded);
            dynamicSound.Play();
        }


        /// <summary>
        /// Used to resume the currently playing song.
        /// </summary>
        public void resume()
        {
            if (dynamicSound == null) return;
            dynamicSound.Resume();
        }

        /// <summary>
        /// Used to stop the currently playing song.
        /// </summary>
        public void stop()
        {

            if (dynamicSound != null)
            {
                dynamicSound.Stop(true);
                dynamicSound.BufferNeeded -= new EventHandler<EventArgs>(DynamicSound_BufferNeeded);
                position = 0;
                count = 0;
                byteArray = new byte[0];
            }
        }

        /// <summary>
        /// Used to change from one playing song to another;
        /// </summary>
        /// <param name="songName"></param>
        public void swap(string pathToNewWavFile)
        {
            this.stop();
            this.path = pathToNewWavFile;
            this.play();
        }

        /// <summary>
        /// Checks if the song is currently playing.
        /// </summary>
        /// <returns></returns>
        public bool isPlaying()
        {
            if (this.dynamicSound == null) return false;
            if (this.dynamicSound.State == SoundState.Playing) return true;
            else return false;
        }

        /// <summary>
        /// Checks if the song is currently paused.
        /// </summary>
        /// <returns></returns>
        public bool isPaused()
        {
            if (this.dynamicSound == null) return false;
            if (this.dynamicSound.State == SoundState.Paused) return true;
            else return false;
        }

        /// <summary>
        /// Checks if the song is currently stopped.
        /// </summary>
        /// <returns></returns>
        public bool isStopped()
        {
            if (this.dynamicSound == null) return false;
            if (this.dynamicSound.State == SoundState.Stopped) return true;
            else return false;
        }

        public Sound clone()
        {
            return new WavSound(this.path);
        }

        public void restart()
        {
            this.stop();
            this.play();
        }

    }
}
