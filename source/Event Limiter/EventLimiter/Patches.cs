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
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using HarmonyLib;

namespace EventLimiter
{
    class Patches
    {
        private static IMonitor monitor;
        private static ModConfig config;
        private static List<int> internalexceptions;

        public static void Hook(Harmony harmony, IMonitor monitor, ModConfig config, List<int> internalexceptions)
        {
            Patches.monitor = monitor;
            Patches.config = config;
            Patches.internalexceptions = internalexceptions;

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
                if (evt.id > 0 && evt.id != 60367 && evt.isFestival == false)
                {
                    // Check if the event is an exception, skip the rest of the method if so
                    if (config.Exceptions != null && config.Exceptions.Count() > 0)
                    {
                        foreach (var exceptionids in config.Exceptions)
                        {
                            if (evt.id.Equals(exceptionids) == true)
                            {
                                monitor.Log("Made exception for event with id " + evt.id);
                                return;
                            }
                        }
                    }

                    // Check for mod added exceptions, skip the rest of the method if so
                    if (internalexceptions != null && internalexceptions.Count() > 0)
                    {
                        foreach (var exceptionids in internalexceptions)
                        {
                            if (evt.id.Equals(exceptionids) == true)
                            {
                                monitor.Log("Made mod added exception for event with id " + evt.id);
                                return;
                            }
                        }
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
                // Increment counters after a non-hardcoded event is finished
                if (__instance.id > 0 && __instance.id != 60367 && __instance.isFestival == false)
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
