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

using StardewMods.Common.Services.Integrations.BetterChests.Interfaces;

/// <inheritdoc cref="StardewMods.Common.Services.Integrations.BetterChests.Interfaces.IItemsDisplaying" />
public class ItemsDisplayingEventArgs : EventArgs, IItemsDisplaying
{
    private IList<Item> items;

    /// <summary>Initializes a new instance of the <see cref="ItemsDisplayingEventArgs" /> class.</summary>
    /// <param name="container">The container with the items being displayed.</param>
    public ItemsDisplayingEventArgs(IStorageContainer container)
    {
        this.Container = container;
        this.items = [..container.Items];
    }

    /// <inheritdoc />
    public IStorageContainer Container { get; }

    /// <inheritdoc />
    public IEnumerable<Item> Items => this.items;

    /// <inheritdoc />
    public void Edit(Func<IEnumerable<Item>, IEnumerable<Item>> operation) =>
        this.items = [..operation.Invoke(this.items)];
}