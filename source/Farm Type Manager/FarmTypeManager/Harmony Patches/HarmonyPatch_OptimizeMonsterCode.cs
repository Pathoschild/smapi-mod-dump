/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Harmony patches that optimize some monster-related code, generally reducing CPU load and improving framerate.</summary>
        public static class HarmonyPatch_OptimizeMonsterCode
        {
            /// <summary>Indicates whether this class's patches are currently enabled.</summary>
            public static bool IsApplied { get; private set; } = false;

            /// <summary>Enables this class's Harmony patches. Does nothing if patches are currently applied.</summary>
            /// <param name="harmony"></param>
            public static void ApplyPatch(Harmony harmony)
            {
                if (IsApplied) //if these patches are currently applied
                    return; //do nothing

                IsApplied = true; //indicate that patches are applied

                //apply Harmony patches
                Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": prefixing SDV method \"GameLocation.isCollidingPosition\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.isCollidingPosition), new[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) }),
                    prefix: new HarmonyMethod(typeof(HarmonyPatch_OptimizeMonsterCode), nameof(GameLocation_isCollidingPosition_Prefix))
                );

                Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": transpiling SDV method \"GameLocation.isTemp\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.isTemp), new Type[] { }),
                    transpiler: new HarmonyMethod(typeof(HarmonyPatch_OptimizeMonsterCode), nameof(GameLocation_isTemp_Transpiler))
                );

                Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": prefixing SDV method \"Monster.findPlayer\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Monster), "findPlayer", new Type[] { }),
                    prefix: new HarmonyMethod(typeof(HarmonyPatch_OptimizeMonsterCode), nameof(Monster_findPlayer_Prefix))
                );

                Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": postfixing SDV method \"Monster.findPlayer\".", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Monster), "findPlayer", new Type[] { }),
                    postfix: new HarmonyMethod(typeof(HarmonyPatch_OptimizeMonsterCode), nameof(Monster_findPlayer_Postfix))
                );
            }

            /// <summary>Disables this class's Harmony patches. Does nothing if patches are not currently applied.</summary>
            /// <param name="harmony">The Harmony instance created for this mod.</param>
            public static void RemovePatch(Harmony harmony)
            {
                if (!IsApplied)
                    return;
                IsApplied = false;

                //disable Harmony patch(es)
                Utility.Monitor.Log($"Removing Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": prefix on SDV method \"GameLocation.isCollidingPosition\".", LogLevel.Trace);
                harmony.Unpatch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.isCollidingPosition), new[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) }),
                    HarmonyPatchType.Prefix,
                    harmony.Id
                );

                Utility.Monitor.Log($"Removing Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": transpiler on SDV method \"GameLocation.isTemp\".", LogLevel.Trace);
                harmony.Unpatch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.isTemp), new Type[] { }),
                    HarmonyPatchType.Transpiler,
                    harmony.Id
                );

                Utility.Monitor.Log($"Removing Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": prefix on SDV method \"Monster.findPlayer\".", LogLevel.Trace);
                harmony.Unpatch(
                    original: AccessTools.Method(typeof(Monster), "findPlayer", new Type[] { }),
                    HarmonyPatchType.Prefix,
                    harmony.Id
                );

                Utility.Monitor.Log($"Removing Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": postfix on SDV method \"Monster.findPlayer\".", LogLevel.Trace);
                harmony.Unpatch(
                    original: AccessTools.Method(typeof(Monster), "findPlayer", new Type[] { }),
                    HarmonyPatchType.Postfix,
                    harmony.Id
                );
            }

            /// <summary>Returns false (i.e. "not colliding") for flying monsters immediately, skipping any additional logic.</summary>
            /// <remarks>
            /// It is possible that this could cause errors if collision detection also performs necessary tasks.
            /// It may also change the original method's result if it would ever return true for flying monsters.
            /// I haven't encountered any such issues so far, though.
            /// </remarks>
            private static bool GameLocation_isCollidingPosition_Prefix(bool glider, Character character, ref bool __result)
            {
                try
                {
                    if (glider && character is Monster) //if this character is a flying monster
                    {
                        __result = false; //return false (prevent collision)
                        return false; //skip the original method
                    }
                    else
                        return true; //call the original method
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(GameLocation_isCollidingPosition_Prefix)}\" has encountered an error and will not be applied:\n{ex.ToString()}", LogLevel.Error);
                    return true; //call the original method
                }
            }

            /// <summary>Modifies any calls to <see cref="string.StartsWith(string)"/> to use faster <see cref="StringComparison.Ordinal"/> comparison.</summary>
            private static IEnumerable<CodeInstruction> GameLocation_isTemp_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                try
                {
                    List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                    MethodInfo oldStartsWith = AccessTools.Method(typeof(string), nameof(string.StartsWith), new[] { typeof(string) }); //get info for the original comparison method
                    MethodInfo newStartsWith = AccessTools.Method(typeof(string), nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) }); //get info for the new comparison method

                    CodeInstruction callNewStartsWith = new CodeInstruction(OpCodes.Callvirt, newStartsWith); //create an instruction that calls the new comparison method
                    CodeInstruction loadComparisonEnum = new CodeInstruction(OpCodes.Ldc_I4_4); //create an instruction that loads 4 (StringComparison.Ordinal's value) onto the stack

                    for (int x = 0; x < patched.Count(); x++) //for each existing instruction
                    {
                        if (patched[x].opcode == OpCodes.Callvirt //if this instruction is a virtual method call
                            && patched[x].operand?.Equals(oldStartsWith) == true) //AND it's calling the old method
                        {
                            patched[x] = callNewStartsWith; //replace this instruction with one that calls the new comparison method
                            patched.Insert(x, loadComparisonEnum); //insert the instruction load before the callvirt
                        }
                    }

                    return patched; //return the patched instructions
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(GameLocation_isTemp_Transpiler)}\" has encountered an error and will not be applied:\n{ex.ToString()}", LogLevel.Error);
                    return instructions; //return the original instructions
                }
            }

            /// <summary>
            /// Causes monsters to skip redundant calculations in single player mode.
            /// </summary>
            private static bool Monster_findPlayer_Prefix(ref Farmer __result)
            {
                try
                {
                    if (!Context.IsMultiplayer) //if this is NOT a multiplayer session
                    {
                        __result = Game1.player; //return the current player
                        return false; //skip the original method
                    }
                    else
                        return true; //call the original method
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(Monster_findPlayer_Prefix)}\" has encountered an error and will not be applied:\n{ex.ToString()}", LogLevel.Error);
                    return true; //call the original method
                }
            }

            /// <summary>
            /// Attempts to avoid a bug where monsters occasionally crash the game due to a null "Monster.Player" during "Monster.behaviorAtGameTick".
            /// </summary>
            /// <remarks>
            /// I cannot yet naturally reproduce this bug, but it has been reported by several players.
            /// atravita found that the null errors occurred in "!Player.isRafting" and fixed the issue by transpiling a null check into that code. If errors persist, consider that solution.
            /// This postfix solution should address issues in other code that reference "Monster.Player" as well. Most of the game's code assumes it cannot be null.</remarks>
            private static void Monster_findPlayer_Postfix(ref Farmer __result)
            {
                try
                {
                    if (__result == null) //if this method failed to return a farmer (possible due to other mods' patches, multiplayer/threading issues, etc)
                    {
                        __result = Game1.player; //assign the local player (should never be null because it causes immediate crashes in most contexts, but may still be possible)

                        if (__result == null) //if the result is somehow still null
                        {
                            Utility.Monitor.LogOnce($"Monster.findPlayer and Game1.player both returned null. If errors occur, please share your full log file with this mod's developer.", LogLevel.Debug);
                            return;
                        }
                        else
                        {
                            Utility.Monitor.LogOnce($"Monster.findPlayer returned null. Using Game1.player instead.", LogLevel.Trace);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(Monster_findPlayer_Postfix)}\" has encountered an error and will not be applied:\n{ex.ToString()}", LogLevel.Error);
                    return; //call the original method
                }
            }
        }
    }
}
