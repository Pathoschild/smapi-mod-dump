/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Classes;

#region using directives

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;

#endregion using directives

/// <summary>Represent a circular arrangement of tiles.</summary>
public class CircleTileGrid
{
    private readonly Vector2 _origin;
    private readonly int _radius;
    private readonly bool[,] _outlineBoolArray;

    /// <summary>Construct an instance.</summary>
    /// <param name="origin">The center tile of the circle in the world reference.</param>
    /// <param name="radius">The radius of the circle in tile units.</param>
    public CircleTileGrid(Vector2 origin, int radius)
    {
        _origin = origin;
        _radius = radius;
        _outlineBoolArray = GetOutline();
    }

    /// <summary>Enumerate all the world tiles within a certain radius from the origin.</summary>
    public IEnumerable<Vector2> Tiles
    {
        get
        {
            // get the origin
            yield return _origin;

            var center = new Vector2(_radius, _radius); // the center of the circle in the grid reference

            // get the central Axes
            for (var i = 0; i < _radius * 2 + 1; ++i)
                if (i != _radius)
                {
                    yield return _origin - center + new Vector2(i, _radius);
                    yield return _origin - center + new Vector2(_radius, i);
                }

            // loop over the first remaining quadrant and mirror matches 3 times
            for (var x = 0; x < _radius; ++x)
                for (var y = 0; y < _radius; ++y)
                    if (Contains(new(x, y)))
                    {
                        yield return _origin - center + new Vector2(y, x);
                        yield return _origin - center + new Vector2(y, 2 * _radius - x);
                        yield return _origin - center + new Vector2(2 * _radius - y, x);
                        yield return _origin - center + new Vector2(2 * _radius - y, 2 * _radius - x);
                    }
        }
    }

    /// <summary>Enumerate all the world tiles at exact radius units from the origin.</summary>
    public IEnumerable<Vector2> Outline
    {
        get
        {
            var center = new Vector2(_radius, _radius); // the center of the circle in the grid reference

            // get the central axis extremities
            yield return _origin - center + new Vector2(0, _radius);
            yield return _origin - center + new Vector2(_radius * 2, _radius);
            yield return _origin - center + new Vector2(_radius, 0);
            yield return _origin - center + new Vector2(_radius, _radius * 2);

            if (_radius <= 1) yield break;

            // loop over the first remaining quadrant and mirror matches 3 times
            for (var x = 0; x < _radius; ++x)
                for (var y = 0; y < _radius; ++y)
                    if (_outlineBoolArray[x, y])
                    {
                        yield return _origin - center + new Vector2(y, x);
                        yield return _origin - center + new Vector2(y, 2 * _radius - x);
                        yield return _origin - center + new Vector2(2 * _radius - y, x);
                        yield return _origin - center + new Vector2(2 * _radius - y, 2 * _radius - x);
                    }
        }
    }

    /// <summary>Determine whether a point is contained within the circle's outline by using ray casting.</summary>
    /// <param name="point">The point to be tested.</param>
    /// <remarks>Remember that the center of the circle is located at (_radius, _radius).</remarks>
    public bool Contains(Point point)
    {
        // handle out of bounds
        if (point.X < 0 || point.Y < 0 || point.X > _radius * 2 || point.Y > _radius * 2) return false;

        // handle edge points
        if (point.X == 0 || point.Y == 0 || point.X == _radius * 2 || point.Y == _radius * 2)
            return _outlineBoolArray[point.Y, point.X];

        // handle central Axes
        if (point.X == _radius || point.Y == _radius) return true;

        // handle remaining outline points
        if (_outlineBoolArray[point.Y, point.X]) return true;

        // mirror point into the first quadrant
        if (point.X > _radius) point.X = _radius - point.X;
        if (point.Y > _radius) point.Y = _radius - point.Y;

        // cast horizontal rays
        for (var i = point.X; i < _radius; ++i)
            if (_outlineBoolArray[point.Y, i])
                return false;

        return true;
    }

    /// <summary>Get a string representation of the complete tile grid.</summary>
    public new string ToString()
    {
        var s = new StringBuilder().AppendLine();
        var height = _outlineBoolArray.GetLength(0);
        var width = _outlineBoolArray.GetLength(1);
        for (var i = 0; i < height; ++i)
        {
            var first = 0;
            var last = width;
            for (var j = 0; j < width; ++j)
            {
                if (!_outlineBoolArray[i, j]) continue;
                first = j;
                break;
            }

            for (var j = width - 1; j >= 0; --j)
            {
                if (!_outlineBoolArray[i, j]) continue;
                last = j;
                break;
            }

            var toggle = false;
            for (var j = 0; j < width; ++j)
            {
                if (j == first || j == last + 1) toggle = !toggle;
                s.Append(toggle ? 'x' : ' ').Append(' ');
            }

            s.AppendLine();
        }

        return s.ToString();
    }

    #region private methods

    /// <summary>Create the circle's outline as a <see cref="bool" /> array.</summary>
    protected bool[,] GetOutline()
    {
        var outline = new bool[_radius * 2 + 1, _radius * 2 + 1];
        var f = 1 - _radius;
        var ddFx = 1;
        var ddFy = -2 * _radius;
        var x = 0;
        var y = _radius;

        outline[_radius, _radius + _radius] = true;
        outline[_radius, _radius - _radius] = true;
        outline[_radius + _radius, _radius] = true;
        outline[_radius - _radius, _radius] = true;

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

            outline[_radius + x, _radius + y] = true;
            outline[_radius - x, _radius + y] = true;
            outline[_radius + x, _radius - y] = true;
            outline[_radius - x, _radius - y] = true;
            outline[_radius + y, _radius + x] = true;
            outline[_radius - y, _radius + x] = true;
            outline[_radius + y, _radius - x] = true;
            outline[_radius - y, _radius - x] = true;
        }

        return outline;
    }

    #endregion private methods
}