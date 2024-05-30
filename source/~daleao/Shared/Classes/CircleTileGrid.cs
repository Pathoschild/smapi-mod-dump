/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Classes;

#region using directives

using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

#endregion using directives

/// <summary>Represents a circular grid of tiles.</summary>
public sealed class CircleTileGrid
{
    private readonly bool[,] _outlineBoolArray;

    /// <summary>Initializes a new instance of the <see cref="CircleTileGrid"/> class.</summary>
    /// <param name="origin">The center tile of the circle in the world reference.</param>
    /// <param name="radius">The radius of the circle in tile units.</param>
    public CircleTileGrid(Vector2 origin, uint radius)
    {
        this.Origin = origin;
        this.Radius = radius;
        this._outlineBoolArray = this.GetOutline();
    }

    /// <summary>Gets the tile at the origin of the circle.</summary>
    public Vector2 Origin { get; }

    /// <summary>Gets the radius of the circle.</summary>
    public uint Radius { get; }

    /// <summary>Enumerates all the tiles in the grid.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Enumerator.")]
    public IEnumerable<Vector2> Tiles
    {
        get
        {
            // get the origin
            yield return this.Origin;

            var center = new Vector2(this.Radius, this.Radius); // the center of the circle in the grid reference

            // get the central Axes
            for (var i = 0; i < (this.Radius * 2) + 1; i++)
            {
                if (i == this.Radius)
                {
                    continue;
                }

                yield return this.Origin - center + new Vector2(i, this.Radius);
                yield return this.Origin - center + new Vector2(this.Radius, i);
            }

            // loop over the first remaining quadrant and mirror matches 3 times
            for (var x = 0; x < this.Radius; x++)
            {
                for (var y = 0; y < this.Radius; y++)
                {
                    if (!this.Contains(new Point(x, y)))
                    {
                        continue;
                    }

                    yield return this.Origin - center + new Vector2(y, x);
                    yield return this.Origin - center + new Vector2(y, (2 * this.Radius) - x);
                    yield return this.Origin - center + new Vector2((2 * this.Radius) - y, x);
                    yield return this.Origin - center + new Vector2((2 * this.Radius) - y, (2 * this.Radius) - x);
                }
            }
        }
    }

    /// <summary>Enumerates only the outline tiles of the grid.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1623:Property summary documentation should match accessors", Justification = "Enumerator.")]
    public IEnumerable<Vector2> Outline
    {
        get
        {
            var center = new Vector2(this.Radius, this.Radius); // the center of the circle in the grid reference

            // get the central axis extremities
            yield return this.Origin - center + new Vector2(0, this.Radius);
            yield return this.Origin - center + new Vector2(this.Radius * 2, this.Radius);
            yield return this.Origin - center + new Vector2(this.Radius, 0);
            yield return this.Origin - center + new Vector2(this.Radius, this.Radius * 2);
            if (this.Radius <= 1)
            {
                yield break;
            }

            // loop over the first remaining quadrant and mirror matches 3 times
            for (var x = 0; x < this.Radius; x++)
            {
                for (var y = 0; y < this.Radius; y++)
                {
                    if (!this._outlineBoolArray[x, y])
                    {
                        continue;
                    }

                    yield return this.Origin - center + new Vector2(y, x);
                    yield return this.Origin - center + new Vector2(y, (2 * this.Radius) - x);
                    yield return this.Origin - center + new Vector2((2 * this.Radius) - y, x);
                    yield return this.Origin - center + new Vector2((2 * this.Radius) - y, (2 * this.Radius) - x);
                }
            }
        }
    }

    /// <summary>Produces the set difference between <paramref name="left"/> and <paramref name="right"/>.</summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of the tiles in <paramref name="left"/> which are not in <paramref name="right"/>.</returns>
    /// <remarks><paramref name="left"/> must be larger than <paramref name="right"/> and both must have the same origin.</remarks>
    public static IEnumerable<Vector2> operator -(CircleTileGrid? left, CircleTileGrid? right) => left?.SetDifference(right) ?? [];

