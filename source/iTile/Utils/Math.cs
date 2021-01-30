/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace iTile.Utils
{
    public static class Math
    {
        public static Point NegativePoint()
        {
            return new Point(-1, -1);
        }

        public static bool OnIntervalStrict(this int num, int a, int b)
        {
            return num > a && num < b;
        }

        public static Point GetDelta(int x1, int x2, int y1, int y2)
        {
            return new Point(x2 - x2, y2 - y1);
        }

        public static Point GetDelta(Point p1, Point p2)
        {
            return new Point(p2.X - p1.X, p2.Y - p1.Y);
        }

        public static Point GetDelta(Rectangle r1, Rectangle r2, bool pos = true)
        {
            return pos ? new Point(r2.X - r1.X, r2.Y - r1.Y) : new Point(r2.Width - r1.Width, r2.Height - r1.Height);
        }
    }
}
