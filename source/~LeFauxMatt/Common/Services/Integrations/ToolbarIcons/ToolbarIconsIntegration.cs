/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Services.Integrations.ToolbarIcons;
#else
namespace StardewMods.Common.Services.Integrations.ToolbarIcons;
#endif

/// <inheritdoc />
internal sealed class ToolbarIconsIntegration : ModIntegration<IToolbarIconsApi>
{
    /// <summary>Initializes a new instance of the <see cref="ToolbarIconsIntegration" /> class.</summary>
    /// <param name="modRegistry">Dependency used for fetching metadata about loaded mods.</param>
    public ToolbarIconsIntegration(IModRegistry modRegistry)
        : base(modRegistry)
    {
        // Nothing
    }

    /// <inheritdoc />
    public override string UniqueId => "furyx639.ToolbarIcons";

    /// <inheritdoc/>
    public override ISemanticVersion Version { get; } = new SemanticVersion(2, 8, 0);
}