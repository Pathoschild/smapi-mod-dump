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
using StardewValley.Companions;
using StardewValley.Monsters;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A Harmony patch that prevents certain monsters being eaten by frogs from the Frog Egg trinket.</summary>
        public class HarmonyPatch_FrogEggImmunity
        {
            /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
            /// <param name="harmony">This mod's Harmony instance.</param>
            public static void ApplyPatch(Harmony harmony)
            {
                try
                {
                    //apply Harmony patches
                    Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FrogEggImmunity)}\": prefixing SDV method \"HungryFrogCompanion.tongueReachedMonster\".", LogLevel.Trace);
                    harmony.Patch(
                        original: AccessTools.Method(typeof(HungryFrogCompanion), nameof(HungryFrogCompanion.tongueReachedMonster)),
                        prefix: new HarmonyMethod(typeof(HarmonyPatch_FrogEggImmunity), nameof(tongueReachedMonster_Prefix))
                    );
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FrogEggImmunity)}\" failed to apply. Modded monsters might be defeated too easily, or in ways that cause bugs. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }

            /// <summary>Causing nothing to happen if the frog's tongue touches a monster with the "instant kill immunity" mod data flag.</summary>
            /// <param name="m">The monster touched by the frog's tongue.</param>
            /// <returns>If false, the original method will be skipped.</returns>
            public static bool tongueReachedMonster_Prefix(Monster m)
            {
                try
                {
                    if (m != null && m.modData.TryGetValue(Utility.ModDataKeys.InstantKillImmunity, out string value) && value.StartsWith("t", StringComparison.OrdinalIgnoreCase)) //if this monster has the "instant kill immunity" setting and the value is true
                    {
                        return false; //skip the original method (do nothing when a frog's tongue touches this monster, as if the monster weren't there)
                    }

                    return true; //call the original method
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_FrogEggImmunity)}\" has encountered an error. Modded monsters might be defeated too easily, or in ways that cause bugs. Full error message: \n{ex.ToString()}", LogLevel.Error);
                    return true; //call the original method
                }
            }
        }
    }
}
