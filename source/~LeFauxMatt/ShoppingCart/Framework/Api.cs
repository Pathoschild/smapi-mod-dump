/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.ShoppingCart.Framework;

using StardewMods.Common.Integrations.ShoppingCart;

/// <inheritdoc />
public sealed class Api : IShoppingCartApi
{
    /// <inheritdoc />
    public IShop? CurrentShop => ModEntry.CurrentShop;
}