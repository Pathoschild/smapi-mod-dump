/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.ConstantsAndEnums;

using Microsoft.Xna.Framework;

namespace MoreFertilizers.Framework;

/// <summary>
/// Handles grabbing the right beverage for the Miraculous Fertilizer.
/// </summary>
internal static class MiraculousFertilizerHandler
{
    private static SObject? keg;

    /// <summary>
    /// Initializes the keg instance used here.
    /// Call no earlier than SaveLoaded.
    /// </summary>
    internal static void Initialize()
    {
        keg = new SObject(Vector2.Zero, (int)VanillaMachinesEnum.Keg);
    }

    /// <summary>
    /// Gets the relevant beverage for the beverage fertilizer.
    /// </summary>
    /// <param name="objindex">The index of the crop.</param>
    /// <returns>The beverage, if any.</returns>
    internal static SObject? GetBeverage(int objindex)
        => GetBeverage(new SObject(objindex, 999));

    /// <summary>
    /// Gets the relevant beverage for the beverage fertilizer.
    /// </summary>
    /// <param name="item">The crop.</param>
    /// <returns>The beverage, if any.</returns>
    internal static SObject? GetBeverage(Item item)
    {
        if (keg is null)
        {
            return null;
        }

        if (item.Stack != item.maximumStackSize())
        {
            item = item.getOne();
            item.Stack = item.maximumStackSize();
        }

        keg.heldObject.Value = null;
        keg.performObjectDropInAction(item, false, Game1.player);
        SObject? heldobj = keg.heldObject.Value;
        if (heldobj?.getOne() is SObject returnobj && Game1.random.NextDouble() < (25.0 + Game1.player.LuckLevel) / Math.Max(heldobj.Price, 150))
        {
            return returnobj;
        }
        return null;
    }
}