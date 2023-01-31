/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class SkillsAddExperiencePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsAddExperiencePatcher"/> class.</summary>
    internal SkillsAddExperiencePatcher()
    {
        this.Target = this.RequireMethod<SpaceCore.Skills>(nameof(SpaceCore.Skills.AddExperience));
    }

    #region harmony patches

    /// <summary>Patch to apply prestige exp multiplier to custom skills.</summary>
    [HarmonyPrefix]
    private static void SkillsAddExperiencePrefix(string skillName, ref int amt)
    {
        if (!ProfessionsModule.Config.EnablePrestige || !SCSkill.Loaded.TryGetValue(skillName, out var skill) ||
            amt <= 0)
        {
            return;
        }

        amt = (int)(amt * skill.BaseExperienceMultiplier * skill.PrestigeExperienceMultiplier);
    }

    #endregion harmony patches
}
