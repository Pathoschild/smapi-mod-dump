/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace QualityFishPonds.Patch
{
    [HarmonyPatch(typeof(FishPond))]
    public static class FishPondPatchs
    {    
        private static IMonitor Monitor;
        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }              

        [HarmonyPostfix]
        [HarmonyPatch("addFishToPond")]
        public static void addFishToPond_Postfix(FishPond __instance, StardewValley.Object fish)
        {                 
            if (__instance.modData.ContainsKey(ModEntry.fishPondIdKey))            
                __instance.modData[ModEntry.fishPondIdKey] += fish.Quality;            
            else            
                Monitor.Log("Couldn't find Fish Pond ID", LogLevel.Info);                   
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FishPond.SpawnFish))]
        public static void SpawnFish_Postfix(FishPond __instance)
        {   
            if (__instance.hasSpawnedFish.Value)
            {
                if (__instance.modData.ContainsKey(ModEntry.fishPondIdKey))
                {
                    double playerLuck = Game1.player.DailyLuck;
                    string pondData = __instance.modData[ModEntry.fishPondIdKey];
                    double random = Game1.random.NextDouble();

                    if(ModEntry.Instance.config.EnableGaranteedIridum && playerLuck < -0.02)
                    {
                        pondData += "2";
                        __instance.modData[ModEntry.fishPondIdKey] = pondData;
                        return;
                    }
                    else if (ModEntry.Instance.config.EnableGaranteedIridum && pondData.Count(x => x == '4') == pondData.Count() && playerLuck >= -0.02)
                    {
                        pondData += "4";
                        __instance.modData[ModEntry.fishPondIdKey] = pondData;
                        return;
                    }
                    else
                    {
                        if (Game1.player.professions.Contains(8) && random < (pondData.Count(x => x == '4') * (Game1.player.LuckLevel / 10)) / (2 * pondData.Count(x => int.TryParse(x.ToString(), out int result) == true)))
                            pondData += "4";
                        if (random < 0.33)
                            pondData += "2";
                        else if (random < 0.66)
                            pondData += "1";
                        else
                            pondData += "0";

                        __instance.modData[ModEntry.fishPondIdKey] = pondData;
                        return;
                    }
                                 
                }
                else
                    Monitor.Log("Couldn't find Fish Pond ID", LogLevel.Info);
                
            }          
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FishPond.ClearPond))]
        public static void ClearPond_Postfix(FishPond __instance)
        {
            __instance.modData[ModEntry.fishPondIdKey] = "";
        }


        [HarmonyPostfix]
        [HarmonyPatch(nameof(FishPond.dayUpdate))]
        public static void dayUpdate_Postfix(FishPond __instance)
        {
            if (!__instance.modData.ContainsKey(ModEntry.fishPondIdKey))
            {
                string fishQualities = "";
                if (__instance.FishCount > 0)
                {
                    fishQualities = "0";
                    for (int x = 1; x < __instance.FishCount; x++)                    
                        fishQualities += "0";                    
                }

                __instance.modData.Add(ModEntry.fishPondIdKey, fishQualities);             
                return;
            }                   
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FishPond.performActionOnConstruction))]
        public static void performActionOnConstruction_Postfix(FishPond __instance)
        {
            if (!__instance.modData.ContainsKey(ModEntry.fishPondIdKey))
            {
                __instance.modData.Add(ModEntry.fishPondIdKey, "");               
                return;
            }
        }
    }
}
