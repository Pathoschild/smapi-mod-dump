/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/gzhynko/StardewMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using StardewModdingAPI;
// ReSharper disable InconsistentNaming

namespace AnimalsNeedWater.Patching
{
    internal class HarmonyPatches
    {
        /// <summary> Patch for the FarmAnimal.dayUpdate method. </summary>
        [HarmonyPriority(500)]
        public static void AnimalDayUpdate(ref FarmAnimal __instance, ref GameLocation environtment)
        {
            try
            {
                HarmonyPatchExecutors.AnimalDayUpdateExecutor(ref __instance, ref environtment);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(AnimalDayUpdate) }:\n{ e }", LogLevel.Error);
            }
        }

        /// <summary> Patch for the FarmAnimal.behaviors method. </summary>
        [HarmonyPriority(600)]
        public static bool AnimalBehaviors(ref bool __result, ref FarmAnimal __instance, ref GameTime time, ref GameLocation location)
        {
            try
            {
                return HarmonyPatchExecutors.AnimalBehaviorsExecutor(ref __result, ref __instance, ref time, ref location);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(AnimalBehaviors) }:\n{ e }", LogLevel.Error);
                
                return true;
            }
        }

        /// <summary> Patch for the AnimalHouse.performToolAction method. </summary>
        [HarmonyPriority(500)]
        public static bool AnimalHouseToolAction(ref AnimalHouse __instance, ref Tool t, ref int tileX, ref int tileY)
        { 
            try
            {
                return HarmonyPatchExecutors.AnimalHouseToolActionExecutor(ref __instance, ref t, ref tileX, ref tileY);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(AnimalHouseToolAction) }:\n{ e }", LogLevel.Error);
                
                return true;
            }
        }

        /// <summary> Patch for the warpFarmer method. </summary>
        [HarmonyPriority(500)]
        public static void WarpFarmer(ref string locationName, ref int tileX, ref int tileY, ref int facingDirectionAfterWarp, ref bool isStructure)
        {
            try
            {
                HarmonyPatchExecutors.WarpFarmerExecutor(ref locationName, ref tileX, ref tileY,
                    ref facingDirectionAfterWarp, ref isStructure);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(WarpFarmer) }:\n{ e }", LogLevel.Error);
            }
        }
    }
}
