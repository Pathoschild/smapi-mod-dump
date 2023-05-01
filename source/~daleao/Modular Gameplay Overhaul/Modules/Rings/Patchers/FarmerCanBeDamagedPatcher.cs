/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanBeDamagedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanBeDamagedPatcher"/> class.</summary>
    internal FarmerCanBeDamagedPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.CanBeDamaged));
    }

    #region harmony patches

    /// <summary>Ring of Yoba rebalance.</summary>
    [HarmonyPrefix]
    private static bool FarmerCanBeDamagedPostfix(Farmer __instance, ref bool __result)
    {
        __result = !__instance.temporarilyInvincible && !__instance.isEating && !Game1.fadeToBlack &&
               (!Game1.buffsDisplay.hasBuff(21) || RingsModule.Config.RebalancedRings);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
