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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Harmony patches that optimize some monster-related code, generally reducing CPU load and improving framerate.</summary>
        public static class HarmonyPatch_OptimizeMonsterCode
        {
            /// <summary>Enables this class's Harmony patches. Does nothing if patches are currently applied.</summary>
            /// <param name="harmony"></param>
            public static void ApplyPatch(Harmony harmony)
            {
                try
                {
                    //apply Harmony patches
                    Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\": prefixing SDV method \"GameLocation.isCollidingPosition\".", LogLevel.Trace);
                    harmony.Patch(
                        original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.isCollidingPosition), new[] { typeof(Microsoft.Xna.Framework.Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool), typeof(bool) }),
                        prefix: new HarmonyMethod(typeof(HarmonyPatch_OptimizeMonsterCode), nameof(GameLocation_isCollidingPosition_Prefix))
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
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_OptimizeMonsterCode)}\" failed to apply. Monsters might slow the game down or cause errors. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }

            /// <summary>Returns false (i.e. "not colliding") for flying monsters immediately, skipping any additional logic.</summary>
            /// <remarks>
            /// It is possible that this could cause errors if collision detection also performs necessary tasks.
            /// It may also change the original method's result if it would ever return true for flying monsters.
            /// As of this writing, it has been in place for a few years without any known errors or side effects.
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
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(GameLocation_isCollidingPosition_Prefix)}\" has encountered an error. Flying monsters might cause the game to run slower. Full error message: \n{ex.ToString()}", LogLevel.Error);
                    return true; //call the original method
                }
            }

            /// <summary>Causes monsters to skip redundant calculations in single player mode.</summary>
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
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(Monster_findPlayer_Prefix)}\" has encountered an error. Monsters might cause the game to run slower in single-player mode. Full error message: \n{ex.ToString()}", LogLevel.Error);
                    return true; //call the original method
                }
            }

            /// <summary>Attempts to avoid a bug where monsters occasionally crash the game due to a null "Monster.Player" during "Monster.behaviorAtGameTick".</summary>
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
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(Monster_findPlayer_Postfix)}\" has encountered an error. Full error message: \n{ex.ToString()}", LogLevel.Error);
                    return; //call the original method
                }
            }
        }
    }
}
