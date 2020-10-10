/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/speeder1/SMAPISprinklerMod
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BetterSprinklers.Framework
{
    /// <summary>Provides methods for working with sprinkler grids.</summary>
    internal static class GridHelper
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get a grid of covered tiles centered on a sprinkler.</summary>
        /// <param name="center">The sprinkler's tile position.</param>
        /// <param name="grid">The grid to convert.</param>
        public static IEnumerable<Vector2> GetCoveredTiles(Vector2 center, int[,] grid)
        {
            int arrayHalfSizeX = grid.GetLength(0) / 2;
            int arrayHalfSizeY = grid.GetLength(1) / 2;
            int minX = (int)center.X - arrayHalfSizeX;
            int minY = (int)center.Y - arrayHalfSizeY;
            int maxX = (int)center.X + arrayHalfSizeX;
            int maxY = (int)center.Y + arrayHalfSizeY;

            for (int gridX = 0, x = minX; x <= maxX; x++, gridX++)
            {
                for (int gridY = 0, y = minY; y <= maxY; y++, gridY++)
                {
                    if (grid[gridX, gridY] > 0)
                        yield return new Vector2(x, y);
                }
            }
        }

        /// <summary>Get a tile grid centered on the given tile position.</summary>
        /// <param name="centerTile">The center tile position.</param>
        /// <param name="grid">The grid to get.</param>
        /// <param name="perform">The action to perform for each tile, given the tile position.</param>
        public static void ForCoveredTiles(Vector2 centerTile, int[,] grid, Action<Vector2> perform)
        {
            foreach (Vector2 tile in GridHelper.GetCoveredTiles(centerTile, grid))
            {
                perform(tile);
            }
        }
    }
}
