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
    public class FrequencySweep
    {
        private static readonly int Divider = Gameboy.TicksPerSec / 128;

        // sweep parameters
        private int _period;
        private bool _negate;
        private int _shift;

        // current process variables
        private int _timer;
        private int _shadowFreq;
        private int _nr13;
        private int _nr14;
        private int _i;
        private bool _overflow;
        private bool _counterEnabled;
        private bool _negging;

        public void Start()
        {
            this._counterEnabled = false;
            this._i = 8192;
        }

        public void Trigger()
        {
            this._negging = false;
            this._overflow = false;

            this._shadowFreq = this._nr13 | ((this._nr14 & 0b111) << 8);
            this._timer = this._period == 0 ? 8 : this._period;
            this._counterEnabled = this._period != 0 || this._shift != 0;

            if (this._shift > 0)
            {
                this.Calculate();
            }
        }

        public void SetNr10(int value)
        {
            this._period = (value >> 4) & 0b111;
            this._negate = (value & (1 << 3)) != 0;
            this._shift = value & 0b111;
            if (this._negging && !this._negate)
            {
                this._overflow = true;
            }
        }

        public void SetNr13(int value) => this._nr13 = value;

        public void SetNr14(int value)
        {
            this._nr14 = value;
            if ((value & (1 << 7)) != 0)
            {
                this.Trigger();
            }
        }

        public int GetNr13() => this._nr13;
        public int GetNr14() => this._nr14;

        public void Tick()
        {
            this._i++;

            if (this._i != Divider) return;

            this._i = 0;

            if (!this._counterEnabled) return;

            this._timer--;

            if (this._timer != 0) return;

            this._timer = this._period == 0 ? 8 : this._period;

            if (this._period == 0) return;

            var newFreq = this.Calculate();

            if (this._overflow || this._shift == 0) return;

            this._shadowFreq = newFreq;
            this._nr13 = this._shadowFreq & 0xff;
            this._nr14 = (this._shadowFreq & 0x700) >> 8;

            this.Calculate();
        }

        private int Calculate()
        {
            var freq = this._shadowFreq >> this._shift;
            if (this._negate)
            {
                freq = this._shadowFreq - freq;
                this._negging = true;
            }
            else
            {
                freq = this._shadowFreq + freq;
            }

            if (freq > 2047)
            {
                this._overflow = true;
            }

            return freq;
        }

        public bool IsEnabled() => !this._overflow;
    }
}