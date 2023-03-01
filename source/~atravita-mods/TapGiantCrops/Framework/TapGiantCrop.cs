/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;

using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.Shims;
using AtraShared.Wrappers;

using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace TapGiantCrops.Framework;

/// <summary>
/// API instance for Tap Giant Crops.
/// </summary>
public sealed class TapGiantCrop : ITapGiantCropsAPI
{
    /// <summary>
    /// A setter to shake a giant crop.
    /// </summary>
    private static readonly Action<GiantCrop, float> GiantCropSetShake = typeof(GiantCrop)
        .GetCachedField("shakeTimer", ReflectionCache.FlagTypes.InstanceFlags)
        .GetInstanceFieldSetter<GiantCrop, float>();

    internal static void ShakeGiantCrop(GiantCrop crop)
    {
        GiantCropSetShake(crop, 100f);
        crop.NeedsUpdate = true;
    }

    private SObject keg = null!;

    /// <inheritdoc />
    public bool CanPlaceTapper(GameLocation loc, Vector2 tile, SObject obj)
    {
        Guard.IsNotNull(loc);
        Guard.IsNotNull(obj);
        if (loc.objects.ContainsKey(tile))
        {
            return false;
        }
        if (obj.Name.Contains("Tapper", StringComparison.OrdinalIgnoreCase))
        {
            return GetGiantCropAt(loc, tile) is not null;
        }
        return false;
    }

    /// <inheritdoc />
    public bool TryPlaceTapper(GameLocation loc, Vector2 tile, SObject obj)
    {
        Guard.IsNotNull(loc);
        Guard.IsNotNull(obj);
        if (this.CanPlaceTapper(loc, tile, obj))
        {
            SObject tapper = (SObject)obj.getOne();
            if (GetGiantCropAt(loc, tile) is GiantCrop giant)
            {
                LargeTerrainFeature? terrain = loc.getLargeTerrainFeatureAt((int)tile.X, (int)tile.Y);
                if (terrain is not null && FarmTypeManagerShims.GetEmbeddedResourceClump?.Invoke(terrain) is GiantCrop crop)
                {
                    // Moving crop to the normal location.
                    loc.largeTerrainFeatures.Remove(terrain);
                    loc.resourceClumps.Add(crop);
                }

                (SObject obj, int days)? output = this.GetTapperProduct(giant, tapper);
                if (output is not null)
                {
                    tapper.heldObject.Value = output.Value.obj;
                    tapper.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, output.Value.days);
                }

                loc.playSound("axe");
                ShakeGiantCrop(giant);
                loc.objects[tile] = tapper;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Given a particular giant crop index, gets the relevant tapper output.
    /// </summary>
    /// <param name="giantCrop">The giant crop.</param>
    /// <param name="tapper">The tapper in question.</param>
    /// <returns>tuple of the item and how long it should take.</returns>
    [Pure]
    public (SObject obj, int days)? GetTapperProduct(GiantCrop giantCrop, SObject tapper)
    {
        if (DynamicGameAssetsShims.IsDGAGiantCrop?.Invoke(giantCrop) == true || giantCrop.parentSheetIndex?.Value is null)
        {
            return null;
        }

        int giantCropIndx = giantCrop.parentSheetIndex.Value;

        SObject? returnobj = AssetManager.GetOverrideItem(giantCropIndx);

        if (returnobj is null)
        {
            // find a keg output.
            SObject crop = new(giantCropIndx, 999);
            this.keg.heldObject.Value = null;
            this.keg.performObjectDropInAction(crop, false, Game1.player);
            SObject? heldobj = this.keg.heldObject.Value;
            this.keg.heldObject.Value = null;
            if (heldobj?.getOne() is SObject obj)
            {
                returnobj = obj;
            }
        }

        // special case: giant flowers make honey
        // this makes no sense.
        if (returnobj is null && giantCropIndx.GetCategoryFromIndex() == SObject.flowersCategory)
        {
            string flowerdata = Game1Wrappers.ObjectInfo[giantCropIndx];
            returnobj = new SObject(340, 1); // honey index.
            string honeyName = $"{flowerdata.GetNthChunk('/', 0).ToString()} Honey";

            returnobj.Name = honeyName;
            if (int.TryParse(flowerdata.GetNthChunk('/', SObject.objectInfoPriceIndex), out int price))
            {
                returnobj.Price += 2 * price;
            }
            returnobj.preservedParentSheetIndex.Value = giantCropIndx;
        }

        if (returnobj is not null)
        {
            int days = returnobj.Price / (25 * giantCrop.width.Value * giantCrop.height.Value);
            if (tapper.ParentSheetIndex == 264)
            {
                days /= 2;
            }
            return (returnobj, Math.Max(1, days));
        }

        // fallback - return sap.
        return (new SObject(92, 20), 2);
    }

    /// <summary>
    /// Initializes the API (gets the keg instance used to find the output).
    /// </summary>
    internal void Init()
        => this.keg = new(Vector2.Zero, (int)VanillaMachinesEnum.Keg);

    private static GiantCrop? GetGiantCropAt(GameLocation loc, Vector2 tile)
    {
        if (loc.resourceClumps is not null)
        {
            foreach (ResourceClump? clump in loc.resourceClumps)
            {
                if (clump is GiantCrop crop && !(DynamicGameAssetsShims.IsDGAGiantCrop?.Invoke(crop) == true))
                {
                    Vector2 offset = tile;
                    offset.Y -= crop.height.Value - 1;
                    offset.X -= crop.width.Value / 2;
                    if (crop.tile.Value.X.WithinMargin(offset.X) && crop.tile.Value.Y.WithinMargin(offset.Y))
                    {
                        return crop;
                    }
                }
            }
        }
        if (FarmTypeManagerShims.GetEmbeddedResourceClump is not null && loc.largeTerrainFeatures is not null)
        {
            LargeTerrainFeature terrain = loc.getLargeTerrainFeatureAt((int)tile.X, (int)tile.Y);
            if (terrain is not null && FarmTypeManagerShims.GetEmbeddedResourceClump(terrain) is GiantCrop crop
                && !(DynamicGameAssetsShims.IsDGAGiantCrop?.Invoke(crop) == true))
            {
                Vector2 offset = tile;
                offset.Y -= crop.height.Value - 1;
                offset.X -= crop.width.Value / 2;
                if (crop.tile.Value.X.WithinMargin(offset.X) && crop.tile.Value.Y.WithinMargin(offset.Y))
                {
                    return crop;
                }
            }
        }

        return null;
    }
}
