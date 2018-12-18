using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSoundManager.Framework
{
    public class XACTSound : Sound
    {
        public WaveBank waveBank;
        public ISoundBank soundBank;
        public string soundName;
        WaveBank vanillaWaveBank;
        ISoundBank vanillaSoundBank;
        Cue song;

        /// <summary>
        /// Make a new Sound Manager to play and manage sounds in a modded wave bank.
        /// </summary>
        /// <param name="newWaveBank">The reference to the wave bank in the mod's asset folder.</param>
        /// <param name="newSoundBank">The reference to the sound bank in the mod's asset folder.</param>
        public XACTSound(WaveBank newWaveBank, ISoundBank newSoundBank,string soundName)
        {
            this.waveBank = newWaveBank;
            this.soundBank = newSoundBank;

            vanillaSoundBank = Game1.soundBank;
            vanillaWaveBank = Game1.waveBank;
            this.soundName = soundName;
            song = this.soundBank.GetCue(this.soundName);
        }

        /// <summary>
        /// Play a sound from the mod's wave bank.
        /// </summary>
        /// <param name="soundName">The name of the sound in the mod's wave bank. This will fail if the sound doesn't exists. This is also case sensitive.</param>
        public void play(string soundName)
        {
            Game1.waveBank = this.waveBank;
            Game1.soundBank = this.soundBank;

            if (this.song == null) return;

            this.song.Play();

            Game1.waveBank = this.vanillaWaveBank;
            Game1.soundBank = this.vanillaSoundBank;
        }

        /// <summary>
        /// Pauses the first instance of this sound.
        /// </summary>
        /// <param name="soundName"></param>
        public void pause(string soundName)
        {
            if (this.song == null) return;
            this.song.Pause();
        }

        /// <summary>
        /// Resume the first instance of the sound that has this name.
        /// </summary>
        public void resume(string soundName)
        {
            if (this.song == null) return;
            this.song.Resume();
        }


        /// <summary>
        /// Stop the first instance of the sound that has this name.
        /// </summary>
        /// <param name="soundName"></param>
        public void stop(string soundName)
        {
            if (this.song == null) return;
            this.song.Stop(AudioStopOptions.Immediate);
        }

        /// <summary>
        /// Resumes a paused song.
        /// </summary>
        public void resume()
        {
            this.resume(soundName);
        }

        /// <summary>
        /// Plays this song.
        /// </summary>
        public void play()
        {
            this.play(this.soundName);
        }

        /// <summary>
        /// Plays this song.
        /// </summary>
        public void play(float volume)
        {
            this.play(this.soundName);
        }

        /// <summary>
        /// Pauses this song.
        /// </summary>
        public void pause()
        {
            this.pause(this.soundName);
        }

        /// <summary>
        /// Stops this somg.
        /// </summary>
        public void stop()
        {
            this.stop(this.soundName);
        }

        /// <summary>
        /// Restarts this song.
        /// </summary>
        public void restart()
        {
            this.stop();
            this.play();
        }

        /// <summary>
        /// Gets a clone of this song.
        /// </summary>
        /// <returns></returns>
        public Sound clone()
        {
            return new XACTSound(this.waveBank, this.soundBank, this.soundName);
        }

        public string getSoundName()
        {
            return this.soundName;
        }

        public bool isStopped()
        {
            if (this.song == null) return true;
            if (this.song.IsStopped) return true;
            return false;
        }


    }
}
