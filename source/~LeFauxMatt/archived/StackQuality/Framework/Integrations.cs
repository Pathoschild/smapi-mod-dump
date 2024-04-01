/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.StackQuality.Framework;

using StardewMods.Common.Integrations.ShoppingCart;

/// <summary>
///     Handles integrations with other mods.
/// </summary>
internal sealed class Integrations
{
#nullable disable
    private static Integrations instance;
#nullable enable

    private readonly IModHelper helper;
    private readonly ShoppingCartIntegration shoppingCartIntegration;

    private Integrations(IModHelper helper)
    {
        this.helper = helper;
        this.shoppingCartIntegration = new(helper.ModRegistry);
    }

    /// <summary>
    ///     Gets Stack Quality integration.
    /// </summary>
    public static ShoppingCartIntegration ShoppingCart => Integrations.instance.shoppingCartIntegration;

    /// <summary>
    ///     Initializes <see cref="Integrations" />.
    /// </summary>
    /// <param name="helper">SMAPI helper for events, input, and content.</param>
    /// <returns>Returns an instance of the <see cref="Integrations" /> class.</returns>
    public static Integrations Init(IModHelper helper)
    {
        return Integrations.instance ??= new(helper);
    }
}