/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using NVorbis;
using System;
using System.IO;


// Ogg loading code from SAAT: https://github.com/Dawilly/SAAT/
// Respective license (MIT) applies.


namespace StardewRoguelike
{
    public class OggLoader
    {
        /// <summary>
        /// Loads the entire content of an .ogg into memory.
        /// </summary>
        /// <param name="filePath">The file path pointing to the ogg file.</param>
        /// <returns>A newly created <see cref="SoundEffect"/> object.</returns>
        public static SoundEffect OpenOggFile(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open);
            using var reader = new VorbisReader(stream, true);

            // At the moment, we're loading everything in. If the number of samples is greater than int.MaxValue, bail.
            if (reader.TotalSamples * reader.Channels > int.MaxValue)
            {
                throw new Exception("TotalSample overflow");
            }

            int totalSamples = (int)reader.TotalSamples * reader.Channels;
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
                throw new Exception("Unable to read samples from Ogg file.");

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

            return new SoundEffect(buffer, sampleRate, (AudioChannels)reader.Channels);
        }
    }
}
