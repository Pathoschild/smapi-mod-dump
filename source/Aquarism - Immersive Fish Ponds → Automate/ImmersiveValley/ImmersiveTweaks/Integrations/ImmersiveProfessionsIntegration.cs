/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Tweex.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.WalkOfLife;

#endregion using directives

internal sealed class ImmersiveProfessionsIntegration : BaseIntegration<IImmersiveProfessionsAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public ImmersiveProfessionsIntegration(IModRegistry modRegistry)
        : base("Immersive Professions", "DaLion.ImmersiveProfessions", "4.0.0", modRegistry) { }

    /// <summary>Cache the Immersive Professions API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.ProfessionsApi = ModApi;
    }
}