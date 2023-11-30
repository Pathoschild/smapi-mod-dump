/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using CoreBoy.cpu.op;

namespace CoreBoy.cpu.opcode
{
    public class Opcode
    {
        public int Value { get; }
        public string Label { get; }
        public List<Op> Ops { get; }
        public int Length { get; }

        public Opcode(OpcodeBuilder builder)
        {
            this.Value = builder.GetOpcode();
            this.Label = builder.GetLabel();
            this.Ops = builder.GetOps();
            this.Length = this.Ops.Count <= 0 ? 0 : this.Ops.Max(o => o.OperandLength());
        }

        public override string ToString() => $"{this.Value:X2} {this.Label}";
    }
}