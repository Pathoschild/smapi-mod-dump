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

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterParseMonsterInfoPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct and instance.</summary>
    internal MonsterParseMonsterInfoPatch()
    {
        Target = RequireMethod<Monster>("parseMonsterInfo");
    }

    #region harmony patches

    /// <summary>Patch to modify combat difficulty.</summary>
    [HarmonyPostfix]
    private static void MonsterParseMonsterInfoPostfix(Monster __instance)
    {
        __instance.Health = (int)Math.Round(__instance.Health * ModEntry.Config.MonsterHealthMultiplier);
        __instance.DamageToFarmer = (int)Math.Round(__instance.DamageToFarmer * ModEntry.Config.MonsterDamageMultiplier);
        __instance.resilience.Value = (int)Math.Round(__instance.resilience.Value * ModEntry.Config.MonsterDefenseMultiplier);
    }

    #endregion harmony patches
}