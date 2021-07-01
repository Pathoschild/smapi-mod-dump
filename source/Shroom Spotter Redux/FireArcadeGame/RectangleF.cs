/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace FireArcadeGame
{
    public class RectangleF
    {
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;
        public float Width { get; set; } = 0;
        public float Height { get; set; } = 0;

        public RectangleF() { }
        public RectangleF( float x, float y, float w, float h )
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public bool Intersects( RectangleF other )
        {
            if ( X + Width < other.X || X > other.X + other.Width ||
                 Y + Height < other.Y || Y > other.Y + other.Height )
            {
                return false;
            }
            return true;
        }

        public static RectangleF operator + ( RectangleF rect, Vector2 vec )
        {
            return new RectangleF( rect.X + vec.X, rect.Y + vec.Y, rect.Width, rect.Height );
        }
    }
}
