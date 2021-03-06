/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/andyruwruw/stardew-valley-water-bot
**
*************************************************/

using Microsoft.Xna.Framework;
using System;

namespace WaterBot.Framework
{
    /// <summary>
    /// Tile on the map, containing path finding and waterable related data.
    /// </summary>
    class Tile
    {
        public bool block;

        public bool water;

        public bool waterable;

        public int x;

        public int y;

        public bool visited;

        public bool watered;

        public bool waterCheck;

        public bool walkableCheck;

        public Tile(int x, int y, bool block, bool water, bool waterable)
        {
            this.x = x;
            this.y = y;

            this.block = block;
            this.water = water;
            this.waterable = waterable;

            this.visited = false;
            this.watered = false;
            this.waterCheck = false;
            this.walkableCheck = false;
        }

        /// <summary>
        /// Finds the Manhattan distance to another point.
        /// </summary>
        /// 
        /// <param name="other"></param>
        public int distanceTo(Tile other)
        {
            return Math.Abs(other.x - this.x) + Math.Abs(other.y - this.y);
        }

        /// <summary>
        /// Finds the Manhattan distance to another point.
        /// </summary>
        /// 
        /// <param name="other"></param>
        public int distanceTo(Point other)
        {
            return Math.Abs(other.X - this.x) + Math.Abs(other.Y - this.y);
        }

        /// <summary>
        /// Resets grouping data.
        /// </summary>
        public void reset()
        {
            this.visited = false;
            this.watered = false;
        }

        /// <summary>
        /// Returns Point instance of Tile
        /// </summary>
        public Point getPoint()
        {
            return new Point(this.x, this.y);
        }

        public bool Equals(Tile other)
        {
            return other.x == this.x && other.y == this.y;
        }

        public bool Equals(Point other)
        {
            return other.X == this.x && other.Y == this.y;
        }

        public override string ToString()
        {
            if (this.water)
            {
                return "~";
            }
            if (this.waterable && this.block)
            {
                return "0";
            }
            if (this.waterable)
            {
                return "O";
            }
            if (this.block)
            {
                return "#";
            }
            return " ";
        }
    }
}
