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
    public class Lfsr
    {
        public int Value { get; private set; }

        public Lfsr() => this.Reset();
        public void Start() => this.Reset();
        public void Reset() => this.Value = 0x7fff;

        public int NextBit(bool widthMode7)
        {
            bool x = ((this.Value & 1) ^ ((this.Value & 2) >> 1)) != 0;
            this.Value = this.Value >> 1;
            this.Value = this.Value | (x ? (1 << 14) : 0);

            if (widthMode7)
            {
                this.Value = this.Value | (x ? (1 << 6) : 0);
            }

            return 1 & ~this.Value;
        }
    }
}