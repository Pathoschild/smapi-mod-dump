/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using StardewValley;

using Netcode;


namespace LittleNPCs.Framework.Patches {
    /// <summary>
    /// Prefix for <code>NPC.checkSchedule</code>.
    /// This is a mix of code from the original method and my own.
    /// I use reflection to access private methods in the NPC class.
    /// </summary>
    class NPCCheckSchedulePatch {
        public static bool Prefix(NPC __instance,
                                  int timeOfDay,
                                  ref Point ___previousEndPoint,
                                  ref string ___extraDialogueMessageToAddThisMorning,
                                  ref SchedulePathDescription ___directionsToNewLocation,
                                  ref Rectangle ___lastCrossroad,
                                  ref NetString ___endOfRouteBehaviorName,
                                  ref bool ___returningToEndPoint) {
            if (__instance is not LittleNPC) {
                return true;
            }

            if (__instance.currentScheduleDelay == 0f && __instance.scheduleDelaySeconds > 0f) {
                __instance.currentScheduleDelay = __instance.scheduleDelaySeconds;
            }
            else {
                if (___returningToEndPoint) {
                    return false;
                }

                __instance.updatedDialogueYet = false;
                ___extraDialogueMessageToAddThisMorning = null;
                if (__instance.ignoreScheduleToday || __instance.Schedule is null) {
                    return false;
                }

                SchedulePathDescription value = null;
                if (__instance.lastAttemptedSchedule < timeOfDay) {
                    __instance.lastAttemptedSchedule = timeOfDay;
                    __instance.Schedule.TryGetValue(timeOfDay, out value);
                    if (value is not null) {
                        __instance.queuedSchedulePaths.Add(new KeyValuePair<int, SchedulePathDescription>(timeOfDay, value));
                    }
                    value = null;
                }

                // If I have curfew, override the normal behavior.
                if (ModEntry.config_.DoChildrenHaveCurfew && !__instance.currentLocation.Equals(Game1.getLocationFromName("FarmHouse"))) {
                    // Send child home for curfew.
                    if(timeOfDay == ModEntry.config_.CurfewTime) {
                        object[] pathfindParams = { __instance.currentLocation.Name, __instance.getTileX(), __instance.getTileY(), "BusStop", -1, 23, 3, null, null };
                        value = ModEntry.helper_
                                        .Reflection
                                        .GetMethod(__instance, "pathfindToNextScheduleLocation", true)
                                        .Invoke<SchedulePathDescription>(pathfindParams);
                        __instance.queuedSchedulePaths.Clear();
                        __instance.queuedSchedulePaths.Add(new KeyValuePair<int, SchedulePathDescription>(timeOfDay, value));
                    }
                    value = null;
                }

                if (__instance.controller is not null && __instance.controller.pathToEndPoint is not null && __instance.controller.pathToEndPoint.Count > 0) {
                    return false;
                }

                if (__instance.queuedSchedulePaths.Count > 0 && timeOfDay >= __instance.queuedSchedulePaths[0].Key) {
                    value = __instance.queuedSchedulePaths[0].Value;
                }

                if (value is null) {
                    return false;
                }

                ModEntry.helper_
                        .Reflection
                        .GetMethod(__instance, "prepareToDisembarkOnNewSchedulePath", true)
                        .Invoke(null);

                if (___returningToEndPoint || __instance.temporaryController is not null) {
                    return false;
                }

                __instance.DirectionsToNewLocation = value;
                if (__instance.queuedSchedulePaths.Count > 0) {
                    __instance.queuedSchedulePaths.RemoveAt(0);
                }

                __instance.controller = new PathFindController(__instance.DirectionsToNewLocation.route, __instance, Utility.getGameLocationOfCharacter(__instance)) {
                    finalFacingDirection = __instance.DirectionsToNewLocation.facingDirection,
                    endBehaviorFunction = ModEntry.helper_
                                                  .Reflection
                                                  .GetMethod(__instance, "getRouteEndBehaviorFunction", true)
                                                  .Invoke<PathFindController.endBehavior>(__instance.DirectionsToNewLocation.endOfRouteBehavior, __instance.DirectionsToNewLocation.endOfRouteMessage)
                };

                if (__instance.controller.pathToEndPoint is null || __instance.controller.pathToEndPoint.Count == 0) {
                    if (__instance.controller.endBehaviorFunction is not null) {
                        __instance.controller.endBehaviorFunction(__instance, __instance.currentLocation);
                    }
                    __instance.controller = null;
                }

                if (__instance.DirectionsToNewLocation is not null && __instance.DirectionsToNewLocation.route is not null) {
                    ___previousEndPoint = ((__instance.DirectionsToNewLocation.route.Count > 0) ? __instance.DirectionsToNewLocation.route.Last() : Point.Zero);
                }
            }

            return false;
        }
    }
}
