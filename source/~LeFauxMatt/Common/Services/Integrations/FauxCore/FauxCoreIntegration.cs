/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.FauxCore;

/// <inheritdoc />
internal sealed class FauxCoreIntegration : ModIntegration<IFauxCoreApi>
{
    private const string ModUniqueId = "furyx639.FauxCore";

    /// <summary>Initializes a new instance of the <see cref="FauxCoreIntegration" /> class.</summary>
    /// <param name="modRegistry">Dependency used for fetching metadata about loaded mods.</param>
    public FauxCoreIntegration(IModRegistry modRegistry)
        : base(modRegistry, FauxCoreIntegration.ModUniqueId)
    {
        // Nothing
    }
}