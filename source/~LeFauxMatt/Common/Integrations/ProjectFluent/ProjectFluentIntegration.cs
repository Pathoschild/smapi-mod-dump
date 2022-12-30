/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Integrations.ProjectFluent;

/// <inheritdoc />
internal sealed class ProjectFluentIntegration : ModIntegration<IProjectFluentApi>
{
    private const string ModUniqueId = "Shockah.ProjectFluent";

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectFluentIntegration" /> class.
    /// </summary>
    /// <param name="modRegistry">SMAPI's mod registry.</param>
    public ProjectFluentIntegration(IModRegistry modRegistry)
        : base(modRegistry, ProjectFluentIntegration.ModUniqueId)
    {
        // Nothing
    }
}