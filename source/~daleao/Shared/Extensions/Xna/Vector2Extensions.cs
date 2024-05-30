/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Extensions.Xna;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Extensions for the <see cref="Vector2"/> struct.</summary>
public static class Vector2Extensions
{
    /// <summary>Gets the angle between this <paramref name="vector"/> and the horizontal, in degrees.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>The angle between the <paramref name="vector"/> and a horizontal line, in degrees.</returns>
    public static double AngleWithHorizontal(this Vector2 vector)
    {
        var (x, y) = vector;
        return Math.Atan2(0f - y, 0f - x) * (180 / Math.PI);
    }

    /// <summary>Gets the angle between this <paramref name="self"/> and <paramref name="other"/>, in degrees.</summary>
    /// <param name="self">The <see cref="Vector2"/>.</param>
    /// <param name="other">Some other <see cref="Vector2"/>.</param>
    /// <returns>The angle between <paramref name="self"/> and <paramref name="other"/>, in degrees.</returns>
    public static double AngleWith(this Vector2 self, Vector2 other)
    {
        var (ax, ay) = self;
        var (bx, by) = other;
        double sin = (ax * by) - (bx * ay);
        double cos = (ax * bx) + (ay * by);
        return Math.Atan2(sin, cos) * (180 / Math.PI);
    }

    /// <summary>Creates a new <see cref="Vector2"/> that is perpendicular to this one.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>A new <see cref="Vector2"/> obtained by a 90-degree anti-clockwise rotation of the original.</returns>
    public static Vector2 Perpendicular(this Vector2 vector)
    {
        var (x, y) = vector;
        return new Vector2(y, -x);
    }

    /// <summary>Creates a new <see cref="Vector2"/> by rotating this <paramref name="vector"/> by the specified <paramref name="angle"/>, in degrees.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <param name="angle">An angle, in degrees, to rotate by.</param>
    /// <returns>A new <see cref="Vector2"/> obtained by the specified rotation of the original.</returns>
    public static Vector2 Rotate(this Vector2 vector, double angle)
    {
        var sin = (float)Math.Sin(angle * Math.PI / 180d);
        var cos = (float)Math.Cos(angle * Math.PI / 180d);
        var tx = vector.X;
        var ty = vector.Y;
        return new Vector2(
            (cos * tx) - (sin * ty),
            (sin * tx) + (cos * ty));
    }

    /// <summary>Gets the 4-connected neighboring tiles.</summary>
    /// <param name="tile">The tile as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the four-connected neighbors of the <paramref name="tile"/>.</returns>
    public static IEnumerable<Vector2> GetFourNeighbors(this Vector2 tile, int width, int height)
    {
        var (x, y) = tile;
        if (x > 0)
        {
            yield return new Vector2(x - 1, y);
        }

        if (x < width - 1)
        {
            yield return new Vector2(x + 1, y);
        }

        if (y > 0)
        {
            yield return new Vector2(x, y - 1);
        }

        if (y < height - 1)
        {
            yield return new Vector2(x, y + 1);
        }
    }

    /// <summary>Gets the 8-connected neighboring tiles.</summary>
    /// <param name="tile">The tile as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the eight-connected neighbors of the <paramref name="tile"/>.</returns>
    public static IEnumerable<Vector2> GetEightNeighbors(this Vector2 tile, int width, int height)
    {
        foreach (var neighbor in GetFourNeighbors(tile, width, height))
        {
            yield return neighbor;
        }

        var (x, y) = tile;
        if (x > 0 && y > 0)
        {
            yield return new Vector2(x - 1, y - 1);
        }

        if (x > 0 && y < height - 1)
        {
            yield return new Vector2(x - 1, y + 1);
        }

        if (x < width - 1 && y > 0)
        {
            yield return new Vector2(x + 1, y - 1);
        }

        if (x < width - 1 && y < height - 1)
        {
            yield return new Vector2(x + 1, y + 1);
        }
    }

