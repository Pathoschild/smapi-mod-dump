/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.BetterChests.Interfaces;

using StardewValley.Menus;

/// <summary>The event arguments before an item is highlighted.</summary>
public interface IItemHighlighting
{
    /// <summary>Gets the container with the item being highlighted.</summary>
    public IStorageContainer Container { get; }

    /// <summary>Gets the item being highlighted.</summary>
    public Item Item { get; }

    /// <summary>Gets a value indicating whether the item is highlighted.</summary>
    public bool IsHighlighted { get; }

    /// <summary>UnHighlight the item.</summary>
    public void UnHighlight();
}