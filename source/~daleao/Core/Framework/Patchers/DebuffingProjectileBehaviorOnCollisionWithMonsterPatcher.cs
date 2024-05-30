/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
internal sealed class DebuffingProjectileBehaviorOnCollisionWithMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DebuffingProjectileBehaviorOnCollisionWithMonsterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal DebuffingProjectileBehaviorOnCollisionWithMonsterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target =
            this.RequireMethod<DebuffingProjectile>(nameof(DebuffingProjectile.behaviorOnCollisionWithMonster));
    }

    #region harmony patches

    /// <summary>Replace vanilla freeze with a better one.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool DebuffingProjectileBehaviorOnCollisionWithMonsterPrefix(DebuffingProjectile __instance, NPC n)
    {
        if (!__instance.damagesMonsters.Value || n is not Monster monster || __instance.debuff.Value != "frozen")
        {
            return true; // run original logic
        }

        monster.Freeze(__instance.debuffIntensity.Value);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
