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
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FarmingExpRebalance.Patches
{
    internal class CropHarvestPatcher : GenericPatcher
    {
        private static IMonitor Monitor;
        public override void Patch(Harmony harmony, IMonitor monitor)
        {
            Monitor = monitor;
            harmony.Patch(
                original: this.getOriginalMethod<Crop>(nameof(Crop.harvest)),
                prefix: this.getHarmonyMethod(nameof(Prefix_Harvest))
            );

            harmony.Patch(
                original: this.getOriginalMethod<Crop>(nameof(Crop.harvest)),
                prefix: this.getHarmonyMethod(nameof(Postfix_Harvest))
            );
        }

        /// <summary>
        /// Patch for <see cref="StardewValley.Crop.harvest"/>. Only checks <c> Game1.player.experiencePoints </c> for farming exp.
        /// </summary>
        /// <returns></returns>
        private static bool Prefix_Harvest(int __state)
        {
            try
            {
                __state = Game1.player.experiencePoints[0];
            }
            catch (Exception e)
            {
                Monitor.Log($"Something went wrong when applying patch FarmingExpRebalance.Patches.Prefix_Harvest:\n{e}", LogLevel.Error);
            }
            return true; // Always execute original function
        }

        private static void Postfix_Harvest(int __state)
        {
            try
            {
                int exp_diff = Game1.player.experiencePoints[0] - __state;
                Monitor.Log($"Exp gained from harvesting: {exp_diff}", LogLevel.Debug);
                if (exp_diff > 0)
                {
                    subtractFarmingExp(Game1.player, (int) (exp_diff * (1 - ModConfig.HarvestingExpMultiplier)));
                }
            }
            catch (Exception e)
            {
                Monitor.Log($"Something went wrong when applying patch FarmingExpRebalance.Patches.Prefix_Harvest:\n{e}", LogLevel.Error);
            }
        }

        private static void subtractFarmingExp(Farmer farmer, int amount)
        {
            // 'old' exp is exp after reduction
            int old_exp = farmer.experiencePoints[0] - amount;
            // 'new' exp is current exp
            int new_exp = farmer.experiencePoints[0];

            int level_after_sub = Farmer.checkForLevelGain(old_exp, new_exp);
            // If level is different, dont subtract as that could maybe break levelling
            if (level_after_sub != -1 && level_after_sub == farmer.FarmingLevel)
            {
                farmer.experiencePoints[0] -= amount;
            }

        }
    }
}
