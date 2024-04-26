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
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EscasModdingPlugins
{
    /// <summary>Allows customization of which fishing "zones" and locations are used from the "Data/Locations" asset. Uses a custom data asset and/or tile properties.</summary>
    /// <remarks>
    /// To access information about this feature in other mods, see <see cref="IEmpApi"/>.
    /// </remarks>
    public static class HarmonyPatch_FishLocations
    {
        /// <summary>The name of the data asset used by this patch.</summary>
        public static string AssetName { get; set; } = null;
        /// <summary>The name of the tile property used by this patch.</summary>
        public static string TilePropertyName { get; set; } = null;

        /// <summary>True if this patch is currently applied.</summary>
        public static bool Applied { get; private set; } = false;
        /// <summary>The SMAPI monitor to use for log messages.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game.</summary>
        /// <param name="harmony">The <see cref="Harmony"/> created with this mod's ID.</param>
        /// <param name="monitor">The <see cref="IMonitor"/> provided by SMAPI. Used for log messages.</param>
        public static void ApplyPatch(Harmony harmony, IMonitor monitor)
        {
            if (Applied)
                return;

            //store args
            Monitor = monitor;

            //initialize assets/properties
            AssetName = ModEntry.AssetPrefix + "FishLocations"; //create asset name
            TilePropertyName = ModEntry.PropertyPrefix + "FishLocations"; //create tile property name
            AssetHelper.SetDefault(AssetName, new Dictionary<string, FishLocationsData>()); //create a default instance for the asset

            //get methods to patch dynamically
            HashSet<Type> crabPotMethods = new HashSet<Type>(); //every type with a unique GameLocation.GetCrabPotFishForTile(Vector2)

            foreach (Type type in AccessTools.AllTypes()) //for every type
            {
                if (typeof(GameLocation).IsAssignableFrom(type)) //if this is a type of GameLocation
                {
                    if (AccessTools.Method(type, nameof(GameLocation.GetCrabPotFishForTile), new[] { typeof(Vector2) }) is MethodInfo ocean) //if this type has a crab pot method
                        crabPotMethods.Add(ocean.DeclaringType); //add the method's declaring type to the set
                }
            }

            Monitor.VerboseLog($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": prefixing method \"GameLocation.getFish\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_FishLocations), nameof(Prefix_getFish))
            );

            Monitor.VerboseLog($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": prefixing method \"GameLocation.GetFishFromLocationData\".");
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.GetFishFromLocationData), new Type[] { typeof(string), typeof(Vector2), typeof(int), typeof(Farmer), typeof(bool), typeof(bool), typeof(GameLocation), typeof(ItemQueryContext) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_FishLocations), nameof(Prefix_GetFishFromLocationData))
            );

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": postfixing every implementation of method \"GameLocation.GetCrabPotFishForTile\".", LogLevel.Trace);
            foreach (var type in crabPotMethods) //for each unique version of the crab pot method
            {
                Monitor.VerboseLog($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": postfixing method \"{type.Name}.GetCrabPotFishForTile\".");
                harmony.Patch(
                    original: AccessTools.Method(type, nameof(GameLocation.GetCrabPotFishForTile), new[] { typeof(Vector2) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_FishLocations), nameof(Postfix_GetCrabPotFishForTile))
                );
            }

            Applied = true;
        }

        /// <summary>Modifies the result of the original method, based on a customizable data asset and/or tile property.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        /// <param name="tile">The tile of the fishing bobber.</param>
        /// <param name="locationName">The name of the location to check for fish data. If null, this GameLocation instance will be used.</param>
        /// <remarks>
        /// If this mod has customization data for the location and tile targeted by this method, this patch will replace the "locationName" and/or "bobberTile" arguments' values.
        /// </remarks>
        private static void Prefix_getFish(GameLocation __instance, ref Vector2 bobberTile, ref string locationName)
        {
            try
            {
                var data = TileData.GetDataForTile<FishLocationsData>(AssetName, TilePropertyName, __instance, (int)bobberTile.X, (int)bobberTile.Y); //get fishing location data for this tile
                if (data?.UseLocation != null) //if a location override exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using fish from another location ({data.UseLocation}) at {__instance?.Name} ({(int)bobberTile.X},{(int)bobberTile.Y}).");
                    locationName = data.UseLocation; //override the target location
                }
                if (data?.UseTile != null) //if a tile override exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using fish from another tile ({data.UseTile.Value.X},{data.UseTile.Value.Y}) at {__instance?.Name} ({(int)bobberTile.X},{(int)bobberTile.Y}).");
                    bobberTile = data.UseTile.Value.AsVector2();
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FishLocations)}.{nameof(Prefix_getFish)}\" has encountered an error. Locations with custom fish might use defaults instead. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Modifies the result of the original method, based on a customizable data asset and/or tile property.</summary>
        /// <param name="locationName">The name of the location to check. Only used if "location" is null.</param>
        /// <param name="bobberTile">The tile location to check.</param>
        /// <param name="location">The location to check. Loaded from "locationName" if null; if still null, data from "locationName" will be used regardless.</param>
        /// <remarks>
        /// If this mod has customization data for the location and tile targeted by this method, this patch will replace the "locationName", "bobberTile", and/or "location" arguments' values.
        /// </remarks>
        private static void Prefix_GetFishFromLocationData(ref string locationName, ref Vector2 bobberTile, ref GameLocation location)
        {
            try
            {
                //imitate the location/name priority of the original method
                GameLocation loc = location;
                if (loc == null)
                    loc = Game1.getLocationFromName(locationName);

                if (loc == null) //if no actual location instance is found
                    return;

                var data = TileData.GetDataForTile<FishLocationsData>(AssetName, TilePropertyName, loc, (int)bobberTile.X, (int)bobberTile.Y); //get fishing location data for this tile
                if (data?.UseLocation != null) //if a location override exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using fish data from another location ({data.UseLocation}) at {loc.Name} ({(int)bobberTile.X},{(int)bobberTile.Y}).");
                    locationName = data.UseLocation;
                    location = Game1.getLocationFromName(data.UseLocation); //get the location to use, or use null if not found
                }
                if (data?.UseTile != null) //if a tile override exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using fish data from another tile ({data.UseTile.Value.X},{data.UseTile.Value.Y}) at {loc.Name} ({(int)bobberTile.X},{(int)bobberTile.Y}).");
                    bobberTile = data.UseTile.Value.AsVector2();
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FishLocations)}.{nameof(Prefix_GetFishFromLocationData)}\" has encountered an error. Locations with custom fish might use defaults instead. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Modifies the result of the original method, based on a customizable data asset and/or tile property.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        /// <param name="tile">The tile location of the crab pot.</param>
        /// <param name="__result">The result of the original method. A list of categories for crab pot results to use from Data/Fish. (In unmodded SDV 1.6, this contains either "ocean" or "freshwater").</param>
        /// <remarks>
        /// If this mod has customization data for the location and tile targeted by this method, this patch will replace the result of this method with a different list of fish types.
        /// </remarks>
        private static void Postfix_GetCrabPotFishForTile(GameLocation __instance, Vector2 tile, ref IList<string> __result)
        {
            try
            {
                var data = TileData.GetDataForTile<FishLocationsData>(AssetName, TilePropertyName, __instance, (int)tile.X, (int)tile.Y); //get fishing location data for this tile

                if (data?.UseCrabPotTypes != null) //if a list of crab types exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using custom crab pot results ({string.Join(", ", data.UseCrabPotTypes)}) at {__instance?.Name} ({tile.X},{tile.Y}).");

                    __result = data.UseCrabPotTypes;
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FishLocations)}.{nameof(Postfix_GetCrabPotFishForTile)}\" has encountered an error. Default crab pot results will be used for this location: \"{__instance?.Name}\". Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}
