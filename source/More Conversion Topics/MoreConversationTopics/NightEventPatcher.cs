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
using StardewValley.Buildings;
using StardewValley.Events;

namespace MoreConversationTopics
{
    // Applies Harmony patches to SoundInTheNightEvent.cs, FairyEvent.cs, and WitchEvent.cs to add conversation topics for various night events
    public class NightEventPatcher
    {
        private static IMonitor Monitor;
        private static ModConfig Config;

        // call this method from your Entry class
        public static void Initialize(IMonitor monitor, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
        }

        // Method to apply harmony patches
        public static void Apply(Harmony harmony)
        {
            try
            {
                Monitor.Log("Adding Harmony postfix to setUp() in SoundInTheNightEvent.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(SoundInTheNightEvent), nameof(SoundInTheNightEvent.setUp)),
                    postfix: new HarmonyMethod(typeof(NightEventPatcher), nameof(NightEventPatcher.SoundInTheNightEvent_setUp_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to sound in the night event with exception: {ex}", LogLevel.Error);
            }

            try
            {
                Monitor.Log("Adding Harmony postfix to setUp() in WitchEvent.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(WitchEvent), nameof(WitchEvent.setUp)),
                    postfix: new HarmonyMethod(typeof(NightEventPatcher), nameof(NightEventPatcher.WitchEvent_setUp_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to witch event with exception: {ex}", LogLevel.Error);
            }

            try
            {
                Monitor.Log("Adding Harmony postfix to setUp() in FairyEvent.cs", LogLevel.Trace);
                harmony.Patch(
                    original: AccessTools.Method(typeof(FairyEvent), nameof(FairyEvent.setUp)),
                    postfix: new HarmonyMethod(typeof(NightEventPatcher), nameof(NightEventPatcher.FairyEvent_setUp_Postfix))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add postfix to fairy event with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to postfix SoundInTheNightEvent
        private static void SoundInTheNightEvent_setUp_Postfix(bool __result, NetInt ___behavior)
        {
            // If the event didn't actually happen, no need to do anything
            if (__result)
            {
                return;
            }

            // Decide what to do based on the type of sound in the night event
            try
            {
                switch ((int)___behavior)
                {
                    // If the sound in the night event is the UFO landing, add UFO landing conversation topic
                    case 0:
                        try
                        {
                            MCTHelperFunctions.AddMaybePreExistingCT("UFOLandedOnFarm", Config.UFOLandedDuration);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Failed to add UFO landed on farm conversation topic with exception: {ex}", LogLevel.Error);
                        }
                        break;
                    // If the sound in the night event in question is a meteorite landing on the farm, add meteorite conversation topic
                    case 1:
                        try
                        {
                            MCTHelperFunctions.AddMaybePreExistingCT("meteoriteLandedOnFarm", Config.MeteoriteLandedDuration);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Failed to add meteorite landed on farm conversation topic with exception: {ex}", LogLevel.Error);
                        }
                        break;
                    // If the sound in the night event in question is an owl statue landing on the farm, add owl statue conversation topic
                    case 3:
                        try
                        {
                            MCTHelperFunctions.AddMaybePreExistingCT("owlStatueLandedOnFarm", Config.OwlStatueDuration);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Failed to add owl statue landed on farm conversation topic with exception: {ex}", LogLevel.Error);
                        }
                        break;
                    // If the sound in the night event in question is the railroad earthquake, add the railroad earthquake conversation topic
                    case 4:
                        try
                        {
                            MCTHelperFunctions.AddMaybePreExistingCT("railroadEarthquake", Config.MeteoriteLandedDuration);
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"Failed to add railroad earthquake conversation topic with exception: {ex}", LogLevel.Error);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to do sound in the night event postfix with exception: {ex}", LogLevel.Error);
            }

        }

        // Method that is used to postfix WitchEvent
        private static void WitchEvent_setUp_Postfix(WitchEvent __instance, bool __result, Building ___targetBuilding)
        {
            // If the event did not trigger, then no need to do anything
            if (__result)
            {
                return;
            }

            // Add the right witch event conversation topics depending on the witch visit
            try
            {
                // If the witch is visiting a Slime Hutch, add the witch slime hut conversation topic
                if (___targetBuilding.buildingType.Equals("Slime Hutch"))
                {
                    MCTHelperFunctions.AddMaybePreExistingCT("witchSlimeHutVisit", Config.WitchVisitDuration);
                }
                // Otherwise, if the witch is visiting a coop, add one of the witch coop conversation topics
                else if (___targetBuilding is Coop)
                {
                    // If the witch is a golden witch (post-perfection coop visit), add the golden witch conversation topic
                    if (__instance.goldenWitch)
                    {
                        MCTHelperFunctions.AddMaybePreExistingCT("goldenWitchCoopVisit", Config.WitchVisitDuration);
                    }
                    // Otherwise add the normal witch coop visit conversation topic
                    else
                    {
                        MCTHelperFunctions.AddMaybePreExistingCT("witchCoopVisit", Config.WitchVisitDuration);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add witch visit conversation topic with exception: {ex}", LogLevel.Error);
            }
        }

        // Method that is used to postfix FairyEvent
        private static void FairyEvent_setUp_Postfix(bool __result)
        {
            // If the event did not trigger, then no need to do anything
            if (__result)
            {
                return;
            }

            // Add the fairy event conversation topic
            try
            {
                MCTHelperFunctions.AddMaybePreExistingCT("fairyFarmVisit", Config.FairyVisitDuration);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to add fairy visit conversation topic with exception: {ex}", LogLevel.Error);
            }
        }
    }
}
