/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

// ReSharper disable InconsistentNaming

namespace CropGrowthAdjustments.Patching
{
    internal static class HarmonyPatches
    {
        /// <summary> Patch for the HoeDirt.dayUpdate method </summary>
        public static bool HoeDirtDayUpdate(HoeDirt __instance)
        {
            try
            {
                return HarmonyPatchExecutors.HoeDirtDayUpdate(__instance);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(HoeDirtDayUpdate) }:\n{ e }", LogLevel.Error);
                
                // run the original method if the patch fails
                return true;
            }
        }
        
        public static void HoeDirtPlant(HoeDirt __instance, ref bool __result, string itemId, Farmer who, bool isFertilizer)
        {
            try
            {
                HarmonyPatchExecutors.HoeDirtPlant(__instance, ref __result);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(HoeDirtPlant) }:\n{ e }", LogLevel.Error);
            }
        }

        /// <summary> Patch for the IndoorPot.DayUpdate method </summary>
        public static bool IndoorPotDayUpdate(IndoorPot __instance)
        {
            try
            {
                return HarmonyPatchExecutors.IndoorPotDayUpdate(__instance);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(IndoorPotDayUpdate) }:\n{ e }", LogLevel.Error);
                
                // run the original method if the patch fails
                return true;
            }
        }

        /// <summary> Patch for the Crop.newDay method. </summary>
        public static void CropNewDay(Crop __instance, int state)
        {
            try
            {
                HarmonyPatchExecutors.CropNewDay(__instance, state);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(CropNewDay) }:\n{ e }", LogLevel.Error);
            }
        }

        public static void GameLocationResetLocalState(Game1 __instance)
        {
            try
            {
                HarmonyPatchExecutors.GameLocationResetLocalState(__instance);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(GameLocationResetLocalState) }:\n{ e }", LogLevel.Error);
            }
        }
    }
}