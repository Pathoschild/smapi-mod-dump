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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace EscasModdingPlugins
{
    /// <summary>Allows mods to designate areas where bushes are destroyable. Uses map and/or tile properties.</summary>
    public static class HarmonyPatch_DestroyableBushes
    {
        /// <summary>The name of the map property used by this patch.</summary>
        public static string MapPropertyName { get; set; } = null;
        /// <summary>The name of the tile property used by this patch.</summary>
        public static string TilePropertyName { get; set; } = null;

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
            MapPropertyName = ModEntry.PropertyPrefix + "DestroyableBushes"; //assign map property name
            TilePropertyName = ModEntry.PropertyPrefix + "DestroyableBushes"; //assign tile property name

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_DestroyableBushes)}\": postfixing SDV method \"Bush.isDestroyable(GameLocation, Vector2)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Bush), nameof(Bush.isDestroyable), new[] { typeof(GameLocation), typeof(Vector2) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_DestroyableBushes), nameof(Bush_isDestroyable))
            );

            Applied = true;
        }

        /// <summary>Determines whether this mod's settings allow bush destruction at the given location and/or tile.</summary>
        /// <remarks>
        /// If both a map property and tile property exist for this tile, the tile property has higher priority.
        /// The tile property may exist on the Paths or Back layers. If both exist, the Paths layer has higher priority.
        /// </remarks>
        /// <param name="location">The location of the bush.</param>
        /// <param name="tile">The tile position of the bush. Typically its left-most "collision" tile.</param>
        /// <returns>True if bushes should be destroyable here; false otherwise.</returns>
        public static bool ShouldBushesBeDestroyableHere(GameLocation location, Vector2 tile)
        {
            if (location == null)
                return false;

            string tileProperty = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, TilePropertyName, "Paths") //try to get the tile property from the Paths layer
                ?? location.doesTileHaveProperty((int)tile.X, (int)tile.Y, TilePropertyName, "Back"); //if Paths did NOT have this tile property, try to get it from the Back layer

            if (tileProperty != null) //if a non-null tile property exists for this tile
            {
                bool result = !tileProperty.Trim().StartsWith("F", StringComparison.OrdinalIgnoreCase); //true if the property's value is NOT "false"

                if (Monitor?.IsVerbose == true)
                {
                    if (result)
                        Monitor.Log($"Allowing bush destruction. Location: {location?.Name}. Tile: {tile.X},{tile.Y}. Tile property value: \"{tileProperty}\".", LogLevel.Trace);
                    else
                        Monitor.Log($"NOT allowing bush destruction. Location: {location?.Name}. Tile: {tile.X},{tile.Y}. Tile property value: \"{tileProperty}\".", LogLevel.Trace);
                }

                return result;
            }
            else if (location.Map.Properties.TryGetValue(MapPropertyName, out var mapPropertyObject)) //if no tile property exists for this tile, but the location has a non-null map property
            {
                string mapProperty = mapPropertyObject?.ToString() ?? ""; //get the map property as a string

                bool result = !mapProperty.Trim().StartsWith("F", StringComparison.OrdinalIgnoreCase); //true if the property's value is NOT "false"

                if (Monitor?.IsVerbose == true)
                {
                    if (result)
                        Monitor.Log($"Allowing bush destruction. Location: {location?.Name}. Tile: {tile.X},{tile.Y}. Map property value: \"{mapProperty}\".", LogLevel.Trace);
                    else
                        Monitor.Log($"NOT allowing bush destruction. Location: {location?.Name}. Tile: {tile.X},{tile.Y}. Map property value: \"{mapProperty}\".", LogLevel.Trace);
                }

                return result;
            }

            if (Monitor?.IsVerbose == true)
                Monitor.Log($"NOT allowing bush destruction; no relevant map or tile property. Location: {location?.Name}. Tile: {tile.X},{tile.Y}.", LogLevel.Trace);

            return false; //default to false
        }

        /// <summary>Allows destruction of normally indestructible bushes based on custom map and/or tile properties.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        /// <param name="location">The location of the bush.</param>
        /// <param name="tile">The tile position of the bush. Typically its left-most "collision" tile.</param>
        /// <param name="__result">The result of the original method. True if the bush can be destroyed; false otherwise.</param>
        [HarmonyPriority(Priority.Low)] //execute this AFTER most other postfixes (to reduce interference with similar patches, e.g. my Destroyable Bushes mod)
        private static void Bush_isDestroyable(Bush __instance, GameLocation location, Vector2 tile, ref bool __result)
        {
            try
            {
                if (__result) //if this bush is already destroyable
                    return; //do nothing

                if (__instance.size.Value == Bush.smallBush || __instance.size.Value == Bush.mediumBush || __instance.size.Value == Bush.largeBush) //if this is a "normal" type of bush (ignore tea/walnut bushes and unknown types)
                {
                    if (ShouldBushesBeDestroyableHere(location, tile)) //if this mod should allow bush destruction at this location/tile
                        __result = true; //treat this bush as destroyable
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_DestroyableBushes)}\" has encountered an error. Bushes might be indestructible at some locations. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return;
            }
        }
    }
}
