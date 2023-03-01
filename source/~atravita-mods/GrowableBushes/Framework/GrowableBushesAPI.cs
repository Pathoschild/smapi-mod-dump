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
        if (loc.largeTerrainFeatures is null)
        {
            return BushSizes.Invalid;
        }

        LargeTerrainFeature? feat = loc.getLargeTerrainFeatureAt((int)tile.X, (int)tile.Y);
        if (feat is Bush bush)
        {
            return this.CanPickUpBush(bush, placedOnly);
        }
        return BushSizes.Invalid;
    }

    /// <inheritdoc />
    public BushSizes CanPickUpBush(Bush bush, bool placedOnly = false)
    {
        BushSizes metaData = bush.modData.GetEnum(InventoryBush.BushModData, BushSizes.Invalid);

        if (placedOnly || metaData != BushSizes.Invalid)
        {
            return metaData;
        }

        BushSizes size = bush.ToBushSize();
        return size == BushSizes.Walnut ? BushSizes.Invalid : size;
    }

    /// <inheritdoc />
    public SObject? TryPickUpBush(GameLocation loc, Vector2 tile, bool placedOnly = false)
    {
        if (loc.largeTerrainFeatures is not null)
        {
            Rectangle tileRect = new((int)tile.X * 64, (int)tile.Y * 64, 64, 64);
            for (int i = loc.largeTerrainFeatures.Count - 1; i >= 0; i--)
            {
                if (loc.largeTerrainFeatures[i] is Bush bush && bush.getBoundingBox().Intersects(tileRect))
                {
                    BushSizes size = this.CanPickUpBush(bush, placedOnly);
                    if (size != BushSizes.Invalid)
                    {
                        InventoryBush.BushShakeMethod(bush, tile, true);
                        loc.largeTerrainFeatures.RemoveAt(i);
                        InventoryBush pickedUpBush = new(size, 1)
                        {
                            TileLocation = bush.tilePosition.Value,
                        };

                        if (ModEntry.Config.PreserveModData)
                        {
                            pickedUpBush.modData.CopyModDataFrom(bush.modData);
                        }

                        return pickedUpBush;
                    }
                }
            }
        }
        return null;
    }

    /// <inheritdoc />
    public bool CanPlaceBush(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryBush bush && bush.CanPlace(loc, tile, relaxed);

    /// <inheritdoc />
    public bool TryPlaceBush(SObject obj, GameLocation loc, Vector2 tile, bool relaxed)
        => obj is InventoryBush bush && bush.PlaceBush(loc, (int)(tile.X * Game1.tileSize), (int)(tile.Y * Game1.tileSize), relaxed);

    /// <inheritdoc />
    public void DrawPickUpGraphics(SObject obj, GameLocation loc, Vector2 tile)
    {
        if (obj is InventoryBush bush)
        {
            bush.DrawPickUpGraphics(loc, tile);
        }
    }
}