    /// <summary>Determines whether a point is contained within the circle by using ray casting.</summary>
    /// <param name="point">The <see cref="Point"/>> to be tested.</param>
    /// <returns><see langword="true"/> if the <paramref name="point"/> is within the bounds of the circle, otherwise <see langword="false"/>.</returns>
    /// <remarks>Remember that the center of the circle is located at (_radius, _radius).</remarks>
    public bool Contains(Point point)
    {
        // handle out of bounds
        if (point.X < 0 || point.Y < 0 || point.X > this.Radius * 2 || point.Y > this.Radius * 2)
        {
            return false;
        }

        // handle edge points
        if (point.X == 0 || point.Y == 0 || point.X == this.Radius * 2 || point.Y == this.Radius * 2)
        {
            return this._outlineBoolArray[point.Y, point.X];
        }

        // handle central Axes
        if (point.X == this.Radius || point.Y == this.Radius)
        {
            return true;
        }

        // handle remaining outline points
        if (this._outlineBoolArray[point.Y, point.X])
        {
            return true;
        }

        // mirror point into the first quadrant
        if (point.X > this.Radius)
        {
            point.X = ((int)this.Radius * 2) - point.X;
        }

        if (point.Y > this.Radius)
        {
            point.Y = ((int)this.Radius * 2) - point.Y;
        }

        // cast horizontal rays
        for (var i = point.X; i < this.Radius; i++)
        {
            if (this._outlineBoolArray[point.Y, i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Determines whether a vector is contained within the circle by using ray casting.</summary>
    /// <param name="vector">The <see cref="Vector2"/>> to be tested.</param>
    /// <returns><see langword="true"/> if the <paramref name="vector"/> is within the bounds of the circle, otherwise <see langword="false"/>.</returns>
    /// <remarks>Remember that the center of the circle is located at (Radius, Radius).</remarks>
    public bool Contains(Vector2 vector)
    {
        return this.Contains(vector.ToPoint());
    }

    /// <summary>Gets a <see cref="string"/> representation of the circle grid.</summary>
    /// <returns>A <see cref="string"/> representation of the circle grid.</returns>
    public new string ToString()
    {
        var s = new StringBuilder().AppendLine();
        var height = this._outlineBoolArray.GetLength(0);
        var width = this._outlineBoolArray.GetLength(1);
        for (var i = 0; i < height; i++)
        {
            var first = 0;
            var last = width;
            for (var j = 0; j < width; j++)
            {
                if (!this._outlineBoolArray[i, j])
                {
                    continue;
                }

                first = j;
                break;
            }

            for (var j = width - 1; j >= 0; j--)
            {
                if (!this._outlineBoolArray[i, j])
                {
                    continue;
                }

                last = j;
                break;
            }

            var toggle = false;
            for (var j = 0; j < width; j++)
            {
                if (j == first || j == last + 1)
                {
                    toggle = !toggle;
                }

                s.Append(toggle ? 'x' : ' ').Append(' ');
            }

            s.AppendLine();
        }

        return s.ToString();
    }

    /// <summary>Creates the circle's outline as a <see cref="bool"/> array.</summary>
    /// <returns>An array of <see cref="bool"/> values, where <see langword="true"/> indicates that the circle's outline crosses over the corresponding tile.</returns>
    private bool[,] GetOutline()
    {
        var outline = new bool[(this.Radius * 2) + 1, (this.Radius * 2) + 1];
        var f = 1 - (int)this.Radius;
        var ddFx = 1;
        var ddFy = -2 * (int)this.Radius;
        var x = 0;
        var y = (int)this.Radius;
        outline[this.Radius, this.Radius + this.Radius] = true;
        outline[this.Radius, this.Radius - this.Radius] = true;
        outline[this.Radius + this.Radius, this.Radius] = true;
        outline[this.Radius - this.Radius, this.Radius] = true;
        while (x < y)
        {
            if (f >= 0)
            {
                y--;
                ddFy += 2;
                f += ddFy;
            }

            x++;
            ddFx += 2;
            f += ddFx;

            outline[this.Radius + x, this.Radius + y] = true;
            outline[this.Radius - x, this.Radius + y] = true;
            outline[this.Radius + x, this.Radius - y] = true;
            outline[this.Radius - x, this.Radius - y] = true;
            outline[this.Radius + y, this.Radius + x] = true;
            outline[this.Radius - y, this.Radius + x] = true;
            outline[this.Radius + y, this.Radius - x] = true;
            outline[this.Radius - y, this.Radius - x] = true;
        }

        return outline;
    }

    /// <summary>Enumerates only the tiles between this grid and <paramref name="other"/>.</summary>
    /// <param name="other">Some other, larger <see cref="CircleTileGrid"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> containing the <see cref="Vector2"/> tiles that are contained within <paramref name="other"/> but not <see langword="this"/>.</returns>
    private IEnumerable<Vector2> SetDifference(CircleTileGrid? other)
    {
        if (other is null || other.Origin != this.Origin || other.Radius >= this.Radius)
        {
            yield break;
        }

        foreach (var tile in this.Tiles)
        {
            if (!other.Contains(tile))
            {
                yield return tile;
            }
        }
    }
}
