/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using StardewValley;
// ReSharper disable InconsistentNaming

namespace GrapesAllYearRound
{
    public class HarmonyPatches
    {
        /// <summary> Patch for the Crop.newDay method. </summary>
        public static void CropNewDay(Crop __instance, int state, int fertilizer, int xTile, int yTile,
            GameLocation environment)
        {
            if (__instance.indexOfHarvest != 398) return; // return if this crop is not grape.
            
            // Return if this crop is still growing,
            // its hoe dirt is not watered or this crop is fiber.
            if ((!__instance.fullyGrown && (__instance.dayOfCurrentPhase != 0 && __instance.currentPhase != 5)) ||
                state != 1 || __instance.indexOfHarvest == 771) return;
            
            if (Game1.currentSeason == "fall") return;
            
            // If the crop is about to finish its growth in any season other than fall,
            // prevent it from producing.
            if (__instance.currentPhase == 5 && __instance.dayOfCurrentPhase == 0)
            {
                __instance.dayOfCurrentPhase.Value = 3;
                __instance.fullyGrown.Value = true;
            }
                
            // Keep the day of the current phase the same throughout the year.
            __instance.dayOfCurrentPhase.Value++;
        }
    }
}
