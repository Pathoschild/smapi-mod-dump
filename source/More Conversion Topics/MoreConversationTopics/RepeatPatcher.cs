/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MoreConversationTopics
**
*************************************************/

using System;
using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Linq;
using StardewValley;

namespace MoreConversationTopics
{
    // Applies Harmony patches to Farmer dayupdate() in order to allow conversation topics from this mod to repeat
    public class RepeatPatcher
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        // Method to apply harmony patch
        public static void Apply(Harmony harmony)
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.dayupdate)),
                    prefix: new HarmonyMethod(typeof(RepeatPatcher), nameof(RepeatPatcher.Farmer_dayupdate_Prefix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add prefix to farmer dayupdate with exception: {ex}", LogLevel.Error);
            }

            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.dayupdate)),
                    postfix: new HarmonyMethod(typeof(RepeatPatcher), nameof(RepeatPatcher.Farmer_dayupdate_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to farmer dayupdate with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to prefix
        private static void Farmer_dayupdate_Prefix(Farmer __instance, out List<string> __state)
        {
            // Create a state variable to check the state in the prefix, used for logic in the postfix
            __state = new List<string>();
            try
            {
                // Get a list of which conversation topics are ending today
                foreach (string s2 in __instance.activeDialogueEvents.Keys.ToList())
                {
                    if (__instance.activeDialogueEvents[s2] < 1)
                    {
                        __state.Add(s2);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to find all ending conversation topics with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to postfix
        private static void Farmer_dayupdate_Postfix(Farmer __instance, List<string> __state)
        {
            // Remove the mail flags associated with each conversation topic added by this mod that is ending today
            try
            {
                foreach (string s in __state)
                {
                    if (ModEntry.isRepeatableCTAddedByMod(s))
                    {
                        foreach (NPC npc in Utility.getAllCharacters())
                        {
                            if (npc.isVillager())
                            {
                                __instance.mailReceived.Remove(npc.Name + "_" + s);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to remove conversation topic mail flags with exception: {ex}", LogLevel.Error);
            }
        }
    }

}