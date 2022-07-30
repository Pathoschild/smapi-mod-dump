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
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Shims;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

namespace TapGiantCrops.Framework;

/// <summary>
/// API instance for Tap Giant Crops.
/// </summary>
public class TapGiantCrop : ITapGiantCropsAPI
{
    private SObject keg = null!;

    /// <inheritdoc />
    public bool CanPlaceTapper(GameLocation loc, Vector2 tile, SObject obj)
    {
        Guard.IsNotNull(loc, nameof(loc));
        Guard.IsNotNull(obj, nameof(obj));
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
        Guard.IsNotNull(loc, nameof(loc));
        Guard.IsNotNull(obj, nameof(obj));
        if (this.CanPlaceTapper(loc, tile, obj))
        {
            SObject tapper = (SObject)obj.getOne();
            GiantCrop? giant = GetGiantCropAt(loc, tile);
            if (giant is not null)
            {
                (SObject obj, int days)? output = this.GetTapperProduct(giant, tapper);
                if (output is not null)
                {
                    tapper.heldObject.Value = output.Value.obj;
                    tapper.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, output.Value.days);
                }
            }
            loc.objects[tile] = tapper;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Given a particular giant crop index, gets the relevant tapper output.
    /// </summary>
    /// <param name="giantCrop">The giant crop.</param>
    /// <param name="tapper">The tapper in question.</param>
    /// <returns>tuple of the item and how long it should take.</returns>
    public (SObject obj, int days)? GetTapperProduct(GiantCrop giantCrop, SObject tapper)
    {
        if (DynamicGameAssetsShims.IsDGAGiantCrop?.Invoke(giantCrop) == true)
        {
            return null;
        }
        int cropindex = giantCrop.which.Value switch
        {
            0 => 190,
            1 => 254,
            2 => 276,
            _ => giantCrop.which.Value,
        };

        SObject crop = new(cropindex, 999);
        this.keg.heldObject.Value = null;
        this.keg.performObjectDropInAction(crop, false, Game1.player);
        SObject? heldobj = this.keg.heldObject.Value;
        this.keg.heldObject.Value = null;
        if (heldobj?.getOne() is SObject returnobj)
        {
            int days = returnobj.Price / (25 * giantCrop.width.Value * giantCrop.height.Value);
            if (tapper.ParentSheetIndex == 264)
            {
                days /= 2;
            }
            return (returnobj, Math.Max(1, days));
        }
        return null;
    }

    /// <summary>
    /// Initializes the API (gets the keg instance used to find the output).
    /// </summary>
    internal void Init()
        => this.keg = new(Vector2.Zero, (int)VanillaMachinesEnum.Keg);

    private static GiantCrop? GetGiantCropAt(GameLocation loc, Vector2 tile)
    {
        foreach(ResourceClump? clump in loc.resourceClumps)
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
        return null;
    }
}
