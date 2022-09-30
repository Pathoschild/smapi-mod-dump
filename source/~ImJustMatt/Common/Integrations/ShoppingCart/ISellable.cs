/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.ShoppingCart;

using System.Collections.Generic;

/// <summary>
///     Represents an item being sold.
/// </summary>
public interface ISellable : ICartItem
{
    /// <summary>
    ///     Attempt to sell an item.
    /// </summary>
    /// <param name="inventory">The inventory to sell items from.</param>
    /// <param name="currency">The shop's currency.</param>
    /// <param name="test">Indicates whether to test only.</param>
    /// <returns>Returns true if item can be sold.</returns>
    public bool TrySell(IList<Item?> inventory, int currency, bool test = false);
}