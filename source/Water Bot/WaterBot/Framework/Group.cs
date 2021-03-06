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
using System.Collections.Generic;

namespace WaterBot.Framework
{
    /// <summary>
    /// A group of waterable adjacent tiles.
    /// </summary>
    class Group
    {
        private List<Tile> list;

        public int index;

        public Group(int index)
        {
            this.index = index;
            this.list = new List<Tile>();
        }

        public List<Tile> getList()
        {
            return this.list;
        }

        /// <summary>
        /// Add a tile to the group
        /// </summary>
        public void Add(Tile tile)
        {
            this.list.Add(tile);
        }

        /// <summary>
        /// Number of tiles in group
        /// </summary>
        public int Count()
        {
            return this.list.Count;
        }

        /// <summary>
        /// A group of waterable adjacent tiles.
        /// </summary>
        /// 
        /// <param name="tile"></param>
        public bool Contains(Tile tile)
        {
            return this.list.Contains(tile);
        }

        /// <summary>
        /// Finds the tile closes to the centroid of all the points.
        /// </summary>
        public Point Centroid(Map map)
        {
            int sumOfX = 0;
            int sumOfY = 0;

            foreach (Tile tile in this.list)
            {
                sumOfX += tile.x;
                sumOfY += tile.y;
            }

            Point centroid = new Point(sumOfX / this.list.Count, sumOfY / this.list.Count);

            int shortest = int.MaxValue;
            Tile centerWalkableTile = null;
            int shortestNonWalkable = int.MaxValue;
            Tile centerNonWalkableTile = null;

            foreach (Tile tile in this.list)
            {
                if (!tile.block && tile.distanceTo(centroid) < shortest)
                {
                    shortest = tile.distanceTo(centroid);
                    centerWalkableTile = tile;
                }else if (tile.block && tile.distanceTo(centroid) < shortestNonWalkable)
                {
                    shortestNonWalkable = tile.distanceTo(centroid);
                    centerNonWalkableTile = tile;
                }
            }

            if (centerWalkableTile == null)
            {
                return map.findClosestWalkableTile(centerNonWalkableTile).getPoint();
            }

            return centerWalkableTile.getPoint();
        }

        public Tuple<Tile, double> findClosestTile(int x, int y)
        {
            Tile shortestTile = null;
            double shortest = float.MaxValue;

            foreach (Tile tile in this.list)
            {
                double distance = Math.Sqrt(Math.Pow(x - tile.x, 2) + Math.Pow(y - tile.y, 2));

                if (distance < shortest || shortestTile == null)
                {
                    shortest = distance;
                    shortestTile = tile;
                }
            }

            return Tuple.Create(shortestTile, shortest);
        }
    }
}
