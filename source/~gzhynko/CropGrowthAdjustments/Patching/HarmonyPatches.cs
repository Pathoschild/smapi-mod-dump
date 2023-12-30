/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
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
        public static bool HoeDirtDayUpdate(HoeDirt __instance, GameLocation environment, Vector2 tileLocation)
        {
            try
            {
                return HarmonyPatchExecutors.HoeDirtDayUpdate(__instance, environment, tileLocation);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(HoeDirtDayUpdate) }:\n{ e }", LogLevel.Error);
                
                // run the original method if the patch fails
                return true;
            }
        }

        /// <summary> Patch for the IndoorPot.DayUpdate method </summary>
        public static bool IndoorPotDayUpdate(IndoorPot __instance, GameLocation location)
        {
            try
            {
                return HarmonyPatchExecutors.IndoorPotDayUpdate(__instance, location);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(IndoorPotDayUpdate) }:\n{ e }", LogLevel.Error);
                
                // run the original method if the patch fails
                return true;
            }
        }

        /// <summary> Patch for the Crop.newDay method. </summary>
        public static void CropNewDay(Crop __instance, int state, int fertilizer, int xTile, int yTile,
            GameLocation environment)
        {
            try
            {
                HarmonyPatchExecutors.CropNewDay(__instance, state, fertilizer, xTile, yTile, environment);
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