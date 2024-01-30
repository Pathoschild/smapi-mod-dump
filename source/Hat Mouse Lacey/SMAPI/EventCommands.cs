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
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ichortower_HatMouseLacey
{
    internal class LCEventCommands
    {
        /*
         * Queue holding viewport move targets.
         * LC_viewportMoveQueue uses this to allow chaining of viewport move
         * commands.
         */
        private static Queue<Vector3> viewportQueue = new Queue<Vector3>();

        /*
         * Map from NPC names to queued warp coordinates.
         * LC_moveWarpQueue uses this to tell NPCs to warp when they are done
         * moving.
         */
        private static Dictionary<string, Vector2> warpQueues = new Dictionary<string, Vector2>();

        /*
         * An UpdateTicked event listener, used by the queueing commands and
         * the gradual sunset to run background tasks.
         * It is created and registered only when needed, then removed when
         * nothing is left to do (or when the event ends).
         */
        private static System.EventHandler<UpdateTickedEventArgs> queueTicker = null!;


        /*
         * Starts the background ticker (UpdateTicked listener) if it's not
         * already running.
         * For brevity's sake, this function includes the ticker delegate
         * itself. That function checks the assorted background items
         * (viewport queue, warp queues, sunset timer) and updates them as
         * needed.
         * If there's nothing left to monitor, the listener is unregistered.
         */
        public static void startTicker()
        {
            if (queueTicker != null) {
                return;
            }
            queueTicker = delegate(object sender, UpdateTickedEventArgs e) {
                StardewValley.Event theEvent = Game1.CurrentEvent;
                if (viewportQueue.Count > 0) {
                    FieldInfo targetField = typeof(StardewValley.Event).GetField(
                            "viewportTarget", BindingFlags.NonPublic | BindingFlags.Instance);
                    Vector3 currentTarget = (Vector3)targetField.GetValue(theEvent);
                    if (currentTarget.Equals(Vector3.Zero)) {
                        Vector3 item = viewportQueue.Dequeue();
                        targetField.SetValue(theEvent, item);
                    }
                }
                foreach (var entry in warpQueues) {
                    StardewValley.Character actor = theEvent.getCharacterByName(entry.Key);
                    if (actor is null) {
                        warpQueues.Remove(entry.Key);
                        continue;
                    }
                    if (actor.isMoving() || actor.isEmoting) {
                        continue;
                    }
                    if (theEvent.npcControllers != null &&
                            theEvent.npcControllers.Exists(c => c.puppet.Equals(actor))) {
                        continue;
                    }
                    actor.setTileLocation(theEvent.OffsetTile(entry.Value));
                    warpQueues.Remove(entry.Key);
                }
                if (LCSunset.startTime != 0) {
                    ambientSunset_tick(Game1.currentGameTime);
                }
                if (Game1.eventOver || !Game1.eventUp || (viewportQueue.Count == 0 &&
                        warpQueues.Count == 0 && LCSunset.startTime == 0)) {
                    stopTicker();
                }
            };
            ModEntry.HELPER.Events.GameLoop.UpdateTicked += queueTicker;
        }

        /*
         * Unregisters the background ticker. This is a no-op if the ticker
         * isn't running.
         */
        public static void stopTicker()
        {
            if (queueTicker != null) {
                ModEntry.HELPER.Events.GameLoop.UpdateTicked -= queueTicker;
                queueTicker = null;
            }
        }

        /*
         * LC_ambientSunset <milliseconds>
         *
         * Starts a smooth sunset shift in ambientLight which lasts the given
         * number of milliseconds. Runs in the background via the ticker.
         * (see also ambientSunset_tick)
         */
        public static void command_ambientSunset(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            theEvent.CurrentCommand++;
            int ms = 30000;
            if (split.Length >= 2) {
                ms = Convert.ToInt32(split[1]);
            }
            LCSunset.startTime = (int)time.TotalGameTime.TotalMilliseconds;
            LCSunset.runTime = ms;
            LCSunset.currentTime = 0;
            LCSunset.ambient[0,0] = Game1.ambientLight.R;
            LCSunset.ambient[0,1] = Game1.ambientLight.G;
            LCSunset.ambient[0,2] = Game1.ambientLight.B;
            startTicker();
        }

        /*
         * LC_crueltyScore <int>
         *
         * Add points to the player's accrued "cruelty score".
         * This is a hidden counter which can cause a temporary punishment
         * and trigger a bonus event.
         * You can use a large negative value to reset it (it clamps to zero).
         */
        public static void command_crueltyScore(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            int n = 0;
            if (split.Length >= 2) {
                n = Convert.ToInt32(split[1]);
            }
            if (n != 0) {
                if (n < 0 && (-1*n) > LCModData.CrueltyScore) {
                    LCModData.CrueltyScore = 0;
                }
                else {
                    LCModData.CrueltyScore += n;
                }
            }
            theEvent.CurrentCommand++;
        }

        /*
         * LC_drawOnTop <NPC> [false]
         *
         * Set the named NPC's 'drawOnTop' field, which causes them to render
         * above objects and other actors during scene draws.
         * Specify 'false' to unset the flag. Absence or any other value will
         * set it to true.
         */
        public static void command_drawOnTop(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            bool top = true;
            if (split.Length >= 2) {
                if (split.Length >= 3 && split[2].Equals("false")) {
                    top = false;
                }
                NPC who = theEvent.getActorByName(split[1]);
                if (who != null) {
                    who.drawOnTop = top;
                }
            }
            theEvent.CurrentCommand++;
        }

        /*
         * LC_forgetThisEvent
         *
         * Avoids flagging this event as seen when it finishes (technically,
         * removes its id from the list of seen events after it is automat-
         * ically added).
         *
         * This adds a callback to onEventFinished, which is run during
         * Game1.eventFinished, which *usually* happens after endBehaviors/
         * exitEvent sets the id as seen. I think it's possible to trigger
         * these out of order, but not in our use case.
         */
        public static void command_forgetThisEvent(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            int id = theEvent.id;
            theEvent.onEventFinished = (Action)Delegate.Combine(
                    theEvent.onEventFinished, new Action(delegate() {
                        Game1.player.eventsSeen.Remove(id);
                    }));
            theEvent.CurrentCommand++;
        }

        /*
         * LC_setDating <NPC>
         *
         * Set the player to be dating the named villager.
         */
        public static void command_setDating(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            Friendship f = Game1.player.friendshipData[split[1]];
            if (f != null && !f.IsDating()) {
                f.Status = FriendshipStatus.Dating;
                NPC n = Game1.getCharacterFromName(split[1], mustBeVillager:true);
                Multiplayer mp = (Multiplayer)typeof(Game1)
                        .GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic)
                        .GetValue(null);
                mp.globalChatInfoMessage("Dating",
                        Game1.player.Name, n.displayName);
            }
            theEvent.CurrentCommand++;
        }

        /*
         * LC_sit <x> <y>
         *
         * Cause the player farmer to sit in the mapSeat at the given (x,y)
         * tile position.
         * Tile must be a seat, must be unoccupied, must be in range.
         */
        public static void command_sit(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            int X = Convert.ToInt32(split[1]);
            int Y = Convert.ToInt32(split[2]);
            foreach (MapSeat chair in location.mapSeats) {
                if (chair.OccupiesTile(X, Y) && !chair.IsBlocked(location)) {
                    theEvent.farmer.CanMove = true;
                    theEvent.farmer.BeginSitting(chair);
                    theEvent.farmer.CanMove = false;
                    break;
                }
            }
            theEvent.CurrentCommand++;
        }

        /*
         * LC_timeAfterFade hhmm(int)
         *
         * Set a time of day to advance to after the current event ends.
         * Works like the festival time skips (machines process, etc.), with
         * one major difference: it doesn't put player spouses to bed unless
         * the target time is 10 pm or later. Other NPCs get reset to their
         * default locations, but that's a cheat since I only use 9pm, 10pm,
         * and 11pm as target times.
         */
        public static void command_timeAfterFade(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            theEvent.CurrentCommand++;
            int targetTime = 0;
            if (split.Length >= 2) {
                targetTime = Convert.ToInt32(split[1]);
            }
            if (targetTime <= Game1.timeOfDay) {
                return;
            }
            int timePass = Utility.CalculateMinutesBetweenTimes(Game1.timeOfDay, targetTime);
            Game1.timeOfDayAfterFade = targetTime;

            /*
             * Most of this copied from Event.exitEvent (the festival time-
             * advancing code), with minor edits.
             */
            foreach (NPC person in theEvent.actors) {
                if (person != null) {
                    theEvent.resetDialogueIfNecessary(person);
                }
            }
            foreach (GameLocation loc in Game1.locations) {
                foreach (Vector2 position in new List<Vector2>(loc.objects.Keys)) {
                    if (loc.objects[position].minutesElapsed(timePass, loc)) {
                        loc.objects.Remove(position);
                    }
                }
                if (loc is Farm) {
                    (loc as Farm).timeUpdate(timePass);
                }
            }
            if (!Game1.IsMasterGame) {
                return;
            }
            foreach (NPC person in Utility.getAllCharacters()) {
                if (!person.isVillager()) {
                    continue;
                }
                Farmer spouseFarmer = person.getSpouse();
                if (spouseFarmer != null && spouseFarmer.isMarried()) {
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
                    else if (targetTime >= 1800) {
                        person.currentMarriageDialogue.Clear();
                        person.checkForMarriageDialogue(1800, home);
                    }
                    else if (targetTime >= 1100) {
                        person.currentMarriageDialogue.Clear();
                        person.checkForMarriageDialogue(1100, home);
                    }
                    continue;
                }
                if ( person.currentLocation != null && person.DefaultMap != null) {
                    person.doingEndOfRouteAnimation.Value = false;
                    person.nextEndOfRouteMessage = null;
                    person.endOfRouteMessage.Value = null;
                    person.controller = null;
                    person.temporaryController = null;
                    person.Halt();
                    Game1.warpCharacter(person, person.DefaultMap,
                            person.DefaultPosition / 64f);
                    person.ignoreScheduleToday = true;
                }
            }
        }

        /*
         * LC_unsit
         *
         * Cause the player farmer to stop sitting on their seat.
         */
        public static void command_unsit(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            theEvent.farmer.CanMove = true;
            theEvent.farmer.StopSitting(true);
            theEvent.farmer.CanMove = false;
            theEvent.CurrentCommand++;
        }

        /*
         * LC_viewportMoveQueue <xspeed> <yspeed> <milliseconds>
         *
         * Works just like "viewport move", but queues the move instead of
         * overwriting any current one. This lets you chain moves while e.g.
         * dialogue occurs (player input makes it impossible to time).
         */
        public static void command_viewportMoveQueue(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            FieldInfo targetField = typeof(StardewValley.Event)
                    .GetField("viewportTarget", BindingFlags.NonPublic | BindingFlags.Instance);
            Vector3 privTarget = (Vector3)targetField.GetValue(theEvent);
            Vector3 newTarget = new Vector3(Convert.ToInt32(split[1]),
                    Convert.ToInt32(split[2]), Convert.ToInt32(split[3]));
            theEvent.CurrentCommand++;
            if (privTarget.Equals(Vector3.Zero)) {
                targetField.SetValue(theEvent, newTarget);
                return;
            }
            viewportQueue.Enqueue(newTarget);
            startTicker();
        }

        /*
         * LC_waitForMovement <actor> [<actor>...]
         *
         * Like waitForAllStationary, except it takes any number of actor
         * names, and waits only for those actors to stop. Other actors
         * may continue moving.
         *   e.g. 'LC_waitForStationary farmer Emily' waits only for
         *     farmer and Emily before proceeding.
         *
         * There is one more subtle difference: waitForAllStationary only
         * checks .isMoving(), which returns false during an advancedMove
         * pause step. This command makes sure there is also no NPCController
         * active on the character, to allow such pauses.
         */
        public static void command_waitForMovement(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            bool wait = false;
            for (int i = 1; i < split.Length; ++i) {
                StardewValley.Character actor = theEvent.getCharacterByName(split[i]);
                if (actor is null) {
                    continue;
                }
                if (actor.isMoving() || actor.isEmoting) {
                    wait = true;
                    break;
                }
                if (theEvent.npcControllers != null &&
                        theEvent.npcControllers.Exists(c => c.puppet.Equals(actor))) {
                    wait = true;
                    break;
                }
            }
            if (!wait) {
                theEvent.CurrentCommand++;
            }
        }

        /*
         * LC_warpQueue <actor> <x> <y>
         *
         * Wait for an actor to stop moving, then warp them to coordinates
         * (x, y). Runs in the background, so the event proceeds after reading
         * this command.
         */
        public static void command_warpQueue(
                GameLocation location, GameTime time, string[] split)
        {
            StardewValley.Event theEvent = Game1.CurrentEvent;
            StardewValley.Character actor = theEvent.getCharacterByName(split[1]);
            if (actor != null) {
                warpQueues[split[1]] = new Vector2(
                        Convert.ToInt32(split[2]), Convert.ToInt32(split[3]));
                startTicker();
            }
            theEvent.CurrentCommand++;
        }



        /* === other, non-command functions === */

        /*
         * this function is the actual logic of the gradual sunset.
         * works out where we are along the runtime, then calculates what the
         * ambientLight value should be at that time based on the three key-
         * frames in the LCSunset class.
         */
        private static void ambientSunset_tick(GameTime time)
        {
            if (!Program.gamePtr.IsActive) {
                return;
            }
            int ms = time.ElapsedGameTime.Milliseconds;
            LCSunset.currentTime += ms;
            if (LCSunset.currentTime >= LCSunset.runTime) {
                Game1.ambientLight = new Color(LCSunset.ambient[3,0],
                        LCSunset.ambient[3,1], LCSunset.ambient[3,2]);
                LCSunset.startTime = 0;
                LCSunset.runTime = 0;
                LCSunset.currentTime = 0;
                return;
            }
            int cut1 = LCSunset.runTime/ 4;
            int cut2 = 2 * LCSunset.runTime / 3;
            int start = 0;
            int end = 1;
            int percent = 0;
            if (LCSunset.currentTime < cut1) {
                percent = LCSunset.currentTime * 100 / cut1;
            }
            else if (LCSunset.currentTime < cut2) {
                start = 1;
                end = 2;
                percent = (LCSunset.currentTime - cut1) * 100 / (cut2 - cut1);
            }
            else {//if (LCSunset.currentTime < LCSunset.runTime)
                start = 2;
                end = 3;
                percent = (LCSunset.currentTime - cut2) * 100 / (LCSunset.runTime - cut2);
            }
            var c = LCSunset.ambient;
            int r = c[start,0] + ((c[end,0] - c[start,0]) * percent) / 100;
            int g = c[start,1] + ((c[end,1] - c[start,1]) * percent) / 100;
            int b = c[start,2] + ((c[end,2] - c[start,2]) * percent) / 100;
            Game1.ambientLight = new Color(r, g, b);
        }
    }

    internal class LCSunset
    {
        public static int[,] ambient = {
            {  0,   0,   0},
            {  0,  20, 120},
            { 30, 120, 120},
            {120, 120,  90}
        };
        public static int startTime = 0;
        public static int runTime = 0;
        public static int currentTime = 0;
    }
}
