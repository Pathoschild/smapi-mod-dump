/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.SpaceCore;

#endregion using directives

internal sealed class SpaceCoreIntegration : BaseIntegration<ISpaceCoreAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public SpaceCoreIntegration(IModRegistry modRegistry)
        : base("SpaceCore", "spacechase0.SpaceCore", "1.8.3", modRegistry) { }

    /// <summary>Cache the SpaceCore API and initialize reflected SpaceCore fields.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.SpaceCoreApi = ModApi;
    }
}