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
using CoreBoy;
using CoreBoy.sound;
using Microsoft.Xna.Framework.Audio;

// TODO: not working perfectly
namespace GameboyArcade
{
    class GameboySoundOutput : ISoundOutput, IDisposable
    {
        private const int BUFFER_SIZE = 2048;
        private readonly int DIVIDER = Gameboy.TicksPerSec / 22050;

        private byte[] Buffer;
        private SoundEffect SoundEffect;

        private int i;
        private int tick;

        public GameboySoundOutput()
        {
            this.Buffer = new byte[BUFFER_SIZE];
        }

        public void Play(int left, int right)
        {
            if (tick++ != 0)
            {
                tick %= DIVIDER;
                return;
            }

            // TODO: make play signiture provide bytes

            Buffer[this.i++] = 0;
            Buffer[this.i++] = (byte)left;
            Buffer[this.i++] = 0;
            Buffer[this.i++] = (byte)right;

            if (this.i >= BUFFER_SIZE)
            {
                this.SoundEffect.Dispose();
                this.SoundEffect = new SoundEffect((byte[])this.Buffer.Clone(), ModEntry.Config.MusicSampleRate, AudioChannels.Stereo);
                // TODO: look into SoundEffect.CreateHandle.  Maybe I can use 8bit PCM natively
                this.SoundEffect.Play();
                
                this.i = 0;
                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    Buffer[i] = 0;
                }
            }
        }

        public void Start()
        {
            for (int i = 0; i < BUFFER_SIZE; i++)
            {
                Buffer[i] = 0;
            }
            this.SoundEffect = new SoundEffect((byte[])this.Buffer.Clone(), ModEntry.Config.MusicSampleRate, AudioChannels.Stereo);
        }

        public void Stop()
        {
            this.SoundEffect.Dispose();
        }

        public void Dispose()
        {
            if (this.SoundEffect is not null && !this.SoundEffect.IsDisposed)
            {
                this.SoundEffect.Dispose();
            }
        }
    }
}
