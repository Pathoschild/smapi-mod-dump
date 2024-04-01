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
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from greeting and/or being greeted by other NPCs. See <see cref="NPC.sayHiTo(Character)"/>.</summary>
    public static class HarmonyPatch_Greetings
    {
        public static void ApplyPatch(Harmony harmony)
        {
            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_Greetings)}\": prefixing SDV method \"NPC.sayHiTo(Character)\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.sayHiTo), new[] { typeof(Character) }),
                prefix: new HarmonyMethod(typeof(HarmonyPatch_Greetings), nameof(NPC_sayHiTo))
            );
        }

        /// <summary>A Harmony postfix patch that excludes a list of NPCs from greeting and/or being greeted by other NPCs.</summary>
        /// <param name="__instance">The NPC saying the greeting.</param>
        /// <param name="c">The character being greeted.</param>
        public static bool NPC_sayHiTo(NPC __instance, Character c)
        {
            try
            {
                Dictionary<string, List<string>> exclusions = DataHelper.GetAllExclusions(); //get all exclusion data

                string greetName = __instance.Name; //get the name of the NPC saying the greeting
                string beGreetedName = c?.Name; //get the name of the character being greeted

                bool SkipReply = false; //true if the recipient should NOT respond to the greeting

                var excludedFromGreet = DataHelper.GetNPCsWithExclusions("All", "OtherEvent", "Greet");
                var excludedFromBeingGreeted = DataHelper.GetNPCsWithExclusions("All", "OtherEvent", "BeGreeted");

                if (excludedFromGreet.Contains(greetName)) //if this NPC is excluded from greeting others
                {
                    if (ModEntry.Instance.Monitor.IsVerbose)
                        ModEntry.Instance.Monitor.Log($"Excluded NPC from greeting someone: {greetName}", LogLevel.Trace);
                    return false; //skip the original method
                }
                else if (excludedFromBeingGreeted.Contains(greetName)) //if the greeter CAN greet others, but is excluded from being greeted
                {
                    SkipReply = true; //skip the recipient's reply (if necessary)
                }

                if (excludedFromBeingGreeted.Contains(beGreetedName)) //if the recipient has exclusion data
                {
                    if (ModEntry.Instance.Monitor.IsVerbose)
                        ModEntry.Instance.Monitor.Log($"Excluded NPC from being greeted: {beGreetedName}", LogLevel.Trace);
                    return false; //skip the original method
                }
                else if (excludedFromGreet.Contains(beGreetedName)) //if the recipient CAN be greeted, but is excluded from greeting others
                {
                    SkipReply = true; //skip the recipient's reply
                }

                if (SkipReply) //if the greeting should still happen, but the recipient's reply should be skipped
                {
                    //imitate the code in "NPC.sayHiTo" that displays the greeting (but not the reply)
                    if (__instance.getHi(beGreetedName) is string greetingText)
                        __instance.showTextAboveHead(greetingText);

                    if (ModEntry.Instance.Monitor.IsVerbose)
                        ModEntry.Instance.Monitor.Log($"Excluded NPC from replying to greeting: {beGreetedName} replying to {greetName}", LogLevel.Trace);
                    return false; //skip the original method
                }

                return true; //run the original method
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(NPC_sayHiTo)}\" has encountered an error and may revert to default behavior. Full error message:\n{ex.ToString()}", LogLevel.Error);
                return true; //run the original method
            }
        }


    }
}