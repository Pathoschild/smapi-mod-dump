using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;

namespace FollowerNPC
{
    public class CompanionsManager
    {
        public class CompanionDialogueInfo
        {
            public Dialogue recruitDialogue;
            public int recruitYesID;
            public Dialogue actionDialogue;
            public int actionDismissID;
        }

        public enum CompanionionshipState
        {
            available = 0,
            recruitDialoguePushed = 1,
            recruitFollowupDialoguePushed = 2,
            actionDialoguePushed = 3,
            actionDialogueNeedsRePushed = 4,
            dismissed = 5
        }

        // Companion Objects //
        public NPC companion;
        public CompanionBuff companionBuff;
        // ***************** //

        // Companion Parameters //
        public int companionSwitchDirectionSpeed;
        public int companionFacingDirection;
        public float companionFollowThreshold;
        // ******************** //

        // Companion Movement Memory //
        public Vector2 companionLastMovementDirection;
        public Vector2 companionLastFrameVelocity;
        public Vector2 companionLastFramePosition;
        public Vector2 companionLastFrameMovement;
        public Vector2 animationUpdateSum;
        public bool companionMovedLastFrame;
        // ************************* //

        // Other //
        public int companionWarpTimer;
        // ***** //

        // Companion A* //
        public aStar companionAStar;
        public Queue<Vector2> companionPath;
        public Vector2 companionPathCurrentNode;
        public float companionPathingNodeGoalTolerance;
        // ************ //

        // Companion Rescheduling //
        public Point companionRescheduleDestinationPoint;
        public string companionRescheduleDestinationLocation;
        public string companionRescheduleEndRouteBehavior;
        public string companionRescheduleEndRouteDialogue;
        public int companionRescheduleFacingDirection;
        // ********************** //

        // Companion Data // 
        public SortedList<string, Dictionary<string, string>> npcDialogueScripts;
        public SortedList<string, string> npcCompanionAvailabilityDays;
        public Dictionary<string, bool> npcsThatCanBeRecruitedToday;
        public Dictionary<string, CompanionDialogueInfo> companionDialogueInfos;
        public Dictionary<string, bool> currentCompanionVisitedLocations;
        public Dictionary<string, CompanionionshipState> companionStates;
        // ************** //

        // Farmer Objects //
        public Farmer farmer;
        public Vector2 farmerLastTile;
        // ************** //

        // Constants //
        public Vector2 negativeOne = new Vector2(-1, -1);
        public int fullTile = Game1.tileSize;
        public int halfTile = Game1.tileSize / 2;
        // ********* //

        public CompanionsManager()
        {
            // Initialize Companion Data //
            SetNPCCompanionDays();
            InitializeNPCsThatCanHangOut();
            companionDialogueInfos = new Dictionary<string, CompanionDialogueInfo>();
            // ************************* //

            // Define Companion Parameters //
            companionFollowThreshold = 2f;
            companionPathingNodeGoalTolerance = 5f;
            // *************************** //

            // Subscribe to Events //
            GameEvents.UpdateTick += UpdateTick;
            GameEvents.QuarterSecondTick += QuarterSecondTick;
            SaveEvents.AfterLoad += AfterLoad;
            PlayerEvents.Warped += PlayerWarped;
            MineEvents.MineLevelChanged += MineLevelChanged;
            MenuEvents.MenuClosed += HandleDialogueClosure;
            TimeEvents.TimeOfDayChanged += TimeOfDayChanged;
            TimeEvents.AfterDayStarted += AfterDayStarted;

        }

        #region Helpers

        #region Events

        private void UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || !(companion != null) || !(farmer != null))
                return;

