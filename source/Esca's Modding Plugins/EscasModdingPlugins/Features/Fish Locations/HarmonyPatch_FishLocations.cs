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
            HashSet<Type> getFishingLocationMethods = new HashSet<Type>(); //every type with a unique GameLocation.getFishingLocation(Vector2)
            HashSet<Type> oceanCrabPotMethods = new HashSet<Type>(); //every type with a unique GameLocation.catchOceanCrabPotFishFromThisSpot(int, int)

            foreach (Type type in AccessTools.AllTypes()) //for every type
            {
                if (typeof(GameLocation).IsAssignableFrom(type)) //if this is a type of GameLocation
                {
                    if (AccessTools.Method(type, nameof(GameLocation.getFishingLocation), new[] { typeof(Vector2) }) is MethodInfo fishing) //if this type has a fishing method
                        getFishingLocationMethods.Add(fishing.DeclaringType); //add the method's declaring type to the set
                    
                    if (AccessTools.Method(type, nameof(GameLocation.catchOceanCrabPotFishFromThisSpot), new[] { typeof(int), typeof(int) }) is MethodInfo ocean) //if this type has a crab pot method
                        oceanCrabPotMethods.Add(ocean.DeclaringType); //add the method's declaring type to the set
                }
            }

            //apply patches
            foreach (var type in getFishingLocationMethods) //for each unique version of the fishing method
            {
                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": postfixing SDV method \"{type.Name}.getFishingLocation(Vector2)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(type, nameof(GameLocation.getFishingLocation), new[] { typeof(Vector2) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_FishLocations), nameof(postfix_getFishingLocation))
                );
            }

            foreach (var type in oceanCrabPotMethods) //for each unique version of the crab pot method
            {
                Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": postfixing SDV method \"{type.Name}.catchOceanCrabPotFishFromThisSpot(int, int)\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(type, nameof(GameLocation.catchOceanCrabPotFishFromThisSpot), new[] { typeof(int), typeof(int) }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_FishLocations), nameof(postfix_catchOceanCrabPotFishFromThisSpot))
                );
            }

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": transpiling SDV method \"CrabPot.DayUpdate(GameLocation)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_FishLocations), nameof(transpiler_CrabPot_DayUpdate))
            );

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FishLocations)}\": prefixing SDV method \"GameLocation.getFish(float, int, int, Farmer, double, Vector2, string)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_FishLocations), nameof(prefix_getFish))
            );

            Applied = true;
        }

        /// <summary>Modifies the result of the original method, based on a customizable data asset and/or tile property.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        /// <param name="tile">The tile to check. As of SDV 1.5.4, this is the tile location of the fishing farmer.</param>
        /// <param name="__result">The final result of the original method. Indicates which fish group(s) should be used from the Data/Locations asset.</param>
        private static void postfix_getFishingLocation(GameLocation __instance, Vector2 tile, ref int __result)
        {
            try
            {
                if (MostRecentBobberTile.HasValue) //if a target bobber tile was stored
                    tile = MostRecentBobberTile.Value; //use it instead of the provided tile (working around a bug as of SDV 1.5.4)

                var data = TileData.GetDataForTile<FishLocationsData>(AssetName, TilePropertyName, __instance, (int)tile.X, (int)tile.Y); //get fishing location data for this tile
                if (data?.UseZone != null) //if a custom fish group exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using custom fish group ({data.UseZone}) at {__instance?.Name} ({tile.X},{tile.Y}).");
                    __result = data.UseZone.Value; //override the result
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FishLocations)}.{nameof(postfix_getFishingLocation)}\" has encountered an error. Default fishing areas will be used for this location: \"{__instance?.Name}\". Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Modifies the result of the original method, based on a customizable data asset and/or tile property.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        /// <param name="x">The horizontal position of the tile to check.</param>
        /// <param name="y">The vertical position of the tile to check.</param>
        /// <param name="__result">The result of the original method. Indicates whether crab pots on this tile should use "ocean" (true) or "freshwater" (false) data.</param>
        private static void postfix_catchOceanCrabPotFishFromThisSpot(GameLocation __instance, int x, int y, ref bool __result)
        {
            try
            {
                var data = TileData.GetDataForTile<FishLocationsData>(AssetName, TilePropertyName, __instance, x, y); //get fishing location data for this tile
                if (data?.UseOceanCrabPots != null) //if custom crab pot data exists for this tile
                {
                    if (Monitor.IsVerbose)
                        Monitor.VerboseLog($"Using custom crab pot results ({(data.UseOceanCrabPots.Value ? "ocean" : "freshwater")}) at {__instance?.Name} ({x},{y}).");
                    __result = data.UseOceanCrabPots.Value; //override the result
                }
                else if (__instance is Beach) //if this tile has no idea, but its location is a Beach type
                {
                    __result = true; //default to true (imitating code removed from CrabPot.DayUpdate via transpiler)
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FishLocations)}.{nameof(postfix_catchOceanCrabPotFishFromThisSpot)}\" has encountered an error. Default crab pot results will be used for this location: \"{__instance?.Name}\". Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>The bobberTile most recently passed to <see cref="GameLocation.getFish"/>.</summary>
        /// <remarks>
        /// Used to work around a SDV bug where it isn't passed to <see cref="GameLocation.getFishingLocation"/> without transpiling that method.
        /// Alternatives include comparing the provided tile to the tile position of the local player (or every farmer at the location).
        /// </remarks>
        private static Vector2? MostRecentBobberTile = null;

        /// <summary>Modifies the locationName argument of the original method, based on a customizable data asset and/or tile property.</summary>
        /// <param name="__instance">The instance calling the original method.</param>
        /// <param name="bobberTile">The tile where the player is fishing.</param>
        /// <param name="locationName">An alternate location name to use when loading fish data.</param>
        private static void prefix_getFish(GameLocation __instance, Vector2 bobberTile, ref string locationName)
        {
            try
            {
                MostRecentBobberTile = bobberTile; //store this for use with getFishingLocation

                var data = TileData.GetDataForTile<FishLocationsData>(AssetName, TilePropertyName, __instance, (int)bobberTile.X, (int)bobberTile.Y); //get fishing location data for this tile
                if (data?.UseLocation != null) //if another location name was provided
                {
                    locationName = data.UseLocation; //override the original method's locationName argument
                }
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FishLocations)}.{nameof(prefix_getFish)}\" has encountered an error. Default fish data will be used for this location: \"{__instance?.Name}\". Full error message: \n{ex.ToString()}", LogLevel.Error);
                MostRecentBobberTile = null; //avoid using incorrect bobber tiles
                return; //run the original method
            }
        }

        /// <summary>Removes a hardcoded "use ocean crab pot results for Beach locations" check from the original method.</summary>
        /// <remarks>
        /// Old C#:
        ///     bool should_catch_ocean_fish = location is Beach || location.catchOceanCrabPotFishFromThisSpot((int)tileLocation.X, (int)tileLocation.Y);
        ///     
        /// New C#:
        ///     bool should_catch_ocean_fish = location.catchOceanCrabPotFishFromThisSpot((int)tileLocation.X, (int)tileLocation.Y);
        ///     
        /// Old IL:
        ///     ldarg.1
        ///     isinst StardewValley.Locations.Beach
        ///     brtrue.s
        ///     
        /// New IL:
        ///     ldnull
        ///     brtrue.s
        /// 
        /// The removed check is reimplemented in this class's postfix for "catchOceanCrabPotFishFromThisSpot", but given lower priority than custom data.
        /// </remarks>
        /// <param name="instructions">The original method's CIL code.</param>
        private static IEnumerable<CodeInstruction> transpiler_CrabPot_DayUpdate(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                for (int x = patched.Count - 1; x >= 1; x--) //for each instruction (looping backward, stopping at 2)
                {
                    if (patched[x].opcode == OpCodes.Isinst && patched[x].operand.Equals(typeof(Beach)) //if this instruction is "is Beach"
                        && patched[x - 1].opcode == OpCodes.Ldarg_1) //and the previous instruction loads argument 1 (GameLocation location)
                    {
                        patched[x] = new CodeInstruction(OpCodes.Ldnull); //replace the "Isinst" with an instruction that loads null/false
                        patched.RemoveAt(x - 1); //remove the previous instruction
                    }
                }

                return patched; //return the patched instructions
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FishLocations)}.{nameof(transpiler_CrabPot_DayUpdate)}\" has encountered an error. Default results will be used for beach crab pots. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }
    }
}
