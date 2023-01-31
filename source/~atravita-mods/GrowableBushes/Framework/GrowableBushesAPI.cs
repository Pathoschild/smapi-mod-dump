/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Utils.Extensions;

using Microsoft.Xna.Framework;

using StardewValley.TerrainFeatures;

namespace GrowableBushes.Framework;

/// <inheritdoc />
public sealed class GrowableBushesAPI : IGrowableBushesAPI
{
    /// <inheritdoc />
    public SObject GetBush(BushSizes size) => new InventoryBush(size, 1);

    /// <inheritdoc />
    public BushSizes GetSizeOfBushIfApplicable(SObject obj)
    {
        if (obj is InventoryBush bush && BushSizesExtensions.IsDefined((BushSizes)bush.ParentSheetIndex))
        {
            return (BushSizes)bush.ParentSheetIndex;
        }
        return BushSizes.Invalid;
    }

    /// <inheritdoc />
    public BushSizes CanPickUpBush(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        LargeTerrainFeature? feat = loc.getLargeTerrainFeatureAt((int)tile.X, (int)tile.Y);
        if (feat is Bush bush)
        {
            BushSizes metaData = bush.modData.GetEnum(InventoryBush.BushModData, BushSizes.Invalid);

            if (placedOnly)
            {
                return metaData;
            }

            BushSizes size = bush.ToBushSize();
            return size == BushSizes.Walnut ? BushSizes.Invalid : size;
        }
        return BushSizes.Invalid;
    }

    /// <inheritdoc />
    public SObject? TryPickUpBush(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        BushSizes size = this.CanPickUpBush(loc, tile, placedOnly);
        return size == BushSizes.Invalid ? null : new InventoryBush(size, 1);
    }

    /// <inheritdoc />
    public bool CanPlaceBush(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryBush bush && bush.CanPlace(loc, tile, relaxed);

    /// <inheritdoc />
    public bool TryPlaceBush(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryBush bush && bush.PlaceBush(loc, (int)(tile.X * Game1.tileSize), (int)(tile.Y * Game1.tileSize), relaxed);
}
