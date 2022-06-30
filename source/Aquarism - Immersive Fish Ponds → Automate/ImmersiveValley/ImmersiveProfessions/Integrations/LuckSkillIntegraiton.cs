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
using StardewModdingAPI;

#endregion using directives

internal class LuckSkillIntegration : BaseIntegration<ILuckSkillAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public LuckSkillIntegration(IModRegistry modRegistry)
        : base("LuckSkill", "spacechase0.LuckSkill", "1.2.3", modRegistry)
    {
    }

    /// <summary>Cache the Luck Skill API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.LuckSkillApi = ModApi;
        ExtendedLuckSkillAPI.Init();
    }
}