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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Events;

namespace MoreConversationTopics
{
    // Applies Harmony patches to WorldChangeEvents.cs to add conversation topics for the Joja greenhouse and Leo's arrival
    public class WorldChangePatcher
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
                    original: AccessTools.Method(typeof(WorldChangeEvent), nameof(WorldChangeEvent.setUp)),
                    postfix: new HarmonyMethod(typeof(WorldChangePatcher), nameof(WorldChangePatcher.WorldChangeEvent_setUp_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to world change events with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to postfix
        private static void WorldChangeEvent_setUp_Postfix(WorldChangeEvent __instance)
        {
            try
            {
                switch ((int)__instance.whichEvent)
                {
                    // If the world change event in question is building the Joja greenhouse, add Joja greenhouse conversation topic
                    case 0:
                        try
                        {
                            Game1.player.activeDialogueEvents.Add("joja_Greenhouse", Config.JojaGreenhouseDuration);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Failed to add Joja greenhouse conversation topic with exception: {ex}", LogLevel.Error);
                        }
                        break;
                    // If the world change event in question is Leo arriving in the valley, add Leo arrival conversation topic
                    case 14:
                        // Add divorce conversation topic to current player
                        try
                        {
                            Game1.player.activeDialogueEvents.Add("leoArrival", Config.LeoArrivalDuration);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Failed to add Leo arrival conversation topic with exception: {ex}", LogLevel.Error);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to do world change event postfix with exception: {ex}", LogLevel.Error);
            }

        }
    }

}
