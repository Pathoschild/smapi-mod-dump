/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

namespace CoreBoy.gpu
{
    public class ColorPixelFifo : IPixelFifo
    {
        private readonly IntQueue _pixels = new IntQueue(16);
        private readonly IntQueue _palettes = new IntQueue(16);
        private readonly IntQueue _priorities = new IntQueue(16);
        private readonly Lcdc _lcdc;
        private readonly IDisplay _display;
        private readonly ColorPalette _bgPalette;
        private readonly ColorPalette _oamPalette;

        public ColorPixelFifo(Lcdc lcdc, IDisplay display, ColorPalette bgPalette, ColorPalette oamPalette)
        {
            this._lcdc = lcdc;
            this._display = display;
            this._bgPalette = bgPalette;
            this._oamPalette = oamPalette;
        }

        public int GetLength() => this._pixels.Size();
        public void PutPixelToScreen() => this._display.PutColorPixel(this.DequeuePixel());

        private int DequeuePixel()
        {
            return this.GetColor(this._priorities.Dequeue(), this._palettes.Dequeue(), this._pixels.Dequeue());
        }

        public void DropPixel() => this.DequeuePixel();

        public void Enqueue8Pixels(int[] pixelLine, TileAttributes tileAttributes)
        {
            foreach (int p in pixelLine)
            {
                this._pixels.Enqueue(p);
                this._palettes.Enqueue(tileAttributes.GetColorPaletteIndex());
                this._priorities.Enqueue(tileAttributes.IsPriority() ? 100 : -1);
            }
        }

        /*
        lcdc.0
    
        when 0 => sprites are always displayed on top of the bg
    
        bg tile attribute.7
    
        when 0 => use oam priority bit
        when 1 => bg priority
    
        sprite attribute.7
    
        when 0 => sprite above bg
        when 1 => sprite above bg color 0
         */

        public void SetOverlay(int[] pixelLine, int offset, TileAttributes spriteAttr, int oamIndex)
        {
            for (int j = offset; j < pixelLine.Length; j++)
            {
                int p = pixelLine[j];
                int i = j - offset;
                if (p == 0)
                {
                    continue; // color 0 is always transparent
                }

                int oldPriority = this._priorities.Get(i);

                bool put = false;
                if ((oldPriority == -1 || oldPriority == 100) && !this._lcdc.IsBgAndWindowDisplay())
                {
                    // this one takes precedence
                    put = true;
                }
                else if (oldPriority == 100)
                {
                    // bg with priority
                    put = this._pixels.Get(i) == 0;
                }
                else if (oldPriority == -1 && !spriteAttr.IsPriority())
                {
                    // bg without priority
                    put = true;
                }
                else if (oldPriority == -1 && spriteAttr.IsPriority() && this._pixels.Get(i) == 0)
                {
                    // bg without priority
                    put = true;
                }
                else if (oldPriority >= 0 && oldPriority < 10)
                {
                    // other sprite
                    put = oldPriority > oamIndex;
                }

                if (put)
                {
                    this._pixels.Set(i, p);
                    this._palettes.Set(i, spriteAttr.GetColorPaletteIndex());
                    this._priorities.Set(i, oamIndex);
                }
            }
        }

        public void Clear()
        {
            this._pixels.Clear();
            this._palettes.Clear();
            this._priorities.Clear();
        }

        private int GetColor(int priority, int palette, int color)
        {
            return priority >= 0 && priority < 10
                ? this._oamPalette.GetPalette(palette)[color]
                : this._bgPalette.GetPalette(palette)[color];
        }
    }
}
