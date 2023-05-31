/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace PlayerCoordinates
{
    public struct Coordinates
    {
        public int x, y;

        public Coordinates(int newX, int newY)
        {
            this.x = newX;
            this.y = newY;
        }

        public static implicit operator Coordinates(Vector2 v)
        {
            return new Coordinates((int)v.X, (int)v.Y);
        }

        public override string ToString()
        {
            return $"X: {this.x}, Y: {this.y}";
        }
    }
}