            if (companion != null)
            {
                PathfindingNodeUpdateCheck();
                MovementAndAnimationUpdate();
                if (companionBuff != null)
                    companionBuff.Update();
            }
        }

        private void QuarterSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || !(companion != null) || !(farmer != null))
                return;

            PathfindingRemakeCheck();
        }

        private void AfterLoad(object sender, EventArgs e)
        {
            farmer = Game1.player;
            companion = null;

            InitializeDialogueScripts();
        }

        private void PlayerWarped(object sender, EventArgsPlayerWarped e)
        {
            if (!Context.IsWorldReady || !(companion != null) || !(farmer != null))
                return;

            companionAStar.gameLocation = farmer.currentLocation;
            if (!farmer.isRidingHorse())
            {
                Game1.warpCharacter(companion, farmer.currentLocation, farmer.getTileLocation());
                handleCompanionLocationSpecificDialogue();
            }
            else
            {
                DelayedWarp(null, Point.Zero, 100, new Action(handleCompanionLocationSpecificDialogue));
            }

        }

        private void MineLevelChanged(object sender, EventArgsMineLevelChanged e)
        {
            if (!Context.IsWorldReady || !(companion != null) || !(farmer != null))
                return;

            Game1.warpCharacter(companion, farmer.currentLocation, farmer.getTileLocation());
        }

        private void HandleDialogueClosure(object sender, EventArgsClickableMenuClosed e)
        {
            // If this is a dialogue menu...
            DialogueBox db = (e.PriorMenu as DialogueBox);
            if (db != null)
            {
                // If this dialogue's speaker isn't null...
                Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                if (d != null && d.speaker != null)
                {
                    NPC n = d.speaker;
                    
                    // Check to see if we should push a Companion Recruit Dialogue
                    if (n.CurrentDialogue.Count == 0 && 
                        companionStates[n.Name] == CompanionionshipState.available && 
                        npcsThatCanBeRecruitedToday.TryGetValue(n.Name, out bool available) && available)
                    {
                        TryPushCompanionRecruitDialogue(n.Name);
                    }

                    // Check to see if this was a response to a Companion Recruit Dialogue
                    else if (companionStates[n.Name] == CompanionionshipState.recruitDialoguePushed && 
                             companionDialogueInfos.TryGetValue(n.Name, out CompanionDialogueInfo cdi) &&
                             d.Equals(cdi.recruitDialogue))
                    {
                        HandleCompanionRecruiting(n.Name, cdi);
                    }

                    // Check to see if this was a Companion Recruit Followup Dialogue
                    else if (companionStates[n.Name] == CompanionionshipState.recruitFollowupDialoguePushed &&
                             companionDialogueInfos.TryGetValue(n.Name, out cdi) &&
                             d.Equals(cdi.recruitDialogue))
                    {
                        farmer.DialogueQuestionsAnswered.Remove(cdi.recruitYesID);
                        TryPushCompanionActionDialogue();
                    }

                    // Check to see if this was a response to a Companion Action Dialgoue
                    else if (companionDialogueInfos.TryGetValue(n.Name, out cdi) &&
                             d.Equals(cdi.actionDialogue))
                    {
                        HandleCompanionAction(n.Name, cdi);
                    }
                    
                    // Deplete the FaceTowardFarmerTimer
                    if (companion != null && n.Equals(companion))
                    {
                        companion.faceTowardFarmerTimer = 0;
                        companion.movementPause = 0;
                    }
                }
            }
        }

        private void TimeOfDayChanged(object sender, EventArgsIntChanged e)
        {
            if (!Context.IsWorldReady || !(companion != null) || !(farmer != null))
                return;

            TryPushCompanionActionDialogue();
        }

        private void AfterDayStarted(object sender, EventArgs e)
        {
            ResetNPCsThatCanHangOut();
        }

        #endregion

        #region Movement & Animation

        /// <summary>
        /// Remakes the white box's path if the farmer has changed tiles since the last time
        /// this function was called. (Every quarter second as of right now)
        /// </summary>
        private void PathfindingRemakeCheck()
        {
            Vector2 farmerCurrentTile = farmer.getTileLocation();

            if (farmerLastTile != farmerCurrentTile)
            {
                companionPath = companionAStar.Pathfind(companion.getTileLocation(), farmerCurrentTile);
                if (companion.getTileLocation() != companionPathCurrentNode)
                    companionPathCurrentNode = companionPath != null && companionPath.Count != 0 ? companionPath.Dequeue() : negativeOne;
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
            if (companionPathCurrentNode != negativeOne && companionPath != null)
            {
                Point w = companion.GetBoundingBox().Center;
                Point n = new Point(((int)companionPathCurrentNode.X * fullTile) + halfTile, ((int)companionPathCurrentNode.Y * Game1.tileSize) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                float nodeDiffLen = nodeDiff.Length();
                if (nodeDiffLen <= companionPathingNodeGoalTolerance)
                {
                    if (companionPath.Count == 0)
                    {
                        companionPath = null;
                        companionPathCurrentNode = negativeOne;
                        return;
                    }
                    companionPathCurrentNode = companionPath.Dequeue();
                    n = new Point(((int)companionPathCurrentNode.X * fullTile) + halfTile, ((int)companionPathCurrentNode.Y * Game1.tileSize) + halfTile);
                    nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                    nodeDiffLen = nodeDiff.Length();
                }
            }
        }

        /// <summary>
        /// Provides updates to the companion's movement.
        /// </summary>
        private void MovementAndAnimationUpdate()
        {
            Point f = farmer.GetBoundingBox().Center;
            Point c = companion.GetBoundingBox().Center;
            Vector2 companionBoundingBox = new Vector2(c.X, c.Y);
            companionLastFrameMovement = companionBoundingBox - companionLastFramePosition;
            
            Vector2 diff = new Vector2(f.X, f.Y) - new Vector2(c.X, c.Y);
            float diffLen = diff.Length();
            if (diffLen > Game1.tileSize * companionFollowThreshold && companionPathCurrentNode != negativeOne)
            {
                Point n = new Point(((int)companionPathCurrentNode.X * fullTile) + halfTile, ((int)companionPathCurrentNode.Y * fullTile) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(c.X, c.Y);
                float nodeDiffLen = nodeDiff.Length();
                if (nodeDiffLen <= companionPathingNodeGoalTolerance)
                    return;
                nodeDiff /= nodeDiffLen;

                companion.xVelocity = nodeDiff.X * farmer.getMovementSpeed();
                companion.yVelocity = -nodeDiff.Y * farmer.getMovementSpeed();
                if (companion.xVelocity != 0 && companion.yVelocity != 0)
                {
                    companion.xVelocity *= 1.26f;
                    companion.yVelocity *= 1.26f;
                }
                HandleWallSliding();
                companionLastFrameVelocity = new Vector2(companion.xVelocity, companion.yVelocity);
                companionLastFramePosition = new Vector2(companion.GetBoundingBox().Center.X, companion.GetBoundingBox().Center.Y);

                animationUpdateSum += new Vector2(companion.xVelocity, -companion.yVelocity);
                AnimationSubUpdate();
                companion.MovePosition(Game1.currentGameTime, Game1.viewport, companion.currentLocation);
                companionLastMovementDirection = companionLastFrameVelocity / companionLastFrameVelocity.Length();

                companionMovedLastFrame = true;
            }
            else if (companionMovedLastFrame)
            {
                companion.Halt();
                companion.Sprite.faceDirectionStandard(GetFacingDirectionFromMovement(new Vector2(companionLastMovementDirection.X, -companionLastMovementDirection.Y)));
                companionMovedLastFrame = false;
            }
        }

        /// <summary>
        /// Provides updates to the companion's animation;
        /// </summary>
        private void AnimationSubUpdate()
        {
            if (++companionSwitchDirectionSpeed == 15)
            {
                companionFacingDirection = GetFacingDirectionFromMovement(animationUpdateSum);
                animationUpdateSum = Vector2.Zero;
                companionSwitchDirectionSpeed = 0;
            }

            if (companionFacingDirection >= 0)
            {
                companion.faceDirection(companionFacingDirection);
                SetMovementDirectionAnimation(companionFacingDirection);
            }
            
        }

        /// <summary>
        /// Allows the Companion to "slide" along walls instead of getting stuck on them
        /// </summary>
        private void HandleWallSliding()
        {
            if (companionLastFrameVelocity != Vector2.Zero && companionLastFrameMovement == Vector2.Zero &&
                (companion.xVelocity != 0 || companion.yVelocity != 0))
            {
                Rectangle wbBB = companion.GetBoundingBox();
                int ts = Game1.tileSize;

                if (companion.xVelocity != 0)
                {
                    int velocitySign = Math.Sign(companion.xVelocity) * 5;
                    int leftOrRight = ((companion.xVelocity > 0 ? wbBB.Right : wbBB.Left) + velocitySign) / ts;
                    bool[] xTiles = new bool[3];
                    xTiles[0] = companionAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Top / ts));
                    xTiles[1] = companionAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Center.Y / ts));
                    xTiles[2] = companionAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Bottom / ts));
                    foreach (bool b in xTiles)
                    {
                        if (!b)
                            companion.xVelocity = 0;
                    }
                }

                if (companion.yVelocity != 0)
                {
                    int velocitySign = Math.Sign(companion.yVelocity) * 5;
                    int topOrBottom = ((companion.yVelocity < 0 ? wbBB.Bottom : wbBB.Top) - velocitySign) / ts;
                    bool[] yTiles = new bool[3];
                    yTiles[0] = companionAStar.IsWalkableTile(new Vector2(wbBB.Left / ts, topOrBottom));
                    yTiles[1] = companionAStar.IsWalkableTile(new Vector2(wbBB.Center.X / ts, topOrBottom));
                    yTiles[2] = companionAStar.IsWalkableTile(new Vector2(wbBB.Right / ts, topOrBottom));
                    foreach (bool b in yTiles)
                    {
                        if (!b)
                            companion.yVelocity = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the proper animation for the Companion based on their direction.
        /// </summary>
        private void SetMovementDirectionAnimation(int dir)
        {
            switch (dir)
            {
                case 0:
                    companion.SetMovingOnlyUp();
                    companion.Sprite.AnimateUp(Game1.currentGameTime, 0, ""); break;
                case 1:
                    companion.SetMovingOnlyRight();
                    companion.Sprite.AnimateRight(Game1.currentGameTime, 0, ""); break;
                case 2:
                    companion.SetMovingOnlyDown();
                    companion.Sprite.AnimateDown(Game1.currentGameTime, 0, ""); break;
                case 3:
                    companion.SetMovingOnlyLeft();
                    companion.Sprite.AnimateLeft(Game1.currentGameTime, 0, ""); break;
            }
        }

        /// <summary>
        /// Returns an int 0, 1, 2, or 3, representing the direction the Companion should face 
        /// based on their current velocity. North is 0, and the rest follow clockwise.
        /// </summary>
        private int GetFacingDirectionFromMovement(Vector2 movement)
        {
            if (movement == Vector2.Zero)
                return -1;
            int dir = 2;
            if (Math.Abs(movement.X) > Math.Abs(movement.Y))
                dir = movement.X > 0 ? 1 : 3;
            else if (Math.Abs(movement.X) < Math.Abs(movement.Y))
                dir = movement.Y > 0 ? 2 : 0;
            return dir;
        }

        #endregion

        #region Dialogue, Scheduling, Recruiting, and Dismissing
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
            npcDialogueScripts = new SortedList<string, Dictionary<string, string>>();
            foreach (string characterName in characterNames)
            {
                npcDialogueScripts.Add(characterName,
                    ModEntry.modHelper.Content.Load<Dictionary<string, string>>("assets/" + characterName + "Companion.json",
                        ContentSource.ModFolder));
                NPC character = Game1.getCharacterFromName(characterName);
                foreach (KeyValuePair<string, string> dialogueKVP in npcDialogueScripts[characterName])
                    character.Dialogue.Add(dialogueKVP.Key, dialogueKVP.Value);
            }
        }

        /// <summary>
        /// Initializes and fills the SortedList which determines which day each NPC 
        /// can hang out with the farmer.
        /// </summary>
        private void SetNPCCompanionDays()
        {
            npcCompanionAvailabilityDays = new SortedList<string, string>();
            npcCompanionAvailabilityDays.Add("Abigail", "Mon"); // Goes around town M/F when married. Plays flute on Tu/Sat. Doc or Seb on Thu.       Wed || Sun                        Wed
            npcCompanionAvailabilityDays.Add("Alex", "Mon"); // Hangs with Haley on Wed. Doc on Tu. Visits parents on Mo when married.                Thu || Fri || Sat || Sun          Fri
            npcCompanionAvailabilityDays.Add("Elliott", "Mon"); // Goes to beach M when married. Doc on Tue. Shops on Thu. Beach on Fri/Sun.          Wed || Sat                        Sat
            npcCompanionAvailabilityDays.Add("Emily", "Mon"); // Works Mon/Wed/Fri/Sat. Works out Tue. Doc on Thu.                                    Sun                               Sun
            npcCompanionAvailabilityDays.Add("Haley", "Mon"); // Photography on Mon. Doc on Tue.                                                      Wed || Thu || Fri || Sat || Sun   Fri
            npcCompanionAvailabilityDays.Add("Harvey", "Mon"); // Working Mon/Tue/Wed/Thu/Fri/Sun                                                     Sat                               Sat
            npcCompanionAvailabilityDays.Add("Leah", "Mon"); // Groceries on Mon. Doc on Tue. Saloon on Fri/Sat.                                      Wed || Thu || Sun                 Thu
            npcCompanionAvailabilityDays.Add("Maru", "Mon"); // Visits parents on Mon when married. Work on Tue/Thu. Personal work on Sat.            Wed || Fri || Sun                 Wed
            npcCompanionAvailabilityDays.Add("Penny", "Mon"); // Out in town Mon when married. Teaches Tue/Wed/Fri. Babysits Sat. Doc on Thu.         Mon || Thu || Sun                 Mon
            npcCompanionAvailabilityDays.Add("Sam", "Mon"); // Works Mon/Wed. Saloon on Friday. Hangs with Seb on Sat.                                Tue || Thu || Sun                 Tue
            npcCompanionAvailabilityDays.Add("Sebastian", "Mon"); // Visits parents Mon. Hangs with Abi on Thu. Saloon on Fri. Hangs with Sam on Sat. Tue || Wed || Sun                 Tue
            npcCompanionAvailabilityDays.Add("Shane", "Mon"); // Works Mon/Tue/Wed/Thu/Fri. Groceries on Sat.                                         Sun                               Sun
        }

        /// <summary>
        /// Initializes and fills the Dictionary which determines which NPC's can currently
        /// hang out with the farmer.
        /// </summary>
        private void InitializeNPCsThatCanHangOut()
        {
            npcsThatCanBeRecruitedToday = new Dictionary<string, bool>();
            npcsThatCanBeRecruitedToday.Add("Abigail", false);
            npcsThatCanBeRecruitedToday.Add("Alex", false);
            npcsThatCanBeRecruitedToday.Add("Elliott", false);
            npcsThatCanBeRecruitedToday.Add("Emily", false);
            npcsThatCanBeRecruitedToday.Add("Haley", false);
            npcsThatCanBeRecruitedToday.Add("Harvey", false);
            npcsThatCanBeRecruitedToday.Add("Leah", false);
            npcsThatCanBeRecruitedToday.Add("Maru", false);
            npcsThatCanBeRecruitedToday.Add("Penny", false);
            npcsThatCanBeRecruitedToday.Add("Sam", false);
            npcsThatCanBeRecruitedToday.Add("Sebastian", false);
            npcsThatCanBeRecruitedToday.Add("Shane", false);

            companionStates = new Dictionary<string, CompanionionshipState>();
            companionStates.Add("Abigail", CompanionionshipState.available);
            companionStates.Add("Alex", CompanionionshipState.available);
            companionStates.Add("Elliott", CompanionionshipState.available);
            companionStates.Add("Emily", CompanionionshipState.available);
            companionStates.Add("Haley", CompanionionshipState.available);
            companionStates.Add("Harvey", CompanionionshipState.available);
            companionStates.Add("Leah", CompanionionshipState.available);
            companionStates.Add("Maru", CompanionionshipState.available);
            companionStates.Add("Penny", CompanionionshipState.available);
            companionStates.Add("Sam", CompanionionshipState.available);
            companionStates.Add("Sebastian", CompanionionshipState.available);
            companionStates.Add("Shane", CompanionionshipState.available);
        }

        /// <summary>
        /// Sets NPC's availabilities to hang out to true if they are specified to be able to
        /// on this day of the week via the SortedList.
        /// </summary>
        private void ResetNPCsThatCanHangOut()
        {
            string dayOfWeek = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            foreach (KeyValuePair<string, string> kvp in npcCompanionAvailabilityDays)
            {
                if (kvp.Value.Equals(dayOfWeek))
                {
                    npcsThatCanBeRecruitedToday[kvp.Key] = true;
                    companionStates[kvp.Key] = CompanionionshipState.available;
                }
                else
                {
                    npcsThatCanBeRecruitedToday[kvp.Key] = false;
                    companionStates[kvp.Key] = CompanionionshipState.dismissed;
                }
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
        /// Pushes a Recruit Dialogue onto the NPC with parameter:name's CurrentDialogue Stack
        /// if the player doesn't already have a companion, and the NPC can be recruited today.
        /// </summary>
        private void TryPushCompanionRecruitDialogue(string name)
        {
            NPC n = Game1.getCharacterFromName(name);
            if (!(companion != null))
            {
                if (!companionDialogueInfos.TryGetValue(name, out CompanionDialogueInfo cdi))
                {
                    companionDialogueInfos[name] = GenerateCompanionDialogueInfo(name);
                }
                else
                {
                    cdi.recruitDialogue = new Dialogue(npcDialogueScripts[name]["Companion"], n);
                }
                
                npcsThatCanBeRecruitedToday[name] = false;
                n.CurrentDialogue.Push(companionDialogueInfos[name].recruitDialogue);
                companionStates[name] = CompanionionshipState.recruitDialoguePushed;
            }
        }

        /// <summary>
        /// Checks a farmer's response to a Recruit Dialogue and sets up the new Companion
        /// if the farmer chose to recruit them.
        /// </summary>
        private void HandleCompanionRecruiting(string name, CompanionDialogueInfo cdi)
        {
            NPC potentialCompanion = Game1.getCharacterFromName(name);
            // Recruit if the farmer said 'yes' in the Recruit Dialogue
            if (farmer.DialogueQuestionsAnswered.Contains(cdi.recruitYesID))
            {
                companion = potentialCompanion;
                companionAStar = new aStar(farmer.currentLocation, name);
                companionBuff = CompanionBuff.InitializeBuffFromCompanionName(name, farmer);
                currentCompanionVisitedLocations = new Dictionary<string, bool>();
                Patches.companion = potentialCompanion;
                companion.faceTowardFarmerTimer = 0;
            }
            // Don't recruit if the farmer didn't say 'yes' in the Recruit Dialogue
            else if (farmer.dialogueQuestionsAnswered.Contains(cdi.recruitYesID + 1))
            {
                farmer.dialogueQuestionsAnswered.Remove(cdi.recruitYesID + 1);
            }

            cdi.recruitDialogue = new Dialogue(npcDialogueScripts[name]["Companion"], potentialCompanion);
            potentialCompanion.CurrentDialogue.Push(cdi.recruitDialogue);
            companionStates[name] = CompanionionshipState.recruitFollowupDialoguePushed;
        }

        /// <summary>
        /// Pushes the "Companion Action Dialogue" onto the Companion's dialogue stack.
        /// This dialogue allows the farmer to dismiss the Companion, as well as potentially
        /// do other actions in the future.
        /// </summary>
        private void TryPushCompanionActionDialogue()
        {
            if (companion != null && companion.CurrentDialogue.Count == 0)
            {
                bool hbk = (bool)typeof(NPC).GetField("hasBeenKissedToday", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(companion);
                if (farmer.spouse.Equals(companion.Name) && farmer.getFriendshipHeartLevelForNPC(companion.Name) > 9 && !hbk)
                    return;
                if (!companionDialogueInfos.TryGetValue(companion.Name, out CompanionDialogueInfo cdi))
                {
                    companionDialogueInfos[companion.Name] = GenerateCompanionDialogueInfo(companion.Name);
                }
                else
                {
                    cdi.actionDialogue = new Dialogue(npcDialogueScripts[companion.Name]["CompanionActions"], companion);
                }
                companion.CurrentDialogue.Push(companionDialogueInfos[companion.Name].actionDialogue);
                companionStates[companion.Name] = CompanionionshipState.actionDialoguePushed;
            }
        }

        /// <summary>
        /// Checks a farmer's response to an Action Dialogue and dismisses the current
        /// Companion if the farmer chose to do so.
        /// </summary>
        private void HandleCompanionAction(string name, CompanionDialogueInfo cdi)
        {
            if (farmer.DialogueQuestionsAnswered.Contains(cdi.actionDismissID))
            {
                farmer.DialogueQuestionsAnswered.Remove(cdi.actionDismissID);
                companionAStar = null;
                companionBuff.RemoveAndDisposeCompanionBuff();
                companionBuff = null;
                companion.Schedule = GetCompanionSchedule(Game1.dayOfMonth);
                if (!(companion.Schedule != null) && farmer.spouse != null && farmer.spouse.Equals(companion.Name))
                {
                    companionRescheduleDestinationLocation = companion.DefaultMap;
                    companionRescheduleDestinationPoint =
                        (Game1.getLocationFromName(companion.DefaultMap) as StardewValley.Locations.FarmHouse)
                        .getKitchenStandingSpot();
                }
                Game1.fadeScreenToBlack();
                companion.faceTowardFarmerTimer = 0;
                companionStates[name] = CompanionionshipState.dismissed;
                DelayedWarp(companionRescheduleDestinationLocation,
                    companionRescheduleDestinationPoint, 500, new Action(CompanionEndCleanup));

                foreach (KeyValuePair<string, bool> npcKvP in npcsThatCanBeRecruitedToday)
                {
                    if (npcKvP.Value)
                        TryPushCompanionRecruitDialogue(npcKvP.Key);
                }
            }
            else if (farmer.dialogueQuestionsAnswered.Contains(cdi.actionDismissID + 1))
            {
                companionStates[name] = CompanionionshipState.actionDialogueNeedsRePushed;
                farmer.dialogueQuestionsAnswered.Remove(cdi.actionDismissID + 1);
                companion.faceTowardFarmerTimer = 0;
            }
        }

        /// <summary>
        /// Sets the companion dialogue for the current location.
        /// </summary>
        private void handleCompanionLocationSpecificDialogue()
        {
            string dialogueKey = "companion" + farmer.currentLocation.Name;
            string dialougeValue;
            if (companion.Dialogue.TryGetValue(dialogueKey, out dialougeValue) && !currentCompanionVisitedLocations.TryGetValue(dialogueKey, out bool v))
            {
                while (companion.CurrentDialogue.Count != 0)
                    companion.CurrentDialogue.Pop();
                companion.CurrentDialogue.Push(new Dialogue(dialougeValue, companion));
                currentCompanionVisitedLocations[dialogueKey] = true;
            }
        }

        /// <summary>
        /// Returns a CompanionDialogueInfo object for the NPC with parameter:name. A CompanionDialogueInfo
        /// contains references to the Recruit Dialogue, 'yes' response ID, and Action Dialogue.
        /// </summary>
        private CompanionDialogueInfo GenerateCompanionDialogueInfo(string name)
        {
            NPC n = Game1.getCharacterFromName(name);
            Dialogue rd = new Dialogue(npcDialogueScripts[name]["Companion"], n);
            Dialogue ad = new Dialogue(npcDialogueScripts[name]["CompanionActions"], n);
            return new CompanionDialogueInfo()
            {
                recruitDialogue = rd,
                recruitYesID = GetYesResponseID(rd),
                actionDialogue = ad,
                actionDismissID = GetYesResponseID(ad)
            };
        }

        /// <summary>
        /// Delays a companion's warp by parameter:milliseconds. Leave parameter:location null to default
        /// the warp location to wherever the farmer is after the timer finishes. Ditto with parameter:tileLocation
        /// except it must be left at {0,0}. parameter:afterWarpAction will be executed after the warp occurs, or
        /// can be left null.
        /// </summary>
        private async void DelayedWarp(String location, Point tileLocation, int milliseconds, Action afterWarpAction)
        {
            await Task.Run(() => Timer(milliseconds));
            location = location != null ? location : farmer.currentLocation.Name;
            tileLocation = tileLocation != Point.Zero ? tileLocation : farmer.getTileLocationPoint();
            Game1.warpCharacter(companion, location, tileLocation);
            afterWarpAction.Invoke();
        }

        /// <summary>
        /// Re-integrates the old companion into regular gameplay again.
        /// </summary>
        private void CompanionEndCleanup()
        {
            typeof(NPC).GetField("previousEndPoint", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(companion, companion.getTileLocationPoint());
            companion.checkSchedule(Game1.timeOfDay);
            // Set end of route behavior, message, and facingDirection
            MethodInfo getRouteEndBehaviorFunction = typeof(NPC).GetMethod("getRouteEndBehaviorFunction",
                BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(string) }, null);
            if (companionRescheduleEndRouteBehavior != null && companionRescheduleEndRouteBehavior != "")
            {
                PathFindController.endBehavior eB = (PathFindController.endBehavior)getRouteEndBehaviorFunction.Invoke(companion,
                    new object[] { companionRescheduleEndRouteBehavior, companionRescheduleEndRouteDialogue });
                eB(companion, companion.currentLocation);
            }
            if (companionRescheduleEndRouteDialogue != null && companionRescheduleEndRouteDialogue != "")
                companion.CurrentDialogue.Push(new Dialogue(companionRescheduleEndRouteDialogue, companion));
            companion.faceDirection(companionRescheduleFacingDirection);

            companion = null;
            Patches.companion = null;
        }
        #endregion

        #endregion

        #region Utilities

        /// <summary>
        /// Returns true if 'a' and 'b' are less than 'threshold' in difference.
        /// </summary>
        private bool FloatsApproximatelyEqual(float a, float b, float threshold)
        {
            return Math.Abs(a - b) < threshold;
        }

        /// <summary>
        /// Blocking lock for a timer that runs for the parameter milliseconds. DO NOT CALL THIS FROM THE MAIN THREAD!
        /// Always call using an asynchronous method & await.
        /// </summary>
        private int Timer(int milliseconds)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < milliseconds) ;
            return 0;
        }

        /// <summary>
        /// The the Companion's schedule for this day. Essentially just a copy of CA's code
        /// which allows me to add the ability to store GameLocations of where Companions
        /// should be at any given point in the day.
        /// </summary>
        private Dictionary<int, SchedulePathDescription> GetCompanionSchedule(int dayOfMonth)
        {
            if (!companion.Name.Equals("Robin") || Game1.player.currentUpgrade != null)
            {
                companion.IsInvisible = false;
            }
            if (companion.Name.Equals("Willy") && Game1.stats.DaysPlayed < 2u)
            {
                companion.IsInvisible = true;
            }
            else if (companion.Schedule != null)
            {
                companion.followSchedule = true;
            }
            Dictionary<string, string> masterSchedule = null;
            Dictionary<int, SchedulePathDescription> result;
            try
            {
                masterSchedule = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name);
            }
            catch (Exception)
            {
                result = null;
                return result;
            }
            if (companion.isMarried())
            {
                string day = Game1.shortDayNameFromDayOfSeason(dayOfMonth);
                if ((companion.Name.Equals("Penny") && (day.Equals("Tue") || day.Equals("Wed") || day.Equals("Fri"))) || (companion.Name.Equals("Maru") && (day.Equals("Tue") || day.Equals("Thu"))) || (companion.Name.Equals("Harvey") && (day.Equals("Tue") || day.Equals("Thu"))))
                {
                    FieldInfo scheduleName = typeof(NPC).GetField("nameOfTodaysSchedule", BindingFlags.NonPublic | BindingFlags.Instance);
                    scheduleName.SetValue(companion, "marriageJob");
                    return MasterScheduleParse(masterSchedule["marriageJob"]);
                }
                if (!Game1.isRaining && masterSchedule.ContainsKey("marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)))
                {
                    FieldInfo scheduleName = typeof(NPC).GetField("nameOfTodaysSchedule", BindingFlags.NonPublic | BindingFlags.Instance);
                    scheduleName.SetValue(companion, "marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth));
                    return MasterScheduleParse(masterSchedule["marriage_" + Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth)]);
                }
                companion.followSchedule = false;
                return null;
            }
            else
            {
                if (masterSchedule.ContainsKey(Game1.currentSeason + "_" + Game1.dayOfMonth))
                {
                    return MasterScheduleParse(masterSchedule[Game1.currentSeason + "_" + Game1.dayOfMonth]);
                }
                int friendship;
                for (friendship = (Game1.player.friendshipData.ContainsKey(companion.Name) ? (Game1.player.friendshipData[companion.Name].Points / 250) : -1); friendship > 0; friendship--)
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
                if (companion.Name.Equals("Pam") && Game1.player.mailReceived.Contains("ccVault"))
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
                friendship = (Game1.player.friendshipData.ContainsKey(companion.Name) ? (Game1.player.friendshipData[companion.Name].Points / 250) : -1);
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
                friendship = (Game1.player.friendshipData.ContainsKey(companion.Name) ? (Game1.player.friendshipData[companion.Name].Points / 250) : -1);
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
        /// Helper for GetCompanionSchedule.
        /// </summary>
        private Dictionary<int, SchedulePathDescription> MasterScheduleParse(string scheduleString)
        {
            int timeOfDay = Game1.timeOfDay;
            string[] split = scheduleString.Split(new char[] { '/' });
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
                string newKey = split[0].Split(new char[] { ' ' })[1];

                if (newKey.ToLower().Equals("season"))
                {
                    newKey = Game1.currentSeason;
                }

                try
                {
                    split =
                        Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)
                            [newKey].Split(new char[] { '/' });
                }
                catch (Exception)
                {
                    return MasterScheduleParse(
                        Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)[
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
                        return this.MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)["spring"]);
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
                split = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)[newKey2].Split(new char[] { '/' });
                routesToSkip = 1;
            }

            Point previousPosition = companion.isMarried() ? new Point(0, 23) : new Point((int)companion.DefaultPosition.X / 64, (int)companion.DefaultPosition.Y / 64);
            string previousGameLocation = companion.isMarried() ? "BusStop" : companion.DefaultMap;
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
                    if (Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name).ContainsKey("default"))
                    {
                        return MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)["default"]);
                    }
                    return MasterScheduleParse(Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)["spring"]);
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
                    SchedulePathDescription spd = (SchedulePathDescription)pathfinder.Invoke(companion, parameters);
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
                            companionRescheduleDestinationPoint = p;
                            companionRescheduleDestinationLocation = location;
                            companionRescheduleEndRouteBehavior = endOfRouteAnimation;
                            companionRescheduleEndRouteDialogue = endOfRouteMessage;
                            companionRescheduleFacingDirection = localFacingDirection;
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
        /// Another helper for GetCompanionSchedule.
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
                if (!Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name).ContainsKey(locationName + "_Replacement"))
                {
                    return true;
                }
                string[] split = Game1.content.Load<Dictionary<string, string>>("Characters\\schedules\\" + companion.Name)[locationName + "_Replacement"].Split(new char[] { ' ' });
                locationName = split[0];
                tileX = Convert.ToInt32(split[1]);
                tileY = Convert.ToInt32(split[2]);
                facingDirection = Convert.ToInt32(split[3]);
            }
            return false;
        }

        #endregion
    }
}
