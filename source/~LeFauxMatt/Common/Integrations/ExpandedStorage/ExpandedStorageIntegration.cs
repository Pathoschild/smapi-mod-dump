/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.ExpandedStorage;

/// <inheritdoc />
internal sealed class ExpandedStorageIntegration : ModIntegration<IExpandedStorageApi>
{
    private const string ModUniqueId = "furyx639.ExpandedStorage";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExpandedStorageIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public ExpandedStorageIntegration(IModRegistry modRegistry)
        : base(modRegistry, ExpandedStorageIntegration.ModUniqueId, "2.0.0")
    {
        // Nothing
    }
}