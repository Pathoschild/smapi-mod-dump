/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.CustomBush;

/// <inheritdoc />
internal sealed class CustomBushIntegration : ModIntegration<ICustomBushApi>
{
    private const string ModUniqueId = "furyx639.CustomBush";

    /// <summary>Initializes a new instance of the <see cref="CustomBushIntegration" /> class.</summary>
    /// <param name="modRegistry">Dependency used for fetching metadata about loaded mods.</param>
    public CustomBushIntegration(IModRegistry modRegistry)
        : base(modRegistry, CustomBushIntegration.ModUniqueId)
    {
        // Nothing
    }
}