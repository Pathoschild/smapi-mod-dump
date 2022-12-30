/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.StackQuality;

/// <inheritdoc />
internal sealed class StackQualityIntegration : ModIntegration<IStackQualityApi>
{
    private const string ModUniqueId = "furyx639.StackQuality";

    /// <summary>
    ///     Initializes a new instance of the <see cref="StackQualityIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public StackQualityIntegration(IModRegistry modRegistry)
        : base(modRegistry, StackQualityIntegration.ModUniqueId, "1.0.0-beta.6")
    {
        // Nothing
    }
}