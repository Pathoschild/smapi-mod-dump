/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomNPCExclusions
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCExclusions
{
    /// <summary>A Harmony patch that excludes a list of NPCs from greeting and/or being greeted by other NPCs. See <see cref="NPC.sayHiTo(Character)"/>.</summary>
    public static class HarmonyPatch_Greetings
    {
        public static void ApplyPatch(HarmonyInstance harmony)
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
                Dictionary<string, List<string>> exclusions = ModEntry.GetAllNPCExclusions(); //get all exclusion data

                string greeterName = __instance.Name; //get the greeter's name (NPC performing the greeting)
                string recipientName = c?.Name; //get the recipient's name (character being greeted)

                bool SkipReply = false; //true if the recipient should NOT respond to the greeting

                if (greeterName != null && exclusions.TryGetValue(greeterName, out List<string> greeterExclusions)) //if the greeter has exclusion data
                {
                    if (greeterExclusions.Exists(entry =>
                        entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if the greeter is excluded from everything
                     || entry.StartsWith("OtherEvent", StringComparison.OrdinalIgnoreCase) //OR if the greeter is excluded from other events
                     || entry.StartsWith("Greet", StringComparison.OrdinalIgnoreCase) //OR if the greeter is excluded from greeting others
                    ))
                    {
                        ModEntry.Instance.Monitor.Log($"Excluded NPC from greeting someone: {greeterName}", LogLevel.Trace);
                        return false; //skip the original method
                    }
                    else if (greeterExclusions.Exists(entry => entry.StartsWith("BeGreeted", StringComparison.OrdinalIgnoreCase))) //if the greeter CAN greet others, but is excluded from being greeted
                    {
                        SkipReply = true; //skip the recipient's reply, if necessary
                    }
                }

                if (recipientName != null && exclusions.TryGetValue(recipientName, out List<string> recipientExclusions)) //if the recipient has exclusion data
                {
                    if (recipientExclusions.Exists(entry =>
                        entry.StartsWith("All", StringComparison.OrdinalIgnoreCase) //if the recipient is excluded from everything
                     || entry.StartsWith("OtherEvent", StringComparison.OrdinalIgnoreCase) //OR if the recipient is excluded from other events
                     || entry.StartsWith("BeGreeted", StringComparison.OrdinalIgnoreCase) //OR if the recipient is excluded from being greeted
                    ))
                    {
                        ModEntry.Instance.Monitor.Log($"Excluded NPC from being greeted: {recipientName}", LogLevel.Trace);
                        return false; //skip the original method
                    }
                    else if (recipientExclusions.Exists(entry => entry.StartsWith("Greet", StringComparison.OrdinalIgnoreCase))) //if the recipient CAN be greeted, but is excluded from greeting others
                    {
                        SkipReply = true; //skip the recipient's reply, if necessary
                    }
                }

                if (SkipReply) //if the greeting should still happen, but the recipient's reply should be skipped
                {
                    if (__instance.getHi(recipientName) is string greetingText) //if possible,
                        __instance.showTextAboveHead(greetingText); //display the greeting (imitating the original code in "NPC.sayHiTo", as of SDV 1.5.4)

                    ModEntry.Instance.Monitor.Log($"Excluded NPC from replying to greeting: {recipientName} replying to {greeterName}", LogLevel.Trace);
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