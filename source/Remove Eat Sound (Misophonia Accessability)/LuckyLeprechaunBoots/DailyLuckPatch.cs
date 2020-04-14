using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyLeprechaunBoots
{
    [HarmonyPatch(typeof(Farmer))]
    [HarmonyPatch("DailyLuck", MethodType.Getter)]
    public class DailyLuckPatch
    {
        /// <summary>
        /// Modifies the Farmer.DailyLuck value based of the configuration
        /// </summary>
        /// <param name="__result"></param>
        public static void PatchDailyLuckPostfix(ref double __result)
        {
            if (BootsUtil.IsWearingLeprechaunBoots())
            {
                double multiplier = LuckyLeprechaunBootsMod.Config.DailyLuckMultiplier;

                // If the daily luck is negative we have to adjust so the value still increases
                if (__result >= 0)
                {
                    __result = __result * multiplier;
                }
                else
                {
                    double absResult = Math.Abs(__result);
                    __result -= (absResult - (absResult * multiplier));
                }

                // Add to the daily luck after doing the multiplier (we're using PEMDAS)
                __result = __result + LuckyLeprechaunBootsMod.Config.DailyLuckToAdd;
            }
        }
    }
}
