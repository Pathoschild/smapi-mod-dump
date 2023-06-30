/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Patches;
using HarmonyLib;
using StardewValley;
using System;

namespace WateringCanGiveExp.Patches;

internal class CropHarvestPatcher : GenericPatcher
{
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: this.GetOriginalMethod<Crop>(nameof(Crop.harvest)),
            prefix: this.GetHarmonyMethod(nameof(Prefix_Harvest))
        );

        harmony.Patch(
            original: this.GetOriginalMethod<Crop>(nameof(Crop.harvest)),
            postfix: this.GetHarmonyMethod(nameof(Postfix_Harvest))
        );
    }

    /// <summary>
    /// Patch for <see cref="StardewValley.Crop.harvest"/>. Only checks <c> Game1.player.experiencePoints </c> for farming exp.
    /// </summary>
    /// <returns></returns>
    private static void Prefix_Harvest(out int __state)
    {
        __state = Game1.player.experiencePoints[0];
    }

    private static void Postfix_Harvest(int __state)
    {
        try
        {
            int exp_diff = Game1.player.experiencePoints[0] - __state;
            AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"Exp gained from harvesting: {exp_diff} (old={__state})");
            if (exp_diff > 0)
            {
                subtractFarmingExp(Game1.player, (int)(exp_diff * (1 - ModEntry.Instance.Config.HarvestingExpMultiplier)));
            }
        }
        catch (Exception e)
        {

            AchtuurCore.Logger.ErrorLog(ModEntry.Instance.Monitor, $"Something went wrong when applying patch FarmingExpRebalance.Patches.Prefix_Harvest:\n{e}");
        }
    }

    private static void subtractFarmingExp(Farmer farmer, int amount)
    {
        // 'old' exp is exp after reduction
        int old_exp = farmer.experiencePoints[0] - amount;
        // 'new' exp is current exp
        int new_exp = farmer.experiencePoints[0];

        int level_after_sub = Farmer.checkForLevelGain(old_exp, new_exp);

        AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"{new_exp} -> {old_exp} ({level_after_sub})");

        // If level is different, dont subtract as that could maybe break levelling
        if (level_after_sub == -1)
        {
            farmer.experiencePoints[0] -= amount;
            AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"Subtracted {amount} exp");
        }

    }
}
