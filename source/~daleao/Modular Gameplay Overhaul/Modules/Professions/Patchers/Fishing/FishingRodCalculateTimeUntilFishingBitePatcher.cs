/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodCalculateTimeUntilFishingBitePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodCalculateTimeUntilFishingBitePatcher"/> class.</summary>
    internal FishingRodCalculateTimeUntilFishingBitePatcher()
    {
        this.Target = this.RequireMethod<FishingRod>("calculateTimeUntilFishingBite");
    }

    #region harmony patches

    /// <summary>Patch to reduce prestiged Fisher nibble delay.</summary>
    [HarmonyPrefix]
    private static bool FishingRodCalculateTimeUntilFishingBitePrefix(FishingRod __instance, ref float __result)
    {
        var who = __instance.getLastFarmerToUse();
        if (!who.HasProfession(Profession.Fisher, true))
        {
            return true; // run original logic
        }

        __result = 50;
        return false; // don't run original logic
    }

    /// <summary>Patch to reduce Fisher nibble delay.</summary>
    [HarmonyPostfix]
    private static void FishingRodCalculateTimeUntilFishingBitePostfix(FishingRod __instance, ref float __result)
    {
        var who = __instance.getLastFarmerToUse();
        if (who.HasProfession(Profession.Fisher))
        {
            __result *= 0.5f;
        }
    }

    #endregion harmony patches
}
