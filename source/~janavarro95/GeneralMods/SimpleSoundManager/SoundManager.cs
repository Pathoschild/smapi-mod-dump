using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSoundManager
{
    class SoundManager
    {

        public Dictionary<string,Sound> sounds;
        public Dictionary<string, XACTMusicPair> musicBanks;


        /// <summary>
        /// Constructor for this class.
        /// </summary>
        public SoundManager()
        {
            this.sounds = new Dictionary<string, Sound>();
            this.musicBanks = new Dictionary<string, XACTMusicPair>();
        }

        /// <summary>
        /// Constructor for wav files.
        /// </summary>
        /// <param name="soundName"></param>
        /// <param name="pathToWav"></param>
        public void loadWavFile(string soundName,string pathToWav)
        {
            WavSound wav = new WavSound(pathToWav);
            this.sounds.Add(soundName,wav);
        }
        
        /// <summary>
        /// Constructor for wav files.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="soundName"></param>
        /// <param name="pathToWav"></param>
        public void loadWavFile(IModHelper helper,string soundName,string pathToWav)
        {
            WavSound wav = new WavSound(helper ,pathToWav);
            this.sounds.Add(soundName,wav);
        }

        /// <summary>
        /// Constructor for wav files.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="songName"></param>
        /// <param name="pathToWav"></param>
        public void loadWavFile(IModHelper helper,string songName,List<string> pathToWav)
        {
            WavSound wav = new WavSound(helper,pathToWav);
            this.sounds.Add(songName,wav);
        }

        /// <summary>
        /// Constructor for XACT files.
        /// </summary>
        /// <param name="waveBank"></param>
        /// <param name="soundBank"></param>
        /// <param name="songName"></param>
        public void loadXACTFile(WaveBank waveBank, ISoundBank soundBank, string songName)
        {
            XACTSound xactSound = new XACTSound(waveBank, soundBank, songName);
            this.sounds.Add(songName, xactSound);
        }

        /// <summary>
        /// Constructor for XACT files based on already added music packs.
        /// </summary>
        /// <param name="pairName"></param>
        /// <param name="songName"></param>
        public void loadXACTFile(string pairName, string songName)
        {
            XACTMusicPair musicPair = getMusicPack(pairName);
            if (pairName == null)
            {
                return;
            }
            loadXACTFile(musicPair.waveBank, musicPair.soundBank, songName);
        }

     
        /// <summary>
        /// Creates a music pack pair that holds .xwb and .xsb music files.
        /// </summary>
        /// <param name="helper">The mod's helper that will handle the path of the files.</param>
        /// <param name="pairName">The name of this music pack pair.</param>
        /// <param name="wavName">The relative path to the .xwb file</param>
        /// <param name="soundName">The relative path to the .xsb file</param>
        public void loadXACTMusicBank(IModHelper helper,string pairName,string wavName, string soundName)
        {
            this.musicBanks.Add(pairName,new XACTMusicPair(helper, wavName, soundName));
        }

        /// <summary>
        /// Gets the music pack pair from the sound pool.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XACTMusicPair getMusicPack(string name)
        {
            foreach(var pack in this.musicBanks)
            {
                if (name == pack.Key) return pack.Value;
            }
            return null;
        }

        /// <summary>
        /// Gets a clone of the loaded sound.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Sound getSoundClone(string name)
        {
            foreach(var sound in this.sounds)
            {
                if (sound.Key == name) return sound.Value.clone();
            }
            return null;
        }

    }
}
