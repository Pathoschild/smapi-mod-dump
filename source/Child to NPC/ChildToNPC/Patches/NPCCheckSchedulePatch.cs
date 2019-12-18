using System;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using Netcode;

namespace ChildToNPC.Patches
{
    /* Prefix for checkSchedule
     * This is a mix of code from the original method and my own.
     * I use reflection to access private methods in the NPC class.
     */
    class NPCCheckSchedulePatch
    {
        public static bool Prefix(NPC __instance, int timeOfDay, ref Point ___previousEndPoint, ref string ___extraDialogueMessageToAddThisMorning, ref SchedulePathDescription ___directionsToNewLocation, ref Rectangle ___lastCrossroad, ref NetString ___endOfRouteBehaviorName)
        {
            if (!ModEntry.IsChildNPC(__instance))
                return true;

            __instance.updatedDialogueYet = false;
            ___extraDialogueMessageToAddThisMorning = null;
            if (__instance.ignoreScheduleToday || __instance.Schedule == null)
                return false;

            __instance.Schedule.TryGetValue(__instance.scheduleTimeToTry == 9999999 ? timeOfDay : __instance.scheduleTimeToTry, out SchedulePathDescription schedulePathDescription);

            //If I have curfew, override the normal behavior
            if (ModEntry.Config.DoChildrenHaveCurfew && !__instance.currentLocation.Equals(Game1.getLocationFromName("FarmHouse")))
            {
                //Send child home for curfew
                if(timeOfDay == ModEntry.Config.CurfewTime)
                {
                    object[] pathfindParams = { __instance.currentLocation.Name, __instance.getTileX(), __instance.getTileY(), "BusStop", -1, 23, 3, null, null };
                    schedulePathDescription = ModEntry.helper.Reflection.GetMethod(__instance, "pathfindToNextScheduleLocation", true).Invoke<SchedulePathDescription>(pathfindParams);
                }
                //Ignore scheduled events after curfew
                else if(timeOfDay > ModEntry.Config.CurfewTime)
                {
                    schedulePathDescription = null;
                }
            }

            if (schedulePathDescription == null)
                return false;

            //Normally I would see a IsMarried check here, but FarmHouse may be better?
            //(I think this section is meant for handling when the character is still walking)
            if (!__instance.currentLocation.Equals(Game1.getLocationFromName("FarmHouse")) || !__instance.IsWalkingInSquare || ___lastCrossroad.Center.X / 64 != ___previousEndPoint.X && ___lastCrossroad.Y / 64 != ___previousEndPoint.Y)
            {
                if (!___previousEndPoint.Equals(Point.Zero) && !___previousEndPoint.Equals(__instance.getTileLocationPoint()))
                {
                    if (__instance.scheduleTimeToTry == 9999999)
                        __instance.scheduleTimeToTry = timeOfDay;
                    return false;
                }
            }

            ___directionsToNewLocation = schedulePathDescription;

            //__instance.prepareToDisembarkOnNewSchedulePath();
            ModEntry.helper.Reflection.GetMethod(__instance, "prepareToDisembarkOnNewSchedulePath", true).Invoke(null);

            if (__instance.Schedule == null)
                return false;

            if (___directionsToNewLocation != null && ___directionsToNewLocation.route != null && ___directionsToNewLocation.route.Count > 0 && (Math.Abs(__instance.getTileLocationPoint().X - ___directionsToNewLocation.route.Peek().X) > 1 || Math.Abs(__instance.getTileLocationPoint().Y - ___directionsToNewLocation.route.Peek().Y) > 1) && __instance.temporaryController == null)
            {
                __instance.scheduleTimeToTry = 9999999;
                return false;
            }

            __instance.controller = new PathFindController(___directionsToNewLocation.route, __instance, Utility.getGameLocationOfCharacter(__instance))
            {
                finalFacingDirection = ___directionsToNewLocation.facingDirection,
                //endBehaviorFunction = this.getRouteEndBehaviorFunction(this.directionsToNewLocation.endOfRouteBehavior, this.directionsToNewLocation.endOfRouteMessage)
                endBehaviorFunction = ModEntry.helper.Reflection.GetMethod(__instance, "getRouteEndBehaviorFunction", true).Invoke<PathFindController.endBehavior>(new object[] { __instance.DirectionsToNewLocation.endOfRouteBehavior, __instance.DirectionsToNewLocation.endOfRouteMessage })
            };
            __instance.scheduleTimeToTry = 9999999;

            if (___directionsToNewLocation != null && ___directionsToNewLocation.route != null)
                ___previousEndPoint = ___directionsToNewLocation.route.Count > 0 ? ___directionsToNewLocation.route.Last() : Point.Zero;

            return false;
        }
    }
}