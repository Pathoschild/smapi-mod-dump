/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;
using StardewValley;

namespace StardewSurvivalProject.source.harmony_patches
{
    class ObjectPatches
    {
        private static IMonitor Monitor = null;
        // call this method from your Entry class
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        //Directly intercept healthRecoveredOnConsumption method to disable healing via eating, only allow items specified in the whitelist
        public static bool CalculateHPGain_Prefix(StardewValley.Object __instance, ref int __result)
        {
            try
            {
                if (__instance == null)
                    return true;

                int gain_value = data.HealingItemDictionary.getHealingValue(__instance.name);
                __result = gain_value;

                if (ModConfig.GetInstance().DisableHPHealingOnEatingFood)
                {
                    return false;
                }
                else if (gain_value == 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(CalculateHPGain_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        public static void ItemPlace_PostFix(StardewValley.Object __instance, GameLocation location, int x, int y, Farmer who = null)
        {
            try
            {
                if (__instance == null || who == null || !who.IsLocalPlayer)
                    return;

                events.CustomEvents.InvokeOnItemPlaced(__instance);
                return;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(ItemPlace_PostFix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }
    }
}
