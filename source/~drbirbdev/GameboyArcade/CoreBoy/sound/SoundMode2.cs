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

namespace CoreBoy.sound
{

    public class SoundMode2 : SoundModeBase
    {
        private int _freqDivider;
        private int _lastOutput;
        private int _i;
        private readonly VolumeEnvelope _volumeEnvelope;

        public SoundMode2(bool gbc)
            : base(0xff15, 64, gbc)
        {
            this._volumeEnvelope = new VolumeEnvelope();
        }

        public override void Start()
        {
            this._i = 0;
            if (this.Gbc)
            {
                this.Length.Reset();
            }

            this.Length.Start();
            this._volumeEnvelope.Start();
        }


        protected override void Trigger()
        {
            this._i = 0;
            this._freqDivider = 1;
            this._volumeEnvelope.Trigger();
        }


        public override int Tick()
        {
            this._volumeEnvelope.Tick();

            bool e = this.UpdateLength();
            e = this.DacEnabled && e;
            if (!e)
            {
                return 0;
            }

            if (--this._freqDivider == 0)
            {
                this.ResetFreqDivider();
                this._lastOutput = (this.GetDuty() & (1 << this._i)) >> this._i;
                this._i = (this._i + 1) % 8;
            }

            return this._lastOutput * this._volumeEnvelope.GetVolume();
        }

        protected override void SetNr1(int value)
        {
            base.SetNr1(value);
            this.Length.SetLength(64 - (value & 0b00111111));
        }

        protected override void SetNr2(int value)
        {
            base.SetNr2(value);
            this._volumeEnvelope.SetNr2(value);
            this.DacEnabled = (value & 0b11111000) != 0;
            this.ChannelEnabled &= this.DacEnabled;
        }

        private int GetDuty()
        {
            int i = this.GetNr1() >> 6;
            return i switch
            {
                0 => 0b00000001,
                1 => 0b10000001,
                2 => 0b10000111,
                3 => 0b01111110,
                _ => throw new InvalidOperationException("Illegal operation")
            };
        }

        private void ResetFreqDivider()
        {
            this._freqDivider = this.GetFrequency() * 4;
        }
    }
}