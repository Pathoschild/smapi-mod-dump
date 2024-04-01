/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using DecidedlyShared.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DecidedlyShared.Utilities;

public class Geometry
{
    public static Corners GetCornerPoints(Vector2 topLeftPoint, int width, int height)
    {
        Corners corners = new Corners();

        corners.topLeft = topLeftPoint;
        corners.topRight = topLeftPoint + new Vector2(width, 0);
        corners.bottomLeft = topLeftPoint + new Vector2(0, height);
        corners.bottomRight = topLeftPoint + new Vector2(width, height);

        return corners;
    }

    public static Corners GetCornerPoints(Rectangle boundingBox)
    {
        Corners corners = new Corners();

        corners.topLeft = new Vector2(boundingBox.Top, boundingBox.Left);
        corners.topRight = new Vector2(boundingBox.Top, boundingBox.Right);
        corners.bottomLeft = new Vector2(boundingBox.Bottom, boundingBox.Left);
        corners.bottomRight = new Vector2(boundingBox.Bottom, boundingBox.Right);

        return corners;
    }

    /// <summary>
    /// Takes in an xTile Rectangle, and returns an XNA Rectangle.
    /// </summary>
    /// <param name="xRect"></param>
    /// <returns></returns>
    public static Microsoft.Xna.Framework.Rectangle RectToRect(xTile.Dimensions.Rectangle xRect)
    {
        return new Microsoft.Xna.Framework.Rectangle(xRect.X, xRect.Y, xRect.Width, xRect.Height);
    }

    public static Vector2 GetLargestString(List<string> toMeasure, SpriteFont font)
    {
        return GetLargestString(toMeasure.ToArray(), font);
    }

    public static Vector2 GetLargestString(string[] toMeasure, SpriteFont font)
    {
        Vector2 longest = new Vector2();

        foreach (string s in toMeasure)
        {
            Vector2 measurement = font.MeasureString(s);

            if (measurement.X > longest.X)
                longest.X = measurement.X;
            if (measurement.Y > longest.Y)
                longest.Y = measurement.Y;
        }

        return longest;
    }
}
