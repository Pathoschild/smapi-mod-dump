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
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EscasModdingPlugins
{
    /// <summary>Allows customization of which fishing "zones" and locations are used from the "Data/Locations" asset. Uses a custom data asset and/or tile properties.</summary>
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


        Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": postfixing every implementation of method \"GameLocation.GetCrabPotFishForTile(Vector2)\".", LogLevel.Trace);
            foreach (var type in crabPotMethods) //for each unique version of the crab pot method
            {
                Monitor.VerboseLog($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": postfixing method \"{type.Name}.GetCrabPotFishForTile(Vector2)\".");
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
        private static void Prefix_getFish(GameLocation __instance, Vector2 bobberTile, ref string locationName)
        {
            try
            {
                var data = TileData.GetDataForTile<FishLocationsData>(AssetName, TilePropertyName, __instance, (int)bobberTile.X, (int)bobberTile.Y); //get fishing location data for this tile
                if (data?.UseLocation != null) //if a custom fish location exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using fish from another location ({data.UseLocation}) at {__instance?.Name} ({bobberTile.X},{bobberTile.Y}).");
                    locationName = data.UseLocation; //override the target location
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FishLocations)}.{nameof(Prefix_getFish)}\" has encountered an error. Locations with custom fish might use defaults instead. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Modifies the result of the original method, based on a customizable data asset and/or tile property.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        /// <param name="tile">The tile location of the crab pot.</param>
        /// <param name="__result">The result of the original method. A list of categories for crab pot results to use from Data/Fish. (In unmodded SDV 1.6, this contains either "ocean" or "freshwater").</param>
        private static void Postfix_GetCrabPotFishForTile(GameLocation __instance, Vector2 tile, ref IList<string> __result)
        {
            try
            {
                var data = TileData.GetDataForTile<FishLocationsData>(AssetName, TilePropertyName, __instance, (int)tile.X, (int)tile.Y); //get fishing location data for this tile
                if (data?.UseOceanCrabPots != null) //if custom crab pot data exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using custom crab pot results ({(data.UseOceanCrabPots.Value ? "ocean" : "freshwater")}) at {__instance?.Name} ({tile.X},{tile.Y}).");

                    if (data.UseOceanCrabPots.Value)
                        __result = GameLocation.OceanCrabPotFishTypes; //override to ocean results
                    else
                        __result = GameLocation.DefaultCrabPotFishTypes; //override to freshwater results
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
