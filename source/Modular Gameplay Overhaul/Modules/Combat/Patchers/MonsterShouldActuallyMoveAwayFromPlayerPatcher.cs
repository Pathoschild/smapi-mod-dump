/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterShouldActuallyMoveAwayFromPlayerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterShouldActuallyMoveAwayFromPlayerPatcher"/> class.</summary>
    internal MonsterShouldActuallyMoveAwayFromPlayerPatcher()
    {
        this.Target =
            this.RequireMethod<Monster>(nameof(Monster.ShouldActuallyMoveAwayFromPlayer));
    }

    #region harmony patches

    /// <summary>Implement fear status.</summary>
    [HarmonyPrefix]
    private static bool MonsterShouldActuallyMoveAwayFromPlayerPrefix(Monster __instance, ref bool __result)
    {
        if (!__instance.IsFeared())
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
