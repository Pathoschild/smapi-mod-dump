/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class RingOnNewLocationPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal RingOnNewLocationPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.onNewLocation));
        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Rebalances Jade and Topaz rings + Crab.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static bool RingOnNewLocationPrefix(Ring __instance) =>
        !ModEntry.Config.TheOneIridiumBand || __instance.indexInTileSheet.Value != Constants.IRIDIUM_BAND_INDEX_I;

    #endregion harmony patches
}