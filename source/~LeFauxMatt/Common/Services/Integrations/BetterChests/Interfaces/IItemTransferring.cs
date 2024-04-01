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

/// <summary>The event arguments before an item is transferred into a container.</summary>
public interface IItemTransferring
{
    /// <summary>Gets a value indicating whether the transfer is being forced.</summary>
    public bool IsForced { get; }

    /// <summary>Gets the destination container to which the item was sent.</summary>
    public IStorageContainer Into { get; }

    /// <summary>Gets the item that was transferred.</summary>
    public Item Item { get; }

    /// <summary>Gets a value indicating whether the the transfer is prevented.</summary>
    public bool IsPrevented { get; }

    /// <summary>Prevent the transfer.</summary>
    public void PreventTransfer();
}