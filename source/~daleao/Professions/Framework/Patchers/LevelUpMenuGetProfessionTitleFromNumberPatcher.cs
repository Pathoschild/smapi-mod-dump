/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetProfessionTitleFromNumberPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuGetProfessionTitleFromNumberPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal LevelUpMenuGetProfessionTitleFromNumberPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getProfessionTitleFromNumber));
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession names.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuGetProfessionTitleFromNumberPrefix(ref string __result, int whichProfession)
    {
        if (!Profession.TryFromValue(whichProfession, out var profession) ||
            (Skill)profession.ParentSkill == Farmer.luckSkill)
        {
            return true; // run original logic
        }

        __result = profession.Title;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
