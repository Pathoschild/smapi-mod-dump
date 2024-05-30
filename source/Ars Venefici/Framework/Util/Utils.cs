/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Util
{
    public static class Utils
    {
        public static T RequireNotNull<T>(T arg)
        {
            if (arg == null)
            {
                throw new ArgumentNullException();
            }
            return arg;
        }

        public static T GetFirstMatching<T>(List<T> list, Predicate<T> predicate)
        {
            foreach (T t in list)
            {
                if (predicate.Invoke(t)) return t;
            }

            return default;
        }

        public static T GetLastMatching<T>(List<T> list, Predicate<T> predicate)
        {
            List<T> l = new List<T>(list);
            l.Reverse();

            return GetFirstMatching(l, predicate);
        }

        /*
         * 
         *  conversion          formula
            ------------------  --------------------------------------------------------------------------------
            absolute => screen  x - Game1.viewport.X, y - Game1.viewport.Y
            absolute => tile    x / Game1.tileSize,   y / Game1.tileSize

            screen => absolute  x + Game1.viewport.X, y + Game1.viewport.Y
            screen => tile      (x + Game1.viewport.X) / Game1.tileSize, (y + Game1.viewport.Y) / Game1.tileSize

            tile => absolute    x * Game1.tileSize, y * Game1.tileSize
            tile => screen      (x * Game1.tileSize) - Game1.viewport.X, (y * Game1.tileSize) - Game1.viewport.Y
        */

        //---------------------------------------
        // Absolute Position Conversions
        //---------------------------------------

        public static Vector2 AbsolutePosToScreenPos(int x, int y)
        {
            return new Vector2(x - Game1.viewport.X, y - Game1.viewport.Y);
        }

        public static Vector2 AbsolutePosToScreenPos(Vector2 pos)
        {
            return new Vector2(pos.X - Game1.viewport.X, pos.Y - Game1.viewport.Y);
        }

        public static Vector2 AbsolutePosToTilePos(int x ,int y)
        {
            return new Vector2(x / Game1.tileSize, y / Game1.tileSize);
        }

        public static Vector2 AbsolutePosToTilePos(Vector2 pos)
        {
            return new Vector2(pos.X / Game1.tileSize, pos.Y / Game1.tileSize);
        }

        //---------------------------------------
        // Screen Position Conversions
        //---------------------------------------

        public static Vector2 ScreenPosToAbsolutePos(int x, int y)
        {
            return new Vector2(x + Game1.viewport.X, y + Game1.viewport.Y);
        }

        public static Vector2 ScreenPosToAbsolutePos(Vector2 pos)
        {
            return new Vector2(pos.X + Game1.viewport.X, pos.Y + Game1.viewport.Y);
        }

        public static Vector2 ScreenPosToTilePos(int x, int y)
        {
            return new Vector2((x + Game1.viewport.X) / Game1.tileSize, (y + Game1.viewport.Y) / Game1.tileSize);
        }

        public static Vector2 ScreenPosToTilePos(Vector2 pos)
        {
            return new Vector2((pos.X + Game1.viewport.X) / Game1.tileSize, (pos.Y + Game1.viewport.Y) / Game1.tileSize);
        }

        //---------------------------------------
        // Tile Position Conversions
        //---------------------------------------

        public static Vector2 TilePosToAbsolutePos(int x, int y)
        {
            return new Vector2(x * Game1.tileSize, y * Game1.tileSize);
        }

        public static Vector2 TilePosToAbsolutePos(Vector2 pos)
        {
            return new Vector2(pos.X * Game1.tileSize, pos.Y * Game1.tileSize);
        }

        public static Vector2 TilePosToScreenPos(int x, int y)
        {
            return new Vector2((x * Game1.tileSize) - Game1.viewport.X, (y * Game1.tileSize) - Game1.viewport.Y);
        }

        public static Vector2 TilePosToScreenPos(Vector2 pos)
        {
            return new Vector2((pos.X * Game1.tileSize) - Game1.viewport.X, (pos.Y * Game1.tileSize) - Game1.viewport.Y);
        }
    }
}
