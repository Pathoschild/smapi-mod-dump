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
    public interface IPixelFifo
    {
        int GetLength();
        void PutPixelToScreen();
        void DropPixel();
        void Enqueue8Pixels(int[] pixels, TileAttributes tileAttributes);
        void SetOverlay(int[] pixelLine, int offset, TileAttributes flags, int oamIndex);
        void Clear();
    }
}