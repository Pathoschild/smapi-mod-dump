/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.sound
{
    public class SoundMode4 : SoundModeBase
    {
        private int _lastResult;
        private readonly VolumeEnvelope _volumeEnvelope;
        private readonly PolynomialCounter _polynomialCounter;
        private readonly Lfsr _lfsr = new Lfsr();

        public SoundMode4(bool gbc) : base(0xff1f, 64, gbc)
        {
            this._volumeEnvelope = new VolumeEnvelope();
            this._polynomialCounter = new PolynomialCounter();
        }

        public override void Start()
        {
            if (this.Gbc)
            {
                this.Length.Reset();
            }

            this.Length.Start();
            this._lfsr.Start();
            this._volumeEnvelope.Start();
        }


        protected override void Trigger()
        {
            this._lfsr.Reset();
            this._volumeEnvelope.Trigger();
        }

        public override int Tick()
        {
            this._volumeEnvelope.Tick();

            if (!this.UpdateLength())
            {
                return 0;
            }

            if (!this.DacEnabled)
            {
                return 0;
            }

            if (this._polynomialCounter.Tick())
            {
                this._lastResult = this._lfsr.NextBit((this.Nr3 & (1 << 3)) != 0);
            }

            return this._lastResult * this._volumeEnvelope.GetVolume();
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

        protected override void SetNr3(int value)
        {
            base.SetNr3(value);
            this._polynomialCounter.SetNr43(value);
        }
    }
}