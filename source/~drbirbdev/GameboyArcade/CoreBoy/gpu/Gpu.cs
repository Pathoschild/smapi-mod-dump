/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Linq;
using CoreBoy.cpu;
using CoreBoy.gpu.phase;
using CoreBoy.memory;

namespace CoreBoy.gpu
{
    public class Gpu : IAddressSpace
    {
        public enum Mode
        {
            HBlank,
            VBlank,
            OamSearch,
            PixelTransfer
        }

        private readonly IAddressSpace _videoRam0;
        private readonly IAddressSpace _videoRam1;
        private readonly IAddressSpace _oamRam;
        private readonly IDisplay _display;
        private readonly InterruptManager _interruptManager;
        private readonly Dma _dma;
        private readonly Lcdc _lcdc;
        private readonly bool _gbc;
        private readonly ColorPalette _bgPalette;
        private readonly ColorPalette _oamPalette;
        private readonly HBlankPhase _hBlankPhase;
        private readonly OamSearch _oamSearchPhase;
        private readonly PixelTransfer _pixelTransferPhase;
        private readonly VBlankPhase _vBlankPhase;
        private readonly MemoryRegisters _r;

        private bool _lcdEnabled = true;
        private int _lcdEnabledDelay;
        private int _ticksInLine;
        private Mode _mode;
        private IGpuPhase _phase;

        public Gpu(IDisplay display, InterruptManager interruptManager, Dma dma, Ram oamRam, bool gbc)
        {
            this._r = new MemoryRegisters(GpuRegister.Values().ToArray());
            this._lcdc = new Lcdc();
            this._interruptManager = interruptManager;
            this._gbc = gbc;
            this._videoRam0 = new Ram(0x8000, 0x2000);
            this._videoRam1 = gbc ? new Ram(0x8000, 0x2000) : null;
            this._oamRam = oamRam;
            this._dma = dma;

            this._bgPalette = new ColorPalette(0xff68);
            this._oamPalette = new ColorPalette(0xff6a);
            this._oamPalette.FillWithFf();

            this._oamSearchPhase = new OamSearch(oamRam, this._lcdc, this._r);
            this._pixelTransferPhase = new PixelTransfer(this._videoRam0, this._videoRam1, oamRam, display, this._lcdc, this._r, gbc, this._bgPalette,
                this._oamPalette);
            this._hBlankPhase = new HBlankPhase();
            this._vBlankPhase = new VBlankPhase();

            this._mode = Mode.OamSearch;
            this._phase = this._oamSearchPhase.Start();

            this._display = display;
        }

        private IAddressSpace GetAddressSpace(int address)
        {
            if (this._videoRam0.Accepts(address) /* && mode != Mode.PixelTransfer*/)
            {
                return this.GetVideoRam();
            }

            if (this._oamRam.Accepts(address) &&
                !this._dma.IsOamBlocked() /* && mode != Mode.OamSearch && mode != Mode.PixelTransfer*/)
            {
                return this._oamRam;
            }

            if (this._lcdc.Accepts(address))
            {
                return this._lcdc;
            }

            if (this._r.Accepts(address))
            {
                return this._r;
            }

            if (this._gbc && this._bgPalette.Accepts(address))
            {
                return this._bgPalette;
            }

            if (this._gbc && this._oamPalette.Accepts(address))
            {
                return this._oamPalette;
            }

            return null;
        }

        private IAddressSpace GetVideoRam()
        {
            if (this._gbc && (this._r.Get(GpuRegister.Vbk) & 1) == 1)
            {
                return this._videoRam1;
            }

            return this._videoRam0;
        }

        public bool Accepts(int address) => this.GetAddressSpace(address) != null;

        public void SetByte(int address, int value)
        {
            if (address == GpuRegister.Stat.Address)
            {
                this.SetStat(value);
                return;
            }

            var space = this.GetAddressSpace(address);
            if (space == this._lcdc)
            {
                this.SetLcdc(value);
                return;
            }

            space?.SetByte(address, value);
        }

        public int GetByte(int address)
        {
            if (address == GpuRegister.Stat.Address)
            {
                return this.GetStat();
            }

            var space = this.GetAddressSpace(address);
            if (space == null)
            {
                return 0xff;
            }

            if (address == GpuRegister.Vbk.Address)
            {
                return this._gbc ? 0xfe : 0xff;
            }

            return space.GetByte(address);
        }

