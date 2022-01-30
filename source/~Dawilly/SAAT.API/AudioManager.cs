/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework.Audio;
using NVorbis;

using StardewModdingAPI;
using StardewValley;

namespace SAAT.API
{
    /// <summary>
    /// Implementation of <see cref="IAudioManager"/>. Handles all operations regarding audio.
    /// </summary>
    public class AudioManager : IAudioManager
    {
        private readonly Dictionary<string, ICue> cueTable;
        private readonly Dictionary<string, Track> trackTable;

        //TO-DO: Implement an engine that handles memory management appropriately, instead of ad-hoc.
        //private readonly IAudioEngine engine;
        private readonly IMonitor monitor;

        /// <inheritdoc/>
        public ISoundBank SoundBank { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="AudioManager"/> class.
        /// </summary>
        public AudioManager(IMonitor monitor)
        {
            this.cueTable = new Dictionary<string, ICue>();
            this.trackTable = new Dictionary<string, Track>();

            //this.engine = Game1.audioEngine;
            this.SoundBank = Game1.soundBank;

            this.monitor = monitor;
        }

        /// <inheritdoc/>
        public bool AddToJukebox(string name, out AudioOperationError error)
        {
            // TO-DO: On audio engine replacement, we will no longer need to try-catch.
            // A non-existing audio file will result in a null being returned.
            // For now... this a bit of a performance cost.
            // Issue: XNA/MG underengineered Audio Engine.
            try
            {
                _ = this.SoundBank.GetCueDefinition(name);
            }
            catch (ArgumentException)
            {
                error = AudioOperationError.AssetNotFound;
                return false;
            }

            if (Game1.player.songsHeard.Contains(name))
            {
                error = AudioOperationError.Exists;
                return false;
            }

            Game1.player.songsHeard.Add(name);
            error = AudioOperationError.None;

            return true;
        }

        /// <inheritdoc/>
        public ICue Load(string filePath, string owner, CreateAudioInfo createInfo)
        {
            if (this.cueTable.ContainsKey(createInfo.Name))
            {
                return this.cueTable[createInfo.Name];
            }

            SoundEffect sfx;
            uint byteSize;

            try
            {
                sfx = AudioManager.LoadFile(filePath, out byteSize);
            }
            catch (Exception e)
            {
                this.monitor.Log($"Unable to load audio: {e.Message}\n{e.StackTrace}");
                return null;
            }

            // Am I being funny yet?
            var cueBall = new CueDefinition(createInfo.Name, sfx, (int)createInfo.Category, createInfo.Loop);

            // Need to add the defition to the bank in order to generate a cue.
            this.SoundBank.AddCue(cueBall);
            var cue = this.SoundBank.GetCue(createInfo.Name);

            this.cueTable.Add(createInfo.Name, cue);

            var track = new Track {
                BufferSize = byteSize,
                Category = createInfo.Category,
                Filepath = filePath,
                Id = createInfo.Name,
                Instance = cue,
                Loop = createInfo.Loop,
                Owner = owner
            };

            this.trackTable.Add(createInfo.Name, track);

            return cue;
        }

        /// <inheritdoc/>
        public void PrintMemoryAllocationInfo()
        {
            var subTotals = new Dictionary<string, uint>();

            string name = "Name";
            string size = "Size (In Kilobytes)";
            string owner = "Owner";

            this.monitor.Log($"##\t{name,-40}{size,-40}{owner}\t##", LogLevel.Info);

            foreach (var track in this.trackTable.Values)
            {
                if (!subTotals.ContainsKey(track.Owner))
                {
                    subTotals.Add(track.Owner, track.BufferSize);
                }
                else
                {
                    subTotals[track.Owner] += track.BufferSize;
                }

                string bufferSize = $"{Utilities.BufferSizeInKilo(track)} KB";
                this.monitor.Log($"  \t{track.Id,-40}{bufferSize,-40}{track.Owner}", LogLevel.Info);
            }

            uint total = 0;
            this.monitor.Log($"##\t {name,-40}{size}\t##", LogLevel.Info);

            foreach (var kvp in subTotals)
            {
                total += kvp.Value;
                string bufferSize = $"{Utilities.BufferSizeInMega(kvp.Value)} MB";
                this.monitor.Log($"  \t{kvp.Key,-40}{bufferSize}", LogLevel.Info);
            }

            this.monitor.Log($"Total Memory Usage: {Utilities.BufferSizeInMega(total)} MB", LogLevel.Info);
        }

        /// <inheritdoc/>
        public void PrintTrackAllocationAndSettings(string id)
        {
            if (!this.trackTable.TryGetValue(id, out var track))
            {
                this.monitor.Log($"Could not find track with the Id: {id}", LogLevel.Info);
                return;
            }

            this.monitor.Log($"Track Id: {track.Id}", LogLevel.Info);
            this.monitor.Log($"File: {track.Filepath}", LogLevel.Info);
            this.monitor.Log($"Size: {Utilities.BufferSizeInKilo(track)} KB", LogLevel.Info);
            this.monitor.Log($"Owner: {track.Owner}\n", LogLevel.Info);

            this.monitor.Log($"Category: {track.Category}", LogLevel.Info);
            this.monitor.Log($"Is Looping: {track.Loop}", LogLevel.Info);
        }

        /// <summary>
        /// Loads an audio file into memory and creates a <see cref="SoundEffect"/> object to access
        /// the audio content.
        /// </summary>
        /// <param name="path">The path to the audio file.</param>
        /// <returns>A newly created <see cref="SoundEffect"/> object. <see cref="null"/> if it failed to load.</returns>
        private static SoundEffect LoadFile(string path, out uint byteSize)
        {
            byteSize = 0;

            var type = Utilities.ParseAudioExtension(path);

            using var stream = new FileStream(path, FileMode.Open);

            switch (type) {
                case AudioFileType.Wav:
                    return AudioManager.OpenWavFile(stream, out byteSize);

                case AudioFileType.Ogg:
                    return AudioManager.OpenOggFile(stream, out byteSize);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Loads the entire content of a .wav into memory.
        /// </summary>
        /// <param name="stream">The file stream pointing to the wav file.</param>
        /// <param name="byteSize">The number of bytes needed for the audio data.</param>
        /// <returns>A newly created <see cref="SoundEffect"/> object.</returns>
        private static SoundEffect OpenWavFile(FileStream stream, out uint byteSize)
        {
            byteSize = 0;

            // We're gonna peak at the number of bytes before we pass this off.
            using var reader = new BinaryReader(stream);
            long riffDataSize = 0;

            do {
                string chunkId = new string(reader.ReadChars(4));
                int chunkSize = reader.ReadInt32();

                switch (chunkId) {
                    case "RIFF":
                        // Set filesize and toss out "WAVE".
                        riffDataSize = chunkSize;
                        reader.ReadChars(4); 
                        break;

                    case "fmt ":
                        // Toss out.
                        reader.ReadBytes(chunkSize);
                        break;

                    case "data":
                        // Set byteSize, we're done.
                        byteSize = (uint)chunkSize;
                        break;

                    default:
                        reader.BaseStream.Seek((long)chunkSize, SeekOrigin.Current);
                        break;
                }
            } while (byteSize == 0 && reader.BaseStream.Position < riffDataSize);

            // Back to the top of the file.
            stream.Position = 0;

            return SoundEffect.FromStream(stream);
        }

        /// <summary>
        /// Loads the entire content of an .ogg into memory.
        /// </summary>
        /// <param name="stream">The file stream pointing to the ogg file.</param>
        /// <param name="byteSize">The number of bytes needed for the audio data.</param>
        /// <returns>A newly created <see cref="SoundEffect"/> object.</returns>
        private static SoundEffect OpenOggFile(FileStream stream, out uint byteSize)
        {
            using var reader = new VorbisReader(stream, true);

            // At the moment, we're loading everything in. If the number of samples is greater than int.MaxValue, bail.
            if (reader.TotalSamples > int.MaxValue)
            {
                throw new Exception("TotalSample overflow");
            }

            int totalSamples = (int)reader.TotalSamples;
            int sampleRate = reader.SampleRate;
            
            // SoundEffect.SampleSizeInBytes has a fault within it. In conjunction with a small amount of percision loss,
            // any decimal points are dropped instead of rounded up. For example: It will calculate the buffer size to be
            // 2141.999984, returning 2141. This should be 2142, as it violates block alignment below.
            int bufferSize = (int)Math.Ceiling(reader.TotalTime.TotalSeconds * (sampleRate * reader.Channels * 16d / 8d));
            byte[] buffer = new byte[bufferSize];
            float[] vorbisBuffer = new float[totalSamples];

            int sampleReadings = reader.ReadSamples(vorbisBuffer, 0, totalSamples);

            // This shouldn't occur. Check just incase and bail out if so.
            if (sampleReadings == 0)
            {
                throw new Exception("Unable to read samples from Ogg file.");
            }

            // Buffers within SoundEffect instances MUST be block aligned. By 2 for Mono, 4 for Stereo.
            int blockAlign = reader.Channels * 2;
            sampleReadings -= sampleReadings % blockAlign;

            // Must convert the audio data to 16-bit PCM, as this is the only format SoundEffect supports.
            for (int i = 0; i < sampleReadings; i++)
            {
                short sh = (short)Math.Max(Math.Min(short.MaxValue * vorbisBuffer[i], short.MaxValue), short.MinValue);
                buffer[i * 2] = (byte)(sh & 0xff);
                buffer[i * 2 + 1] = (byte)((sh >> 8) & 0xff);
            }

            byteSize = (uint)buffer.Length;

            return new SoundEffect(buffer, sampleRate, (AudioChannels)reader.Channels);
        }
    }
}
