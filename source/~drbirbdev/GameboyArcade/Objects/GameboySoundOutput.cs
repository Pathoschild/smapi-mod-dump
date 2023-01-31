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
using System.Threading;
using BirbShared;
using CoreBoy;
using CoreBoy.sound;
using Microsoft.Xna.Framework.Audio;
using StardewValley;

// TODO: not working
namespace GameboyArcade
{
    class GameboySoundOutput : ISoundOutput, IDisposable
    {
        private const int SAMPLE_RATE = 22050;
        private const int BUFFER_SIZE = 1024;
        private readonly int DIVIDER = Gameboy.TicksPerSec / SAMPLE_RATE;

        private byte[] Buffer;
        private SoundEffect Effect;
        private CueDefinition Cue;
        private SoundEffectInstance SoundInstance;

        private int i;
        private int tick;

        public GameboySoundOutput()
        {
            this.Buffer = new byte[BUFFER_SIZE];
            this.Effect = new SoundEffect(this.Buffer, SAMPLE_RATE, AudioChannels.Stereo);

            this.Cue = new CueDefinition();
            this.Cue.name = "gameboySound";
            this.Cue.instanceLimit = 1;
            this.Cue.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;
            this.Cue.SetSound(Effect, Game1.audioEngine.GetCategoryIndex("Sound"), false);
            Game1.soundBank.AddCue(this.Cue);

        }

        public void Play(int left, int right)
        {
            if (tick++ != 0)
            {
                tick %= DIVIDER;
                return;
            }

            // TODO: make play signiture provide bytes

            Buffer[this.i++] = (byte)left;
            Buffer[this.i++] = (byte)right;
            if (this.i >= BUFFER_SIZE)
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                this.Cue.SetSound(new SoundEffect(this.Buffer, SAMPLE_RATE, AudioChannels.Stereo), Game1.audioEngine.GetCategoryIndex("Sound"));
#pragma warning restore CA2000 // Dispose objects before losing scope
                Game1.playSound("gameboySound");
                this.i = 0;
                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    Buffer[i] = 0;
                }
            }
        }

        public void Start()
        {
            if (SoundInstance is not null)
            {
                Log.Debug("Sound already started");
                return;
            }
            Log.Debug("Start Sound");
            /*
            SoundInstance = Effect.CreateInstance();
            SoundInstance.Play();
            */
            Game1.playSound("gameboySound");
            for (int i = 0; i < BUFFER_SIZE; i++)
            {
                Buffer[i] = 0;
            }
        }

        public void Stop()
        {
            Log.Debug("Stop sound called, but ignored");
            /*
            if (SoundInstance is null)
            {
                Log.Debug("Sound wasn't started");
                return;
            }

            SoundInstance.IsLooped = false;
            while (SoundInstance.State == SoundState.Playing)
            {
                Thread.Sleep(1);
            }
            SoundInstance.Dispose();
            SoundInstance = null;
            */
        }

        public void Dispose()
        {
            this.Effect.Dispose();
        }
    }
}
