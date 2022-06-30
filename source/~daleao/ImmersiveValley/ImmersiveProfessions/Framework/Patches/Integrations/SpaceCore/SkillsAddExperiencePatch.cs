/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using DaLion.Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Linq;
using Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsAddExperiencePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillsAddExperiencePatch()
    {
        try
        {
            Target = "SpaceCore.Skills".ToType().RequireMethod("AddExperience");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to apply prestige exp multiplier to custom skills.</summary>
    [HarmonyPrefix]
    private static void SkillsAddExperiencePrefix(Farmer farmer, string skillName, ref int amt)
    {
        if (!ModEntry.Config.EnablePrestige || !ModEntry.CustomSkills.TryGetValue(skillName, out var skill) ||
            amt < 0) return;

        amt = Math.Min(
            (int)(amt * Math.Pow(1f + ModEntry.Config.BonusSkillExpPerReset,
                farmer.GetProfessionsForSkill(skill, true).Count())), Experience.VANILLA_CAP_I - skill.CurrentExp);
    }

    #endregion harmony patches
}