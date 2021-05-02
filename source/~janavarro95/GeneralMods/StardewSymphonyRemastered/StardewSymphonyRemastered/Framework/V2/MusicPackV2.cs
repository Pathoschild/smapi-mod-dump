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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Vorbis;
using NAudio.Wave;
using StardewModdingAPI;
using StardewValley;
using StardustCore.UIUtilities;

namespace StardewSymphonyRemastered.Framework.V2
{
    /// <summary>A content pack which can provide music and sounds.</summary>
    public class MusicPackV2
    {
        /*********
        ** Fields
        *********/
        /// <summary>The name of the folder which contains the saved player settings.</summary>
        private readonly string DataFolderName = "data";

        /// <summary>The name of the folder which contains available music.</summary>
        private readonly string MusicFolderName = "songs";


        /*********
        ** Accessors
        *********/
        /// <summary>The underlying content pack.</summary>
        public IContentPack ContentPack { get; }

        /// <summary>The songs to play when.</summary>
        public SongSpecificsV2 SongInformation { get; }

        /// <summary>The music pack icon.</summary>
        public Texture2DExtended Icon { get; }

        /// <summary>The current song name being played, if any.</summary>
        public string CurrentSongName { get; private set; }

        /// <summary>The currently sound being played, if any.</summary>
        public SoundEffectInstance CurrentSound { get; private set; }

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
        public MusicPackV2(IContentPack contentPack)
        {
            this.ContentPack = contentPack;
            this.SongInformation = new SongSpecificsV2();
            this.Icon = this.LoadIcon();
            this.LoadMusicFiles();
        }

        /// <summary>Play a song.</summary>
        /// <param name="name">The song name to play.</param>
        public void PlaySong(string name)
        {
            // get sound
            if (!this.Sounds.TryGetValue(name, out SoundEffectInstance sound))
            {
                StardewSymphony.ModMonitor.Log("An error occured where we can't find the song anymore. Weird. Please contact Omegasis with a SMAPI Log and describe when/how the event occured.");
                return;
            }

            // play sound
            this.StopSong();
            this.CurrentSongName = name;
            this.CurrentSound = sound;
            this.CurrentSound.Volume = Game1.options.musicVolumeLevel;
            this.CurrentSound.Play();
        }

        /// <summary>Stop the currently playing song.</summary>
        public void StopSong()
        {
            Game1.currentSong?.Stop(AudioStopOptions.Immediate);
            this.CurrentSound?.Stop(true);
            this.CurrentSongName = null;
        }

        /// <summary>Get whether the content pack is currently playing a song.</summary>
        public bool IsPlaying()
        {
            return this.CurrentSound?.State == SoundState.Playing;
        }

        /// <summary>Write the song conditions to disk.</summary>
        public virtual void SaveSettings()
        {
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log($"Saving music for {this.Name}...");

            //Clear out directory so that changes can be reflected.
            DirectoryInfo dataFolder = new DirectoryInfo(Path.Combine(this.ContentPack.DirectoryPath, this.DataFolderName));

            foreach (var list in this.SongInformation.songs)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log($"   Saving song: {list.Key}...");
                this.ContentPack.WriteJsonFile($"{this.DataFolderName}/{list.Value.name}.json", list.Value);
            }
        }

        /// <summary>Read the song conditions from disk.</summary>
        public virtual void LoadSettings()
        {
            DirectoryInfo dataFolder = new DirectoryInfo(Path.Combine(this.ContentPack.DirectoryPath, this.DataFolderName));
            if (dataFolder.Exists)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log($"Loading music for {this.Name}. This may take quite some time.");

                foreach (FileInfo file in dataFolder.GetFiles())
                {
                    SongInformation node = this.ContentPack.ReadJsonFile<SongInformation>($"{this.DataFolderName}/{file.Name}");
                    if (this.SongInformation.songs.ContainsKey(Path.GetFileNameWithoutExtension(file.Name)))
                    {
                        this.SongInformation.songs[Path.GetFileNameWithoutExtension(file.Name)].songConditionals = node.songConditionals;
                    }
                    else
                    {
                        this.SongInformation.songs.Add(Path.GetFileNameWithoutExtension(file.Name), node);
                    }
                }
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Load the icon from the content pack.</summary>
        private Texture2DExtended LoadIcon()
        {
            try
            {
                return new Texture2DExtended(this.ContentPack,"icon.png");
            }
            catch (Exception err)
            {
                if (StardewSymphony.Config.EnableDebugLog)
                    StardewSymphony.ModMonitor.Log(err.ToString());
                return null;
            }
        }

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
                                StardewSymphony.ModMonitor.Log($"Converting: {tempPath}");

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
                                StardewSymphony.DebugLog($"Converting: {tempPath}");

                                WaveFileWriter.CreateWaveFile(tempPath, vorbisStream.ToWaveProvider16());
                                using (Stream tempStream = File.OpenRead(tempPath))
                                    effect = SoundEffect.FromStream(tempStream);
                                File.Delete(tempPath);
                            }
                            break;

                        default:
                            StardewSymphony.ModMonitor.Log($"Unsupported file extension {file.Extension}.", LogLevel.Warn);
                            break;
                    }
                }
                if (effect == null)
                    continue;

                // add sound
                SoundEffectInstance instance = effect.CreateInstance();
                this.Sounds.Add(name, instance);
                //this.SongInformation.listOfSongsWithoutTriggers.Add(name);
                this.SongInformation.songs.Add(name, new V2.SongInformation(name));
            }

            // log loading time
            if (StardewSymphony.Config.EnableDebugLog)
                StardewSymphony.ModMonitor.Log($"Time to load WAV music pack {this.Name}: {startTime.Subtract(DateTime.Now)}");
        }
    }
}
