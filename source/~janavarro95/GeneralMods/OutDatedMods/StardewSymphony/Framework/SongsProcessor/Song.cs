using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.StardewSymphony.Framework.SongsProcessor
{
    public class Song
    {
        public string name;
        public string fileLocation;
        public bool existsInMusicXNBFile;


        public DynamicSoundEffectInstance dynamicSound;
        public int position;
        public int count;
        public byte[] byteArray;

        public EventHandler<EventArgs> bufferHandler;

        public SongsProcessor.SongState songState;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Name">Name of the song.</param>
        /// <param name="FileLocation">Path to the song.</param>
        /// <param name="ExistsInXNBWavePack">Checks if this song comes from a .xwb file or a .wav file.</param>
        public Song(string Name, string FileLocation, bool ExistsInXNBWavePack)
        {
            this.name = Name;
            this.fileLocation = FileLocation;
            this.existsInMusicXNBFile = ExistsInXNBWavePack;
        }

        /// <summary>
        /// Load the song from the path so that we can stream it.
        /// </summary>
        /// <returns></returns>
        public DynamicSoundEffectInstance loadSongIntoStream()
        {

            System.IO.Stream waveFileStream = TitleContainer.OpenStream(this.fileLocation);

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
            count = dynamicSound.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(100));
            bufferHandler= new EventHandler<EventArgs>(DynamicSound_BufferNeeded);
            dynamicSound.BufferNeeded += bufferHandler;

            return this.dynamicSound;
        }

        /// <summary>
        /// Null the song out so that we can remove it from memory and switch to another song???
        /// </summary>
        public void unloadSongFromStream()
        {
            dynamicSound.Stop();
            dynamicSound.BufferNeeded -= bufferHandler;
            dynamicSound = null;
        }

        /// <summary>
        /// Taken from an example. I'm sure this is necessary to keep streaming the audio.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DynamicSound_BufferNeeded(object sender, EventArgs e)
        {
            dynamicSound.SubmitBuffer(byteArray, position, count / 2);
            dynamicSound.SubmitBuffer(byteArray, position + count / 2, count / 2);

            position += count;
            if (position + count > byteArray.Length)
            {
                position = 0;
            }
        }

        /// <summary>
        /// Stop the currently playing song.
        /// </summary>
        public void stop()
        {
            if (this.dynamicSound != null)
            {
                if(this.songState==SongState.Playing || this.songState == SongState.Paused)
                {
                    this.dynamicSound.Stop();
                    this.songState = SongState.Stopped;
                }
            }
        }

        /// <summary>
        /// Plays the current song.
        /// </summary>
        public void play()
        {
            if (this.dynamicSound != null)
            {
                if (getSongState() == SongState.Stopped || getSongState() == SongState.Paused)
                {
                    this.dynamicSound.Play();
                    this.songState = SongState.Playing;
                }
            }
        }


        /// <summary>
        /// Resume the current song from being paused.
        /// </summary>
        public void resume()
        {
            if (this.dynamicSound != null)
            {
                if (getSongState() == SongState.Stopped || getSongState() == SongState.Paused)
                {
                    this.dynamicSound.Resume();
                    this.songState = SongState.Playing;
                }
            }
        }

        /// <summary>
        /// Pauses the current song.
        /// </summary>
        public void pause()
        {
            if (getSongState() == SongState.Playing || getSongState() == SongState.Stopped)
            {
                this.dynamicSound.Pause();
                this.songState = SongState.Paused;
            }
        }

        /// <summary>
        /// Changes the volume of the song playing.
        /// </summary>
        /// <param name="newVolumeAmount"></param>
        public void changeVolume(float newVolumeAmount)
        {
            if (this.dynamicSound != null)
            {
                this.dynamicSound.Volume = newVolumeAmount;
            }
        }

        /// <summary>
        /// Returns the state of the song so that users know if the song is playing, stopped, or paused.
        /// </summary>
        /// <returns></returns>
        public SongState getSongState()
        {
            return this.songState;
        }


        /// <summary>
        /// Checks if the song is playing or not.
        /// </summary>
        /// <returns></returns>
        public bool isPlaying()
        {
            if (getSongState() == SongState.Playing) return true;
            else return false;
        }

        /// <summary>
        /// Checks is the song is paused or not.
        /// </summary>
        /// <returns></returns>
        public bool isPaused()
        {
            if (getSongState() == SongState.Paused) return true;
            else return false;
        }

        /// <summary>
        /// Checks if the song is stopped or not.
        /// </summary>
        /// <returns></returns>
        public bool isStopped()
        {
            if (getSongState() == SongState.Stopped) return true;
            else return false;
        }
    }
}
