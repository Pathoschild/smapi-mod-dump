/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Integration.UiInfoSuite;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Annosz.UiInfoSuite2", version: "2.3.3")]
internal sealed class ExperienceBarGetExperienceRequiredToLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ExperienceBarGetExperienceRequiredToLevelPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ExperienceBarGetExperienceRequiredToLevelPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = "UIInfoSuite2.UIElements.ExperienceBar"
            .ToType()
            .RequireMethod("GetExperienceRequiredToLevel");
    }

    #region harmony patches

    /// <summary>Patch to reflect adjusted base experience + extended progression experience.</summary>
    [HarmonyPrefix]
    private static bool ExperienceBarGetExperienceRequiredToLevelPrefix(ref int __result, int currentLevel)
    {
        if (currentLevel < 10)
        {
            return true; // run original logic
        }

        __result = currentLevel >= 20
            ? 0
            : ISkill.LEVEL_10_EXP + ((currentLevel - 10 + 1) * (int)Config.Masteries.ExpPerPrestigeLevel);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
