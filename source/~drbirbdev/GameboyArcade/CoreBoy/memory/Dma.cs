/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using CoreBoy.cpu;

namespace CoreBoy.memory
{
    public class Dma : IAddressSpace
    {
        private readonly IAddressSpace _addressSpace;
        private readonly IAddressSpace _oam;
        private readonly SpeedMode _speedMode;

        private bool _transferInProgress;
        private bool _restarted;
        private int _from;
        private int _ticks;
        private int _regValue = 0xff;

        public Dma(IAddressSpace addressSpace, IAddressSpace oam, SpeedMode speedMode)
        {
            this._addressSpace = new DmaAddressSpace(addressSpace);
            this._speedMode = speedMode;
            this._oam = oam;
        }

        public bool Accepts(int address)
        {
            return address == 0xff46;
        }

        public void Tick()
        {
            if (!this._transferInProgress) return;
            if (++this._ticks < 648 / this._speedMode.GetSpeedMode()) return;

            this._transferInProgress = false;
            this._restarted = false;
            this._ticks = 0;

            for (var i = 0; i < 0xa0; i++)
            {
                this._oam.SetByte(0xfe00 + i, this._addressSpace.GetByte(this._from + i));
            }
        }

        public void SetByte(int address, int value)
        {
            this._from = value * 0x100;
            this._restarted = this.IsOamBlocked();
            this._ticks = 0;
            this._transferInProgress = true;
            this._regValue = value;
        }

        public int GetByte(int address) => this._regValue;
        public bool IsOamBlocked() => this._restarted || (this._transferInProgress && this._ticks >= 5);
    }
}