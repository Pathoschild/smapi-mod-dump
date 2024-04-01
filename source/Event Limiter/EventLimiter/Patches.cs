/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/EventLimiter
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using HarmonyLib;

namespace EventLimiter
{
    class Patches
    {
        private static IMonitor monitor;
        private static ModConfig config;
        private static List<string> internalexceptions;
        private static List<string> NormalisedEventids;

        public static void Hook(Harmony harmony, IMonitor monitor, ModConfig config, List<string> internalexceptions, List<string> normalisedeventids)
        {
            Patches.monitor = monitor;
            Patches.config = config;
            Patches.internalexceptions = internalexceptions;
            Patches.NormalisedEventids = normalisedeventids;

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.startEvent)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.startEvent_postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.exitEvent)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Patches.exitEvent_postfix)));

            monitor.Log("Initialised harmony patches...");
        }

        public static void startEvent_postfix(GameLocation __instance, Event evt)
        {
            try
            {
                if (evt.isWedding || evt.isFestival || ModEntry.StoryProgressionEvents.Contains(evt.id) == true)
                {
                    monitor.Log("Current event is important and unskippable!");
                    return;
                }

                else
                {
                    // Check if the event is an exception, skip the rest of the method if so
                    if (NormalisedEventids != null && NormalisedEventids.Contains(evt.id) == true)
                    {
                        monitor.Log("Made exception for event with id " + evt.id);
                        return;
                    }

                    // Check for mod added exceptions, skip the rest of the method if so
                    if (internalexceptions != null && internalexceptions.Contains(evt.id) == true)
                    {
                        monitor.Log("Made mod added exception for event with id " + evt.id);
                        return;
                    }

                    // Check if day limit is reached, skip event if so
                    if (ModEntry.EventCounterDay.Value >= config.EventsPerDay)
                    {
                        monitor.Log("Day limit reached! Skipping event...");
                        Game1.eventUp = false;
                        Game1.displayHUD = true;
                        Game1.player.CanMove = true;
                        __instance.currentEvent = null;
                        return;
                    }

                    // Check if row limit is reached, skip event if so
                    else if (ModEntry.EventCounterRow.Value >= config.EventsInARow)
                    {
                        monitor.Log("Continuous event limit reached! Skipping event...");
                        Game1.eventUp = false;
                        Game1.displayHUD = true;
                        Game1.player.CanMove = true;
                        __instance.currentEvent = null;
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(startEvent_postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void exitEvent_postfix(Event __instance)
        {
            try
            {
                // Exit method if counters shouldn't increment (event exception not counting towards limit)
                if (config.ExemptEventsCountTowardsLimit == false 
                    && (
                        (NormalisedEventids != null && NormalisedEventids.Contains(__instance.id) == true) 
                        || 
                        (internalexceptions != null && internalexceptions.Contains(__instance.id) == true)
                       )
                   )
                {
                    return;
                }

                // Increment counters after a non-hardcoded event is finished
                if (__instance.isWedding == false || __instance.isFestival == false || ModEntry.StoryProgressionEvents.Contains(__instance.id) == false)
                {
                    ModEntry.EventCounterDay.Value++;
                    ModEntry.EventCounterRow.Value++;
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(exitEvent_postfix)}:\n{ex}", LogLevel.Error);
            }

        }
    }
}
