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
using StardewValley.Delegates;
using StardewValley.Locations;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ichortower_HatMouseLacey
{
    internal class LCEventCommands
    {
        /*
         * Queue holding viewport move targets.
         * viewportMoveQueue uses this to allow chaining of viewport move
         * commands.
         */
        private static Queue<Vector3> viewportQueue = new Queue<Vector3>();

        /*
         * Map from NPC names to queued warp coordinates.
         * warpQueue uses this to tell NPCs to warp when they are done moving.
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
         * Registers the event commands.
         */
        public static void Register()
        {
            MethodInfo[] funcs = typeof(LCEventCommands).GetMethods(
                    BindingFlags.Public | BindingFlags.Static);
            foreach (var func in funcs) {
                if (!func.Name.StartsWith("command_")) {
                    continue;
                }
                string key = func.Name.Replace("command_",
                        $"{HML.CPId}_");
                StardewValley.Event.RegisterCommand(key,
                        (EventCommandDelegate) Delegate.CreateDelegate(
                        typeof(EventCommandDelegate), func));
            }
        }

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
            HML.ModHelper.Events.GameLoop.UpdateTicked += queueTicker;
        }

        /*
         * Unregisters the background ticker. This is a no-op if the ticker
         * isn't running.
         */
        public static void stopTicker()
        {
            if (queueTicker != null) {
                HML.ModHelper.Events.GameLoop.UpdateTicked -= queueTicker;
                queueTicker = null;
            }
        }

        /*
         * _ambientSunset <milliseconds>
         *
         * Starts a smooth sunset shift in ambientLight which lasts the given
         * number of milliseconds. Runs in the background via the ticker.
         * (see also ambientSunset_tick)
         */
        public static void command_ambientSunset(
                Event evt, string[] args, EventContext context)
        {
            evt.CurrentCommand++;
            int ms = ArgUtility.GetInt(args, 1, 30000);
            LCSunset.startTime = (int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds;
            LCSunset.runTime = ms;
            LCSunset.currentTime = 0;
            LCSunset.ambient[0,0] = Game1.ambientLight.R;
            LCSunset.ambient[0,1] = Game1.ambientLight.G;
            LCSunset.ambient[0,2] = Game1.ambientLight.B;
            startTicker();
        }

        /*
         * _crueltyScore <int>
         *
         * Add points to the player's accrued "cruelty score".
         * This is a hidden counter which can cause a temporary punishment
         * and trigger a bonus event.
         * You can use a large negative value to reset it (it clamps to zero).
         */
        public static void command_crueltyScore(
                Event evt, string[] args, EventContext context)
        {
            evt.CurrentCommand++;
            int n = ArgUtility.GetInt(args, 1, 0);
            if (n != 0) {
                if (n < 0 && (-1*n) > LCModData.CrueltyScore) {
                    LCModData.CrueltyScore = 0;
                }
                else {
                    LCModData.CrueltyScore += n;
                }
            }
        }

        /*
         * _drawOnTop <NPC> [false]
         *
         * Set the named NPC's 'drawOnTop' field, which causes them to render
         * above objects and other actors during scene draws.
         * Specify 'true' or 'false'. Omitting the flag is the same as 'true'.
         */
        public static void command_drawOnTop(
                Event evt, string[] args, EventContext context)
        {
            evt.CurrentCommand++;
            string npc;
            string err;
            bool top = true;
            if (!ArgUtility.TryGet(args, 1, out npc, out err) ||
                    !ArgUtility.TryGetOptionalBool(args, 2, out top,
                        out err, defaultValue:true)) {
                Log.Warn($"{args[0]} failed to parse: {err}");
                return;
            }
            NPC who = evt.getActorByName(npc);
            if (who != null) {
                who.drawOnTop = top;
            }
        }


        /*
         * _setDating <NPC>
         *
         * Set the player to be dating the named villager.
         */
        public static void command_setDating(
                Event evt, string[] args, EventContext context)
        {
            evt.CurrentCommand++;
            string npc;
            if (!ArgUtility.TryGet(args, 1, out npc, out string err)) {
                Log.Warn($"{args[0]} failed to parse: {err}");
                return;
            }
            Friendship f = Game1.player.friendshipData[npc];
            if (f != null && !f.IsDating()) {
                f.Status = FriendshipStatus.Dating;
                NPC n = Game1.getCharacterFromName(npc, mustBeVillager:true);
                Multiplayer mp = (Multiplayer)typeof(Game1)
                        .GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic)
                        .GetValue(null);
                mp.globalChatInfoMessage("Dating",
                        Game1.player.Name, n.displayName);
            }
        }

        /*
         * _sit <x> <y>
         *
         * Cause the player farmer to sit in the MapSeat at the given (x,y)
         * tile position.
         * Will fail if tile is not a MapSeat.
         * Will *NOT* fail if the seat is occupied! See Patcher for more info
         * (MapSeat__IsBlocked__Postfix).
         */
        public static void command_sit(
                Event evt, string[] args, EventContext context)
        {
            evt.CurrentCommand++;
            int X;
            int Y;
            string err;
            if (!ArgUtility.TryGetInt(args, 1, out X, out err) ||
                    !ArgUtility.TryGetInt(args, 2, out Y, out err)) {
                Log.Warn($"{args[0]} failed to parse: {err}");
                return;
            }
            foreach (MapSeat chair in context.Location.mapSeats) {
                if (chair.OccupiesTile(X, Y)) {
                    evt.farmer.CanMove = true;
                    evt.farmer.BeginSitting(chair);
                    evt.farmer.CanMove = false;
                    return;
                }
            }
            Log.Warn($"{args[0]}: no MapSeat found at ({X},{Y})");
        }


        /*
         * _unsit
         *
         * Cause the player farmer to stop sitting on their seat.
         * I don't use this one, so I left it commented out.
         */
        /*
        public static void command_unsit(
                Event evt, string[] args, EventContext context)
        {
            evt.CurrentCommand++;
            evt.farmer.CanMove = true;
            evt.farmer.StopSitting(true);
            evt.farmer.CanMove = false;
        }
        */

        /*
         * _viewportMoveQueue <xspeed> <yspeed> <milliseconds>
         *
         * Works just like "viewport move", but queues the move instead of
         * overwriting any current one. This lets you chain moves while e.g.
         * dialogue occurs (player input makes it impossible to time).
         */
        public static void command_viewportMoveQueue(
                Event evt, string[] args, EventContext context)
        {
            evt.CurrentCommand++;
            FieldInfo targetField = typeof(StardewValley.Event)
                    .GetField("viewportTarget", BindingFlags.NonPublic | BindingFlags.Instance);
            Vector3 privTarget = (Vector3)targetField.GetValue(evt);
            string err;
            if (!ArgUtility.TryGetInt(args, 1, out int x, out err) ||
                    !ArgUtility.TryGetInt(args, 2, out int y, out err) ||
                    !ArgUtility.TryGetInt(args, 3, out int z, out err)) {
                Log.Warn($"{args[0]} failed to parse: {err}");
                return;
            }

            Vector3 newTarget = new Vector3(x, y, z);
            if (privTarget.Equals(Vector3.Zero)) {
                targetField.SetValue(evt, newTarget);
            }
            else {
                viewportQueue.Enqueue(newTarget);
                startTicker();
            }
        }

        /*
         * _waitForMovement <actor> [<actor>...]
         *
         * Like waitForAllStationary, except it takes any number of actor
         * names, and waits only for those actors to stop. Other actors
         * may continue moving.
         *   e.g. '_waitForMovement farmer Emily' waits only for
         *     farmer and Emily before proceeding.
         *
         * There is one more subtle difference: waitForAllStationary only
         * checks .isMoving(), which returns false during an advancedMove
         * pause step. This command makes sure there is also no NPCController
         * active on the character, to allow such pauses.
         */
        public static void command_waitForMovement(
                Event evt, string[] args, EventContext context)
        {
            bool wait = false;
            for (int i = 1; i < args.Length; ++i) {
                Character actor = evt.getCharacterByName(args[i]);
                if (actor is null) {
                    continue;
                }
                if (actor.isMoving() || actor.isEmoting) {
                    wait = true;
                    break;
                }
                if (evt.npcControllers != null &&
                        evt.npcControllers.Exists(c => c.puppet.Equals(actor))) {
                    wait = true;
                    break;
                }
            }
            if (!wait) {
                evt.CurrentCommand++;
            }
        }

        /*
         * _warpQueue <actor> <x> <y>
         *
         * Wait for an actor to stop moving, then warp them to coordinates
         * (x, y). Runs in the background, so the event proceeds after reading
         * this command.
         */
        public static void command_warpQueue(
                Event evt, string[] args, EventContext context)
        {
            evt.CurrentCommand++;
            string err;
            if (!ArgUtility.TryGet(args, 1, out string name, out err) ||
                    !ArgUtility.TryGetInt(args, 2, out int X, out err) ||
                    !ArgUtility.TryGetInt(args, 3, out int Y, out err)) {
                Log.Warn($"{args[0]} failed to parse: {err}");
                return;
            }
            Character actor = evt.getCharacterByName(name);
            if (actor != null) {
                warpQueues[name] = new Vector2(X, Y);
                startTicker();
            }
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
