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
    // Adds Harmony Patches for conversation topics related to Ginger Island
    public class IslandPatcher
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
                Monitor.Log("Adding Harmony postfix to PerformCompleteAnimation() in ParrotUpgradePerch.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(ParrotUpgradePerch), nameof(ParrotUpgradePerch.PerformCompleteAnimation)),
                    postfix: new HarmonyMethod(typeof(IslandPatcher), nameof(IslandPatcher.ParrotUpgradePerch_PerformCompleteAnimation_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to parrot perch completion with exception: {ex}", LogLevel.Error);
            }
        }

        private static void ParrotUpgradePerch_PerformCompleteAnimation_Postfix(NetString ___upgradeName)
        {
            // Check if the upgrade name is null and then whether it's the right upgrade, and don't do anything if it's not
            if (___upgradeName.Value is not "Resort")
            {
                return;
            }

            // Make sure to add resort built conversation topic to all farmers in the game
            try
            {
                foreach (Farmer f in Game1.getAllFarmers())
                {
                    try
                    {
                        MCTHelperFunctions.AddOrExtendCT(f, "islandResortUnlocked", Config.IslandResortDuration);
                    }
                    catch (Exception ex)
                    {// Check to see if the farmer in question is online, and return different exception messages depending
                        if (!Game1.getOnlineFarmers().Contains(f))
                        {
                            Monitor.Log($"Failed to add resort built conversation topic to an offline farmer with exception: {ex}", LogLevel.Error);
                        }
                        else
                        {
                            Monitor.Log($"Failed to add resort built conversation topic to an online farmer with exception: {ex}", LogLevel.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add resort built conversation topic with exception: {ex}", LogLevel.Error);
                return;
            }
        }
    }
}
