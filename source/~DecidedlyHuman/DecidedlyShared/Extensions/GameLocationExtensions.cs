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
using StardewValley;
using StardewValley.TerrainFeatures;

namespace DecidedlyShared.Extensions;

public static class GameLocationExtensions
{
    public static bool HasSObject(this GameLocation location, SObject obj, SearchType search)
    {
        return false;
    }

    public static bool HasTerrainFeature(this GameLocation location, TerrainFeature feature, SearchType search)
    {
        return false;
    }

    public static bool IsTileOccupied(this GameLocation location, Vector2 tile)
    {
        return false;
    }

    public static bool TryGetSObject(this GameLocation location, Vector2 tile, out SObject? foundObject)
    {
        foundObject = null;

        if (location.Objects.ContainsKey(tile))
        {
            foundObject = location.Objects[tile];
            return true;
        }

        return false;
    }

    public static SObject GetSObject(this GameLocation location, Vector2 tile)
    {
        return location.Objects[tile];
    }

    public static bool TryGetTerrainFeature(this GameLocation location, Vector2 tile, out TerrainFeature? foundFeature)
    {
        foundFeature = null;

        if (location.terrainFeatures.ContainsKey(tile))
        {
            foundFeature = location.terrainFeatures[tile];
            return true;
        }

        return false;
    }

    public static TerrainFeature GetTerrainFeature(this GameLocation location, Vector2 tile)
    {
        return location.terrainFeatures[tile];
    }
}
