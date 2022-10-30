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
using HarmonyLib;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace MoreConversationTopics
{
    // Adds Harmony Patches for conversation topics related to the dark shrine of selfishness event
    public class DarkShrinePatcher
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
                Monitor.Log("Adding Harmony postfix to getRidOfChildren() in Farmer.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getRidOfChildren)),
                    postfix: new HarmonyMethod(typeof(DarkShrinePatcher), nameof(DarkShrinePatcher.Farmer_getRidOfChildren_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to farmer dark shrine of selfishness action with exception: {ex}", LogLevel.Error);
            }
        }

        private static void Farmer_getRidOfChildren_Postfix()
        {
            try
            {
                MCTHelperFunctions.AddOrExtendCT("childrenDoved", Config.SelfishShrineDuration);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add dark shrine of selfishness conversation topic with exception: {ex}", LogLevel.Error);
            }
        }
    }
}