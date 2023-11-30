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

    public class SoundMode1 : SoundModeBase
    {
        private int FreqDivider;
        private int LastOutput;
        private int I;
        private readonly FrequencySweep FrequencySweep;
        private readonly VolumeEnvelope VolumeEnvelope;

        public SoundMode1(bool gbc) : base(0xff10, 64, gbc)
        {
            this.FrequencySweep = new FrequencySweep();
            this.VolumeEnvelope = new VolumeEnvelope();
        }

        public override void Start()
        {
            this.I = 0;
            if (this.Gbc)
            {
                this.Length.Reset();
            }

            this.Length.Start();
            this.FrequencySweep.Start();
            this.VolumeEnvelope.Start();
        }

        protected override void Trigger()
        {
            this.I = 0;
            this.FreqDivider = 1;
            this.VolumeEnvelope.Trigger();
        }

        public override int Tick()
        {
            this.VolumeEnvelope.Tick();

            bool e = this.UpdateLength();
            e = this.UpdateSweep() && e;
            e = this.DacEnabled && e;
            if (!e)
            {
                return 0;
            }

            if (--this.FreqDivider == 0)
            {
                this.ResetFreqDivider();
                this.LastOutput = (this.GetDuty() & (1 << this.I)) >> this.I;
                this.I = (this.I + 1) % 8;
            }

            return this.LastOutput * this.VolumeEnvelope.GetVolume();
        }

        protected override void SetNr0(int value)
        {
            base.SetNr0(value);
            this.FrequencySweep.SetNr10(value);
        }

        protected override void SetNr1(int value)
        {
            base.SetNr1(value);
            this.Length.SetLength(64 - (value & 0b00111111));
        }

        protected override void SetNr2(int value)
        {
            base.SetNr2(value);
            this.VolumeEnvelope.SetNr2(value);
            this.DacEnabled = (value & 0b11111000) != 0;
            this.ChannelEnabled &= this.DacEnabled;
        }

        protected override void SetNr3(int value)
        {
            base.SetNr3(value);
            this.FrequencySweep.SetNr13(value);
        }

        protected override void SetNr4(int value)
        {
            base.SetNr4(value);
            this.FrequencySweep.SetNr14(value);
        }

        protected override int GetNr3()
        {
            return this.FrequencySweep.GetNr13();
        }

        protected override int GetNr4()
        {
            return (base.GetNr4() & 0b11111000) | (this.FrequencySweep.GetNr14() & 0b00000111);
        }

        private int GetDuty()
        {
            return (this.GetNr1() >> 6) switch
            {
                0 => 0b00000001,
                1 => 0b10000001,
                2 => 0b10000111,
                3 => 0b01111110,
                _ => throw new InvalidOperationException("Illegal state exception"),
            };
        }

        private void ResetFreqDivider()
        {
            this.FreqDivider = this.GetFrequency() * 4;
        }

        protected bool UpdateSweep()
        {
            this.FrequencySweep.Tick();
            if (this.ChannelEnabled && !this.FrequencySweep.IsEnabled())
            {
                this.ChannelEnabled = false;
            }

            return this.ChannelEnabled;
        }
    }

}
