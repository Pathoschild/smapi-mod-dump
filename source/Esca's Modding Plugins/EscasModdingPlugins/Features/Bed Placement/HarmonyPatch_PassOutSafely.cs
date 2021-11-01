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
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace EscasModdingPlugins
{
    /// <summary>Allows mods to designate areas where players who pass out (faint from lack of energy/stamina) will not lose money or receive a letter about being rescued. Uses map properties.</summary>
    public static class HarmonyPatch_PassOutSafely
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
            MapPropertyName = ModEntry.PropertyPrefix + "PassOutSafely"; //assign map property name

            Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_PassOutSafely)}\": transpiling SDV method \"Farmer.performPassoutWarp(Farmer, string, Point, bool)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.performPassoutWarp)),
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_PassOutSafely), nameof(Farmer_performPassoutWarp))
            );

            Applied = true;
        }

        /// <summary>Replaces the given location with Farmhouse if this mod should allow players to safely pass out.</summary>
        /// <param name="location">The location of the player passing out.</param>
        /// <returns>The Farmhouse location if players should pass out safely; the original location otherwise.</returns>
        public static GameLocation ModifyPassOutLocation(GameLocation location)
        {
            if (location == null || location is FarmHouse || location is IslandFarmHouse || location is Cellar) //if this location is null OR already allows safe passout
                return location; //don't replace it

            if (location.Map.Properties.TryGetValue(MapPropertyName, out var mapPropertyObject)) //if the location has a non-null map property
            {
                string mapProperty = mapPropertyObject?.ToString() ?? ""; //get the map property as a string

                bool result = !mapProperty.Trim().StartsWith("F", StringComparison.OrdinalIgnoreCase); //true if the property's value is NOT "false"

                if (Monitor?.IsVerbose == true)
                {
                    if (result)
                        Monitor.Log($"Allowing player to pass out from exhaustion safely. Location: {location?.Name}. Map property value: \"{mapProperty}\".", LogLevel.Trace);
                    else
                        Monitor.Log($"NOT allowing player to pass out from exhaustion safely. Location: {location?.Name}. Map property value: \"{mapProperty}\".", LogLevel.Trace);
                }

                if (result)
                    return Game1.getLocationFromName("FarmHouse") ?? location; //replace the location with FarmHouse (resulting in a safe passout check)
                else
                    return location; //don't replace it
            }

            if (Monitor?.IsVerbose == true)
                Monitor.Log($"NOT allowing player to pass out from exhaustion safely; no relevant map or tile property. Location: {location?.Name}.", LogLevel.Trace);

            return location; //don't replace it
        }

        /// <summary>Overwrites the location used to check whether a player passed out safely. Uses <see cref="ModifyPassOutLocation(GameLocation)"/>.</summary>
        /// <remarks>
        /// Old C#:
        ///     GameLocation passOutLocation = who.currentLocationRef.Value;
        ///     
        /// New C#:
        ///     GameLocation passOutLocation = ModifyPassOutLocation(who.currentLocationRef.Value);
        ///     
        /// Old IL:
        /// 	IL_0020: callvirt instance class StardewValley.GameLocation StardewValley.Network.NetLocationRef::get_Value()
        ///     IL_0025: stfld class StardewValley.GameLocation StardewValley.Farmer/'<>c__DisplayClass691_0'::passOutLocation
        ///     
        /// New IL:
        /// 	IL_0020: callvirt instance class StardewValley.GameLocation StardewValley.Network.NetLocationRef::get_Value()
        ///         (?): call static StardewValley.GameLocation EscasModdingPlugins.HarmonyPatch_PassOutSafely::ModifyPassOutLocation(StardewValley.GameLocation location)
        ///     IL_0025: stfld class StardewValley.GameLocation StardewValley.Farmer/'<>c__DisplayClass691_0'::passOutLocation
        /// </remarks>
        /// <param name="instructions">The original method's CIL code.</param>
        private static IEnumerable<CodeInstruction> Farmer_performPassoutWarp(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                var modifierMethod = AccessTools.Method(typeof(HarmonyPatch_PassOutSafely), nameof(ModifyPassOutLocation)); //get the modifier method's info
                CodeInstruction modifierInstruction = new CodeInstruction(OpCodes.Call, modifierMethod);

                for (int x = 0; x < patched.Count; x++) //for each instruction
                {
                    if (patched[x].opcode == OpCodes.Stfld //if this code is "store field"
                     && patched[x].operand?.ToString().Contains("passOutLocation") == true) //and its operand is the local field "passOutLocation"
                    {
                        patched.Insert(x, modifierInstruction); //call the modifier method before this instruction
                        Monitor.VerboseLog($"Transpiler inserted a call to {nameof(ModifyPassOutLocation)} at line {x}.");
                        break; //stop after finding 1 match
                    }
                }

                return patched; //return the patched instructions
            }
            catch (Exception ex)
            {
                Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_PassOutSafely)}\" has encountered an error. Transpiler \"{nameof(Farmer_performPassoutWarp)}\" will not be applied. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }
    }
}
