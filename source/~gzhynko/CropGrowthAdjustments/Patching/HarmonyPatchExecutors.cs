/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System.Linq;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

// ReSharper disable InconsistentNaming

namespace CropGrowthAdjustments.Patching
{
    internal static class HarmonyPatchExecutors
    {
        /// <summary> Patch for the HoeDirt.dayUpdate method </summary>
        public static bool HoeDirtDayUpdate(HoeDirt __instance)
        {
            if (__instance.crop == null) return true;
            
            // change sprites to special if needed
            Utility.ChangeSpritesToSpecial(__instance);

            foreach (var contentPack in ModEntry.ContentPackManager.ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    // skip if this crop is not the desired one.
                    if (__instance.crop.indexOfHarvest.Value != adjustment.CropProduceItemId) continue;

                    // run the original method if this crop is supposed to die in winter.
                    if (adjustment.GetSeasonsToGrowIn().All(season => season != Season.Winter)) return true;
                    
                    // do not run the original method if the current season is winter and the crop is not supposed to die.
                    if (Game1.GetSeasonForLocation(__instance.Location) == Season.Winter)
                    {
                        // run newDay on the planted crop
                        __instance.crop.newDay(__instance.state.Value);
                        // remove water if needed
                        if (!__instance.paddyWaterCheck() || !Game1.random.NextBool(__instance.GetFertilizerWaterRetentionChance()))
                            __instance.state.Value = 0;
                        
                        // skip the original method
                        return false;
                    }
                }
            }

            return true;
        }
        
        /// <summary> Patch for the HoeDirt.plant method </summary>
        public static void HoeDirtPlant(HoeDirt __instance, ref bool __result)
        {
            if (!__result) return;
            
            // change sprites to special if needed
            Utility.ChangeSpritesToSpecial(__instance);
        }
        
        /// <summary> Patch for the IndoorPot.DayUpdate method </summary>
        public static bool IndoorPotDayUpdate(IndoorPot __instance)
        {
            var hoeDirt = __instance.hoeDirt?.Value;
            if (hoeDirt?.crop == null) return true;

            // if this indoor pot does indeed have a crop planted, treat it as a regular HoeDirt
            return HoeDirtDayUpdate(hoeDirt);
        }
        
        /// <summary> Patch for the Crop.newDay method. </summary>
        public static void CropNewDay(Crop __instance, int state)
        {
            // do not run any additional logic if this crop is not watered.
            // this will just run the Crop.newDay method without any modifications
            if (state != 1) return;

            var cropPhasesCount = __instance.phaseDays.Count;
            // the game adds a phase at the end of the phaseDays list in the constructor of Crop,
            // and we don't want to count that state, so decrement the phases count
            if (cropPhasesCount > 0 && __instance.phaseDays.Last() == 99999)
            {
                cropPhasesCount--;
            }
            
            foreach (var contentPack in ModEntry.ContentPackManager.ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    // skip if this crop is not the desired one.
                    if (__instance.indexOfHarvest.Value != adjustment.CropProduceItemId) continue;
                    
                    // (debug info, uncomment to show)
                    /*
                    ModEntry.ModMonitor.Log(
                        $"[cropNewDay] crop: {adjustment.CropProduceName}, regrowAfterHarvest: {__instance.regrowAfterHarvest}, " +
                        $"currentPhase: {__instance.currentPhase}, dayOfCurrentPhase: {__instance.dayOfCurrentPhase}, " +
                        $"fullyGrown: {__instance.fullyGrown}, phaseDays: {JsonConvert.SerializeObject(__instance.phaseDays)}", LogLevel.Info);
                    */

                    // return if the crop is planted in any of the locations where it should maintain default behavior
                    if (Utility.IsInAnyOfSpecifiedLocations(adjustment.GetLocationsWithDefaultSeasonBehavior(), 
                            __instance.currentLocation)) return;
                    
                    // return if the crop is already in its produce season.
                    if (adjustment.GetSeasonsToProduceIn().Any(
                            season => Game1.season == season)) return;

                    // kill the crop if it's out of its growth seasons.
                    if (adjustment.GetSeasonsToGrowIn().All(
                            season => Game1.season != season))
                    {
                        __instance.Kill();
                        return;
                    }

                    // at this point, we are only dealing with crops that are NOT in their produce season
                    
                    // handle regrowing crops
                    if (__instance.RegrowsAfterHarvest())
                    {
                        // if the regrowing crop is about to reach its final (grown) phase, go back to the first day in its pre-final phase
                        if (__instance.currentPhase.Value == cropPhasesCount - 1)
                        {
                            if (__instance.dayOfCurrentPhase.Value == __instance.phaseDays[cropPhasesCount - 2] - 1)
                            {
                                __instance.dayOfCurrentPhase.Value = 0;
                            }
                        }
                        // otherwise, if the crop is already in its final phase and is about to produce again, revert it to the "previous day"
                        else if (__instance.currentPhase.Value == cropPhasesCount)
                        {
                            if (__instance.dayOfCurrentPhase.Value == 1)
                            {
                                __instance.dayOfCurrentPhase.Value = 2;
                            }
                        }
                    }
                    // handle non-regrowing crops
                    else
                    {
                        // if the crop is about to reach the final growth phase, revert it to the previous phase
                        // note that here we do not care about the number of days left in the phase, the game itself should handle resetting it
                        if (__instance.currentPhase.Value == cropPhasesCount - 1)
                        {
                            __instance.currentPhase.Value = cropPhasesCount - 2;
                        }  
                    }
                }
            }
        }

        /// <summary>
        /// Patch for the GameLocation.resetLocalState method.
        /// 
        /// This is responsible for updating the crop sprites when the player goes to a new location.
        /// Note to self: the game appears to do this automatically when changing locations, is this patch really necessary?
        /// </summary>
        public static void GameLocationResetLocalState(Game1 __instance)
        {
            ModEntry.ModHelper.GameContent.InvalidateCache("TileSheets/Crops");
        }
    }
}
