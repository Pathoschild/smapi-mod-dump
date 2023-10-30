/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/stardew-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

// ReSharper disable InconsistentNaming

namespace GrapesAllYearRound
{
    public class HarmonyPatches
    {
        private const string GreenhouseMapName = "Greenhouse";
        private const string IslandMapName = "IslandWest";

        /// <summary> Patch for the HoeDirt.dayUpdate </summary>
        public static bool HoeDirtDayUpdate(HoeDirt __instance, GameLocation environment)
        {
            // Avoid running if no crop is planted.
            if (__instance.crop == null) return true;
            
            // Avoid running if this crop is in the greenhouse or on the Ginger Island.
            if (environment.Name == GreenhouseMapName || environment.Name == IslandMapName) return true;
            
            // Avoid running if this is not grape.
            if (__instance.crop.indexOfHarvest.Value != 398) return true;
            
            // Skip the original method if the current season is winter to prevent the grape from dying.
            if (Game1.currentSeason == "winter") return false;

            return true;
        }

        /// <summary> 
        /// Patch for the HoeDirt.draw method.
        /// 
        /// The performance hit doesn't seem to be too large, so I went with this for updating grape textures.
        /// </summary>
        public static void HoeDirtDraw(HoeDirt __instance, Vector2 tileLocation)
        {
            // Avoid running if no crop is planted.
            if (__instance.crop == null) return;
            
            // Avoid running if this is not grape.
            if (__instance.crop.indexOfHarvest.Value != 398) return;
            
            // Avoid running if this crop is in the greenhouse or on the Ginger Island.
            if (__instance.currentLocation.Name == GreenhouseMapName || __instance.currentLocation.Name == IslandMapName) return;

            var previousRow = __instance.crop.rowInSpriteSheet.Value;
            
            // If it's winter, set the grape sprite to the winter variant.
            __instance.crop.rowInSpriteSheet.Value = Game1.currentSeason == "winter" ? 48 : 38;
            
            // Update crop draw math if the texture (row) was changed.
            if(__instance.crop.rowInSpriteSheet.Value != previousRow)
                __instance.crop.updateDrawMath(tileLocation);
        }
        
        /// <summary> Patch for the Crop.newDay method. </summary>
        public static void CropNewDay(Crop __instance, int state, int fertilizer, int xTile, int yTile,
            GameLocation environment)
        {
            if (environment.Name == GreenhouseMapName || environment.Name == IslandMapName) return; // return if the crop is planted in the greenhouse or on the Ginger Island.

            if (__instance.indexOfHarvest.Value != 398) return; // return if this crop is not grape.
            
            // Return if this crop is still growing,
            // its hoe dirt is not watered or this crop is fiber.
            if ((!__instance.fullyGrown.Value && (__instance.dayOfCurrentPhase.Value != 0 && __instance.currentPhase.Value != 5)) ||
                state != 1 || __instance.indexOfHarvest.Value == 771) return;
            
            if (Game1.currentSeason == "fall") return;

            // If the crop is about to finish its growth in any season other than fall,
            // prevent it from producing.
            if (__instance.currentPhase.Value == 5 && __instance.dayOfCurrentPhase.Value == 0)
            {
                __instance.dayOfCurrentPhase.Value = 3;
                __instance.fullyGrown.Value = true;
            }
        }
    }
}
