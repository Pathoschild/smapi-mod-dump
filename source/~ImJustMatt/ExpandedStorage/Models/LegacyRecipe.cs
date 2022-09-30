/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.ExpandedStorage.Models;

using System.Collections.Generic;

/// <summary>
///     Represents a Crafting Recipe which is formatted in DGA format.
/// </summary>
internal sealed class LegacyRecipe
{
    /// <summary>
    ///     Gets or sets the Id of the recipe.
    /// </summary>
    public string ID { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the ingredients used to craft the recipe.
    /// </summary>
    public List<Item>? Ingredients { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the item is known by default.
    /// </summary>
    public bool KnownByDefault { get; set; }

    /// <summary>
    ///     Gets or sets the item produced by the recipe.
    /// </summary>
    public Item? Result { get; set; }

    /// <summary>
    ///     Gets or sets teh skill level required to unlock the recipe.
    /// </summary>
    public int SkillUnlockLevel { get; set; }

    /// <summary>
    ///     Gets or sets the skill required to unlock the recipe.
    /// </summary>
    public string SkillUnlockName { get; set; } = string.Empty;

    /// <summary>
    ///     Represents an item to require or produce.
    /// </summary>
    public sealed class Item
    {
        /// <summary>
        ///     Gets or sets the quantity of an item to use or produce.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        ///     Gets or sets the item type.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the item value.
        /// </summary>
        public object? Value { get; set; }
    }
}