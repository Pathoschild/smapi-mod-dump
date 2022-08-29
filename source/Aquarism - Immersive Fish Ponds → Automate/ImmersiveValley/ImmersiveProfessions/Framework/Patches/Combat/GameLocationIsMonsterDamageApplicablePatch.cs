/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsMonsterDamageApplicablePatch : DaLion.Common.Harmony.HarmonyPatch
{
    private const int SLIME_INDEX_I = 766;

    /// <summary>Construct an instance.</summary>
    internal GameLocationIsMonsterDamageApplicablePatch()
    {
        Target = RequireMethod<GameLocation>("isMonsterDamageApplicable");
    }

    #region harmony patches

    /// <summary>Club smash aoe ignores gliders.</summary>
    [HarmonyPrefix]
    private static bool GameLocationIsMonsterDamageApplicablePrefix(GameLocation __instance, ref bool __result,
        Farmer who, Monster monster)
    {
        if (!monster.IsSlime() || who.CurrentTool is not Slingshot slingshot ||
            slingshot.attachments[0].ParentSheetIndex != SLIME_INDEX_I)
            return true; // run original logic

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}