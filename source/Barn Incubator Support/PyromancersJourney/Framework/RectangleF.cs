/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace PyromancersJourney.Framework
{
    internal class RectangleF
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public RectangleF() { }
        public RectangleF(float x, float y, float w, float h)
        {
            this.X = x;
            this.Y = y;
            this.Width = w;
            this.Height = h;
        }

        public bool Intersects(RectangleF other)
        {
            if (this.X + this.Width < other.X || this.X > other.X + other.Width || this.Y + this.Height < other.Y || this.Y > other.Y + other.Height)
            {
                return false;
            }
            return true;
        }

        public static RectangleF operator +(RectangleF rect, Vector2 vec)
        {
            return new(rect.X + vec.X, rect.Y + vec.Y, rect.Width, rect.Height);
        }
    }
}
