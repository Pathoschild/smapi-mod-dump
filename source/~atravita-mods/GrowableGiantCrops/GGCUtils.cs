/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops;
internal static class GGCUtils
{
    internal static bool IsTilePlaceableForResourceClump(GameLocation location, int tileX, int tileY, bool relaxed)
    {
        if (location is null || location.doesTileHaveProperty(tileX, tileY, "Water", "Back") is not null)
        {
            return false;
        }

        Rectangle position = new(tileX * 64, tileY * 64, 64, 64);

        foreach (Farmer farmer in location.farmers)
        {
            if (farmer.GetBoundingBox().Intersects(position))
            {
                return false;
            }
        }

        foreach (Character character in location.characters)
        {
            if (character.GetBoundingBox().Intersects(position))
            {
                return false;
            }

        }

        foreach (ResourceClump? clump in location.resourceClumps)
        {
            if (clump.getBoundingBox(clump.currentTileLocation).Intersects(position))
            {
                return false;
            }
        }

        return relaxed || !location.isTileOccupied(new Vector2(tileX, tileY));
    }
}
