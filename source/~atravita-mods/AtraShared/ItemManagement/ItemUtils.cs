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

using AtraShared.ConstantsAndEnums;

using Microsoft.Xna.Framework;
using StardewValley.Objects;
using StardewValley.Tools;

namespace AtraShared.ItemManagement;

/// <summary>
/// Methods to help deal with items.
/// </summary>
public static class ItemUtils
{
    /// <summary>
    /// Get an item from the string identifier system.
    /// Returns null if it's not something that can be handled.
    /// </summary>
    /// <param name="type">Identifier string (like "O" or "B").</param>
    /// <param name="id">int id.</param>
    /// <returns>Item if possible.</returns>
    /// <remarks>Carries the no-inlining attribute so other mods can patch this.</remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Item? GetItemFromIdentifier(string type, int id)
    => type switch
    {
        "F" or "f" => Furniture.GetFurnitureInstance(id),
        "O" or "o" => new SObject(id, 1),
        "BL" or "bl" => new SObject(id, 1, isRecipe: true), // big craftable recipes.
        "BO" or "bo" => new SObject(Vector2.Zero, id),
        "BBL" or "bbl" => new SObject(Vector2.Zero, id, isRecipe: true),
        "R" or "r" => new Ring(id),
        "B" or "b" => new Boots(id),
        "W" or "w" => new MeleeWeapon(id),
        "H" or "h" => new Hat(id),
        "C" or "c" => new Clothing(id),
        _ => null,
    };

    /// <summary>
    /// Get an item using the enum as a category.
    /// Returns null if it's not something that can be handled.
    /// </summary>
    /// <param name="type">Identifier string (like "O" or "B").</param>
    /// <param name="id">int id.</param>
    /// <returns>Item if possible.</returns>
    /// <remarks>Carries the no-inlining attribute so other mods can patch this.</remarks>
    [Pure]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Item? GetItemFromIdentifier(ItemTypeEnum type, int id)
        => type switch
        {
            ItemTypeEnum.Furniture => Furniture.GetFurnitureInstance(id),
            ItemTypeEnum.SObject => new SObject(id, 1),
            ItemTypeEnum.BigCraftable => new SObject(Vector2.Zero, id),
            ItemTypeEnum.Ring => new Ring(id),
            ItemTypeEnum.Boots => new Boots(id),
            ItemTypeEnum.Weapon => new MeleeWeapon(id),
            ItemTypeEnum.Hat => new Hat(id),
            ItemTypeEnum.Clothing => new Clothing(id),
            _ => null,
        };
}
