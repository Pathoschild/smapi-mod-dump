/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using StardewValley;
using System;

namespace BNWCore.Patches
{
    internal class SkillPatches
    {
        public static bool GiveWateringExp(HoeDirt __instance, ref Tool t)
        {
            try
            {
                double chance = InternalConfig.WateringChanceToGetXP / 100.0;

                if (t != null && t is WateringCan && __instance != null && __instance.state.Value == HoeDirt.dry && __instance.needsWatering() && __instance.crop != null && !__instance.crop.dead.Value)
                {
                    if (Game1.random.NextDouble() < chance)
                    {
                        if (__instance.crop.isWildSeedCrop() && InternalConfig.ForageSeedWateringGrantsForagingXP)
                        {
                            if (t.getLastFarmerToUse() != null)
                            {
                                t.getLastFarmerToUse().gainExperience(2, InternalConfig.WateringExperienceAmount);
                            }
                        }
                        else
                        {
                            if (t.getLastFarmerToUse() != null)
                            {
                                t.getLastFarmerToUse().gainExperience(0, InternalConfig.WateringExperienceAmount);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return true;
            }
        }
    }
}