    /// <summary>Gets the 24-connected neighboring tiles.</summary>
    /// <param name="tile">The tile as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the entire region.</param>
    /// <param name="height">The height of the entire region.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the eight-connected neighbors of the <paramref name="tile"/>.</returns>
    public static IEnumerable<Vector2> GetTwentyFourNeighbors(this Vector2 tile, int width, int height)
    {
        foreach (var neighbor in GetEightNeighbors(tile, width, height))
        {
            yield return neighbor;
        }

        var x = (int)tile.X;
        var y = (int)tile.Y;
        if (y - 2 >= 0)
        {
            for (var i = -2; i <= 2; i++)
            {
                if ((x + i).IsIn(..width))
                {
                    yield return new Vector2(x + i, y - 2);
                }
            }
        }

        if (y + 2 < height)
        {
            for (var i = -2; i <= 2; i++)
            {
                if ((x + i).IsIn(..width))
                {
                    yield return new Vector2(x + i, y + 2);
                }
            }
        }

        if (x - 2 >= 0)
        {
            for (var j = -1; j <= 1; j++)
            {
                if ((y + j).IsIn(..height))
                {
                    yield return new Vector2(x - 2, y + j);
                }
            }
        }

        if (x + 2 < width)
        {
            for (var j = -1; j <= 1; j++)
            {
                if ((y + j).IsIn(..height))
                {
                    yield return new Vector2(x + 2, y + j);
                }
            }
        }
    }

    /// <summary>Gets the horizontal and vertical unit vector projections of this <paramref name="vector"></paramref>.</summary>
    /// <param name="vector">The <see cref="Vector2"/>.</param>
    /// <returns>Two unit vectors which point in the same direction as the components of <paramref name="vector"/>.</returns>
    public static (Vector2 Horizontal, Vector2 Vertical) GetUnitComponents(this Vector2 vector)
    {
        var horizontal = vector.X > 0f ? VectorUtils.RightVector() : vector.X < 0f ? VectorUtils.LeftVector() : Vector2.Zero;
        var vertical = vector.Y > 0f ? VectorUtils.DownVector() : vector.Y < 0f ? VectorUtils.UpVector() : Vector2.Zero;
        return (horizontal, vertical);
    }

    /// <summary>Searches for region boundaries using a Flood Fill algorithm.</summary>
    /// <param name="origin">The starting point for the fill, as a <see cref="Vector2"/>.</param>
    /// <param name="width">The width of the region.</param>
    /// <param name="height">The height of the region.</param>
    /// <param name="boundary">The boundary condition.</param>
    /// <param name="minDistance">The shortest Chebyshev distance acceptable from the origin.</param>
    /// <param name="maxDistance">The largest Chebyshev distance acceptable from the origin.</param>
    /// <returns>The list of <see cref="Vector2"/>s belonging to the enclosed region.</returns>
    public static IReadOnlyList<Vector2> FloodFill(this Vector2 origin, int width, int height, Func<Vector2, bool> boundary, int minDistance = 0, int maxDistance = int.MaxValue)
    {
        var flooded = new List<Vector2>();
        var tested = new HashSet<Vector2> { origin };
        var queue = new Queue<Vector2>();
        var distanceByTile = new Dictionary<Vector2, int>();
        foreach (var neighbor in origin.GetEightNeighbors(width, height))
        {
            queue.Enqueue(neighbor);
            distanceByTile[neighbor] = 1;
        }

        while (queue.Count > 0)
        {
            var tile = queue.Dequeue();
            if (tile.X < 0 || tile.Y < 0 || tile.X >= width || tile.Y >= height || !tested.Add(tile) ||
                !boundary(tile))
            {
                continue;
            }

            flooded.Add(tile);
            foreach (var neighbor in tile.GetEightNeighbors(width, height))
            {
                if (tested.Contains(neighbor) || queue.Contains(neighbor))
                {
                    continue;
                }

                queue.Enqueue(neighbor);
                distanceByTile[neighbor] = distanceByTile[tile] + 1;
            }
        }

        return flooded.Where(tile => distanceByTile[tile] < minDistance || distanceByTile[tile] > maxDistance).ToList();
    }
}
