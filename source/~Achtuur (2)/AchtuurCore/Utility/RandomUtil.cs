/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Framework;
using Microsoft.Xna.Framework;
using StardewValley.GameData.FloorsAndPaths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Utility;
public static class RandomUtil
{
    static System.Random random = new System.Random();
    public static Vector2 GetPointOnRectangle(Rectangle rect)
    {
        switch (random.Next(4))
        {
            default:
            case 0: // Top
                return new Vector2(random.Next(rect.Left, rect.Right), rect.Top);
            case 1: // Right
                return new Vector2(rect.Right, random.Next(rect.Top, rect.Bottom));
            case 2: // Bottom
                return new Vector2(random.Next(rect.Left, rect.Right), rect.Bottom);
            case 3: // Left
                return new Vector2(rect.Left, random.Next(rect.Top, rect.Bottom));
        }
    }

    public static Vector2 GetPointOnCircle(Circle circle)
    {
        float angle = GetFloat(0, 2f * Math.PI);
        return circle.PointAt(angle);
    }

    public static Vector2 GetPointOnEllipse(Ellipse ellipse)
    {
        float angle = GetFloat(0, 2f * Math.PI);
        return ellipse.PointAt(angle);
    }

    public static Vector2 GetUnitVector()
    {
        Vector2 v = new Vector2(GetFloat(-1, 1), GetFloat(-1, 1));
        v.Normalize();
        return v;
    }

    public static float GetFloat(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    public static float GetFloat(double min, double max)
    {
        return (float)(random.NextDouble() * (max - min) + min);
    }
}
