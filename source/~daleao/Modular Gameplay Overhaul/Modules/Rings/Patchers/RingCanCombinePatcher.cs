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

using DaLion.Overhaul.Modules.Rings.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCanCombinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RingCanCombinePatcher"/> class.</summary>
    internal RingCanCombinePatcher()
    {
        this.Target = this.RequireMethod<Ring>(nameof(Ring.CanCombine));
        this.Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Allows feeding up to four gemstone rings into an Infinity Band.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingCanCombinePrefix(Ring __instance, ref bool __result, Ring ring)
    {
        if (!RingsModule.Config.TheOneInfinityBand || !Globals.InfinityBandIndex.HasValue)
        {
            return true; // run original logic
        }

        if (__instance.ParentSheetIndex == ItemIDs.IridiumBand ||
            ring.ParentSheetIndex == ItemIDs.IridiumBand ||
            ring.ParentSheetIndex == Globals.InfinityBandIndex.Value)
        {
            return false; // don't run original logic
        }

        if (__instance.ParentSheetIndex != Globals.InfinityBandIndex.Value)
        {
            return true; // run original logic
        }

        __result = ring.IsGemRing() &&
                   (__instance is not CombinedRing combined || combined.combinedRings.Count < 4);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
