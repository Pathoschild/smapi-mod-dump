/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Extensions;

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Check with DGA if the instance is a Hero Soul.</summary>
    public static bool IsHeroSoul(this Item item) => ModEntry.DynamicGameAssetsApi!.GetDGAItemId(item) ==
                                                     ModEntry.Manifest.UniqueID + "/Hero Soul";

    /// <summary>Whether the ammo should make squishy noises upon collision.</summary>
    public static bool IsSquishyAmmo(this Item ammo) =>
        ammo.Category is SObject.EggCategory or SObject.FruitsCategory or SObject.VegetableCategory;
}