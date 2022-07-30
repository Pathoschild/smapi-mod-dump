/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.BetterCrafting;

using StardewModdingAPI;

/// <inheritdoc />
internal class BetterCraftingIntegration : ModIntegration<IBetterCraftingApi>
{
    private const string ModUniqueId = "leclair.bettercrafting";

    /// <summary>
    ///     Initializes a new instance of the <see cref="BetterCraftingIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public BetterCraftingIntegration(IModRegistry modRegistry)
        : base(modRegistry, BetterCraftingIntegration.ModUniqueId)
    {
    }
}