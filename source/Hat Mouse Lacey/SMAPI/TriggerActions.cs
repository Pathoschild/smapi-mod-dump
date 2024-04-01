/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Locations;
using StardewValley.Pathfinding;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ichortower_HatMouseLacey
{
    internal class LCActions
    {
        public static void Register()
        {
            MethodInfo[] funcs = typeof(LCActions).GetMethods(
                    BindingFlags.Public | BindingFlags.Static);
            foreach (var func in funcs) {
                if (!func.Name.StartsWith("action_")) {
                    continue;
                }
                string key = func.Name.Replace("action_", $"{HML.CPId}_");
                TriggerActionManager.RegisterAction(key,
                        (TriggerActionDelegate) Delegate.CreateDelegate(
                        typeof(TriggerActionDelegate), func));
            }
        }


        /*
         * Advance the day to the specified time (hhmm, 600-2600). Works like
         * the festival time skip: machines and objects process, etc.
         * NPCs get warped along their schedule to where they should be, so
         * they can resume correctly.
         * Spouses with no schedule will get warped to bed if the target time
         * is after 2200 (10 pm).
         *
         * If an event is currently playing, the time advance code runs as
         * normal, but the time isn't set immediately; instead, this sets
         * Game1.timeOfDayAfterFade, so it is set when the event ends.
         *
         * Aborts without doing anything in multiplayer contexts: this is
         * intended to be used by events, and events don't freeze time in
         * multiplayer, so there's no need to force time to pass.
         */
        public static bool action_AdvanceTime(string[] args,
                TriggerActionContext context,
                out string error)
        {
            int targetTime = 0;
            if (!ArgUtility.TryGetInt(args, 1, out targetTime, out error)) {
                return false;
            }
            if (targetTime <= Game1.timeOfDay) {
                error = "Target time is not in the future. " +
                    $"({targetTime} <= {Game1.timeOfDay})";
                return false;
            }
            if (targetTime >= 2600) {
                error = $"Target time is too late. ({targetTime} >= 2600)";
                return false;
            }
            // ignore completely unless in single-player mode
            if (Game1.multiplayerMode != Game1.singlePlayer) {
                Log.Info($"Ignoring AdvanceTime: game is not in single-player mode.");
                error = null;
                return true;
            }

            int timePass = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, targetTime);
            if (Game1.eventUp) {
                Game1.timeOfDayAfterFade = targetTime;
            }
            else {
                Game1.timeOfDay = targetTime;
            }
            // advance time for machines and other objects
            foreach (GameLocation loc in Game1.locations) {
                foreach (Vector2 position in new List<Vector2>(loc.objects.Keys)) {
                    if (loc.objects[position].minutesElapsed(timePass)) {
                        loc.objects.Remove(position);
                    }
                }
                (loc as Farm)?.timeUpdate(timePass);
            }

            // advance time for NPCs
            foreach (NPC person in Utility.getAllVillagers()) {
                if (person.IsInvisible) {
                    continue;
                }
                // spouses (sometimes) and certain NPCs have null schedules.
                // null-schedule spouses should go to bed after 10 pm
                if (person.Schedule is null) {
                    Farmer spouseFarmer = person.getSpouse();
                    if (spouseFarmer != null && spouseFarmer.isMarriedOrRoommates()) {
                        FarmHouse home = Utility.getHomeOfFarmer(spouseFarmer);
                        if (targetTime >= 2200) {
                            person.controller = null;
                            person.temporaryController = null;
                            person.Halt();
                            Game1.warpCharacter(person, home,
                                    Utility.PointToVector2(home.getSpouseBedSpot(spouseFarmer.spouse)));
                            if (home.GetSpouseBed() != null) {
                                FarmHouse.spouseSleepEndFunction(person, home);
                            }
                            person.ignoreScheduleToday = true;
                        }
                        // for earlier times, refresh marriage dialogue
                        if (targetTime >= 1800) {
                            person.currentMarriageDialogue.Clear();
                            person.checkForMarriageDialogue(1800, home);
                        }
                        else if (targetTime >= 1100) {
                            person.currentMarriageDialogue.Clear();
                            person.checkForMarriageDialogue(1100, home);
                        }
                    }
                    continue;
                }
                // find the last schedule entry preceding the target time
                // (or the last entry if it's already too late)
                SchedulePathDescription target = null;
                foreach (var entry in person.Schedule.Values) {
                    if (entry.time > targetTime) {
                        break;
                    }
                    target = entry;
                }
                if (target != null) {
                    // remove controllers from NPC so they don't keep walking
                    person.controller = null;
                    person.temporaryController = null;
                    person.DirectionsToNewLocation = null;
                    // synchronously end the current route animation. to do
                    // this, we have to call finishRouteBehavior (if needed),
                    // THEN zero out the closing animation and tell the route
                    // behavior to stop (if zeroed, finishRouteBehavior won't
                    // be called, since routeEndAnimationFinished, which normally
                    // runs after it, clears the field it checks).
                    FieldInfo seorb = typeof(NPC).GetField(
                            "_startedEndOfRouteBehavior",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                    string behavior = (string)seorb.GetValue(person);
                    if (behavior != null) {
                        MethodInfo frb = typeof(NPC).GetMethod(
                                "finishRouteBehavior",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                        frb.Invoke(person, new object[]{behavior});
                    }
                    FieldInfo reo = typeof(NPC).GetField("routeEndOutro",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                    reo.SetValue(person, new int[]{});
                    person.EndActivityRouteEndBehavior();
                    // move character to destination. face direction, then
                    // force sprite to face that direction for real (resets
                    // frame to neutral standing pose).
                    Game1.warpCharacter(person,
                            target.targetLocationName,
                            target.targetTile);
                    person.faceDirection(target.facingDirection);
                    person.Sprite.faceDirectionStandard(target.facingDirection);
                    // activate route behavior (e.g. animation). manually set
                    // the route message if present, since we are bypassing the
                    // PathFindController, which usually does it.
                    person.StartActivityRouteEndBehavior(
                            target.endOfRouteBehavior,
                            target.endOfRouteMessage);
                    person.endOfRouteMessage.Value = person.nextEndOfRouteMessage
                            ?.Replace("\"", "");
                }
            }

            error = null;
            return true;
        }


        /*
         * Warp the player out of the current location.
         * Works like the event command 'end warpOut' (finds the first valid
         * exit and follows it).
         *
         * If an event is running, this sets the event's exit position
         * instead of warping immediately.
         */
        public static bool action_WarpOut(string[] args,
                TriggerActionContext context,
                out string error)
        {
            Warp w = Game1.player.currentLocation.GetFirstPlayerWarp();
            if (Game1.eventUp && Game1.CurrentEvent != null) {
                Game1.CurrentEvent.setExitLocation(w);
            }
            else {
                LocationRequest req = Game1.getLocationRequest(w.TargetName);
                Game1.warpFarmer(req, (int)w.TargetX, (int)w.TargetY,
                        Game1.player.FacingDirection);
            }
            error = null;
            return true;
        }

        /*
         * Warp the player home, to the front porch of their house.
         *
         * If an event is running, this sets the event's exit position
         * instead of warping immediately.
         */
        public static bool action_WarpHome(string[] args,
                TriggerActionContext context,
                out string error)
        {
            Point porch;
            if (Game1.IsMasterGame) {
                porch = Game1.getFarm().GetMainFarmHouseEntry();
            }
            else {
                porch = Utility.getHomeOfFarmer(Game1.player).getPorchStandingSpot();
            }

            if (Game1.eventUp && Game1.CurrentEvent != null) {
                Game1.CurrentEvent.setExitLocation("Farm", porch.X, porch.Y);
            }
            else {
                LocationRequest req = Game1.getLocationRequest("Farm");
                Game1.warpFarmer(req, (int)porch.X, (int)porch.Y, 2);
            }
            error = null;
            return true;
        }
    }
}
