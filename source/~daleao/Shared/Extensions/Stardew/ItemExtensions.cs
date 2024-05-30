/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Shared.Extensions.Stardew;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Enums;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Determines whether the <paramref name="item"/> is an artisan good.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is created by an artisan machine, otherwise <see langword="false"/>.</returns>
    public static bool IsArtisanGood(this Item item)
    {
        return item.Category is (int)ObjectCategory.ArtisanGoods or (int)ObjectCategory.Syrups ||
               item.QualifiedItemId == QualifiedObjectIds.Coffee;
    }

    /// <summary>Determines whether the <paramref name="item"/> is Salmonberry or Blackberry.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is Salmonberry or Blackberry, otherwise <see langword="false"/>.</returns>
    public static bool IsWildBerry(this Item item)
    {
        return item.QualifiedItemId.IsWildBerryId();
    }

    /// <summary>Determines whether the <paramref name="item"/> is a mushroom.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a mushroom, otherwise <see langword="false"/>.</returns>
    public static bool IsMushroom(this Item item)
    {
        return item.QualifiedItemId.IsMushroomId();
    }

    /// <summary>Determines whether the <paramref name="item"/> is a syrup.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a syrup, otherwise <see langword="false"/>.</returns>
    public static bool IsSyrup(this Item item)
    {
        return item.Category == SObject.syrupCategory;
    }

    /// <summary>
    ///     Determines whether the <paramref name="item"/> is a fish typically caught with a
    ///     <see cref="StardewValley.Tools.FishingRod"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is any fish, otherwise <see langword="false"/>.</returns>
    public static bool IsFish(this Item item)
    {
        return item.Category == SObject.FishCategory;
    }

    /// <summary>
    ///     Determines whether the <paramref name="item"/> is a fish typically caught with a
    ///     <see cref="CrabPot"/>.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a <see cref="CrabPot"/> fish, otherwise <see langword="false"/>.</returns>
    public static bool IsTrapFish(this Item item)
    {
        return item.HasContextTag("fish_crab_pot");
    }

    /// <summary>
    ///     Determines whether the <paramref name="item"/> is a legendary fish.
    /// </summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a legendary fish, otherwise <see langword="false"/>.</returns>
    public static bool IsBossFish(this Item item)
    {
        return item.HasContextTag("fish_legendary");
    }

    /// <summary>Determines whether the <paramref name="item"/> is trash.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is any trash item, otherwise <see langword="false"/>.</returns>
    public static bool IsTrash(this Item item)
    {
        return item.Category == SObject.junkCategory;
    }

    /// <summary>Determines whether the <paramref name="item"/> is algae or seaweed.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is any of the three algae, otherwise <see langword="false"/>.</returns>
    public static bool IsAlgae(this Item item)
    {
        return item.QualifiedItemId.IsAlgaeId();
    }

    /// <summary>Determines whether the <paramref name="item"/> is a gem or mineral.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a gem or mineral, otherwise <see langword="false"/>.</returns>
    public static bool IsGemOrMineral(this Item item)
    {
        return item.Category is SObject.GemCategory or SObject.mineralsCategory;
    }

    /// <summary>Determines whether the <paramref name="item"/> is a foraged mineral.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a Quartz, Fire Quartz, Frozen Tear or Earth Crystal, otherwise <see langword="false"/>.</returns>
    public static bool IsForagedMineral(this Item item)
    {
        return item.QualifiedItemId is QualifiedObjectIds.Quartz or QualifiedObjectIds.FireQuartz or QualifiedObjectIds.FrozenTear
            or QualifiedObjectIds.EarthCrystal;
    }

    /// <summary>Determines whether the <paramref name="item"/> is a forage item typically found at the beach.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a forage item typically found at the beach, otherwise <see langword="false"/>.</returns>
    public static bool IsBeachForage(this Item item)
    {
        return item.HasContextTag("forage_item_beach");
    }

    /// <summary>Determines whether the <paramref name="item"/> is a simple Stone.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a mining node not containing only stone, otherwise <see langword="false"/>.</returns>
    public static bool IsStone(this Item item)
    {
        return item.Name == "Stone";
    }

    /// <summary>Determines whether the <paramref name="item"/> is a twig.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is twig, otherwise <see langword="false"/>.</returns>
    public static bool IsTwig(this Item item)
    {
        return item.QualifiedItemId is QualifiedObjectIds.Twig0 or QualifiedObjectIds.Twig1;
    }

    /// <summary>Determines whether the <paramref name="item"/> is a weed.</summary>
    /// <param name="item">The <see cref="Item"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is weed, otherwise <see langword="false"/>.</returns>
    public static bool IsWeed(this Item item)
    {
        return item is not Chest && item.Name == "Weeds";
    }
}
