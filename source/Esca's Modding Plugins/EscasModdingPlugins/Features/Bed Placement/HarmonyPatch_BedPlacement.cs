/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;

namespace EscasModdingPlugins
{
    /// <summary>Allows mods to designate areas where furniture beds can be placed by players. Uses map properties.</summary>
    public static class HarmonyPatch_BedPlacement
    {
        /// <summary>The name of the map property used by this patch.</summary>
        public static string MapPropertyName { get; set; } = null;

        /// <summary>True if this patch is currently applied.</summary>
        public static bool Applied { get; private set; } = false;
        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game.</summary>
        /// <param name="harmony">The <see cref="Harmony"/> created with this mod's ID.</param>
        /// <param name="monitor">The <see cref="IMonitor"/> provided to this mod by SMAPI. Used for log messages.</param>
        public static void ApplyPatch(Harmony harmony, IMonitor monitor)
        {
            if (Applied)
                return;

            //store args
            Monitor = monitor;

            //initialize assets/properties
            MapPropertyName = ModEntry.PropertyPrefix + "BedPlacement"; //assign map property name

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_BedPlacement)}\": postfixing SDV method \"GameLocation.CanPlaceThisFurnitureHere(Furniture)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.CanPlaceThisFurnitureHere), new[] { typeof(Furniture) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_BedPlacement), nameof(GameLocation_CanPlaceFurnitureHere))
            );

            Applied = true;
        }

        /// <summary>Determines whether this mod's settings allow furniture beds to be placed at the given location.</summary>
        /// <param name="location">The location of the bed.</param>
        /// <returns>True if beds should be placeable here; false if they should not.</returns>
        public static bool ShouldBedsBePlaceableHere(GameLocation location)
        {
            if (location == null)
                return false;

            if (location.Map.Properties.TryGetValue(MapPropertyName, out var mapPropertyObject)) //if the location has a non-null map property
            {
                string mapProperty = mapPropertyObject?.ToString() ?? ""; //get the map property as a string

                bool result = !mapProperty.Trim().StartsWith("F", StringComparison.OrdinalIgnoreCase); //true if the property's value is NOT "false"

                if (Monitor?.IsVerbose == true)
                {
                    if (result)
                        Monitor.Log($"Allowing bed placement. Location: {location?.Name}. Map property value: \"{mapProperty}\".", LogLevel.Trace);
                    else
                        Monitor.Log($"NOT allowing bed placement. Location: {location?.Name}. Map property value: \"{mapProperty}\".", LogLevel.Trace);
                }

                return result;
            }

            if (Monitor?.IsVerbose == true)
                Monitor.Log($"NOT allowing bed placement; no relevant map or tile property. Location: {location?.Name}.", LogLevel.Trace);

            return false; //default to null
        }

        /// <summary>Allows placement of bed furniture based on custom map properties.</summary>
        /// <param name="__instance">The instance calling the original method.</param>'
        /// <param name="furniture">The furniture being checked.</param>
        /// <param name="__result">The result of the original method. True if the furniture can be placed; false otherwise.</param>
        private static void GameLocation_CanPlaceFurnitureHere(GameLocation __instance, Furniture furniture, ref bool __result)
        {
            try
            {
                if (furniture.furniture_type.Value == 15 || furniture is BedFurniture) //if the furniture is a bed
                {
                    if (ShouldBedsBePlaceableHere(__instance)) //if this mod should allow bed placement at this tile
                        __result = true; //allow it
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_BedPlacement)}\" has encountered an error. Players might be unable to place beds at additional locations. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return;
            }
        }
    }
}
