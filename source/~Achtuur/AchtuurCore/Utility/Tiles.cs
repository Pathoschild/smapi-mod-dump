/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using SObject = StardewValley.Object;

namespace AchtuurCore.Utility;

public static class Tiles
{


    /// <summary>
    /// Get all tiles that have a manhattan distance of <paramref name="radius"/> around <paramref name="farmer"/>
    /// </summary>
    /// <param name="farmer">Farmer to get tiles around</param>
    /// <param name="radius">Radius of tiles</param>
    /// <returns></returns>
    public static IEnumerable<Vector2> GetTilesInManhattan(this Farmer farmer, int radius, int min_radius = 0)
    {
        return GetTilesInManhattan(farmer.Tile, radius, min_radius);
    }

    public static IEnumerable<Vector2> GetTilesInRadius(this Farmer farmer, int radius, int min_radius)
    {
        return GetTilesInRadius(farmer.Tile, radius, min_radius);
    }

    /// <summary>
    /// Get all tiles that have a manhattan distance smaller or equal to <paramref name="radius"/> around <paramref name="center"/>. Always returns <paramref name="center"/> first.
    /// </summary>
    /// <param name="center">Center tile. Will return tiles around this one</param>
    /// <param name="radius">Radius of tiles to look in, in manhattan distance</param>
    /// <param name="min_radius">Minimum radius/manhattan distance tiles will be taken in</param>
    /// <returns></returns>
    public static IEnumerable<Vector2> GetTilesInManhattan(Vector2 center, int radius, int min_radius = 0)
    {
        if (min_radius == 0)
        {
            yield return center;
            min_radius = 1;
        }

        if (radius == 0)
        {
            yield break;
        }

        for (int r = min_radius; r <= radius; r++)
        {
            // Loop around edge of radius r
            // Start at the tip of the diagonals, walk to next tip clockwise
            for (int c = 0; c < r; c++)
            {

                // Top to right
                yield return center + new Vector2(c, r - c);

                // Right to bottom
                yield return center + new Vector2(r - c, -c);

                // Bottom to left
                yield return center + new Vector2(-c, c - r);

                // Left to top
                yield return center + new Vector2(c - r, c);
            }
        }
    }

    public static IEnumerable<Vector2> GetTilesInRadius(Vector2 center, int radius, int min_radius = 0)
    {

        if (min_radius == 0)
        {
            yield return center;
        }

        if (radius == 0)
        {
            yield break;
        }

        for (int r = min_radius; r <= radius; r++)
        {
            int x = 0;
            int y = r;
            int r_sq = radius * radius;

            while (x <= y)
            {
                foreach (Vector2 octant_mirror in MirrorInOctants(center, x, y))
                {
                    yield return octant_mirror;
                }

                // Attempt to go right, if out of radius's range, go down
                // This makes x and y walk the perimeter of the circle at radius r
                int xp = x + 1;

                if (xp * xp + y * y > r_sq)
                {
                    y--;
                }
                x++;
            }
        }
    }

    /// <summary>
    /// Returns tiles in <paramref name="side_length"/> by <paramref name="side_length"/> square around <paramref name="center"/>, including <paramref name="center"/>.
    /// </summary>
    /// <param name="center">Center of the square</param>
    /// <param name="side_length">Length of the sides of the square</param>
    /// <returns></returns>
    public static IEnumerable<Vector2> GetTilesInSquareRange(Vector2 center, int side_length)
    {
        for (int x = -side_length; x <= side_length; x++)
        {
            for (int y = -side_length; y <= side_length; y++)
            {
                yield return center + new Vector2(x, y);
            }
        }
    }

    public static IEnumerable<Vector2> MirrorInQuadrants(Vector2 center, float x, float y)
    {
        if (x == 0 && y == 0)
        {
            yield return center;
        }
        if (x == 0)
        {
            yield return center + new Vector2(0, y);
            yield return center + new Vector2(0, -y);
        }
        else if (y == 0)
        {
            yield return center + new Vector2(x, 0);
            yield return center + new Vector2(-x, 0);
        }
        else if (x == y)
        {
            yield return center + new Vector2(x, x);
            yield return center + new Vector2(x, -x);
            yield return center + new Vector2(-x, x);
            yield return center + new Vector2(-x, -x);
        }
        else
        {
            yield return center + new Vector2(x, y);
            yield return center + new Vector2(x, -y);
            yield return center + new Vector2(-x, y);
            yield return center + new Vector2(-x, -y);
        }
    }
    public static IEnumerable<Vector2> MirrorInOctants(Vector2 center, float x, float y)
    {
        // Mirror in 4 quadrants
        foreach (Vector2 quad in MirrorInQuadrants(center, x, y))
            yield return quad;

        // if x == y, then mirrored_quad == quad
        if (x == y)
            yield break;

        // Mirror in 4 quadrants with x and y flipped, resulting in mirrored octants
        foreach (Vector2 mirrored_quad in MirrorInQuadrants(center, y, x))
        {
            yield return mirrored_quad;
        }
    }

    /// <summary>
    /// <para>Get visible tiles, taken from Pathoschild's Tilehelper.GetVisibleTiles</para>
    /// 
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/TileHelper.cs#L95"/>
    /// </summary>
    /// <returns></returns>
    public static Rectangle GetVisibleArea(int expand = 0)
    {
        return new Rectangle(
            x: (Game1.viewport.X / Game1.tileSize) - expand,
            y: (Game1.viewport.Y / Game1.tileSize) - expand,
            width: (int)Math.Ceiling(Game1.viewport.Width / (decimal)Game1.tileSize) + (expand * 2),
            height: (int)Math.Ceiling(Game1.viewport.Height / (decimal)Game1.tileSize) + (expand * 2)
        );
    }

    public static IEnumerable<Vector2> GetVisibleTiles(int expand = 0)
    {
        return Tiles.GetVisibleArea(expand).GetTiles();
    }


    /// <summary>
    /// <para> Get tiles in a rectangle. Taken from Pathoschild's TileHelper.GetTiles </para>
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Common/TileHelper.cs#L21"/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static IEnumerable<Vector2> GetTiles(int x, int y, int width, int height)
    {
        for (int curX = x, maxX = x + width - 1; curX <= maxX; curX++)
        {
            for (int curY = y, maxY = y + height - 1; curY <= maxY; curY++)
                yield return new Vector2(curX, curY);
        }
    }

    public static IEnumerable<Vector2> GetTiles(this Rectangle rect)
    {
        return Tiles.GetTiles(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static Vector2 GetTileScreenCoords(Vector2 tile)
    {
        return new Vector2
        (
            tile.X * Game1.tileSize - Game1.viewport.X,
            tile.Y * Game1.tileSize - Game1.viewport.Y
        );
    }

    /// <summary>
    /// Checks whether object with <paramref name="qualified_id"/> is on <paramref name="tile"/>
    /// </summary>
    /// <param name="tile">Tile to check for object</param>
    /// <param name="qualified_id">Id of object that is checked whether it is on this tile</param>
    /// <returns></returns>
    public static bool ContainsObject(this Vector2 tile, string qualified_id, GameLocation location = null)
    {
        location ??= Game1.currentLocation;
        SObject obj = location.getObjectAtTile((int)tile.X, (int)tile.Y);
        return obj != null && obj.QualifiedItemId == qualified_id;
    }

}
