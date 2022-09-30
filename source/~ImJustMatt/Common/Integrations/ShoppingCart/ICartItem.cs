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

using System;

/// <summary>
///     Represents an item being bought or sold.
/// </summary>
public interface ICartItem : IComparable<ICartItem>
{
    /// <summary>
    ///     Gets available quantity of the item.
    /// </summary>
    public int Available { get; }

    /// <summary>
    ///     Gets the item.
    /// </summary>
    public ISalable Item { get; }

    /// <summary>
    ///     Gets the individual sale price of an item.
    /// </summary>
    public int Price { get; }

    /// <summary>
    ///     Gets or sets the quantity to buy/sell.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    ///     Gets the total price.
    /// </summary>
    public long Total { get; }
}