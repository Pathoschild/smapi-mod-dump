/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Text;
using static CoreBoy.cpu.BitUtils;

namespace CoreBoy.cpu
{
    public class Flags
    {
        public int FlagsByte { get; private set; }

        private static readonly int Z_POS = 7;
        private static readonly int N_POS = 6;
        private static readonly int H_POS = 5;
        private static readonly int C_POS = 4;

        public bool IsZ() => GetBit(this.FlagsByte, Z_POS);
        public bool IsN() => GetBit(this.FlagsByte, N_POS);
        public bool IsH() => GetBit(this.FlagsByte, H_POS);
        public bool IsC() => GetBit(this.FlagsByte, C_POS);
        public void SetZ(bool z) => this.FlagsByte = SetBit(this.FlagsByte, Z_POS, z);
        public void SetN(bool n) => this.FlagsByte = SetBit(this.FlagsByte, N_POS, n);
        public void SetH(bool h) => this.FlagsByte = SetBit(this.FlagsByte, H_POS, h);
        public void SetC(bool c) => this.FlagsByte = SetBit(this.FlagsByte, C_POS, c);
        public void SetFlagsByte(int flags) => this.FlagsByte = flags & 0xf0;

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append(this.IsZ() ? 'Z' : '-');
            result.Append(this.IsN() ? 'N' : '-');
            result.Append(this.IsH() ? 'H' : '-');
            result.Append(this.IsC() ? 'C' : '-');
            result.Append("----");
            return result.ToString();
        }
    }
}