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
    public class LengthCounter
    {
        private long _i;
        private readonly int _divider = Gameboy.TicksPerSec / 256;
        private readonly int _fullLength;

        public bool Enabled { get; private set; }
        public int Length { get; private set; }

        public LengthCounter(int fullLength)
        {
            this._fullLength = fullLength;
        }

        public void Start()
        {
            this._i = 8192;
        }

        public void Tick()
        {
            this._i++;

            if (this._i == this._divider)
            {
                this._i = 0;
                if (this.Enabled && this.Length > 0)
                {
                    this.Length--;
                }
            }
        }

        public void SetLength(int len)
        {
            this.Length = len == 0 ? this._fullLength : len;
        }

        public void SetNr4(int value)
        {
            var enable = (value & (1 << 6)) != 0;
            var trigger = (value & (1 << 7)) != 0;

            if (this.Enabled)
            {
                if (this.Length == 0 && trigger)
                {
                    if (enable && this._i < this._divider / 2)
                    {
                        this.SetLength(this._fullLength - 1);
                    }
                    else
                    {
                        this.SetLength(this._fullLength);
                    }
                }
            }
            else if (enable)
            {
                if (this.Length > 0 && this._i < this._divider / 2)
                {
                    this.Length--;
                }

                if (this.Length == 0 && trigger && this._i < this._divider / 2)
                {
                    this.SetLength(this._fullLength - 1);
                }
            }
            else
            {
                if (this.Length == 0 && trigger)
                {
                    this.SetLength(this._fullLength);
                }
            }

            this.Enabled = enable;
        }

        public override string ToString()
        {
            return $"LengthCounter[l={this.Length},f={this._fullLength},c={this._i},{(this.Enabled ? "enabled" : "disabled")}]";
        }

        public void Reset()
        {
            this.Enabled = true;
            this._i = 0;
            this.Length = 0;
        }
    }

}