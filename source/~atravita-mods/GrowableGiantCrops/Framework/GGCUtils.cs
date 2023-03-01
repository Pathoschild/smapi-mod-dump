/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Runtime.CompilerServices;

using AtraBase.Toolkit;
using AtraBase.Toolkit.Extensions;

using AtraShared.Utils.Shims;
using AtraShared.Wrappers;

using GrowableGiantCrops.Framework.InventoryModels;

using Microsoft.Xna.Framework;

using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

using XLocation = xTile.Dimensions.Location;

namespace GrowableGiantCrops.Framework;

/// <summary>
/// The utility class for this mod.
/// </summary>
internal static class GGCUtils
{
    /// <summary>
    /// Checks to see if this tile is valid for trees.
    /// </summary>
    /// <param name="location">The game location to check.</param>
    /// <param name="relaxed">Whether or not to use relaxed placement restrictions.</param>
    /// <param name="tileX">the X tile location.</param>
    /// <param name="tileY">the Y tile location.</param>
    /// <param name="wildTree">Whether or not the tree is a wild tree.</param>
    /// <returns>True if it's placeable, false otherwise.</returns>
    internal static bool CanPlantTreesAtLocation(GameLocation? location, bool relaxed, int tileX, int tileY, bool wildTree)
    {
        if (location?.terrainFeatures is null || Utility.isPlacementForbiddenHere(location))
        {
            return false;
        }
        if (relaxed || (wildTree && location.IsOutdoors && location.doesTileHavePropertyNoNull(tileX, tileY, "Type", "Back") == "Dirt"))
        {
            return true;
        }
        return location.IsGreenhouse || location.map?.Properties?.ContainsKey("ForceAllowTreePlanting") == true || location is Farm or IslandWest;
    }

    /// <summary>
    /// Gets the large object <see cref="LargeTerrainFeature"/> or <see cref="ResourceClump"/> at a specific location.
    /// </summary>
    /// <param name="loc">Location to check.</param>
    /// <param name="nonTileX">X coord.</param>
    /// <param name="nonTileY">Y coord.</param>
    /// <param name="placedOnly">Whether or not to only include what the player has placed.</param>
    /// <returns>The feature, or null for not found.</returns>
    internal static TerrainFeature? GetLargeObjectAtLocation(this GameLocation loc, int nonTileX, int nonTileY, bool placedOnly = false)
    {
        if (loc.resourceClumps is not null)
        {
            foreach (ResourceClump? clump in loc.resourceClumps)
            {
                if (clump is not null && clump.getBoundingBox(clump.tile.Value).Contains(nonTileX, nonTileY)
                    && (!placedOnly || clump.modData?.ContainsKey(InventoryResourceClump.ResourceModdata) == true))
                {
                    return clump;
                }
            }
        }

        if (placedOnly)
        {
            return null;
        }

        // handle secret woods clumps
        if (loc is Woods woods)
        {
            foreach (ResourceClump? stump in woods.stumps)
            {
                if (stump is not null && stump.getBoundingBox(stump.tile.Value).Contains(nonTileX, nonTileY))
                {
                    return stump;
                }
            }
        }

        // the log blocking off the secret forest.
        if (loc is Forest forest)
        {
            if (forest.log is not null && forest.log.getBoundingBox(forest.log.tile.Value).Contains(nonTileX, nonTileX))
            {
                return forest.log;
            }
        }

        LargeTerrainFeature? terrain = loc.getLargeTerrainFeatureAt(nonTileX / Game1.tileSize, nonTileY / Game1.tileSize);
        if (terrain is null)
        {
            return null;
        }

        if (FarmTypeManagerShims.GetEmbeddedResourceClump?.Invoke(terrain) is ResourceClump embedded)
        {
            return embedded;
        }
        return terrain;
    }

