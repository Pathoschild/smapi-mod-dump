/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using CoreBoy.gpu;

namespace CoreBoy.cpu.op
{
    public abstract class Op
    {
        public virtual bool ReadsMemory() => false;
        public virtual bool WritesMemory() => false;
        public virtual int OperandLength() => 0;
        public virtual int Execute(Registers registers, IAddressSpace addressSpace, int[] args, int context) => context;
        public virtual void SwitchInterrupts(InterruptManager interruptManager)
        {
        }

        public virtual bool Proceed(Registers registers) => true;
        public virtual bool ForceFinishCycle() => false;
        public virtual SpriteBug.CorruptionType? CausesOemBug(Registers registers, int context) => null;
        protected static bool InOamArea(int address) => address >= 0xfe00 && address <= 0xfeff;
    }
}