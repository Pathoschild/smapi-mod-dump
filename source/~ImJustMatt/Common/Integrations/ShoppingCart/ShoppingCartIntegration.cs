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

/// <inheritdoc />
internal sealed class ShoppingCartIntegration : ModIntegration<IShoppingCartApi>
{
    private const string ModUniqueId = "furyx639.ShoppingCart";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ShoppingCartIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public ShoppingCartIntegration(IModRegistry modRegistry)
        : base(modRegistry, ShoppingCartIntegration.ModUniqueId)
    {
        // Nothing
    }
}