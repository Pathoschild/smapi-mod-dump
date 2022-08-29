/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using StardewValley;

#endregion using directives

/// <summary>Extensions for the <see cref="Item"/> class.</summary>
public static class ItemExtensions
{
    /// <summary>Whether the ammo is a stone or mineral ore.</summary>
    public static bool IsMineralAmmo(this Item ammo) => ammo.ParentSheetIndex.IsMineralAmmoIndex();

    /// <summary>Whether the ammo should make squishy noises upon collision.</summary>
    public static bool IsSquishyAmmo(this Item ammo) =>
        ammo.Category is SObject.EggCategory or SObject.FruitsCategory or SObject.VegetableCategory;
}