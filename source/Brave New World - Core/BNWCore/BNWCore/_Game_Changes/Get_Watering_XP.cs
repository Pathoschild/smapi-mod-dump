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

namespace BNWCore
{
    public class Get_Watering_XP_Patches
    {
        public static bool GiveWateringExp(HoeDirt __instance, ref Tool t)
        {
            if (!ModEntry.Config.BNWCoreEnableWateringXP)
                return true;
            else
            {
                try
                {
                    double chance = Watering_Config.WateringChanceToGetXP / 100.0;

                    if (t != null && t is WateringCan && __instance != null && __instance.state.Value == HoeDirt.dry && __instance.needsWatering() && __instance.crop != null && !__instance.crop.dead.Value)
                    {
                        if (Game1.random.NextDouble() < chance)
                        {
                            if (__instance.crop.isWildSeedCrop() && Watering_Config.ForageSeedWateringGrantsForagingXP)
                            {
                                if (t.getLastFarmerToUse() != null)
                                {
                                    t.getLastFarmerToUse().gainExperience(2, Watering_Config.WateringExperienceAmount);
                                }
                            }
                            else
                            {
                                if (t.getLastFarmerToUse() != null)
                                {
                                    t.getLastFarmerToUse().gainExperience(0, Watering_Config.WateringExperienceAmount);
                                }
                            }
                        }
                    }
                }
                catch (Exception){}
                return true;
            }      
        }
    }
}