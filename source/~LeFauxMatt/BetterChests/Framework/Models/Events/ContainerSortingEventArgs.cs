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

/// <summary>The event arguments before a container is sorted.</summary>
internal sealed class ContainerSortingEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="ContainerSortingEventArgs" /> class.</summary>
    /// <param name="container">The container being sorted.</param>
    public ContainerSortingEventArgs(IStorageContainer container)
    {
        this.Container = container;
        this.OriginalItems = [..container.Items];
    }

    /// <summary>Gets the container being sorted.</summary>
    public IStorageContainer Container { get; }

    /// <summary>Gets the original list of items.</summary>
    public IReadOnlyList<Item> OriginalItems { get; }
}