        public Mode? Tick()
        {
            if (!this._lcdEnabled)
            {
                if (this._lcdEnabledDelay != -1)
                {
                    if (--this._lcdEnabledDelay == 0)
                    {
                        this._display.Enabled = true;
                        this._lcdEnabled = true;
                    }
                }
            }

            if (!this._lcdEnabled)
            {
                return null;
            }

            var oldMode = this._mode;
            this._ticksInLine++;
            if (this._phase.Tick())
            {
                // switch line 153 to 0
                if (this._ticksInLine == 4 && this._mode == Mode.VBlank && this._r.Get(GpuRegister.Ly) == 153)
                {
                    this._r.Put(GpuRegister.Ly, 0);
                    this.RequestLycEqualsLyInterrupt();
                }
            }
            else
            {
                switch (oldMode)
                {
                    case Mode.OamSearch:
                        this._mode = Mode.PixelTransfer;
                        this._phase = this._pixelTransferPhase.Start(this._oamSearchPhase.GetSprites());
                        break;

                    case Mode.PixelTransfer:
                        this._mode = Mode.HBlank;
                        this._phase = this._hBlankPhase.Start(this._ticksInLine);
                        this.RequestLcdcInterrupt(3);
                        break;

                    case Mode.HBlank:
                        this._ticksInLine = 0;
                        if (this._r.PreIncrement(GpuRegister.Ly) == 144)
                        {
                            this._mode = Mode.VBlank;
                            this._phase = this._vBlankPhase.Start();
                            this._interruptManager.RequestInterrupt(InterruptManager.InterruptType.VBlank);
                            this.RequestLcdcInterrupt(4);
                        }
                        else
                        {
                            this._mode = Mode.OamSearch;
                            this._phase = this._oamSearchPhase.Start();
                        }

                        this.RequestLcdcInterrupt(5);
                        this.RequestLycEqualsLyInterrupt();
                        break;

                    case Mode.VBlank:
                        this._ticksInLine = 0;
                        if (this._r.PreIncrement(GpuRegister.Ly) == 1)
                        {
                            this._mode = Mode.OamSearch;
                            this._r.Put(GpuRegister.Ly, 0);
                            this._phase = this._oamSearchPhase.Start();
                            this.RequestLcdcInterrupt(5);
                        }
                        else
                        {
                            this._phase = this._vBlankPhase.Start();
                        }

                        this.RequestLycEqualsLyInterrupt();
                        break;
                }
            }

            if (oldMode == this._mode)
            {
                return null;
            }

            return this._mode;
        }

        public int GetTicksInLine()
        {
            return this._ticksInLine;
        }

        private void RequestLcdcInterrupt(int statBit)
        {
            if ((this._r.Get(GpuRegister.Stat) & (1 << statBit)) != 0)
            {
                this._interruptManager.RequestInterrupt(InterruptManager.InterruptType.Lcdc);
            }
        }

        private void RequestLycEqualsLyInterrupt()
        {
            if (this._r.Get(GpuRegister.Lyc) == this._r.Get(GpuRegister.Ly))
            {
                this.RequestLcdcInterrupt(6);
            }
        }

        private int GetStat()
        {
            return this._r.Get(GpuRegister.Stat) | (int)this._mode |
                   (this._r.Get(GpuRegister.Lyc) == this._r.Get(GpuRegister.Ly) ? (1 << 2) : 0) | 0x80;
        }

        private void SetStat(int value)
        {
            this._r.Put(GpuRegister.Stat, value & 0b11111000); // last three bits are read-only
        }

        private void SetLcdc(int value)
        {
            this._lcdc.Set(value);
            if ((value & (1 << 7)) == 0)
            {
                this.DisableLcd();
            }
            else
            {
                this.EnableLcd();
            }
        }

        private void DisableLcd()
        {
            this._r.Put(GpuRegister.Ly, 0);
            this._ticksInLine = 0;
            this._phase = this._hBlankPhase.Start(250);
            this._mode = Mode.HBlank;
            this._lcdEnabled = false;
            this._lcdEnabledDelay = -1;
            this._display.Enabled = false;
        }

        private void EnableLcd()
        {
            this._lcdEnabledDelay = 244;
        }

        public bool IsLcdEnabled()
        {
            return this._lcdEnabled;
        }

        public Lcdc GetLcdc()
        {
            return this._lcdc;
        }
    }
}