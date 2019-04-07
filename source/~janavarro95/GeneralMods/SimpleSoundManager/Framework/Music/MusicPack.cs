using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Vorbis;
using NAudio.Wave;
using StardewModdingAPI;
using StardewValley;

namespace SimpleSoundManager.Framework
{
    /// <summary>A content pack which can provide music and sounds.</summary>
    public class MusicPack
    {
        /*********
        ** Fields
        *********/
        /// <summary>The name of the folder which contains the saved player settings.</summary>
        private readonly string DataFolderName = "data";

        /// <summary>The name of the folder which contains available music.</summary>
        private readonly string MusicFolderName = "music";


        /*********
        ** Accessors
        *********/
        /// <summary>The underlying content pack.</summary>
        public IContentPack ContentPack { get; }

        /// <summary>The current song name being played, if any.</summary>
        public string lastPlayedSoundName { get; private set; }

        /// <summary>The currently sound being played, if any.</summary>
        public SoundEffectInstance lastPlayedSound { get; private set; }

        public Dictionary<string, List<SoundEffectInstance>> playingSounds;

        /// <summary>The manifest info.</summary>
        public IManifest Manifest => this.ContentPack.Manifest;

        /// <summary>The name of the music pack.</summary>
        public string Name => this.ContentPack.Manifest.Name;

        /// <summary>The available sounds.</summary>
        public Dictionary<string, SoundEffectInstance> Sounds { get; } = new Dictionary<string, SoundEffectInstance>();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="contentPack">The underlying content pack.</param>
        public MusicPack(IContentPack contentPack)
        {
            this.ContentPack = contentPack;
            this.playingSounds = new Dictionary<string, List<SoundEffectInstance>>();
            this.LoadMusicFiles();
        }

        /// <summary>Play a song.</summary>
        /// <param name="name">The song name to play.</param>
        public void PlaySound(string name)
        {
            // get sound
            if (!this.Sounds.TryGetValue(name, out SoundEffectInstance sound))
            {
                ModCore.ModMonitor.Log("An error occured where we can't find the song anymore. Weird. Please contact Omegasis with a SMAPI Log and describe when/how the event occured.");
                return;
            }

            // play sound
            this.lastPlayedSoundName = name;
            this.lastPlayedSound = sound;
            this.lastPlayedSound.Play();

            if (this.playingSounds.ContainsKey(name))
            {
                this.playingSounds[name].Add(sound);
            }
            else
            {
                this.playingSounds.Add(name,new List<SoundEffectInstance>());
                this.playingSounds[name].Add(sound);
            }

        }

        /// <summary>Stop the currently playing song.</summary>
        public void StopSound()
        {
            this.lastPlayedSound?.Stop(true);
            this.lastPlayedSoundName = null;
        }

        /// <summary>
        /// Stops all of the currently playing sounds.
        /// </summary>
        public void stopAllSounds()
        {
            foreach (string soundName in playingSounds.Keys)
            {
                for (int i = 0; i < playingSounds[soundName].Count; i++)
                {
                    if (playingSounds[soundName][i].State == SoundState.Stopped)
                    {
                        playingSounds[soundName][i].Stop(true);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the music pack to clean up all sounds that have been stopped.
        /// </summary>
        public void update()
        {
            ///Clean up the list.
            foreach(string soundName in playingSounds.Keys)
            {
                for(int i = 0; i < playingSounds[soundName].Count; i++)
                {
                    if(playingSounds[soundName][i].State== SoundState.Stopped)
                    {
                        playingSounds[soundName].RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }
        }

        /// <summary>Get whether the content pack is currently playing a sound.</summary>
        public bool IsPlaying()
        {
            return this.lastPlayedSound?.State == SoundState.Playing;
        }

        /// <summary>
        /// Checks if there is a sound with said name playing.
        /// </summary>
        /// <param name="SoundName"></param>
        /// <returns></returns>
        public bool IsPlaying(string SoundName)
        {
            return this.playingSounds[SoundName].FindAll(s => s.State== SoundState.Playing).Count>0;
        }

        /*********
        ** Private methods
        *********/

        /// <summary>Load in the music files from the pack's respective Directory/Songs folder. Typically Content/Music/Wav/FolderName/Songs</summary>
        private void LoadMusicFiles()
        {
            DateTime startTime = DateTime.Now;

            DirectoryInfo songFolder = new DirectoryInfo(Path.Combine(this.ContentPack.DirectoryPath, this.MusicFolderName));
            foreach (FileInfo file in songFolder.GetFiles())
            {
                // get name
                string name = Path.GetFileNameWithoutExtension(file.Name);
                if (this.Sounds.ContainsKey(name))
                    continue;

                // load data
                SoundEffect effect = null;
                using (Stream waveFileStream = File.OpenRead(file.FullName))
                {
                    switch (file.Extension)
                    {
                        case ".wav":
                            effect = SoundEffect.FromStream(waveFileStream);
                            break;

                        case ".mp3":
                            using (Mp3FileReader reader = new Mp3FileReader(waveFileStream))
                            using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                            {
                                string tempPath = Path.Combine(songFolder.FullName, $"{name}.wav");
                                ModCore.ModMonitor.Log($"Converting: {tempPath}");

                                WaveFileWriter.CreateWaveFile(tempPath, pcmStream);
                                using (Stream tempStream = File.OpenRead(tempPath))
                                    effect = SoundEffect.FromStream(tempStream);
                                File.Delete(tempPath);
                            }
                            break;

                        case ".ogg":
                            // Credits: https://social.msdn.microsoft.com/Forums/vstudio/en-US/100a97af-2a1c-4b28-b464-d43611b9b5d6/converting-multichannel-ogg-to-stereo-wav-file?forum=csharpgeneral
                            using (VorbisWaveReader vorbisStream = new VorbisWaveReader(file.FullName))
                            {
                                string tempPath = Path.Combine(songFolder.FullName, $"{name}.wav");
                                ModCore.DebugLog($"Converting: {tempPath}");

                                WaveFileWriter.CreateWaveFile(tempPath, vorbisStream.ToWaveProvider16());
                                using (Stream tempStream = File.OpenRead(tempPath))
                                    effect = SoundEffect.FromStream(tempStream);
                                File.Delete(tempPath);
                            }
                            break;

                        default:
                            ModCore.ModMonitor.Log($"Unsupported file extension {file.Extension}.", LogLevel.Warn);
                            break;
                    }
                }
                if (effect == null)
                    continue;

                // add sound
                SoundEffectInstance instance = effect.CreateInstance();
                this.Sounds.Add(name, instance);
            }

            // log loading time
            if (ModCore.Config.EnableDebugLog)
                ModCore.ModMonitor.Log($"Time to load WAV music pack {this.Name}: {startTime.Subtract(DateTime.Now)}");
        }
    }
}
