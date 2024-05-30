/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodGetTackleQualifiedItemIDsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodGetTackleQualifiedItemIDsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishingRodGetTackleQualifiedItemIDsPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.GetTackleQualifiedItemIDs));
    }

    #region harmony patches

    /// <summary>Patch to include Angler memorized tackle effects.</summary>
    [HarmonyPostfix]
    private static void PondQueryMenuDrawPostfix(FishingRod __instance, List<string> __result)
    {
        var tackleMemory = Data.Read(__instance, DataKeys.FirstMemorizedTackle);
        if (!string.IsNullOrEmpty(tackleMemory))
        {
            __result.Add(tackleMemory);
        }

        tackleMemory = Data.Read(__instance, DataKeys.SecondMemorizedTackle);
        if (!string.IsNullOrEmpty(tackleMemory))
        {
            __result.Add(tackleMemory);
        }
    }

    #endregion harmony patches
}
