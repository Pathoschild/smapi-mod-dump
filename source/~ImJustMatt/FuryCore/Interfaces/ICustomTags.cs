/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.FuryCore.Interfaces;

using System;
using StardewValley;

/// <summary>
///     Add custom context tags to <see cref="Item" />.
/// </summary>
internal interface ICustomTags
{
    /// <summary>
    ///     Custom context tag for items that are Artifacts.
    /// </summary>
    public const string CategoryArtifact = "category_artifact";

    /// <summary>
    ///     Custom context tag for items that are Furniture.
    /// </summary>
    public const string CategoryFurniture = "category_furniture";

    /// <summary>
    ///     Custom context tag for items that can be donated to the Community Center.
    /// </summary>
    public const string DonateBundle = "donate_bundle";

    /// <summary>
    ///     Custom context tag for items that can be donated to the Museum.
    /// </summary>
    public const string DonateMuseum = "donate_museum";

    /// <summary>
    ///     Adds a context tag to any item that currently meets the predicate.
    /// </summary>
    /// <param name="tag">The tag to add to the item.</param>
    /// <param name="predicate">The predicate to test items that should have the context tag added.</param>
    public void AddContextTag(string tag, Func<Item, bool> predicate);
}