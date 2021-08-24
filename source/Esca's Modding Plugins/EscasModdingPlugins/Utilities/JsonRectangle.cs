/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using Rectangle = System.Drawing.Rectangle;

namespace EscasModdingPlugins
{
    /// <summary>A basic <see cref="Rectangle"/> wrapper for convenient JSON serialization.</summary>
    public struct JsonRectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public JsonRectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>Creates an equivalent rectangle instance.</summary>
        /// <returns>A new rectangle matching this instance.</returns>
        public Rectangle AsRect() { return new Rectangle(X, Y, Width, Height); }
    }
}
