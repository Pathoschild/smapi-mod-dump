/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.JsonAssets;

/// <inheritdoc />
internal sealed class JsonAssetsIntegration : ModIntegration<IApi>
{
    private const string ModUniqueId = "spacechase0.JsonAssets";

    /// <summary>
    ///     Initializes a new instance of the <see cref="JsonAssetsIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public JsonAssetsIntegration(IModRegistry modRegistry)
        : base(modRegistry, JsonAssetsIntegration.ModUniqueId)
    {
        // Nothing
    }
}