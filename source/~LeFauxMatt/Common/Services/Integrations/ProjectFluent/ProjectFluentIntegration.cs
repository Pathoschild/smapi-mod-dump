/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.ProjectFluent;

/// <inheritdoc />
internal sealed class ProjectFluentIntegration : ModIntegration<IProjectFluentApi>
{
    private const string ModUniqueId = "Shockah.ProjectFluent";

    /// <summary>Initializes a new instance of the <see cref="ProjectFluentIntegration" /> class.</summary>
    /// <param name="modRegistry">Dependency used for fetching metadata about loaded mods.</param>
    public ProjectFluentIntegration(IModRegistry modRegistry)
        : base(modRegistry, ProjectFluentIntegration.ModUniqueId, "2.0.0-alpha.20230814")
    {
        // Nothing
    }
}