    /// <summary>
    /// Gets the large object <see cref="LargeTerrainFeature"/> or <see cref="ResourceClump"/> at a specific location, and removes it.
    /// </summary>
    /// <param name="loc">Location to check.</param>
    /// <param name="nonTileX">X coord.</param>
    /// <param name="nonTileY">Y coord.</param>
    /// <param name="placedOnly">Whether or not to only include what the player has placed.</param>
    /// <returns>The feature, or null for not found.</returns>
    internal static TerrainFeature? RemoveLargeObjectAtLocation(this GameLocation loc, int nonTileX, int nonTileY, bool placedOnly = false)
    {
        if (loc.resourceClumps is not null)
        {
            for (int i = loc.resourceClumps.Count - 1; i >= 0; i--)
            {
                ResourceClump? clump = loc.resourceClumps[i];
                if (clump is not null && clump.getBoundingBox(clump.tile.Value).Contains(nonTileX, nonTileY)
                    && (!placedOnly || clump.modData?.ContainsKey(InventoryResourceClump.ResourceModdata) == true))
                {
                    loc.resourceClumps.RemoveAt(i);
                    return clump;
                }
            }
        }

        if (placedOnly)
        {
            return null;
        }

        // handle secret woods clumps
        if (loc is Woods woods)
        {
            for (int i = woods.stumps.Count - 1; i >= 0; i--)
            {
                ResourceClump? clump = woods.stumps[i];
                if (clump is not null && clump.getBoundingBox(clump.tile.Value).Contains(nonTileX, nonTileY))
                {
                    woods.stumps.RemoveAt(i);
                    return clump;
                }
            }
        }

        // the log blocking off the secret forest.
        if (loc is Forest forest)
        {
            if (forest.log is not null && forest.log.getBoundingBox(forest.log.tile.Value).Contains(nonTileX, nonTileY))
            {
                ResourceClump log = forest.log;
                forest.log = null;
                return log;
            }
        }

        if (loc.largeTerrainFeatures is not null)
        {
            Rectangle tileRect = new(nonTileX, nonTileY, 64, 64);
            for (int i = loc.largeTerrainFeatures.Count - 1; i >= 0; i--)
            {
                if (loc.largeTerrainFeatures[i] is LargeTerrainFeature terrain && terrain.getBoundingBox().Intersects(tileRect))
                {
                    loc.largeTerrainFeatures.RemoveAt(i);
                    if (FarmTypeManagerShims.GetEmbeddedResourceClump?.Invoke(terrain) is ResourceClump embedded)
                    {
                        return embedded;
                    }
                    return terrain;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Checks to see if I can stick a resource clump at a specific tile.
    /// </summary>
    /// <param name="location">Game location to check.</param>
    /// <param name="tileX">x coordinate of tile.</param>
    /// <param name="tileY">y coordinate of tile.</param>
    /// <param name="relaxed">whether or not to use relaxed placement rules.</param>
    /// <returns>True if placeable, false otherwise.</returns>
    [MethodImpl(TKConstants.Hot)]
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

        if (location is IAnimalLocation hasAnimals)
        {
            foreach (FarmAnimal? animal in hasAnimals.Animals.Values)
            {
                if (animal.GetBoundingBox().Intersects(position))
                {
                    return false;
                }
            }
        }

        if (relaxed)
        {
            return true;
        }

        Vector2 tile = new(tileX, tileY);
        if (location is BuildableGameLocation buildable)
        {
            foreach (Building? building in buildable.buildings)
            {
                if (!building.isTilePassable(tile))
                {
                    return false;
                }
            }
        }

        if ((location.terrainFeatures?.TryGetValue(tile, out var terrain) == true
            && (terrain is not HoeDirt dirt || dirt.crop is not null))
            || location.Objects?.ContainsKey(tile) == true)
        {
            return false;
        }

        return !location.isTileOccupied(tile) && location.isTilePassable(new XLocation(tileX, tileY), Game1.viewport);
    }

    /// <summary>
    /// Tries to get the name of an SObject.
    /// </summary>
    /// <param name="idx">index of that SOBject.</param>
    /// <returns>Name or placeholder if not found.</returns>
    internal static string GetNameOfSObject(int idx)
        => Game1Wrappers.ObjectInfo.TryGetValue(idx, out string? data)
            ? data.GetNthChunk('/').ToString()
            : "NoNameFound";

    /// <summary>
    /// Whether or not there's a tree of any type within radius two.
    /// </summary>
    /// <param name="loc">Game location to check.</param>
    /// <param name="tileX">X tile.</param>
    /// <param name="tileY">Y tile.</param>
    /// <returns>True if there's a tree of any type, false otherwise.</returns>
    internal static bool HasTreeInRadiusTwo(this GameLocation? loc, int tileX, int tileY)
    {
        if (loc?.terrainFeatures is null)
        {
            return false;
        }
        for (int x = tileX - 2; x <= tileX + 2; x++)
        {
            for (int y = tileY - 2; y <= tileY + 2; y++)
            {
                Vector2 tile = new(x, y);
                if (loc.terrainFeatures.TryGetValue(tile, out TerrainFeature? terrain)
                    && terrain is Tree or FruitTree)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
