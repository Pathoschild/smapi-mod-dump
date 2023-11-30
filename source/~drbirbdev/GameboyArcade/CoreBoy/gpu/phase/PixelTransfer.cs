/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using CoreBoy.memory;

namespace CoreBoy.gpu.phase
{
    public class PixelTransfer : IGpuPhase
    {
        private readonly IPixelFifo _fifo;
        private readonly Fetcher _fetcher;
        private readonly MemoryRegisters _r;
        private readonly Lcdc _lcdc;
        private readonly bool _gbc;
        private OamSearch.SpritePosition[] _sprites;
        private int _droppedPixels;
        private int _x;
        private bool _window;

        public PixelTransfer(IAddressSpace videoRam0, IAddressSpace videoRam1, IAddressSpace oemRam, IDisplay display,
            Lcdc lcdc, MemoryRegisters r, bool gbc, ColorPalette bgPalette, ColorPalette oamPalette)
        {
            this._r = r;
            this._lcdc = lcdc;
            this._gbc = gbc;

            this._fifo = gbc
                ? new ColorPixelFifo(lcdc, display, bgPalette, oamPalette)
                : new DmgPixelFifo(display, r);

            this._fetcher = new Fetcher(this._fifo, videoRam0, videoRam1, oemRam, lcdc, r, gbc);
        }

        public PixelTransfer Start(OamSearch.SpritePosition[] sprites)
        {
            this._sprites = sprites;
            this._droppedPixels = 0;
            this._x = 0;
            this._window = false;

            this._fetcher.Init();
            if (this._gbc || this._lcdc.IsBgAndWindowDisplay())
            {
                this.StartFetchingBackground();
            }
            else
            {
                this._fetcher.FetchingDisabled();
            }

            return this;
        }

        public bool Tick()
        {
            this._fetcher.Tick();
            if (this._lcdc.IsBgAndWindowDisplay() || this._gbc)
            {
                if (this._fifo.GetLength() <= 8)
                {
                    return true;
                }

                if (this._droppedPixels < this._r.Get(GpuRegister.Scx) % 8)
                {
                    this._fifo.DropPixel();
                    this._droppedPixels++;
                    return true;
                }

                if (!this._window && this._lcdc.IsWindowDisplay() && this._r.Get(GpuRegister.Ly) >= this._r.Get(GpuRegister.Wy) &&
                    this._x == this._r.Get(GpuRegister.Wx) - 7)
                {
                    this._window = true;
                    this.StartFetchingWindow();
                    return true;
                }
            }

            if (this._lcdc.IsObjDisplay())
            {
                if (this._fetcher.SpriteInProgress())
                {
                    return true;
                }

                bool spriteAdded = false;
                for (int i = 0; i < this._sprites.Length; i++)
                {
                    var s = this._sprites[i];
                    if (s == null)
                    {
                        continue;
                    }

                    if (this._x == 0 && s.GetX() < 8)
                    {
                        this._fetcher.AddSprite(s, 8 - s.GetX(), i);
                        spriteAdded = true;

                        this._sprites[i] = null;
                    }
                    else if (s.GetX() - 8 == this._x)
                    {
                        this._fetcher.AddSprite(s, 0, i);
                        spriteAdded = true;

                        this._sprites[i] = null;
                    }

                    if (spriteAdded)
                    {
                        return true;
                    }
                }
            }

            this._fifo.PutPixelToScreen();
            if (++this._x == 160)
            {
                return false;
            }

            return true;
        }

        private void StartFetchingBackground()
        {
            int bgX = this._r.Get(GpuRegister.Scx) / 0x08;
            int bgY = (this._r.Get(GpuRegister.Scy) + this._r.Get(GpuRegister.Ly)) % 0x100;

            this._fetcher.StartFetching(this._lcdc.GetBgTileMapDisplay() + (bgY / 0x08 * 0x20), this._lcdc.GetBgWindowTileData(), bgX,
                this._lcdc.IsBgWindowTileDataSigned(), bgY % 0x08);
        }

        private void StartFetchingWindow()
        {
            int winX = (this._x - this._r.Get(GpuRegister.Wx) + 7) / 0x08;
            int winY = this._r.Get(GpuRegister.Ly) - this._r.Get(GpuRegister.Wy);

            this._fetcher.StartFetching(this._lcdc.GetWindowTileMapDisplay() + (winY / 0x08 * 0x20), this._lcdc.GetBgWindowTileData(),
                winX, this._lcdc.IsBgWindowTileDataSigned(), winY % 0x08);
        }

    }
}
