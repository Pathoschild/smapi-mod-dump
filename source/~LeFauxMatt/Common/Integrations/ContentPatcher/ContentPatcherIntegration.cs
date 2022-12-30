/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.ContentPatcher;

/// <inheritdoc />
internal sealed class ContentPatcherIntegration : ModIntegration<IContentPatcherApi>
{
    private const string ModUniqueId = "Pathoschild.ContentPatcher";
    private const string ModVersion = "1.28.0";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContentPatcherIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public ContentPatcherIntegration(IModRegistry modRegistry)
        : base(modRegistry, ContentPatcherIntegration.ModUniqueId, ContentPatcherIntegration.ModVersion)
    {
        // Nothing
    }
}