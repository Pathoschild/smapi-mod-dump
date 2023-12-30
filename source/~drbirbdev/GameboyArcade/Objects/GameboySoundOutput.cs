/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Diagnostics;
using System.IO;
using BirbCore.Attributes;
using CoreBoy;
using CoreBoy.sound;
using Microsoft.Xna.Framework.Audio;

// TODO: not working perfectly
namespace GameboyArcade
{
    class GameboySoundOutput : ISoundOutput, IDisposable
    {
        private const int BUFFER_SIZE = 1024;
        private readonly int DIVIDER = Gameboy.TicksPerSec / 22050;

        const int RIFF_ENCODING = 0x46464952;
        const int WAVE_ENCODING = 0x45564157;
        const int FMT_ENCODING = 0x20746D66;
        const int DATA_ENCODING = 0x61746164;
        const int FORMAT_CHUNK_SIZE = 16;
        const int HEADER_SIZE = 8;
        const short FORMAT_TYPE = 1;
        const short CHANNELS = 2;
        const int SAMPLES_PER_SECOND = 12000;
        const short BITS_PER_SAMPLE = 8;
        const short FRAME_SIZE = CHANNELS * ((BITS_PER_SAMPLE + 7) / 8);
        const int BYTES_PER_SECOND = SAMPLES_PER_SECOND * CHANNELS * BITS_PER_SAMPLE / 8;
        const int WAVE_SIZE = 4;
        const int SAMPLES = BUFFER_SIZE / 2;
        const int DATA_CHUNK_SIZE = SAMPLES * FRAME_SIZE;
        const int FILE_SIZE = WAVE_SIZE + HEADER_SIZE + FORMAT_CHUNK_SIZE + HEADER_SIZE + DATA_CHUNK_SIZE;


        private int I;
        private long Generated = 0;
        private int Tick;
        private readonly Stopwatch Stopwatch = new Stopwatch();
        private MemoryStream Stream;
        private BinaryWriter Writer;
        private SoundEffectInstance Sound;

        public GameboySoundOutput()
        {
            this.Stream = new MemoryStream(FILE_SIZE);
            this.Writer = new BinaryWriter(this.Stream);
            this.Writer.Write(RIFF_ENCODING);
            this.Writer.Write(FILE_SIZE);
            this.Writer.Write(WAVE_ENCODING);
            this.Writer.Write(FMT_ENCODING);
            this.Writer.Write(FORMAT_CHUNK_SIZE);
            this.Writer.Write(FORMAT_TYPE);
            this.Writer.Write(CHANNELS);
            this.Writer.Write(SAMPLES_PER_SECOND);
            this.Writer.Write(BYTES_PER_SECOND);
            this.Writer.Write(FRAME_SIZE);
            this.Writer.Write(BITS_PER_SAMPLE);
            this.Writer.Write(DATA_ENCODING);
            this.Writer.Write(DATA_CHUNK_SIZE);
        }

        public void Play(byte left, byte right)
        {
            if (this.Tick++ != 0)
            {
                this.Tick %= this.DIVIDER;
                return;
            }

            // TODO: correct conversion to unsigned waveform
            this.Writer.Write((byte)(left + 127));
            this.Writer.Write((byte)(right + 127));
            this.I += 2;

            if (this.I >= BUFFER_SIZE)
            {
                this.Stream.Seek(0, SeekOrigin.Begin);

                while (this.Sound is not null && this.Sound.State == SoundState.Playing)
                {
                    continue;
                }
                this.Sound?.Dispose();

                this.Sound = SoundEffect.FromStream(this.Stream).CreateInstance();
                this.Sound.Play();

                this.Stream.DisposeAsync();
                this.Stream = new MemoryStream(FILE_SIZE);
                this.Writer.DisposeAsync();
                this.Writer = new BinaryWriter(this.Stream);

                this.Writer.Write(RIFF_ENCODING);
                this.Writer.Write(FILE_SIZE);
                this.Writer.Write(WAVE_ENCODING);
                this.Writer.Write(FMT_ENCODING);
                this.Writer.Write(FORMAT_CHUNK_SIZE);
                this.Writer.Write(FORMAT_TYPE);
                this.Writer.Write(CHANNELS);
                this.Writer.Write(SAMPLES_PER_SECOND);
                this.Writer.Write(BYTES_PER_SECOND);
                this.Writer.Write(FRAME_SIZE);
                this.Writer.Write(BITS_PER_SAMPLE);
                this.Writer.Write(DATA_ENCODING);
                this.Writer.Write(DATA_CHUNK_SIZE);

                this.I = 0;
                this.Generated++;
                if (this.Generated % 100 == 0)
                {
                    Log.Info($"{this.Stopwatch.Elapsed.TotalMilliseconds / 100} ms per sound frame");
                    this.Stopwatch.Restart();
                }
            }

        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
            //this.Stream.Dispose();
            //this.Writer.Dispose();
        }
    }
}
