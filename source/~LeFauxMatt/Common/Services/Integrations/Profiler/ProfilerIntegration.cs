/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.Common.Services.Integrations.Profiler;

/// <inheritdoc />
internal sealed class ProfilerIntegration : ModIntegration<IProfilerApi>
{
    private const string ModUniqueId = "SinZ.Profiler";

    /// <summary>Initializes a new instance of the <see cref="ProfilerIntegration" /> class.</summary>
    /// <param name="modRegistry">Dependency used for fetching metadata about loaded mods.</param>
    public ProfilerIntegration(IModRegistry modRegistry)
        : base(modRegistry, ProfilerIntegration.ModUniqueId, "2.0.0")
    {
        // Nothing
    }
}