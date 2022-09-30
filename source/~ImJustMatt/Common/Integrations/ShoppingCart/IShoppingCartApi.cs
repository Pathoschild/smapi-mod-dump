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

/// <summary>
///     API for Shopping Cart.
/// </summary>
public interface IShoppingCartApi
{
    /// <summary>
    ///     Gets the current shop.
    /// </summary>
    IShop? CurrentShop { get; }
}