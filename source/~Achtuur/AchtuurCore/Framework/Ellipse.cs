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
using StardewValley.GameData.FloorsAndPaths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AchtuurCore.Framework;
public struct Ellipse
{

    public Vector2 Center;
    public float RadiusX;
    public float RadiusY;

    public Ellipse(Vector2 center, float radiusX, float radiusY)
    {
        Center = center;
        RadiusX = radiusX;
        RadiusY = radiusY;
    }

    public Vector2 PointAt(float angle)
    {
        return Center + new Vector2(
            (float)Math.Cos(angle) * RadiusX,
            (float)Math.Sin(angle) * RadiusY
        );
    }

    public bool Contains(Vector2 point)
    {
        return Math.Pow((point.X - Center.X) / RadiusX, 2) + Math.Pow((point.Y - Center.Y) / RadiusY, 2) <= 1;
    }
}
