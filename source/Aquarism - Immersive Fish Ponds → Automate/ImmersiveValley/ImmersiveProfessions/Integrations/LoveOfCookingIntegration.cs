/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.LoveOfCooking;

#endregion using directives

internal sealed class LoveOfCookingIntegration : BaseIntegration<ICookingSkillAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public LoveOfCookingIntegration(IModRegistry modRegistry)
        : base("LoveOfCooking", "blueberry.LoveOfCooking", "1.0.27", modRegistry) { }

    /// <summary>Cache the Love Of Cooking API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.CookingSkillApi = ModApi;
    }
}