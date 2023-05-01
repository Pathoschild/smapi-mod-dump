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

using DaLion.Overhaul.Modules.Rings.Events;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnMonsterSlayPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingOnMonsterSlayPatcher"/> class.</summary>
    internal RingOnMonsterSlayPatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.onMonsterSlay));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Jinx up the Savage and Warrior ring.</summary>
    [HarmonyPrefix]
    private static bool RingOnMonsterSlayPrefix(Ring __instance, Farmer who)
    {
        if (!who.IsLocalPlayer || !RingsModule.Config.RebalancedRings)
        {
            return true; // run original logic
        }

        switch (__instance.ParentSheetIndex)
        {
            case ItemIDs.WarriorRing:
                RingsModule.State.WarriorKillCount++;
                EventManager.Enable<WarriorUpdateTickedEvent>();
                break;
            case ItemIDs.SavangeRing:
                RingsModule.State.SavageExcitedness = 9;
                EventManager.Enable<SavageUpdateTickedEvent>();
                break;
            default:
                return true; // run original logic
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
