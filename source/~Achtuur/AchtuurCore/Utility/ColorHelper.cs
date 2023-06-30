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

namespace AchtuurCore.Utility;

public static class ColorHelper
{
    public static Color AddColor(this Color lhs, Color rhs)
    {
        return new Color(lhs.R + rhs.R, lhs.G + rhs.G, lhs.B + rhs.B, lhs.A + rhs.A);
    }

    public static Color ToGrayScale(this Color color)
    {
        int gs = (int)(0.299f * color.R) + (int)(0.587f * color.G) + (int)(0.114f * color.B);
        return new Color(gs, gs, gs, color.A);
    }
}
