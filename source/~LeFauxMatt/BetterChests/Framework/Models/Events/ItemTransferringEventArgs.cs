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

/// <inheritdoc cref="StardewMods.Common.Services.Integrations.BetterChests.Interfaces.IItemTransferring" />
internal sealed class ItemTransferringEventArgs : EventArgs, IItemTransferring
{
    /// <summary>Initializes a new instance of the <see cref="ItemTransferringEventArgs" /> class.</summary>
    /// <param name="into">The container being transferred into.</param>
    /// <param name="item">The item being transferred.</param>
    public ItemTransferringEventArgs(IStorageContainer into, Item item)
    {
        this.Into = into;
        this.Item = item;
    }

    /// <inheritdoc />
    public IStorageContainer Into { get; }

    /// <inheritdoc />
    public Item Item { get; }

    /// <inheritdoc />
    public bool IsAllowed { get; private set; }

    /// <inheritdoc />
    public bool IsPrevented { get; private set; }

    /// <inheritdoc />
    public void AllowTransfer() => this.IsAllowed = true;

    /// <inheritdoc />
    public void PreventTransfer() => this.IsPrevented = true;
}