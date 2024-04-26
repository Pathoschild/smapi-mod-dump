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

/// <summary>The event arguments before items are displayed.</summary>
public interface IItemsDisplaying
{
    /// <summary>Gets the container with the items being displayed.</summary>
    public IStorageContainer Container { get; }

/// <summary>Gets the items being displayed.</summary>
    public IEnumerable<Item> Items { get; }

    /// <summary>Updates the collection of items using the specified operation.</summary>
    /// <param name="operation">A function that takes a collection of items and returns a modified collection.</param>
    public void Edit(Func<IEnumerable<Item>, IEnumerable<Item>> operation);
}