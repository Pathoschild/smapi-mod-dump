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

namespace CoreBoy.gpu
{
    public class DmgPixelFifo : IPixelFifo
    {
        public IntQueue Pixels { get; } = new IntQueue(16);
        private readonly IntQueue _palettes = new IntQueue(16);
        private readonly IntQueue _pixelType = new IntQueue(16); // 0 - bg, 1 - sprite

        private readonly IDisplay _display;
        private readonly MemoryRegisters _registers;

        public DmgPixelFifo(IDisplay display, MemoryRegisters registers)
        {
            this._display = display;
            this._registers = registers;
        }

        public int GetLength() => this.Pixels.Size();
        public void PutPixelToScreen() => this._display.PutDmgPixel(this.DequeuePixel());
        public void DropPixel() => this.DequeuePixel();

        public int DequeuePixel()
        {
            this._pixelType.Dequeue();
            return GetColor(this._palettes.Dequeue(), this.Pixels.Dequeue());
        }

        public void Enqueue8Pixels(int[] pixelLine, TileAttributes tileAttributes)
        {
            foreach (var p in pixelLine)
            {
                this.Pixels.Enqueue(p);
                this._palettes.Enqueue(this._registers.Get(GpuRegister.Bgp));
                this._pixelType.Enqueue(0);
            }
        }

        public void SetOverlay(int[] pixelLine, int offset, TileAttributes flags, int oamIndex)
        {
            var priority = flags.IsPriority();
            var overlayPalette = this._registers.Get(flags.GetDmgPalette());

            for (var j = offset; j < pixelLine.Length; j++)
            {
                var p = pixelLine[j];
                var i = j - offset;

                if (this._pixelType.Get(i) == 1)
                {
                    continue;
                }

                if ((priority && this.Pixels.Get(i) == 0) || (!priority && p != 0))
                {
                    this.Pixels.Set(i, p);
                    this._palettes.Set(i, overlayPalette);
                    this._pixelType.Set(i, 1);
                }
            }
        }

        private static int GetColor(int palette, int colorIndex) => 0b11 & (palette >> (colorIndex * 2));

        public void Clear()
        {
            this.Pixels.Clear();
            this._palettes.Clear();
            this._pixelType.Clear();
        }
    }
}