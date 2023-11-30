/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using static CoreBoy.cpu.BitUtils;

namespace CoreBoy.cpu
{
    public class Registers
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int E { get; set; }
        public int H { get; set; }
        public int L { get; set; }
        public int SP { get; set; }
        public int PC { get; set; }

        public Flags Flags { get; } = new Flags();

        public int AF => (this.A << 8) | this.Flags.FlagsByte;
        public int BC => (this.B << 8) | this.C;
        public int DE => (this.D << 8) | this.E;
        public int HL => (this.H << 8) | this.L;

        public void SetAf(int af)
        {
            this.A = GetMsb(af);
            this.Flags.SetFlagsByte(GetLsb(af));
        }

        public void SetBc(int bc)
        {
            this.B = GetMsb(bc);
            this.C = GetLsb(bc);
        }

        public void SetDe(int de)
        {
            this.D = GetMsb(de);
            this.E = GetLsb(de);
        }

        public void SetHl(int hl)
        {
            this.H = GetMsb(hl);
            this.L = GetLsb(hl);
        }

        public void IncrementPc() => this.PC = (this.PC + 1) & 0xffff;
        public void IncrementSp() => this.SP = (this.SP + 1) & 0xffff;
        public void DecrementSp() => this.SP = (this.SP - 1) & 0xffff;

        public override string ToString()
        {
            return
                $"AF={this.AF:X4}, BC={this.BC:X4}, DE={this.DE:X4}, HL={this.HL:X4}, SP={this.SP:X4}, PC={this.PC:X4}, {this.Flags}";
        }
    }
}