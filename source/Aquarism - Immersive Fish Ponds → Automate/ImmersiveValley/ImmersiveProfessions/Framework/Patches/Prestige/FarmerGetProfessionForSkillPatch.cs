/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using Extensions;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetProfessionForSkillPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerGetProfessionForSkillPatch()
    {
        Target = RequireMethod<Farmer>(nameof(Farmer.getProfessionForSkill));
    }

    #region harmony patches

    /// <summary>Patch to force select most recent profession for skill.</summary>
    [HarmonyPrefix]
    private static bool FarmerGetProfessionForSkillPrefix(Farmer __instance, ref int __result, int skillType,
        int skillLevel)
    {
        if (!ModEntry.Config.EnablePrestige || skillType == Farmer.luckSkill ||
            !Skill.TryFromValue(skillType, out var skill)) return true; // run original logic

        var branch = __instance.GetCurrentBranchForSkill(skill);
        if (branch < 0)
        {
            __result = -1;
            return false; // don't run original logic
        }

        __result = skillLevel switch
        {
            5 => branch,
            10 => __instance.GetCurrentProfessionForBranch(Profession.FromValue(branch)),
            _ => -1
        };

        return false; // don't run original logic
    }

    #endregion harmony patches
}