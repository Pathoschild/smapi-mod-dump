/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishingRodCalculateTimeUntilFishingBitePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodCalculateTimeUntilFishingBitePatch()
    {
        Original = RequireMethod<FishingRod>("calculateTimeUntilFishingBite");
    }

    #region harmony patches

    /// <summary>Patch to reduce prestiged Fisher nibble delay.</summary>
    [HarmonyPrefix]
    private static bool FishingRodCalculateTimeUntilFishingBitePrefix(FishingRod __instance, ref float __result)
    {
        var who = __instance.getLastFarmerToUse();
        if (!who.HasProfession(Profession.Fisher, true)) return true; // run original logic

        __result = 50;
        return false; // don't run original logic
    }

    /// <summary>Patch to reduce Fisher nibble delay.</summary>
    [HarmonyPostfix]
    private static void FishingRodCalculateTimeUntilFishingBitePostfix(FishingRod __instance, ref float __result)
    {
        var who = __instance.getLastFarmerToUse();
        if (who.HasProfession(Profession.Fisher)) __result *= 0.5f;
    }

    #endregion harmony patches
}