/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.BetterChests.Framework.Models.Events;

using StardewMods.Common.Services.Integrations.BetterChests;

/// <summary>The event arguments before an item is highlighted.</summary>
public class ItemHighlightingEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ItemHighlightingEventArgs" /> class.</summary>
    /// <param name="container">The container with the item being highlighted.</param>
    /// <param name="item">The item being highlighted.</param>
    public ItemHighlightingEventArgs(IStorageContainer container, Item item)
    {
        this.Container = container;
        this.Item = item;
    }

    /// <summary>Gets the container with the item being highlighted.</summary>
    public IStorageContainer Container { get; }

    /// <summary>Gets the item being highlighted.</summary>
    public Item Item { get; }

    /// <summary>Gets a value indicating whether the item is highlighted.</summary>
    public bool IsHighlighted { get; private set; } = true;

    /// <summary>UnHighlight the item.</summary>
    public void UnHighlight() => this.IsHighlighted = false;
}