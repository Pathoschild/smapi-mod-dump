/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretWoodsSnorlax
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using NVorbis;
using StardewValley;
using System.IO;
using System.Text;

namespace ichortower.SecretWoodsSnorlax
{
    internal class Ogg
    {
        /*
         * Loads entire .ogg file into memory, converting to waveform.
         * Takes time, so best to thread when calling.
         * (cf. Hat Mouse Lacey)
         */
        public static void LoadSound(string cueName, string fullPath)
        {
            /* same vanilla/SAAT workaround as HML */
            try {
                var haveCue = Game1.soundBank.GetCue(cueName);
                if (haveCue.Name.Equals("Default")) {
                    throw new System.Exception("enoent");
                }
                return;
            }
            catch {}

            var ogg = new NVorbis.VorbisReader(fullPath);
            var channels = ogg.Channels;
            var sampleRate = ogg.SampleRate;
            var buffer = new float[channels * sampleRate * 1]; // 1s per read

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            WriteHeader(writer, channels, sampleRate);
            int count = 0;
            /* convert float samples to short when writing (save memory). */
            while ((count = ogg.ReadSamples(buffer, 0, buffer.Length)) > 0) {
                for (var i = 0; i < count; ++i) {
                    writer.Write((short)(buffer[i] * 32767));
                }
            }
            WriteSizes(writer, stream);
            stream.Position = 0;

            SoundEffect mus = SoundEffect.FromStream(stream);
            bool loop = false;
            var cueDef = new CueDefinition(cueName, mus,
                    Game1.audioEngine.GetCategoryIndex("Sound"), loop);
            Game1.soundBank.AddCue(cueDef);
        }

        const string BLANK_HEADER = "RIFF\0\0\0\0WAVEfmt ";
        const string BLANK_DATA = "data\0\0\0\0";

        private static void WriteHeader(BinaryWriter writer, int channels, int sampleRate)
        {
            /* total header length: 44 bytes */
            writer.Write(Encoding.UTF8.GetBytes(BLANK_HEADER));
            writer.Write((int)16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write((int)sampleRate);
            var blockSize = channels * sizeof(short);
            writer.Write((int)(sampleRate * blockSize));
            writer.Write((short)blockSize);
            writer.Write((short)(sizeof(short) * 8));
            writer.Write(Encoding.UTF8.GetBytes(BLANK_DATA));
        }

        private static void WriteSizes(BinaryWriter writer, MemoryStream stream)
        {
            writer.Seek(4, SeekOrigin.Begin);
            writer.Write((uint)(stream.Length - 8));
            writer.Seek(40, SeekOrigin.Begin);
            writer.Write((uint)(stream.Length - 44));
        }
    }
}
