/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework;
public struct Circle
{
    public Vector2 Center;
    public float Radius;

    public float Diameter => Radius * 2;

    public Circle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    /// <summary>
    /// Returns the point on the circle at the given angle.
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Vector2 PointAt(float angle)
    {
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Radius + Center;
    }

    public float ThetaAt(Vector2 point)
    {
        return (float)Math.Atan2(point.Y - Center.Y, point.X - Center.X);
    }

    /// <summary>
    /// Returns the tangent (unit) vector at the given angle. This function gives the direction.
    /// The actual tangent vector is equal to <c>PointAt(angle) + TangentAt(angle)</c>
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Vector2 TangentAt(float angle)
    {
        float phi = angle + (float)Math.PI / 2;
        return new Vector2((float)Math.Cos(phi), (float)Math.Sin(phi));
    }

    public bool Contains(Vector2 point)
    {
        return Vector2.DistanceSquared(Center, point) <= Radius * Radius;
    }
}
