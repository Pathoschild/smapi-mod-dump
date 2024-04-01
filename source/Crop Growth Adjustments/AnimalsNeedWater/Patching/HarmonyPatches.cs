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
using System;
using HarmonyLib;
using StardewModdingAPI;
// ReSharper disable InconsistentNaming

namespace AnimalsNeedWater.Patching
{
    internal class HarmonyPatches
    {
        /// <summary> Patch for the FarmAnimal.dayUpdate method. </summary>
        [HarmonyPriority(500)]
        public static void AnimalDayUpdate(ref FarmAnimal __instance, ref GameLocation environment)
        {
            try
            {
                HarmonyPatchExecutors.AnimalDayUpdateExecutor(ref __instance, ref environment);
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

        /// <summary> Patch for the GameLocation.performToolAction method. </summary>
        [HarmonyPriority(500)]
        public static bool GameLocationToolAction(ref GameLocation __instance, ref Tool t, ref int tileX, ref int tileY)
        { 
            try
            {
                return HarmonyPatchExecutors.GameLocationToolActionExecutor(ref __instance, ref t, ref tileX, ref tileY);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(GameLocationToolAction) }:\n{ e }", LogLevel.Error);
                
                return true;
            }
        }

        /// <summary> Patch for the OnLocationChanged method. </summary>
        [HarmonyPriority(500)]
        public static void OnLocationChanged(GameLocation oldLocation, GameLocation newLocation)
        {
            try
            {
                HarmonyPatchExecutors.OnLocationChangedExecutor(oldLocation, newLocation);
            }
            catch (Exception e)
            {
                ModEntry.ModMonitor.Log($"Failed in { nameof(OnLocationChanged) }:\n{ e }", LogLevel.Error);
            }
        }
    }
}
