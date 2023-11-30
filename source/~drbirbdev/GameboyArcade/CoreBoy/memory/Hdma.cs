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
using CoreBoy.gpu;

namespace CoreBoy.memory
{
    public class Hdma : IAddressSpace
    {
        private const int Hdma1 = 0xff51;
        private const int Hdma2 = 0xff52;
        private const int Hdma3 = 0xff53;
        private const int Hdma4 = 0xff54;
        private const int Hdma5 = 0xff55;

        private readonly IAddressSpace _addressSpace;
        private readonly Ram _hdma1234 = new Ram(Hdma1, 4);
        private Gpu.Mode? _gpuMode;

        private bool _transferInProgress;
        private bool _hblankTransfer;
        private bool _lcdEnabled;

        private int _length;
        private int _src;
        private int _dst;
        private int _tick;

        public Hdma(IAddressSpace addressSpace) => this._addressSpace = addressSpace;
        public bool Accepts(int address) => address >= Hdma1 && address <= Hdma5;

        public void Tick()
        {
            if (!this.IsTransferInProgress())
            {
                return;
            }

            if (++this._tick < 0x20)
            {
                return;
            }

            for (int j = 0; j < 0x10; j++)
            {
                this._addressSpace.SetByte(this._dst + j, this._addressSpace.GetByte(this._src + j));
            }

            this._src += 0x10;
            this._dst += 0x10;
            if (this._length-- == 0)
            {
                this._transferInProgress = false;
                this._length = 0x7f;
            }
            else if (this._hblankTransfer)
            {
                this._gpuMode = null; // wait until next HBlank
            }
        }

        public void SetByte(int address, int value)
        {
            if (this._hdma1234.Accepts(address))
            {
                this._hdma1234.SetByte(address, value);
            }
            else if (address == Hdma5)
            {
                //if (_transferInProgress && (address & (1 << 7)) == 0) // Apparently the second part of this expression is always true
                if (this._transferInProgress)
                {
                    this.StopTransfer();
                }
                else
                {
                    this.StartTransfer(value);
                }
            }
        }

        public int GetByte(int address)
        {
            if (this._hdma1234.Accepts(address))
            {
                return 0xff;
            }

            if (address == Hdma5)
            {
                return (this._transferInProgress ? 0 : (1 << 7)) | this._length;
            }

            throw new ArgumentException();
        }

        public void OnGpuUpdate(Gpu.Mode newGpuMode) => this._gpuMode = newGpuMode;
        public void OnLcdSwitch(bool lcdEnabled) => this._lcdEnabled = lcdEnabled;

        public bool IsTransferInProgress()
        {
            if (!this._transferInProgress)
            {
                return false;
            }

            if (this._hblankTransfer && (this._gpuMode == Gpu.Mode.HBlank || !this._lcdEnabled))
            {
                return true;
            }

            return !this._hblankTransfer;
        }

        private void StartTransfer(int reg)
        {
            this._hblankTransfer = (reg & (1 << 7)) != 0;
            this._length = reg & 0x7f;

            this._src = (this._hdma1234.GetByte(Hdma1) << 8) | (this._hdma1234.GetByte(Hdma2) & 0xf0);
            this._dst = ((this._hdma1234.GetByte(Hdma3) & 0x1f) << 8) | (this._hdma1234.GetByte(Hdma4) & 0xf0);
            this._src = this._src & 0xfff0;
            this._dst = (this._dst & 0x1fff) | 0x8000;

            this._transferInProgress = true;
        }

        private void StopTransfer() => this._transferInProgress = false;
    }
}
