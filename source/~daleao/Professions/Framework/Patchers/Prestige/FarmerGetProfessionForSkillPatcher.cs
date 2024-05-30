/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetProfessionForSkillPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetProfessionForSkillPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerGetProfessionForSkillPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.getProfessionForSkill));
    }

    #region harmony patches

    /// <summary>Patch to force select most recent profession for skill.</summary>
    [HarmonyPrefix]
    private static bool FarmerGetProfessionForSkillPrefix(
        Farmer __instance, ref int __result, int skillType, int skillLevel)
    {
        if (!Config.Skills.EnableSkillReset || skillType == Farmer.luckSkill)
        {
            return true; // run original logic
        }

        var skill = Skill.FromValue(skillType);
        var rootIndex = __instance.GetCurrentRootProfessionForSkill(skill);
        switch (rootIndex)
        {
            case < 0:
                __result = -1;
                return false; // don't run original logic
            case >= 100:
                rootIndex -= 100;
                break;
        }

        var root = Profession.FromValue(rootIndex);
        __result = skillLevel switch
        {
            5 => rootIndex,
            10 => __instance.GetCurrentBranchingProfessionForRoot(root),
            _ => -1,
        };

        return false; // don't run original logic
    }

    #endregion harmony patches
}
