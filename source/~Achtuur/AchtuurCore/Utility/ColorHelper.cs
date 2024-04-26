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
using System;
using System.Collections.Generic;

namespace AchtuurCore.Utility;

public static class ColorHelper
{
    /// <summary>
    /// List of colors, used for random generation. Loosely sorted based on color from Red, Orange, Yellow, Green, Blue, Purple, Pink
    /// </summary>
    public static readonly List<Color> ColorList = new List<Color>()
    {
        Color.Red,
        Color.OrangeRed,
        Color.Orange,
        Color.Yellow,
        //Color.Gold,
        Color.LimeGreen,
        Color.Green,
        Color.LawnGreen,
        //Color.Turquoise,
        Color.Cyan,
        //Color.Blue,
        Color.DeepSkyBlue,
        Color.LightSkyBlue,
        Color.BlueViolet,
        //Color.Purple,
        Color.Magenta,
        Color.Orchid,
        Color.HotPink,
    };

    public static List<Color> UniqueColorList = new List<Color>(ColorList);

    public static Color AddColor(this Color lhs, Color rhs)
    {
        return new Color(lhs.R + rhs.R, lhs.G + rhs.G, lhs.B + rhs.B, lhs.A + rhs.A);
    }

    /// <summary>
    /// Multiply only R, G and B channels of <paramref name="lhs"/> with <paramref name="rhs"/>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static Color MultiplyColor(this Color lhs, float rhs)
    {
        return new Color((int)(lhs.R * rhs), (int)(lhs.G * rhs), (int)(lhs.B * rhs), lhs.A);
    }

    public static Color ToGrayScale(this Color color)
    {
        int gs = (int)(0.299f * color.R) + (int)(0.587f * color.G) + (int)(0.114f * color.B);
        return new Color(gs, gs, gs, color.A);
    }

    public static Color GetRandomColor(int alpha = 255)
    {
        Random r = new Random();
        int i = r.Next(ColorList.Count);

        return new Color(ColorList[i], alpha);
    }

    public static Color GetRandomColor(int alpha = 255, int seed = 0)
    {
        int i = seed * 26535 % ColorList.Count;
        if (i < 0)
            i = -i;
        return new Color(ColorList[i], alpha);
    }

    /// <summary>
    /// Generates random color that will be unique every time function is called, until color list is exhausted.
    /// After list is exhaused, it is reset
    /// </summary>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static Color GetUniqueColor(int alpha = 255)
    {
        // Refresh color list if exhausted
        if (UniqueColorList.Count == 0)
            UniqueColorList = new List<Color>(ColorList);

        // Generate random index
        Random r = new Random(UniqueColorList.Count);
        int i = r.Next(UniqueColorList.Count - 1);

        // Take and remove color from color list
        Color c = UniqueColorList[i];
        UniqueColorList.RemoveAt(i);

        return c;
    }
}
