/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

// ReSharper disable InconsistentNaming

namespace CropGrowthAdjustments.Patching
{
    internal class HarmonyPatchExecutors
    {
        /// <summary> Patch for the HoeDirt.dayUpdate method </summary>
        public static bool HoeDirtDayUpdate(HoeDirt __instance, GameLocation environment, Vector2 tileLocation)
        {
            // Avoid running if no crop is planted.
            if (__instance.crop == null) return true;
            
            // change sprites to special if needed
            Utility.ChangeSpritesToSpecial(__instance, environment, tileLocation);

            foreach (var contentPack in ModEntry.ContentPackManager.ContentPacks)
            {
                foreach (var adjustment in contentPack.CropAdjustments)
                {
                    // skip if this crop is not the desired one.
                    if (__instance.crop.indexOfHarvest.Value != adjustment.CropProduceItemId) continue;

                    // run the original method if this crop is supposed to die in winter.
                    if (adjustment.GetSeasonsToGrowIn().All(season => season != "winter")) return true;
                    
                    // do not run the original method if the current season is winter and the crop is not supposed to die.
                    if (Game1.GetSeasonForLocation(environment) == "winter")
                    {
                        __instance.crop.newDay((int) __instance.state.Value, (int) __instance.fertilizer.Value, (int) tileLocation.X, (int) tileLocation.Y, environment);
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary> Patch for the IndoorPot.DayUpdate method </summary>
        public static bool IndoorPotDayUpdate(IndoorPot __instance, GameLocation location)
        {
            var hoeDirt = __instance.hoeDirt?.Value;

            // Avoid running if no crop is planted.
            if (hoeDirt?.crop == null) return true;

            return HoeDirtDayUpdate(hoeDirt, location, __instance.TileLocation);
        }
        
        /// <summary> Patch for the Crop.newDay method. </summary>
        public static void CropNewDay(Crop __instance, int state, int fertilizer, int xTile, int yTile,
            GameLocation environment)
        {
            // return if this crop is still growing or its hoe dirt is not watered.
            if ((!__instance.fullyGrown.Value && (__instance.dayOfCurrentPhase.Value != 0 && __instance.currentPhase.Value != 5)) ||
                state != 1) return;

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
                    if (Utility.IsCropInAnyOfSpecifiedLocations(adjustment.GetLocationsWithDefaultSeasonBehavior(), 
                            environment)) return;
                    
                    // return if the crop is already in its produce season.
                    if (adjustment.GetSeasonsToProduceIn().Any(
                            season => Utility.CompareTwoStringsCaseAndSpaceIndependently(Game1.currentSeason, season))) return;

                    // kill the crop if it's out of its growth seasons.
                    if (adjustment.GetSeasonsToGrowIn().All(
                            season => !Utility.CompareTwoStringsCaseAndSpaceIndependently(Game1.currentSeason, season)))
                    {
                        __instance.Kill();
                        return;
                    }

                    // if the crop is about to finish its regrowth period in any season other than the produce seasons,
                    // prevent it from producing.
                    if (__instance.currentPhase.Value == 5 && __instance.dayOfCurrentPhase.Value == 0)
                    {
                        __instance.dayOfCurrentPhase.Value = 1;
                        __instance.fullyGrown.Value = true;
                    }
                }
            }
        }
    }
}
