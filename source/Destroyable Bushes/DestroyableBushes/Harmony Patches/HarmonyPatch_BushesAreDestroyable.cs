/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/DestroyableBushes
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using System;

namespace DestroyableBushes
{
    /// <summary>A Harmony patch that makes bushes destroyable based on config.json file settings.</summary>
    public static class HarmonyPatch_BushesAreDestroyable
    {
        /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
        /// <param name="harmony">This mod's Harmony instance.</param>
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BushesAreDestroyable)}\": postfixing SDV method \"Bush.isDestroyable()\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), nameof(Bush.isDestroyable)),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_BushesAreDestroyable), nameof(isDestroyable_Postfix))
            );
        }

        /// <summary>Makes all bushes destroyable by appropriate tools.</summary>
        /// <remarks>
        /// This causes <see cref="Bush.isDestroyable()"/> to return true, allowing axes with at least 1 upgrade to destroy bushes.
        /// </remarks>
        /// <param name="__instance">The <see cref="Bush"/> being checked.</param>
        /// <param name="__result">True if this bush is destroyable.</param>
        public static void isDestroyable_Postfix(Bush __instance, ref bool __result)
        {
            try
            {
                if (ModEntry.Config.DestroyableBushLocations?.Count > 0) //if the location list has any entries
                {
                    foreach (string locationName in ModEntry.Config.DestroyableBushLocations) //for each name in the list
                    {
                        if (locationName.Equals(__instance.Location?.Name ?? "", StringComparison.OrdinalIgnoreCase)) //if the listed name matches the bush's location name
                        {
                            __result = true; //return true
                            return;
                        }
                    }
                }
                else //if the location list has no entries
                {
                    switch (__instance.size.Value)
                    {
                        case Bush.smallBush:
                            if (ModEntry.Config.DestroyableBushTypes.SmallBushes) //if allow to destroy this bush size
                                __result = true; //return true
                            return;
                        case Bush.mediumBush:
                            if (ModEntry.Config.DestroyableBushTypes.MediumBushes) //if allow to destroy this bush size
                                __result = true; //return true
                            return;
                        case Bush.largeBush:
                            if (ModEntry.Config.DestroyableBushTypes.LargeBushes) //if allow to destroy this bush size
                                __result = true; //return true
                            return;
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
