using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using Harmony;

namespace DestroyableBushes
{
    /// <summary>A Harmony patch that makes bushes destroyable based on config.json file settings.</summary>
    public static class HarmonyPatch_BushesAreDestroyable
    {
        /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
        /// <param name="harmony">This mod's Harmony instance.</param>
        public static void ApplyPatch(HarmonyInstance harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BushesAreDestroyable)}\": postfixing SDV method \"Bush.isDestroyable(GameLocation, Vector2)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), nameof(Bush.isDestroyable), new[] { typeof(GameLocation), typeof(Vector2) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_BushesAreDestroyable), nameof(isDestroyable_Postfix))
            );
        }

        /// <summary>Makes all bushes destroyable by appropriate tools.</summary>
        /// <remarks>
        /// This causes <see cref="Bush.isDestroyable(GameLocation, Vector2)"/> to always return true.
        /// Currently, this change allows axes with at least 1 upgrade to chop down any bush.
        /// </remarks>
        /// <param name="__instance">The <see cref="Bush"/> being checked.</param>
        /// <param name="__result">True if this bush is destroyable.</param>
        public static void isDestroyable_Postfix(GameLocation location, Bush __instance, ref bool __result)
        {
            try
            {
                if (ModEntry.Config.AllBushesAreDestroyable) //if all bushes should be destroyable
                {
                    __result = true; //return true
                    return; //end of patch
                }
                else if (location != null && ModEntry.Config.DestroyableBushLocations != null) //if this location and the relevant config data aren't null
                {
                    foreach (string locationName in ModEntry.Config.DestroyableBushLocations) //for each name in the list
                    {
                        if (locationName.Equals(location.Name, StringComparison.OrdinalIgnoreCase)) //if the listed name matches the bush's location name
                        {
                            __result = true; //return true
                            return; //end of patch
                        }
                    }
                }

                //return the original result without modifying it
                return; //end of patch 
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(isDestroyable_Postfix)}\" has encountered an error:\n{ex.ToString()}", LogLevel.Error);
            }
        }
    }
}
