/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnUnequipPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal RingOnUnequipPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.onUnequip));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnUnequipPrefix(Ring __instance, Farmer who)
    {
        if (ModEntry.Config.TheOneIridiumBand &&
            __instance.indexInTileSheet.Value == Constants.IRIDIUM_BAND_INDEX_I) return false; // don't run original logic

        if (!ModEntry.Config.RebalancedRings) return true; // run original logic

        switch (__instance.indexInTileSheet.Value)
        {
            case Constants.TOPAZ_RING_INDEX_I: // topaz to give +3 defense
                who.resilience -= 3;
                return false; // don't run original logic
            case Constants.JADE_RING_INDEX_I: // jade ring to give +30% crit. power
                who.critPowerModifier -= 0.3f;
                return false; // don't run original logic
            case Constants.CRAB_RING_INDEX_I: // crab ring to give +10 defense
                who.resilience -= 10;
                return false; // don't run original logic
            default:
                return true; // run original logic
        }
    }

    #endregion harmony patches
}