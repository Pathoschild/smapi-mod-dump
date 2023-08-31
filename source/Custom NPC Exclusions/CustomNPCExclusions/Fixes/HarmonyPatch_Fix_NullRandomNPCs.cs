/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomNPCExclusions
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;

namespace CustomNPCExclusions.Fixes
{
    /// <summary>A Harmony patch that attempts to prevent null results when getting random town NPCs.</summary>
    /// <remarks>
    /// Some mods and/or custom NPCs cause rare (1-3% chance per call in testing) null return values from "getRandomTownNPC".
    /// These methods are treated as null-safe in Stardew v1.5 and v1.6. Null values cause repeating errors that freeze the game overnight.
    /// While the original methods avoid returning null when possible, Harmony patches can subvert that behavior.
    /// </remarks>
    public static class HarmonyPatch_Fix_NullRandomNPCs
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_Fix_NullRandomNPCs)}\": postfixing SDV method \"Utility.getRandomTownNPC()\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getRandomTownNPC), new Type[0]),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_Fix_NullRandomNPCs), nameof(Utility_getRandomTownNPC_Postfix))
            );

            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_Fix_NullRandomNPCs)}\": postfixing SDV method \"Utility.getRandomTownNPC(Random)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getRandomTownNPC), new[] { typeof(Random) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_Fix_NullRandomNPCs), nameof(Utility_getRandomTownNPC_Random_Postfix))
            );
        }

        /// <summary>True while this patch is attempting to fix a null result. Intended to prevent unnecessary recursion.</summary>
        private static bool currentlyFixing = false;

        /// <summary>Forces the method to call itself again until a non-null result is found.</summary>
        /// /// <remarks>"Non-seeded" randomization overload.</remarks>
        [HarmonyPriority(Priority.Low)] //use low priority to run after most other postfixes
        private static void Utility_getRandomTownNPC_Postfix(ref NPC __result)
        {
            try
            {
                if (!currentlyFixing && __result == null) //if this the "first" call to this postfix AND the return NPC is null
                {
                    currentlyFixing = true; //disable any further calls to this postfix until this call is complete

                    ModEntry.Instance.Monitor.Log($"The method \"Utility.getRandomTownNPC()\" returned null (nothing) instead of an NPC. This is sometimes caused by custom NPC mods with unusual behavior. Retrying until a valid NPC is found...", LogLevel.Trace);

                    NPC newResult = Utility.getRandomTownNPC(); //try again with the same Random
                    int count = 1; //the number of retry attempts
                    while (newResult == null) //while the result is still null, keep trying
                    {
                        newResult = Utility.getRandomTownNPC();
                        count++;
                    }

                    __result = newResult; //override the return value

                    ModEntry.Instance.Monitor.Log($"Fix complete. The method was called again {count} time(s) to find a valid NPC and prevent errors.", LogLevel.Trace);

                    currentlyFixing = false; //task complete, so re-enable other calls to this postfix
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_Fix_NullRandomNPCs)}\" has encountered an error. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }

        /// <summary>Forces the method to re-call itself again until a non-null result is found.</summary>
        /// <remarks>"Seeded" randomization overload.</remarks>
        [HarmonyPriority(Priority.Low)] //use low priority to run after most other postfixes
        private static void Utility_getRandomTownNPC_Random_Postfix(Random r, ref NPC __result)
        {
            try
            {
                if (!currentlyFixing && __result == null) //if this the "first" call to this postfix AND the return NPC is null
                {
                    currentlyFixing = true; //disable any further calls to this postfix until this call is complete

                    ModEntry.Instance.Monitor.Log($"The method \"Utility.getRandomTownNPC(Random)\" returned null (nothing) instead of an NPC. This is sometimes caused by custom NPC mods with unusual behavior. Retrying until a valid NPC is found...", LogLevel.Trace);

                    NPC newResult = Utility.getRandomTownNPC(r); //try again with the same Random
                    int count = 1; //the number of retry attempts
                    while (newResult == null) //while the result is still null, keep trying
                    {
                        newResult = Utility.getRandomTownNPC(r);
                        count++;
                    }

                    __result = newResult; //override the return value

                    ModEntry.Instance.Monitor.Log($"Fix complete. The method was called again {count} time(s) to find a valid NPC and prevent errors.", LogLevel.Trace);

                    currentlyFixing = false; //task complete, so re-enable other calls to this postfix
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_Fix_NullRandomNPCs)}\" has encountered an error. Full error message: \n{ex.ToString()}", LogLevel.Error);
                return; //run the original method
            }
        }
    }
}