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

using Common;
using HarmonyLib;
using Netcode;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class RingCombinePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal RingCombinePatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.Combine));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Changes combined ring to iridium band when combining.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingCombinePrefix(Ring __instance, ref Ring __result, Ring ring)
    {
        if (!ModEntry.Config.TheOneIridiumBand || __instance.ParentSheetIndex != Constants.IRIDIUM_BAND_INDEX_I)
            return true; // run original logic

        try
        {
            var toCombine = new List<Ring>();
            if (__instance is CombinedRing combined)
            {
                if (combined.combinedRings.Count >= 4)
                    ThrowHelper.ThrowInvalidOperationException("Unexpected number of combined rings.");

                toCombine.AddRange(combined.combinedRings);
            }

            toCombine.Add(ring);
            var combinedRing = new CombinedRing(880);
            combinedRing.combinedRings.AddRange(toCombine);
            combinedRing.ParentSheetIndex = Constants.IRIDIUM_BAND_INDEX_I;
            ModEntry.ModHelper.Reflection.GetField<NetInt>(combinedRing, nameof(Ring.indexInTileSheet)).GetValue()
                .Set(Constants.IRIDIUM_BAND_INDEX_I);
            combinedRing.UpdateDescription();
            __result = combinedRing;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}