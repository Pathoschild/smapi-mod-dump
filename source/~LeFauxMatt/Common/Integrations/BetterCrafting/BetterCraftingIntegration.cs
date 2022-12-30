/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.BetterCrafting;

/// <inheritdoc />
internal sealed class BetterCraftingIntegration : ModIntegration<IBetterCrafting>
{
    private const string ModUniqueId = "leclair.bettercrafting";

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetterCraftingIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public BetterCraftingIntegration(IModRegistry modRegistry)
        : base(modRegistry, BetterCraftingIntegration.ModUniqueId, "1.2.0")
    {
        // Nothing
    }
}