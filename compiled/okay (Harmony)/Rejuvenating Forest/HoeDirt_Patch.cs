using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;
using StardewValley;
using HarmonyLib;
using StardewValley.TerrainFeatures;

namespace RejuvenatingForest
{
    class HoeDirt_Patch : HarmonyPatches
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Utilized by Harmony.")]
        private static void HoeDirt_plant_Patch()
        {
            HarmonyPatcher.ApplyPatch(
                caller: typeof(HoeDirt_Patch).GetMethod("plant_Postfix"),
                original: typeof(HoeDirt).GetMethod("plant"),
                prefix: new HarmonyMethod(typeof(HoeDirt_Patch).GetMethod("plant_Prefix")),
                postfix: new HarmonyMethod(typeof(HoeDirt_Patch).GetMethod("plant_Postfix"))
            );
        }

        /// <summary>
        /// Before the plant() method is called, check to see if Magic Fertilizer (ID: 3001) is being planted,
        /// and there is also no crop already on the tile. If so, skip the plant() method to prevent a plant sound effect from playing.
        /// </summary>
        /// <param name="__instance">Tile of hoed ground that is being planted on</param>
        /// <param name="index">Object ID of the crop/fertilizer (applies logic if this is 3001)</param>
        /// <param name="tileX">X coord of the hoed dirt on the map</param>
        /// <param name="tileY">Y coord of the hoed dirt on the map</param>
        /// <param name="who">Farmer that's planting the crop/fertilizer</param>
        /// <param name="isFertilizer">true if the object is a fertilizer, false if it's a crop (or other)</param>
        /// <param name="location">GameLocation for the map that the hoed dirt is on</param>
        /// <returns>True to continue execution of the original method, or false to skip it. Skipping the method will still execute the postfix regardless.</returns>
        public static bool plant_Prefix(HoeDirt __instance, int index, int tileX, int tileY, Farmer who, bool isFertilizer, GameLocation location)
        {
            Globals.Monitor.Log("Applying plant prefix", LogLevel.Debug);

            // Object ID of Magic Fertilizer is 3001
            if (index == 3001 && __instance.crop == null)
                return false; // Skip running plant(...) since it would play a sound effect

            // Continue onwards with HoeDirt.plant(...)
            return true;
        }

        /// <summary>
        /// Apply fertilizer logic for Magic Fertilizer (ID: 3001), which is a one-time use that instantly grows the crop
        /// </summary>
        /// <param name="__instance">Tile of hoed ground that is being planted on</param>
        /// <param name="index">Object ID of the crop/fertilizer (applies logic if this is 3001)</param>
        /// <param name="tileX">X coord of the hoed dirt on the map</param>
        /// <param name="tileY">Y coord of the hoed dirt on the map</param>
        /// <param name="who">Farmer that's planting the crop/fertilizer</param>
        /// <param name="isFertilizer">true if the object is a fertilizer, false if it's a crop (or other)</param>
        /// <param name="location">GameLocation for the map that the hoed dirt is on</param>
        /// <param name="__result">Reference to the return value of the HoeDirt.plant(...)</param>
        public static void plant_Postfix(HoeDirt __instance, int index, int tileX, int tileY, Farmer who, bool isFertilizer, GameLocation location, ref bool __result)
        {
            Globals.Monitor.Log("Applying plant postfix", LogLevel.Debug);

            // Object ID of Magic Fertilizer is 3001
            if (index == 3001)
            {
                // Clear the fertilizer from the ground
                __instance.fertilizer.Value = 0;
                
                // If there's no crop, refund the item
                if (__instance.crop == null)
                {
                    __result = false;
                    return;
                }
                
                // Magic fertilizer is being used and the crop is being planted
                __instance.crop.growCompletely();
            }
        }
    }
}
