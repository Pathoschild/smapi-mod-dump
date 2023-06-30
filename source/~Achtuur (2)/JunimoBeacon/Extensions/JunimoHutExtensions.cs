/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using System.Collections.Generic;
using System.Linq;

namespace JunimoBeacon.Extensions;

internal static class JunimoHutExtensions
{
    /// <summary>
    /// Returns tile range of <paramref name="hut"/> as a rectangle.
    /// </summary>
    /// <param name="hut"></param>
    /// <returns></returns>
    public static Rectangle GetTileRangeAsRect(this JunimoHut hut)
    {
        return new Rectangle(hut.tileX.Value - 7, hut.tileY.Value - 7, 17, 17);
    }

    public static IEnumerable<Vector2> GetTileRange(this JunimoHut hut)
    {
        foreach (Vector2 tile in hut.GetTileRangeAsRect().GetTiles())
            yield return tile;
    }
}
