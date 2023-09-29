/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using System.IO;
using System.Text;
using StardewModdingAPI;
using StardewValley;
using NVorbis;
using Microsoft.Xna.Framework.Audio;

namespace ichortower_HatMouseLacey
{
    internal class LCMusicLoader
    {
        /*
         * Song loader. Loads an ogg vorbis music file, writes it to a
         * waveform in memory, then adds the cue to SDV's Game1.soundBank.
         * This requires extra CPU time, but SDV uses XACT and that requires
         * waveforms. I chose ogg files for distribution since they yield
         * 90%+ compression over .wav.
         *
         * Should be safe to call repeatedly, since it will avoid doing work
         * if the cue is already defined. But also, I think that means if you
         * use a vanilla music cue name here, your song won't load.
         *
         * Ideally, call this from a non-main thread, so it doesn't block the
         * game or especially the UI. Loading a song takes a noticeable amount
         * of time (each of mine costs about half a second), so it's best to
         * avoid interrupting gameplay.
         */
        public void LoadOggSong(string cueName, string fullPath)
        {
            /*
             * This setup looks odd, but the unmodified soundBank's GetCue
             * throws if the cue doesn't exist, and returns the value if it
             * does. Meanwhile, the SAAT mod replaces the soundBank with a
             * different wrapper; that GetCue function returns blank audio
             * (called "Default") instead of throwing.
             * This is designed to handle both of these situations.
             */
            try {
                var haveCue = Game1.soundBank.GetCue(cueName);
                if (haveCue.Name.Equals("Default")) {
                    throw new System.Exception("enoent");
                }
                return;
            }
            catch {}

            ModEntry.MONITOR.Log($"Loading song '{cueName}' from '{fullPath}'", LogLevel.Trace);
            var ogg = new NVorbis.VorbisReader(fullPath);
            var channels = ogg.Channels;
            var sampleRate = ogg.SampleRate;
            var buffer = new float[channels * sampleRate * 1]; // 1s per read

            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            WriteHeader(writer, channels, sampleRate);
            int count = 0;
            /* convert float samples to short when writing. this cuts memory
             * use in half and is probably about a wash on load time (the math
             * costs a little, but we move fewer bytes around). I haven't
             * profiled it. */
            while ((count = ogg.ReadSamples(buffer, 0, buffer.Length)) > 0) {
                for (var i = 0; i < count; ++i) {
                    writer.Write((short)(buffer[i] * 32767));
                }
            }
            WriteSizes(writer, stream);
            stream.Position = 0;

            SoundEffect mus = SoundEffect.FromStream(stream);
            bool loop = true;
            var cueDef = new CueDefinition(cueName, mus,
                    Game1.audioEngine.GetCategoryIndex("Music"), loop);
            Game1.soundBank.AddCue(cueDef);
            ModEntry.MONITOR.Log($"Finished loading '{cueName}'", LogLevel.Trace);
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
