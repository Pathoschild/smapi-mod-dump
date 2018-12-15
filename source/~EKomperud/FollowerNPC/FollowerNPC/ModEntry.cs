using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace FollowerNPC
{
    public class ModEntry : Mod
    {
        #region Members and Entry Function
        public static ModConfig config;
        public static IMonitor monitor;
        public static IModHelper modHelper;
        public bool spawned;
        public NPC whiteBox;
        public float whiteBoxStraightSpeed;
        public float whiteBoxAnimationSpeed;
        public float whiteBoxFollowThreshold;
        public Vector2 whiteBoxLastMovementDirection;
        public Vector2 whiteBoxLastFrameVelocity;
        public Vector2 whiteBoxLastFramePosition;
        public Vector2 whiteBoxLastFrameMovement;
        public bool whiteBoxMovedLastFrame;
        public bool whiteBoxNeedsWarp;
        public int whiteBoxWarpTimer;
        public aStar whiteBoxAStar;
        public Queue<Vector2> whiteBoxPath;
        public Vector2 whiteBoxPathNode;
        public float whiteBoxPathfindNodeGoalTolerance;
        public bool whiteBoxFollow;
        public Point whiteBoxScheduleCurrentDestinationPoint;
        public string whiteBoxScheduleCurrentDestinationLocation;
        private Vector2 negativeOne = new Vector2(-1, -1);

        public SortedList<string, Dictionary<string,string>> dialogueScripts;
        public Dialogue responseDialogue;
        public Dialogue actionDialogue;
        public SortedList<string, string> npcCompanionDays;
        public Dictionary<string, bool> npcsThatCanHangOut;
        public int yesResponseID;

        public Farmer farmer;
        public Vector2 farmerLastTile;

        public int fullTile;
        public int halfTile;

        public override void Entry(IModHelper helper)
        {
            // Initialize variables //
            config = Helper.ReadConfig<ModConfig>();
            monitor = Monitor;
            modHelper = helper;
            SetNPCCompanionDays();
            InitializeNPCsThatCanHangOut();
            //**********************//

            // Define constants //
            whiteBoxFollowThreshold = 2;
            whiteBoxPathfindNodeGoalTolerance = 5f;
            fullTile = Game1.tileSize;
            halfTile = (int) (Game1.tileSize * 0.5f);
            //**********************//

            // Patch methods //
            HarmonyInstance harmony = HarmonyInstance.Create("Redwood.FollowerNPC");

            Type[] isCollidingPositionTypes0 = new Type[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) };
            Type[] isCollidingPositionTypes1 = new Type[] { typeof(GameLocation), typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) };
            MethodInfo isCollidingPositionOriginal = typeof(GameLocation).GetMethod("isCollidingPosition", isCollidingPositionTypes0);
            MethodInfo isCollidingPositionprefix = typeof(Patches).GetMethod("Prefix", isCollidingPositionTypes1);
            MethodInfo isCollidingPositionpostfix = typeof(Patches).GetMethod("Postfix", isCollidingPositionTypes1);
            harmony.Patch(isCollidingPositionOriginal, new HarmonyMethod(isCollidingPositionprefix), new HarmonyMethod(isCollidingPositionpostfix));

            Type[] updateMovementTypes0 = new Type[] {typeof(GameLocation), typeof(GameTime)};
            Type[] updateMovementTypes1 = new Type[] {typeof(NPC), typeof(GameLocation), typeof(GameTime)};
            MethodInfo updateMovementOriginal = typeof(NPC).GetMethod("updateMovement", updateMovementTypes0);
            MethodInfo updateMovementPrefix = typeof(Patches).GetMethod("Prefix", updateMovementTypes1);
            harmony.Patch(updateMovementOriginal, new HarmonyMethod(updateMovementPrefix), null);
            //**********************//

            // Subscribe to events //
            ControlEvents.KeyReleased += ControlEvents_KeyReleased;
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            GameEvents.QuarterSecondTick += GameEvents_QuarterSecondTick;
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            PlayerEvents.Warped += PlayerEvents_Warped;
            MineEvents.MineLevelChanged += MineEvents_MineLevelChanged;
            MenuEvents.MenuClosed += MenuEvents_MenuClosed;
            TimeEvents.AfterDayStarted += TimeEvents_AfterDayStarted;
            TimeEvents.TimeOfDayChanged += TimeEvents_TimeOfDayChanged;
            //**********************//
        }
        #endregion

        // To-Do: Finish organizing all the code in these into proper helper functions
        #region Event Functions

        // Just used for debug commands
        private void ControlEvents_KeyReleased(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.KeyPressed == Microsoft.Xna.Framework.Input.Keys.L)
            {
                foreach (GameLocation l in Game1.locations)
                    monitor.Log(l.Name);
            }

            //else if (e.KeyPressed == Keys.U && spawned)
            //{
            //    monitor.Log(whiteBox?.currentLocation.Name + " : " + whiteBox?.getTileLocation());
            //}

            //else if (e.KeyPressed == Keys.I)
            //{
            //    monitor.Log(farmer?.currentLocation.Name + " : " + farmer?.getTileLocation());
            //}

        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || !(whiteBox != null) || !(farmer != null))
                return;

            if (whiteBoxFollow)
            {
                PathfindingNodeUpdateCheck();
                MovementAndAnimationUpdate();
            }
        }

        private void GameEvents_QuarterSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || !(whiteBox != null) || !(farmer != null))
                return;

            DelayedWarp();

            if (whiteBoxFollow)
                PathfindingRemakeCheck();
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            farmer = Game1.player;
            spawned = false;
            whiteBox = null;

            InitializeDialogueScripts();
        }

        private void PlayerEvents_Warped(object sender, EventArgsPlayerWarped e)
        {
            if (!Context.IsWorldReady || !spawned || !(whiteBox != null) || !(farmer != null))
                return;

            whiteBoxAStar.gameLocation = farmer.currentLocation;
            if (!farmer.isRidingHorse())
            {
                Game1.warpCharacter(whiteBox, farmer.currentLocation, farmer.getTileLocation());
                handleCompanionLocationSpecificDialogue();
            }
            else
            {
                whiteBoxNeedsWarp = true;
                whiteBoxFollow = false;
                whiteBoxWarpTimer = 4;
            }
            
        }

        private void MineEvents_MineLevelChanged(object sender, EventArgsMineLevelChanged e)
        {
            if (!Context.IsWorldReady || !spawned || !(whiteBox != null) || !(farmer != null))
                return;

            Game1.warpCharacter(whiteBox, farmer.currentLocation, farmer.getTileLocation());
        }

        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            if (e.PriorMenu.GetType() == typeof(DialogueBox))
            {
                DialogueBox db = (e.PriorMenu as DialogueBox);

                Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                if (d != null && d.speaker != null)
                {
                    NPC n = d.speaker;

                    // Push Companion Ask Dialogue
                    if (n.CurrentDialogue.Count == 0 && npcsThatCanHangOut.TryGetValue(n.Name, out bool canHangOut) && canHangOut)
                    {
                        responseDialogue = new Dialogue(dialogueScripts[n.Name]["Companion"], n);
                        yesResponseID = GetYesResponseID(responseDialogue);
                        n.CurrentDialogue.Push(responseDialogue);
                        npcsThatCanHangOut[n.Name] = false;
                    }
                    // Check Companion Ask Dialogue for answer
                    else if (d.Equals(responseDialogue))
                    {
                        responseDialogue.speaker.CurrentDialogue.Push(new Dialogue(dialogueScripts[n.Name]["Companion"], n));
                        if (farmer.DialogueQuestionsAnswered.Contains(yesResponseID))
                        {
                            farmer.DialogueQuestionsAnswered.Remove(yesResponseID);

                            whiteBox = n;
                            whiteBoxAStar = new aStar(farmer.currentLocation, whiteBox.Name);
                            Patches.companion = whiteBox;
                            whiteBoxAnimationSpeed = 10f;
                            whiteBoxFollow = true;
                            spawned = true;
                        }
                        else if (farmer.dialogueQuestionsAnswered.Contains(yesResponseID + 1))
                        {
                            farmer.dialogueQuestionsAnswered.Remove(yesResponseID + 1);
                        }
                        responseDialogue = null;
                    }
                    
                    // Check Companion Actions Dialogue
                    else if (d.Equals(actionDialogue))
                    {
                        if (farmer.DialogueQuestionsAnswered.Contains(yesResponseID))
                        {
                            whiteBoxFollow = false;
                            spawned = false;
                            farmer.DialogueQuestionsAnswered.Remove(yesResponseID);
                            whiteBoxAStar = null;

                            whiteBox.Schedule = GetWhiteBoxSchedule(Game1.dayOfMonth);
                            Game1.warpCharacter(whiteBox, whiteBoxScheduleCurrentDestinationLocation,
                                whiteBoxScheduleCurrentDestinationPoint);
                            typeof(NPC).GetField("previousEndPoint", BindingFlags.NonPublic | BindingFlags.Instance)
                                .SetValue(whiteBox, whiteBox.getTileLocationPoint());
                            whiteBox.checkSchedule(Game1.timeOfDay);
                            // Set end of route behavior, message, and facingDirection

                            whiteBox = null;
                            Patches.companion = null;
                        }
                        else if (farmer.dialogueQuestionsAnswered.Contains(yesResponseID + 1))
                        {
                            farmer.dialogueQuestionsAnswered.Remove(yesResponseID + 1);
                        }
                    }
                }
            }
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            ResetNPCsThatCanHangOut();
        }

        private void TimeEvents_TimeOfDayChanged(object sender, EventArgsIntChanged e)
        {
            if (!Context.IsWorldReady || !spawned || !(whiteBox != null) || !(farmer != null))
                return;

            PrepareCompanionActionDialogue();
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Remakes the white box's path if the farmer has changed tiles since the last time
        /// this function was called. (Every quarter second as of right now)
        /// </summary>
        private void PathfindingRemakeCheck()
        {
            Vector2 farmerCurrentTile = farmer.getTileLocation();

            if (farmerLastTile != farmerCurrentTile)
            {
                whiteBoxPath = whiteBoxAStar.Pathfind(whiteBox.getTileLocation(), farmerCurrentTile);
                if (whiteBox.getTileLocation() != whiteBoxPathNode)
                    whiteBoxPathNode = whiteBoxPath != null && whiteBoxPath.Count != 0 ? whiteBoxPath.Dequeue() : negativeOne;
            }

            farmerLastTile = farmerCurrentTile;
        }

        /// <summary>
        /// Iterates to the next goal node in the white box's current path if the current
        /// goal node has been reached since the last time this function was called.
        /// (Every 1/60 of a second as of right now)
        /// </summary>
        private void PathfindingNodeUpdateCheck()
        {
            if (whiteBoxPathNode != negativeOne && whiteBoxPath != null)
            {
                Point w = whiteBox.GetBoundingBox().Center;
                Point n = new Point(((int)whiteBoxPathNode.X * fullTile) + halfTile, ((int)whiteBoxPathNode.Y * Game1.tileSize) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                float nodeDiffLen = nodeDiff.Length();
                if (nodeDiffLen <= whiteBoxPathfindNodeGoalTolerance)
                {
                    if (whiteBoxPath.Count == 0)
                    {
                        whiteBoxPath = null;
                        whiteBoxPathNode = negativeOne;
                        return;
                    }
                    whiteBoxPathNode = whiteBoxPath.Dequeue();
                    n = new Point(((int)whiteBoxPathNode.X * fullTile) + halfTile, ((int)whiteBoxPathNode.Y * Game1.tileSize) + halfTile);
                    nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                    nodeDiffLen = nodeDiff.Length();
                }
            }
        }

        /// <summary>
        /// Provides updates to the white box's movement and animation.
        /// </summary>
        private void MovementAndAnimationUpdate()
        {
            Vector2 whiteBoxBoundingBox =
                new Vector2(whiteBox.GetBoundingBox().Center.X, whiteBox.GetBoundingBox().Center.Y);
            whiteBoxLastFrameMovement = whiteBoxBoundingBox - whiteBoxLastFramePosition;

            Point f = farmer.GetBoundingBox().Center;
            Point w = whiteBox.GetBoundingBox().Center;
            Vector2 diff = new Vector2(f.X, f.Y) - new Vector2(w.X, w.Y);
            float diffLen = diff.Length();
            if (diffLen > Game1.tileSize * whiteBoxFollowThreshold && whiteBoxPathNode != negativeOne)
            {
                Point n = new Point(((int)whiteBoxPathNode.X * fullTile) + halfTile, ((int)whiteBoxPathNode.Y * fullTile) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                float nodeDiffLen = nodeDiff.Length();
                if (nodeDiffLen <= whiteBoxPathfindNodeGoalTolerance)
                    return;
                nodeDiff /= nodeDiffLen;

                whiteBox.xVelocity = nodeDiff.X * farmer.getMovementSpeed();
                whiteBox.yVelocity = -nodeDiff.Y * farmer.getMovementSpeed();
                if (whiteBox.xVelocity != 0 && whiteBox.yVelocity != 0)
                {
                    whiteBox.xVelocity *= 1.25f;
                    whiteBox.yVelocity *= 1.25f;
                }
                HandleWallSliding();
                whiteBoxLastFrameVelocity = new Vector2(whiteBox.xVelocity, whiteBox.yVelocity);
                whiteBoxLastFramePosition = new Vector2(whiteBox.GetBoundingBox().Center.X, whiteBox.GetBoundingBox().Center.Y);

                whiteBox.faceDirection(GetFacingDirectionFromMovement(nodeDiff));
                SetMovementDirectionAnimation(whiteBox.FacingDirection);
                whiteBox.MovePosition(Game1.currentGameTime, Game1.viewport, whiteBox.currentLocation);
                whiteBoxLastMovementDirection = nodeDiff;

                whiteBoxMovedLastFrame = true;
            }
            else if (whiteBoxMovedLastFrame)
            {
                whiteBox.Halt();
                whiteBox.Sprite.faceDirectionStandard(GetFacingDirectionFromMovement(whiteBoxLastMovementDirection));
                whiteBoxMovedLastFrame = false;
            }
        }

        /// <summary>
        /// Used when the farmer changes locations while riding a horse. The Companion will
        /// warp to the tile location the farmer was in the previous map otherwise.
        /// </summary>
        private void DelayedWarp()
        {
            if (whiteBoxNeedsWarp)
                if (--whiteBoxWarpTimer <= 0)
                {
                    whiteBoxFollow = true;
                    Game1.warpCharacter(whiteBox, farmer.currentLocation, farmer.getTileLocation());
                    whiteBoxNeedsWarp = false;
                    handleCompanionLocationSpecificDialogue();
                }
        }

        /// <summary>
        /// Returns an int 0, 1, 2, or 3, representing the direction the Companion should face 
        /// based on their current velocity. North is 0, and the rest follow clockwise.
        /// </summary>
        private int GetFacingDirectionFromMovement(Vector2 movement)
        {
            int dir = 2;
            if (Math.Abs(movement.X) > Math.Abs(movement.Y))
                dir = movement.X > 0 ? 1 : 3;
            else if (Math.Abs(movement.X) < Math.Abs(movement.Y))
                dir = movement.Y > 0 ? 2 : 0;
            return dir;
        }

        /// <summary>
        /// Sets the proper animation for the Companion based on their direction.
        /// </summary>
        private void SetMovementDirectionAnimation(int dir)
        {
            switch (dir)
            {
                case 0:
                    whiteBox.SetMovingOnlyUp();
                    whiteBox.Sprite.AnimateUp(Game1.currentGameTime, (int)(whiteBoxStraightSpeed * -whiteBoxAnimationSpeed), ""); break;
                case 1:
                    whiteBox.SetMovingOnlyRight();
                    whiteBox.Sprite.AnimateRight(Game1.currentGameTime, (int)(whiteBoxStraightSpeed * -whiteBoxAnimationSpeed), ""); break;
                case 2:
                    whiteBox.SetMovingOnlyDown();
                    whiteBox.Sprite.AnimateDown(Game1.currentGameTime, (int)(whiteBoxStraightSpeed * -whiteBoxAnimationSpeed), ""); break;
                case 3:
                    whiteBox.SetMovingOnlyLeft();
                    whiteBox.Sprite.AnimateLeft(Game1.currentGameTime, (int)(whiteBoxStraightSpeed * -whiteBoxAnimationSpeed), ""); break;

            }
        }

        /// <summary>
        /// Allows the Companion to "slide" along walls instead of getting stuck on them
        /// </summary>
        private void HandleWallSliding()
        {
            if (whiteBoxLastFrameVelocity != Vector2.Zero && whiteBoxLastFrameMovement == Vector2.Zero &&
                (whiteBox.xVelocity != 0 || whiteBox.yVelocity != 0))
            {
                Rectangle wbBB = whiteBox.GetBoundingBox();
                int ts = Game1.tileSize;

                if (whiteBox.xVelocity != 0)
                {
                    int velocitySign = Math.Sign(whiteBox.xVelocity) * 5;
                    int leftOrRight = ((whiteBox.xVelocity > 0 ? wbBB.Right : wbBB.Left) + velocitySign) / ts;
                    bool[] xTiles = new bool[3];
                    xTiles[0] = whiteBoxAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Top / ts));
                    xTiles[1] = whiteBoxAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Center.Y / ts));
                    xTiles[2] = whiteBoxAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Bottom / ts));
                    foreach (bool b in xTiles)
                    {
                        if (!b)
                            whiteBox.xVelocity = 0;
                    }
                }

                if (whiteBox.yVelocity != 0)
                {
                    int velocitySign = Math.Sign(whiteBox.yVelocity) * 5;
                    int topOrBottom = ((whiteBox.yVelocity < 0 ? wbBB.Bottom : wbBB.Top) - velocitySign) / ts;
                    bool[] yTiles = new bool[3];
                    yTiles[0] = whiteBoxAStar.IsWalkableTile(new Vector2(wbBB.Left / ts, topOrBottom));
                    yTiles[1] = whiteBoxAStar.IsWalkableTile(new Vector2(wbBB.Center.X / ts, topOrBottom));
                    yTiles[2] = whiteBoxAStar.IsWalkableTile(new Vector2(wbBB.Right / ts, topOrBottom));
                    foreach (bool b in yTiles)
                    {
                        if (!b)
                            whiteBox.yVelocity = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if 'a' and 'b' are less than 'threshold' in difference.
        /// </summary>
        private bool FloatsApproximatelyEqual(float a, float b, float threshold)
        {
            return Math.Abs(a - b) < threshold;
        }

        /// <summary>
        /// Initializes and fills the SortedList which determines which day each NPC 
        /// can hang out with the farmer.
        /// </summary>
        private void SetNPCCompanionDays()
        {
            npcCompanionDays = new SortedList<string, string>();
            npcCompanionDays.Add("Abigail", "Wed"); // Goes around town M/F when married. Plays flute on Tu/Sat. Doc or Seb on Thu.       Wed || Sun
            npcCompanionDays.Add("Alex", "Fri"); // Hangs with Haley on Wed. Doc on Tu. Visits parents on Mo when married.                Thu || Fri || Sat || Sun
            npcCompanionDays.Add("Elliott", "Sat"); // Goes to beach M when married. Doc on Tue. Shops on Thu. Beach on Fri/Sun.          Wed || Sat
            npcCompanionDays.Add("Emily", "Sun"); // Works Mon/Wed/Fri/Sat. Works out Tue. Doc on Thu.                                    Sun
            npcCompanionDays.Add("Haley", "Fri"); // Photography on Mon. Doc on Tue.                                                      Wed || Thu || Fri || Sat || Sun
            npcCompanionDays.Add("Harvey", "Sat"); // Working Mon/Tue/Wed/Thu/Fri/Sun                                                     Sat
            npcCompanionDays.Add("Leah", "Thu"); // Groceries on Mon. Doc on Tue. Saloon on Fri/Sat.                                      Wed || Thu || Sun
            npcCompanionDays.Add("Maru", "Wed"); // Visits parents on Mon when married. Work on Tue/Thu. Personal work on Sat.            Wed || Fri || Sun
            npcCompanionDays.Add("Penny", "Mon"); // Out in town Mon when married. Teaches Tue/Wed/Fri. Babysits Sat. Doc on Thu.         Mon || Thu || Sun
            npcCompanionDays.Add("Sam", "Tue"); // Works Mon/Wed. Saloon on Friday. Hangs with Seb on Sat.                                Tue || Thu || Sun
            npcCompanionDays.Add("Sebastian", "Tue"); // Visits parents Mon. Hangs with Abi on Thu. Saloon on Fri. Hangs with Sam on Sat. Tue || Wed || Sun
            npcCompanionDays.Add("Shane", "Sun"); // Works Mon/Tue/Wed/Thu/Fri. Groceries on Sat.                                         Sun
        }

        /// <summary>
        /// Initializes and fills the Dictionary which determines which NPC's can currently
        /// hang out with the farmer.
        /// </summary>
        private void InitializeNPCsThatCanHangOut()
        {
            npcsThatCanHangOut = new Dictionary<string, bool>();
            npcsThatCanHangOut.Add("Abigail", false);
            npcsThatCanHangOut.Add("Alex", false);
            npcsThatCanHangOut.Add("Elliott", false);
            npcsThatCanHangOut.Add("Emily", false);
            npcsThatCanHangOut.Add("Haley", false);
            npcsThatCanHangOut.Add("Harvey", false);
            npcsThatCanHangOut.Add("Leah", false);
            npcsThatCanHangOut.Add("Maru", false);
            npcsThatCanHangOut.Add("Penny", false);
            npcsThatCanHangOut.Add("Sam", false);
            npcsThatCanHangOut.Add("Sebastian", false);
            npcsThatCanHangOut.Add("Shane", false);
        }

        /// <summary>
        /// Sets NPC's availabilities to hang out to true if they are specified to be able to
        /// on this day of the week via the SortedList.
        /// </summary>
        private void ResetNPCsThatCanHangOut()
        {
            string dayOfWeek = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            foreach (KeyValuePair<string, string> kvp in npcCompanionDays)
            {
                if (kvp.Value.Equals(dayOfWeek))
                    npcsThatCanHangOut[kvp.Key] = true;
            }
        }

        /// <summary>
        /// Loads each NPC's companion dialogue script and adds it to their dialogue field
        /// </summary>
        private void InitializeDialogueScripts()
        {
            string[] characterNames =
            {
                "Abigail", "Alex", "Elliott", "Emily", "Haley", "Harvey", "Leah", "Maru", "Penny", "Sam", "Sebastian",
                "Shane"
            };
            dialogueScripts = new SortedList<string, Dictionary<string, string>>();
            foreach (string characterName in characterNames)
            {
                dialogueScripts.Add(characterName,
                    modHelper.Content.Load<Dictionary<string, string>>("assets/"+characterName+"Companion.json",
                        ContentSource.ModFolder));
                NPC character = Game1.getCharacterFromName(characterName);
                foreach (KeyValuePair<string, string> dialogueKVP in dialogueScripts[characterName])
                    character.Dialogue.Add(dialogueKVP.Key, dialogueKVP.Value);
            }
        }

        /// <summary>
        /// Returns the dialogue response ID of a farmer saying that they want to hang out
        /// from the parameter Dialogue if it exists. Returns -1 otherwise.
        /// </summary>
        private int GetYesResponseID(Dialogue d)
        {
            List<NPCDialogueResponse> npcdrs = (List<NPCDialogueResponse>)typeof(Dialogue).GetField("playerResponses", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(d);
            foreach (NPCDialogueResponse npcdr in npcdrs)
            {
                if (npcdr.id % 25 == 0)
                    return npcdr.id;
            }
            return -1;
        }

        /// <summary>
        /// Pushes the "Companion Action Dialogue" onto the Companion's dialogue stack.
        /// This dialogue allows the farmer to dismiss the Companion, as well as potentially
        /// do other actions in the future.
        /// </summary>
        private void PrepareCompanionActionDialogue()
        {
            if (whiteBox != null && whiteBox.CurrentDialogue.Count == 0)
            {
                bool hbk = (bool)typeof(NPC).GetField("hasBeenKissedToday", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(whiteBox);
                if (farmer.spouse.Equals(whiteBox.Name) && farmer.getFriendshipHeartLevelForNPC(whiteBox.Name) > 9 && !hbk)
                    return;
                actionDialogue = new Dialogue(dialogueScripts[whiteBox.Name]["CompanionActions"], whiteBox);
                yesResponseID = GetYesResponseID(actionDialogue);
                whiteBox.CurrentDialogue.Push(actionDialogue);
            }
        }

        /// <summary>
        /// The the Companion's schedule for this day. Essentially just a copy of CA's code
        /// which allows me to add the ability to store GameLocation s of where Companions
        /// should be at any given point in the day.
        /// </summary>
        private Dictionary<int, SchedulePathDescription> GetWhiteBoxSchedule(int dayOfMonth)
        {
            if (!whiteBox.Name.Equals("Robin") || Game1.player.currentUpgrade != null)
            {
                whiteBox.IsInvisible = false;
            }
            if (whiteBox.Name.Equals("Willy") && Game1.stats.DaysPlayed < 2u)
            {
                whiteBox.IsInvisible = true;
            }
            else if (whiteBox.Schedule != null)
            {
                whiteBox.followSchedule = true;
            }
            Dictionary<string, string> masterSchedule = null;
            Dictionary<int, SchedulePathDescription> result;
            try
            {
                masterSchedule = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name);
            }
            catch (Exception)
            {
                result = null;
                return result;
            }
            if (whiteBox.isMarried())
            {
                string day = Game1.shortDayNameFromDayOfSeason(dayOfMonth);
                if ((whiteBox.Name.Equals("Penny") && (day.Equals("Tue") || day.Equals("Wed") || day.Equals("Fri"))) || (whiteBox.Name.Equals("Maru") && (day.Equals("Tue") || day.Equals("Thu"))) || (whiteBox.Name.Equals("Harvey") && (day.Equals("Tue") || day.Equals("Thu"))))
                {
                    FieldInfo scheduleName = typeof(NPC).GetField("nameOfTodaysSchedule", BindingFlags.NonPublic | BindingFlags.Instance);
                    scheduleName.SetValue(whiteBox,"marriageJob");
                    return MasterScheduleParse(masterSchedule["marriageJob"]);
                }
                if (!Game1.isRaining && masterSchedule.ContainsKey("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    FieldInfo scheduleName = typeof(NPC).GetField("nameOfTodaysSchedule", BindingFlags.NonPublic | BindingFlags.Instance);
                    scheduleName.SetValue(whiteBox, "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth));
                    return MasterScheduleParse(masterSchedule["marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)]);
                }
                whiteBox.followSchedule = false;
                return null;
            }
            else
            {
                if (masterSchedule.ContainsKey(Game1.currentSeason + "_" + Game1.dayOfMonth))
                {
                    return MasterScheduleParse(masterSchedule[Game1.currentSeason + "_" + Game1.dayOfMonth]);
                }
                int friendship;
                for (friendship = (Game1.player.friendshipData.ContainsKey(whiteBox.Name) ? (Game1.player.friendshipData[whiteBox.Name].Points / 250) : -1); friendship > 0; friendship--)
                {
                    if (masterSchedule.ContainsKey(Game1.dayOfMonth + "_" + friendship))
                    {
                        return MasterScheduleParse(masterSchedule[Game1.dayOfMonth + "_" + friendship]);
                    }
                }
                if (masterSchedule.ContainsKey(string.Empty + Game1.dayOfMonth))
                {
                    return MasterScheduleParse(masterSchedule[string.Empty + Game1.dayOfMonth]);
                }
                if (whiteBox.Name.Equals("Pam") && Game1.player.mailReceived.Contains("ccVault"))
                {
                    return MasterScheduleParse(masterSchedule["bus"]);
                }
                if (Game1.isRaining)
                {
                    if (Game1.random.NextDouble() < 0.5 && masterSchedule.ContainsKey("rain2"))
                    {
                        return MasterScheduleParse(masterSchedule["rain2"]);
                    }
                    if (masterSchedule.ContainsKey("rain"))
                    {
                        return MasterScheduleParse(masterSchedule["rain"]);
                    }
                }
                List<string> key = new List<string>
                {
                    Game1.currentSeason,
                    Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)
                };
                friendship = (Game1.player.friendshipData.ContainsKey(whiteBox.Name) ? (Game1.player.friendshipData[whiteBox.Name].Points / 250) : -1);
                while (friendship > 0)
                {
                    key.Add(string.Empty + friendship);
                    if (masterSchedule.ContainsKey(string.Join("_", key)))
                    {
                        return MasterScheduleParse(masterSchedule[string.Join("_", key)]);
                    }
                    friendship--;
                    key.RemoveAt(key.Count - 1);
                }
                if (masterSchedule.ContainsKey(string.Join("_", key)))
                {
                    return MasterScheduleParse(masterSchedule[string.Join("_", key)]);
                }
                if (masterSchedule.ContainsKey(Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return MasterScheduleParse(masterSchedule[Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)]);
                }
                if (masterSchedule.ContainsKey(Game1.currentSeason))
                {
                    return MasterScheduleParse(masterSchedule[Game1.currentSeason]);
                }
                if (masterSchedule.ContainsKey("spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    return MasterScheduleParse(masterSchedule["spring_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)]);
                }
                key.RemoveAt(key.Count - 1);
                key.Add("spring");
                friendship = (Game1.player.friendshipData.ContainsKey(whiteBox.Name) ? (Game1.player.friendshipData[whiteBox.Name].Points / 250) : -1);
                while (friendship > 0)
                {
                    key.Add(string.Empty + friendship);
                    if (masterSchedule.ContainsKey(string.Join("_", key)))
                    {
                        return MasterScheduleParse(masterSchedule[string.Join("_", key)]);
                    }
                    friendship--;
                    key.RemoveAt(key.Count - 1);
                }
                if (masterSchedule.ContainsKey("spring"))
                {
                    return MasterScheduleParse(masterSchedule["spring"]);
                }
                return null;
            }
        }

        /// <summary>
        /// Helper for GetWhiteBoxSchedule.
        /// </summary>
        private Dictionary<int, SchedulePathDescription> MasterScheduleParse(string scheduleString)
        {
            int timeOfDay = Game1.timeOfDay;
            string[] split = scheduleString.Split(new char[]{ '/' });
            Dictionary<int, SchedulePathDescription> oneDaySchedule = new Dictionary<int, SchedulePathDescription>();
            Type[] pathfinderTypes = new Type[]
            {
                typeof(string), typeof(int), typeof(int), typeof(string), typeof(int), typeof(int), typeof(int),
                typeof(string), typeof(string)
            };
            MethodInfo pathfinder = typeof(NPC).GetMethod("pathfindToNextScheduleLocation",
                BindingFlags.NonPublic | BindingFlags.Instance, null, pathfinderTypes, null);
            int routesToSkip = 0;
            int previousTime = 0;
            if (split[0].Contains("GOTO"))
            {
                string newKey = split[0].Split(new char[] {' '})[1];

                if (newKey.ToLower().Equals("season"))
                {
                    newKey = Game1.currentSeason;
                }

                try
                {
                    split =
                        Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name)
                            [newKey].Split(new char[] {'/'});
                }
                catch (Exception)
                {
                    return MasterScheduleParse(
                        Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name)[
                            "spring"]);
                }
            }

            if (split[0].Contains("NOT"))
            {
                string[] commandSplit = split[0].Split(new char[] { ' ' });
                string a = commandSplit[1].ToLower();
                if (a == "friendship")
                {
                    string who = commandSplit[2];
                    int level = Convert.ToInt32(commandSplit[3]);
                    bool conditionMet = false;
                    using (IEnumerator<Farmer> enumerator = Game1.getAllFarmers().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.getFriendshipLevelForNPC(who) >= level)
                            {
                                conditionMet = true;
                                break;
                            }
                        }
                    }
                    if (conditionMet)
                    {
                        return this.MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name)["spring"]);
                    }
                    routesToSkip++;
                }
            }

            if (split[routesToSkip].Contains("GOTO"))
            {
                string newKey2 = split[routesToSkip].Split(new char[] { ' ' })[1];
                if (newKey2.ToLower().Equals("season"))
                {
                    newKey2 = Game1.currentSeason;
                }
                split = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name)[newKey2].Split(new char[] { '/' });
                routesToSkip = 1;
            }

            Point previousPosition = whiteBox.isMarried() ? new Point(0, 23) : new Point((int)whiteBox.DefaultPosition.X / 64, (int)whiteBox.DefaultPosition.Y / 64);
            string previousGameLocation = whiteBox.isMarried() ? "BusStop" : whiteBox.DefaultMap;
            int i = routesToSkip;

            while (i < split.Length && split.Length > 1)
            {
                int index = 0;
                string[] newDestinationDescription = split[i].Split(new char[] { ' ' });
                int time = Convert.ToInt32(newDestinationDescription[index]);
                index++;
                string location = newDestinationDescription[index];
                string endOfRouteAnimation = null;
                string endOfRouteMessage = null;
                int tmp;
                if (int.TryParse(location, out tmp))
                {
                    location = previousGameLocation;
                    index--;
                }
                index++;
                int xLocation = Convert.ToInt32(newDestinationDescription[index]);
                index++;
                int yLocation = Convert.ToInt32(newDestinationDescription[index]);
                index++;
                int localFacingDirection = 2;
                try
                {
                    localFacingDirection = Convert.ToInt32(newDestinationDescription[index]);
                    index++;
                }
                catch (Exception)
                {
                    localFacingDirection = 2;
                }
                if (changeScheduleForLocationAccessibility(ref location, ref xLocation, ref yLocation, ref localFacingDirection))
                {
                    if (Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name).ContainsKey("default"))
                    {
                        return MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name)["default"]);
                    }
                    return MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name)["spring"]);
                }
                else
                {
                    if (index < newDestinationDescription.Length)
                    {
                        if (newDestinationDescription[index].Length > 0 && newDestinationDescription[index][0] == '"')
                        {
                            endOfRouteMessage = split[i].Substring(split[i].IndexOf('"'));
                        }
                        else
                        {
                            endOfRouteAnimation = newDestinationDescription[index];
                            index++;
                            if (index < newDestinationDescription.Length && newDestinationDescription[index].Length > 0 && newDestinationDescription[index][0] == '"')
                            {
                                endOfRouteMessage = split[i].Substring(split[i].IndexOf('"')).Replace("\"", "");
                            }
                        }
                    }

                    object[] parameters = new object[]
                    {
                        previousGameLocation, previousPosition.X, previousPosition.Y, location, xLocation,
                        yLocation, localFacingDirection, endOfRouteAnimation, endOfRouteMessage
                    };
                    SchedulePathDescription spd = (SchedulePathDescription)pathfinder.Invoke(whiteBox, parameters);
                    oneDaySchedule.Add(time, spd);
                    previousPosition.X = xLocation;
                    previousPosition.Y = yLocation;
                    if (timeOfDay >= time)
                    {
                        Stack<Point> sp = oneDaySchedule[time].route;
                        Point p = new Point();
                        while (sp.Count > 1)
                            sp.Pop();
                        while (sp.Count != 0)
                            p = sp.Pop();

                        if (previousTime < time)
                        {
                            whiteBoxScheduleCurrentDestinationPoint = p;
                            whiteBoxScheduleCurrentDestinationLocation = location;
                        }
                    }
                    previousTime = time;
                    previousGameLocation = location;
                    i++;
                }
            }
            return oneDaySchedule;
        }

        /// <summary>
        /// Another helper for GetWhiteBoxSchedule.
        /// </summary>
        private bool changeScheduleForLocationAccessibility(ref string locationName, ref int tileX, ref int tileY, ref int facingDirection)
        {
            string a = locationName;
            if (!(a == "JojaMart") && !(a == "Railroad"))
            {
                if (a == "CommunityCenter")
                {
                    return !Game1.isLocationAccessible(locationName);
                }
            }
            else if (!Game1.isLocationAccessible(locationName))
            {
                if (!Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name).ContainsKey(locationName + "_Replacement"))
                {
                    return true;
                }
                string[] split = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + whiteBox.Name)[locationName + "_Replacement"].Split(new char[] { ' ' });
                locationName = split[0];
                tileX = Convert.ToInt32(split[1]);
                tileY = Convert.ToInt32(split[2]);
                facingDirection = Convert.ToInt32(split[3]);
            }
            return false;
        }

        /// <summary>
        /// Sets the companion dialogue for the current location.
        /// </summary>
        private void handleCompanionLocationSpecificDialogue()
        {
            string dialogueKey = "companion" + farmer.currentLocation.Name;
            string dialougeValue;
            if (whiteBox.Dialogue.TryGetValue(dialogueKey, out dialougeValue))
            {
                while (whiteBox.CurrentDialogue.Count != 0)
                    whiteBox.CurrentDialogue.Pop();
                whiteBox.CurrentDialogue.Push(new Dialogue(dialougeValue, whiteBox));
            }
        }

        #endregion
    }

    /// <summary>
    /// A collection of Harmony patches that allow me to modify, add, or remove certain
    /// functionalities of CA's code to make it work nicely with mine. 
    /// </summary>
    class Patches
    {
        static public NPC companion;

        /// <summary>
        /// A weird, roundabout way of allowing Companions to pass through invisible
        /// barriers that normally block NPC's. Might want to consider making this a
        /// little less, strange(?), in the future.
        /// </summary>
        #region isCollidingPosition
        static public bool flag;

        static public void Prefix(GameLocation __instance, Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if (companion != null
                && character != null
                && character.Name != null
                && character.Name.Equals(companion.Name)
                && !character.eventActor)
                character.eventActor = flag = true;
        }

        static public void Postfix(GameLocation __instance, Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if (flag)
                character.eventActor = flag = false;
        }
        #endregion

        /// <summary>
        /// The Companion's daily schedule is remade from scratch once they are dismissed.
        /// This pops off any routes from their schedule that are in the past until it reaches
        /// the most recent route they would have traveled. It then pops off that route and stores
        /// it as the location they will be warped to after they are finished being dismissed.
        /// </summary>
        #region checkSchedule
        static public Point scheduleCurrentDestination;

        static public void Postfix(NPC __instance, int timeOfDay)
        {
            if (companion != null && companion == __instance)
            {
                SchedulePathDescription spd;
                if (__instance.Schedule.TryGetValue(timeOfDay, out spd) && spd != null)
                {
                    while (spd.route.Count != 0)
                        scheduleCurrentDestination = spd.route.Pop();
                }
            }
        }
        #endregion

        /// <summary>
        /// Prevents the Companion from updating their movement via CA's movementUpdate function
        /// while they are the farmer's companion.
        /// </summary>
        #region updateMovement
        static public bool Prefix(NPC __instance, GameLocation location, GameTime time)
        {
            return !(companion != null) && __instance.Equals(companion);
        }
        #endregion
    }

    /// <summary>
    /// Harmony patche(s) that I use(d) for debug purposes. These are not (or should not) be
    /// used in any release build of this mod.
    /// </summary>
    class DebugPatches
    {
        static public NPC testPC;
        static public bool debugging = false;

        static public bool Prefix(NPC __instance, int timeOfDay)
        {
            if (debugging && __instance.Name.Equals("Penny"))
            {
                debugCheckSchedule(Game1.timeOfDay);
                return false;
            }
            return true;
        }

        public static void debug(NPC n)
        {
            if (n != null)
            {
                testPC = n;
                debugging = true;
            }
            else
            {
                testPC = null;
                debugging = false;
            }
        }

        public static void debugCheckSchedule(int timeOfDay)
        {
            testPC.updatedDialogueYet = false;
            typeof(NPC).GetField("extraDialogueMessageToAddThisMorning", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(testPC, null);
            //testPC.extraDialogueMessageToAddThisMorning = null;
            if (testPC.ignoreScheduleToday)
            {
                return;
            }
            if (testPC.Schedule != null)
            {
                SchedulePathDescription possibleNewDirections;
                int time2Try = (int)typeof(NPC).GetField("scheduleTimeToTry", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(testPC);
                testPC.Schedule.TryGetValue((time2Try == 9999999) ? timeOfDay : time2Try, out possibleNewDirections);
                if (possibleNewDirections != null)
                {
                    bool walkingInSquare = (bool)typeof(NPC).GetField("isWalkingInSquare", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(testPC);
                    Rectangle lastCrossroad = (Rectangle)typeof(NPC).GetField("lastCrossroad", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(testPC);
                    Point prevEndPoint = (Point)typeof(NPC).GetField("previousEndPoint", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(testPC);
                    if (!testPC.isMarried() && (!walkingInSquare || (lastCrossroad.Center.X / 64 != prevEndPoint.X && lastCrossroad.Y / 64 != prevEndPoint.Y)))
                    {
                        Point arg_A0_0 = prevEndPoint;
                        if (!prevEndPoint.Equals(Point.Zero) && !prevEndPoint.Equals(testPC.getTileLocationPoint()))
                        {
                            if (time2Try == 9999999)
                            {
                                typeof(NPC).GetField("scheduleTimeToTry", BindingFlags.NonPublic | BindingFlags.Instance)
                                    .SetValue(testPC, timeOfDay);
                                return;
                            }
                            return;
                        }
                    }

                    FieldInfo d2nL =
                        typeof(NPC).GetField("directionsToNewLocation",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                    d2nL.SetValue(testPC, possibleNewDirections);
                    typeof(NPC).GetMethod("prepareToDisembarkOnNewSchedulePath",
                        BindingFlags.NonPublic | BindingFlags.Instance).Invoke(testPC, null);
                    if (testPC.Schedule == null)
                    {
                        return;
                    }

                    SchedulePathDescription d2nLValue = (SchedulePathDescription)d2nL.GetValue(testPC);
                    if (d2nLValue != null && d2nLValue.route != null && d2nLValue.route.Count > 0 && (Math.Abs(testPC.getTileLocationPoint().X - d2nLValue.route.Peek().X) > 1 || Math.Abs(testPC.getTileLocationPoint().Y - d2nLValue.route.Peek().Y) > 1) && testPC.temporaryController == null)
                    {
                        typeof(NPC).GetField("scheduleTimeToTry", BindingFlags.NonPublic | BindingFlags.Instance)
                            .SetValue(testPC, 9999999);
                        return;
                    }
                    object[] parameters = new object[] { d2nLValue.endOfRouteBehavior, d2nLValue.endOfRouteMessage };
                    testPC.controller = new PathFindController(d2nLValue.route, testPC, Utility.getGameLocationOfCharacter(testPC))
                    {
                        finalFacingDirection = d2nLValue.facingDirection,
                        endBehaviorFunction = (PathFindController.endBehavior)typeof(NPC).GetMethod("getRouteEndBehaviorFunction", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(testPC, parameters)
                    };
                    typeof(NPC).GetField("scheduleTimeToTry", BindingFlags.NonPublic | BindingFlags.Instance)
                        .SetValue(testPC, 9999999);
                    if (d2nLValue != null && d2nLValue.route != null)
                    {
                        typeof(NPC).GetField("previousEndPoint", BindingFlags.NonPublic | BindingFlags.Instance)
                            .SetValue(testPC, ((d2nLValue.route.Count > 0) ? d2nLValue.route.Last() : Point.Zero));
                    }
                }
            }
        }
    }

}

