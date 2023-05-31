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

using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnEquipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnEquipPatcher"/> class.</summary>
    internal RingOnEquipPatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onEquip));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnEquipPrefix(Ring __instance, Farmer who)
    {
        if (RingsModule.Config.TheOneInfinityBand &&
            __instance.indexInTileSheet.Value == ItemIDs.IridiumBand)
        {
            return false; // don't run original logic
        }

        if (!RingsModule.Config.RebalancedRings)
        {
            return true; // run original logic
        }

        switch (__instance.indexInTileSheet.Value)
        {
            case ItemIDs.TopazRing: // topaz to give defense
                who.resilience += 3;
                return false; // don't run original logic
            case ItemIDs.JadeRing: // jade ring to give +50% crit. power
                who.critPowerModifier += 0.5f;
                return false; // don't run original logic
            case ItemIDs.WarriorRing: // reset warrior kill count
                RingsModule.State.WarriorKillCount = 0;
                return true;
            case ItemIDs.ImmunityRing:
                who.immunity += 10;
                return false;
            default:
                if (!Globals.GarnetRingIndex.HasValue || __instance.ParentSheetIndex != Globals.GarnetRingIndex)
                {
                    return true; // run original logic
                }

                // garnet ring to give +10% cdr
                who.IncrementCooldownReduction();
                return false; // don't run original logic
        }
    }

    #endregion harmony patches
}
