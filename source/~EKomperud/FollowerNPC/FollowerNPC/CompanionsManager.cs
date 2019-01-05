using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using FollowerNPC.AI_States;

namespace FollowerNPC
{
    public class CompanionsManager
    {
        public class CompanionDialogueInfo
        {
            public Dialogue recruitDialogue;
            public Dialogue actionDialogue;
            public Dialogue automaticDismissDialogue;
            public Dialogue locationUniqueDialogue;
        }

        public enum CompanionionshipState
        {
            unavailable = -1,
            available = 0,
            recruitDialoguePushed = 1,
            locationDialoguePushed = 2,
            actionDialoguePushed = 3,
            actionDialogueNeedsRePushed = 4,
            dismissed = 5,
            rejected = 6
        }

        // Companion Objects //
        public NPC companion;
        public CompanionBuff companionBuff;
        public AI_StateMachine companionAI;
        // ***************** //

        // Companion Parameters //
        public int companionHeartThreshold;
        // ******************** //

        // Other //
        public int companionWarpTimer;
        // ***** //

        // Companion Rescheduling //
        public Point companionRescheduleDestinationPoint;
        public string companionRescheduleDestinationLocation;
        public string companionRescheduleEndRouteBehavior;
        public string companionRescheduleEndRouteDialogue;
        public int companionRescheduleFacingDirection;
        // ********************** //

        // Companion Data // 
        public SortedList<string, Dictionary<string, string>> npcDialogueScripts;
        public SortedList<string, string[]> npcCompanionAvailabilityDays;
        public Dictionary<string, bool> npcsThatCanBeRecruitedToday;
        public Dictionary<string, CompanionDialogueInfo> companionDialogueInfos;
        public Dictionary<string, bool> currentCompanionVisitedLocations;
        public Dictionary<string, CompanionionshipState> companionStates;
        public Stack<Dialogue> combatWithheldDialogue;
        // ************** //

        // Farmer Objects //
        public Farmer farmer;
        public Vector2 farmerLastTile;
        // ************** //

        // Constants //
        public const int recruitYesID = 592800;
        public const int recruitNoID = 9249200;
        public const int actionDismissID = 4736775;
        public const int actionContinueID = 7298075;
        // ********* //

        public CompanionsManager()
        {
            // Initialize Companion Data //
            SetNPCCompanionDays();
            InitializeNPCsThatCanHangOut();
            companionDialogueInfos = new Dictionary<string, CompanionDialogueInfo>();
            combatWithheldDialogue = new Stack<Dialogue>();
            // ************************* //

            // Define Companion Parameters //
            companionHeartThreshold = ModEntry.config.heartThreshold;
            // *************************** //

            // Subscribe to Events //
            ModEntry.modHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            ModEntry.modHelper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            ModEntry.modHelper.Events.Player.Warped += Player_Warped;
            ModEntry.modHelper.Events.Display.MenuChanged += Display_MenuChanged;
            ModEntry.modHelper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            ModEntry.modHelper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            ModEntry.modHelper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
            ModEntry.modHelper.Events.World.NpcListChanged += World_NpcListChanged;
        }

        #region Helpers

        #region Events

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !(companion != null) || 
                !(farmer != null) ||
                (Game1.activeClickableMenu != null && !Game1.IsMultiplayer))
                return;


            if (companion != null && companionAI != null && companionAI.currentState != null)
            {
                companionAI.currentState.Update(e);
                if (companionBuff != null)
                    companionBuff.Update();
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            farmer = Game1.player;
            companion = null;

            InitializeDialogueScripts();
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady || !(companion != null) || !(farmer != null) || companion.currentLocation == null)
                return;

            if (!farmer.isRidingHorse() && farmer.currentLocation.Equals(e.NewLocation))
            {
                Game1.warpCharacter(companion, farmer.currentLocation, farmer.getTileLocation());
                HandleCompanionLocationSpecificDialogue();
            }
            else
            {
                WarpButWaitForFarmer(e.NewLocation.Name, Point.Zero,
                    new Action(HandleCompanionLocationSpecificDialogue));
                //DelayedWarp(null, Point.Zero, 100, new Action(HandleCompanionLocationSpecificDialogue));
            }
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu != null)
            {
                // If this is a dialogue menu...
                DialogueBox db = (e.OldMenu as DialogueBox);
                if (db != null)
                {
                    // If this dialogue's speaker isn't null...
                    Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                    if (d != null && 
                        d.speaker != null &&
                        companionStates.TryGetValue(d.speaker.Name, out CompanionionshipState cs) &&
                        d.speaker.CurrentDialogue.Count == 0 &&
                        farmer.getFriendshipHeartLevelForNPC(d.speaker.Name) >= companionHeartThreshold &&
                        Game1.timeOfDay < 2200)
                    {
                        NPC n = d.speaker;

                        // Check to see if we should push a Companion Recruit Dialogue
                        if (cs == CompanionionshipState.available &&
                            npcsThatCanBeRecruitedToday.TryGetValue(n.Name, out bool available) && available)
                        {
                            TryPushCompanionRecruitDialogue(n.Name);
                        }

                        // Check to see if this was a response to a Companion Recruit Dialogue
                        else if (cs == CompanionionshipState.recruitDialoguePushed &&
                                 companionDialogueInfos.TryGetValue(n.Name, out CompanionDialogueInfo cdi) &&
                                 d.Equals(cdi.recruitDialogue))
                        {
                            HandleCompanionRecruiting(n.Name, cdi);
                        }

                        // Check to see if this was a response to a Companion Action Dialgoue
                        else if (companionDialogueInfos.TryGetValue(n.Name, out cdi) &&
                                 d.Equals(cdi.actionDialogue))
                        {
                            HandleCompanionAction(n.Name, cdi);
                        }

                        // Check to see if this was an Automatic Dismissal Dialogue
                        else if (companionDialogueInfos.TryGetValue(n.Name, out cdi) &&
                                 d.Equals(cdi.automaticDismissDialogue))
                        {
                            ActionDismissCompanion(n.Name);
                        }

                        if (companionDialogueInfos.TryGetValue(n.Name, out CompanionDialogueInfo cdi2) &&
                                 d.Equals(cdi2.locationUniqueDialogue))
                        {
                            currentCompanionVisitedLocations["companion"+n.currentLocation.Name] = true;
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
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !(companion != null) || !(farmer != null))
                return;

            TryPushCompanionActionDialogue();
            CheckForAutomaticCompanionDismissal();
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            ResetNPCsThatCanHangOut();
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            if (!(companion != null) || !(farmer != null))
                return;

            FarmerSleepingDismissCompanion();
        }

        private void World_NpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !(companion != null) || !(farmer != null))
                return;

            CheckForSwarmRoomLadderSpawn(e);
            CheckForCombatWithheldDialogue(e);
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
            npcCompanionAvailabilityDays = new SortedList<string, string[]>();
            npcCompanionAvailabilityDays.Add("Abigail", ModEntry.config.Abigail); // Goes around town M/F when married. Plays flute on Tu/Sat. Doc or Seb on Thu.       Wed || Sun                        Wed
            npcCompanionAvailabilityDays.Add("Alex", ModEntry.config.Alex); // Hangs with Haley on Wed. Doc on Tu. Visits parents on Mo when married.                Thu || Fri || Sat || Sun          Fri
            npcCompanionAvailabilityDays.Add("Elliott", ModEntry.config.Elliott); // Goes to beach M when married. Doc on Tue. Shops on Thu. Beach on Fri/Sun.          Wed || Sat                        Sat
            npcCompanionAvailabilityDays.Add("Emily", ModEntry.config.Emily); // Works Mon/Wed/Fri/Sat. Works out Tue. Doc on Thu.                                    Sun                               Sun
            npcCompanionAvailabilityDays.Add("Haley", ModEntry.config.Haley); // Photography on Mon. Doc on Tue.                                                      Wed || Thu || Fri || Sat || Sun   Fri
            npcCompanionAvailabilityDays.Add("Harvey", ModEntry.config.Harvey); // Working Mon/Tue/Wed/Thu/Fri/Sun                                                     Sat                               Sat
            npcCompanionAvailabilityDays.Add("Leah", ModEntry.config.Leah); // Groceries on Mon. Doc on Tue. Saloon on Fri/Sat.                                      Wed || Thu || Sun                 Thu
            npcCompanionAvailabilityDays.Add("Maru", ModEntry.config.Maru); // Visits parents on Mon when married. Work on Tue/Thu. Personal work on Sat.            Wed || Fri || Sun                 Wed
            npcCompanionAvailabilityDays.Add("Penny", ModEntry.config.Penny); // Out in town Mon when married. Teaches Tue/Wed/Fri. Babysits Sat. Doc on Thu.         Mon || Thu || Sun                 Mon
            npcCompanionAvailabilityDays.Add("Sam", ModEntry.config.Sam); // Works Mon/Wed. Saloon on Friday. Hangs with Seb on Sat.                                Tue || Thu || Sun                 Tue
            npcCompanionAvailabilityDays.Add("Sebastian", ModEntry.config.Sebastian); // Visits parents Mon. Hangs with Abi on Thu. Saloon on Fri. Hangs with Sam on Sat. Tue || Wed || Sun                 Tue
            npcCompanionAvailabilityDays.Add("Shane", ModEntry.config.Shane); // Works Mon/Tue/Wed/Thu/Fri. Groceries on Sat.                                         Sun                               Sun
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
            farmer.DialogueQuestionsAnswered.Remove(592800);
            farmer.DialogueQuestionsAnswered.Remove(9249200);
            farmer.DialogueQuestionsAnswered.Remove(4736775);
            farmer.DialogueQuestionsAnswered.Remove(7298075);
            companionDialogueInfos.Clear();
            string dayOfWeek = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            foreach (KeyValuePair<string, string[]> kvp in npcCompanionAvailabilityDays)
            {
                foreach (string day in kvp.Value)
                {
                    if (day.Equals(dayOfWeek) && 
                        (!Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason) || ModEntry.config.festivalHangouts))
                    {
                        npcsThatCanBeRecruitedToday[kvp.Key] = true;
                        companionStates[kvp.Key] = CompanionionshipState.available;
                        companionDialogueInfos.Add(kvp.Key, GenerateCompanionDialogueInfo(kvp.Key));
                        break;
                    }
                    else
                    {
                        npcsThatCanBeRecruitedToday[kvp.Key] = false;
                        companionStates[kvp.Key] = CompanionionshipState.unavailable;
                    }
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
            if (!(companion != null))
            {
                NPC n = Game1.getCharacterFromName(name);
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
            if (farmer.DialogueQuestionsAnswered.Contains(recruitYesID))
            {
                farmer.dialogueQuestionsAnswered.Remove(recruitYesID);
                companion = potentialCompanion;

                companionAI = new AI_StateMachine(this);

                companionBuff = CompanionBuff.InitializeBuffFromCompanionName(name, farmer, this);
                currentCompanionVisitedLocations = new Dictionary<string, bool>();
                Patches.companion = potentialCompanion;
                companion.faceTowardFarmerTimer = 0;
                HandleCompanionLocationSpecificDialogue();
                TryPushCompanionActionDialogue();
            }
            // Don't recruit if the farmer didn't say 'yes' in the Recruit Dialogue
            else if (farmer.dialogueQuestionsAnswered.Contains(recruitNoID))
            {
                // Do remove the answer, but DON'T repush the dialogue
                farmer.dialogueQuestionsAnswered.Remove(recruitNoID);
                companionStates[name] = CompanionionshipState.rejected;
            }

        }

        /// <summary>
        /// Pushes the "Companion Action Dialogue" onto the Companion's dialogue stack.
        /// This dialogue allows the farmer to dismiss the Companion, as well as potentially
        /// do other actions in the future.
        /// </summary>
        private void TryPushCompanionActionDialogue()
        {
            if (companion != null && companion.CurrentDialogue.Count == 0 && combatWithheldDialogue.Count == 0)
            {
                bool hbk = (bool)typeof(NPC).GetField("hasBeenKissedToday", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(companion);
                // check spouse for null
                if (farmer.spouse != null && farmer.spouse.Equals(companion.Name) && farmer.getFriendshipHeartLevelForNPC(companion.Name) > 9 && !hbk)
                    return;
                if (companionStates[companion.Name] == CompanionionshipState.actionDialogueNeedsRePushed)
                    companionDialogueInfos[companion.Name].actionDialogue =
                        new Dialogue(npcDialogueScripts[companion.Name]["CompanionActions"], companion);
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
            if (farmer.DialogueQuestionsAnswered.Contains(actionDismissID))
            {
                farmer.DialogueQuestionsAnswered.Remove(actionDismissID);
                ActionDismissCompanion(name);
            }
            else if (farmer.dialogueQuestionsAnswered.Contains(actionContinueID))
            {
                farmer.dialogueQuestionsAnswered.Remove(actionContinueID);
                companionStates[name] = CompanionionshipState.actionDialogueNeedsRePushed;
                companion.faceTowardFarmerTimer = 0;
            }
        }

        /// <summary>
        /// Checks to see if a companion needs to be dismissed, and draws an Automatic
        /// Dismissal Dialogue if so.
        /// </summary>
        private void CheckForAutomaticCompanionDismissal()
        {
            if (Game1.timeOfDay >= 2200)
            {
                string dialogueKey = "CompanionAutomaticDismissal";
                string dialogueValue;
                if (companion.Dialogue.TryGetValue(dialogueKey, out dialogueValue))
                {
                    CompanionDialogueInfo cdi = companionDialogueInfos[companion.Name];
                    farmer.dialogueQuestionsAnswered.Remove(recruitYesID);
                    farmer.dialogueQuestionsAnswered.Remove(recruitNoID);
                    farmer.dialogueQuestionsAnswered.Remove(actionDismissID);
                    farmer.dialogueQuestionsAnswered.Remove(actionContinueID);

                    while (companion.CurrentDialogue.Count != 0)
                        companion.CurrentDialogue.Pop();
                    companion.CurrentDialogue.Push(companionDialogueInfos[companion.Name].automaticDismissDialogue);
                    Game1.drawDialogue(companion);
                }
            }
        }

        /// <summary>
        /// Handles all the cleanup for a Companion Dismissal
        /// </summary>
        private void ActionDismissCompanion(string name)
        {
            farmer.dialogueQuestionsAnswered.Remove(recruitYesID);
            farmer.dialogueQuestionsAnswered.Remove(recruitNoID);
            farmer.dialogueQuestionsAnswered.Remove(actionContinueID);
            farmer.dialogueQuestionsAnswered.Remove(actionDismissID);
            companionAI.Dispose();
            companionAI = null;
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
        }

        /// <summary>
        /// Dismisses a companion without resetting their schedule.
        /// </summary>
        private void FarmerSleepingDismissCompanion()
        {
            farmer.dialogueQuestionsAnswered.Remove(recruitYesID);
            farmer.dialogueQuestionsAnswered.Remove(recruitNoID);
            farmer.dialogueQuestionsAnswered.Remove(actionContinueID);
            farmer.dialogueQuestionsAnswered.Remove(actionDismissID);
            companionAI.Dispose();
            companionAI = null;
            companionBuff.RemoveAndDisposeCompanionBuff();
            companionBuff = null;
            companion.faceTowardFarmerTimer = 0;
            companionStates[companion.Name] = CompanionionshipState.dismissed;
            companion = null;
            Patches.companion = null;
        }

        /// <summary>
        /// Sets the companion dialogue for the current location.
        /// </summary>
        private void HandleCompanionLocationSpecificDialogue()
        {
            MineShaft ms = companion.currentLocation as MineShaft;
            // If this is a mineshaft and there are enemies, remove current dialogue
            if (ms != null && ms.characters.Count > 1)
            {
                if (companion.CurrentDialogue.Count != 0)
                {
                    while (companion.CurrentDialogue.Count != 0)
                        combatWithheldDialogue.Push(companion.CurrentDialogue.Pop());
                }
            }
            // Otherwise,
            else
            {
                // Re-push combat withheld dialgoue if there is some
                if (combatWithheldDialogue.Count > 0)
                {
                    while (combatWithheldDialogue.Count > 0)
                        companion.CurrentDialogue.Push(combatWithheldDialogue.Pop());
                }
                else
                {
                    string dialogueKey = "companion" + farmer.currentLocation.Name;
                    string dialogueValue;
                    // push this location's unique dialogue if there is some...
                    if (companion.Dialogue.TryGetValue(dialogueKey, out dialogueValue))
                    {
                        if (!currentCompanionVisitedLocations.TryGetValue(dialogueKey, out bool v) || !v)
                        {
                            while (companion.CurrentDialogue.Count != 0)
                                companion.CurrentDialogue.Pop();
                            Dialogue d = new Dialogue(dialogueValue, companion);
                            companionDialogueInfos[companion.Name].locationUniqueDialogue = d;
                            companion.CurrentDialogue.Push(d);
                            currentCompanionVisitedLocations[dialogueKey] = false;
                        }
                    }
                    // or if there isn't location-unique dialogue, remove the current lcation-unique dialogue.
                    else
                    {
                        // if we're not in a mineshaft, and the companion's current dialogue is a location-unique dialogue
                        if (ms == null &&
                        companion.CurrentDialogue.Count != 0 &&
                        companion.CurrentDialogue.Peek().Equals(companionDialogueInfos[companion.Name].locationUniqueDialogue))
                        {
                            while (companion.CurrentDialogue.Count != 0)
                                companion.CurrentDialogue.Pop();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check to see if the companion's dialogue needs to be withheld for combat,
        /// and withhold it if so.
        /// </summary>
        private void CheckForCombatWithheldDialogue(NpcListChangedEventArgs e)
        {
            MineShaft ms = e.Location as MineShaft;
            if (combatWithheldDialogue.Count > 0 && ms != null && e.Removed != null && e.Removed.Count() > 0)
            {
                NPC r = e.Removed.First();
                StardewValley.Monsters.Monster m = r as StardewValley.Monsters.Monster;
                if (m != null && !CheckForMonstersInThisLocation(ms))
                {
                    while (combatWithheldDialogue.Count > 0)
                        companion.CurrentDialogue.Push(combatWithheldDialogue.Pop());
                }
            }
        }

        /// <summary>
        /// Returns true if the farmer is in the mines, and there are any monsters on their floor
        /// </summary>
        private bool CheckForMonstersInThisLocation(GameLocation l)
        {
            foreach (NPC n in l.characters)
            {
                StardewValley.Monsters.Monster m = n as StardewValley.Monsters.Monster;
                if (m != null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a CompanionDialogueInfo object for the NPC with parameter:name. A CompanionDialogueInfo
        /// contains references to the Recruit Dialogue, 'yes' response ID, and Action Dialogue.
        /// </summary>
        private CompanionDialogueInfo GenerateCompanionDialogueInfo(string name)
        {
            NPC n = Game1.getCharacterFromName(name);
            return new CompanionDialogueInfo()
            {
                recruitDialogue = new Dialogue(npcDialogueScripts[name]["Companion"], n),
                actionDialogue = new Dialogue(npcDialogueScripts[name]["CompanionActions"], n),
                automaticDismissDialogue = new Dialogue(npcDialogueScripts[name]["CompanionAutomaticDismissal"], n)
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
            if (companion.currentLocation !=  null)
                Game1.warpCharacter(companion, location, tileLocation);
            afterWarpAction.Invoke();
        }

        private async void WarpButWaitForFarmer(String location, Point tileLocation, Action afterWarpAction)
        {
            await Task.Run(() => Timer(50));
            while (farmer.currentLocation.Name != location)
                await Task.Run(() => Timer(15));
            location = location != null ? location : farmer.currentLocation.Name;
            tileLocation = tileLocation != Point.Zero ? tileLocation : farmer.getTileLocationPoint();
            //if (companion.currentLocation != null)
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
                companion.CurrentDialogue.Push(new Dialogue(Game1.content.LoadString(companionRescheduleEndRouteDialogue), companion));
            companion.faceDirection(companionRescheduleFacingDirection);

            companion = null;
            Patches.companion = null;

            List<string> changes = new List<string>(12);
            foreach (KeyValuePair<string, bool> npcKvP in npcsThatCanBeRecruitedToday)
            {
                NPC n = Game1.getCharacterFromName(npcKvP.Key);
                if (npcKvP.Value && n.CurrentDialogue.Count == 0)
                {
                    if (!(companion != null))
                    {
                        changes.Add(npcKvP.Key);
                        n.CurrentDialogue.Push(companionDialogueInfos[npcKvP.Key].recruitDialogue);
                        companionStates[npcKvP.Key] = CompanionionshipState.recruitDialoguePushed;
                    }
                }
            }

            foreach (string change in changes)
            {
                npcsThatCanBeRecruitedToday[change] = false;
            }
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
        

        /// <summary>
        /// Checks if you are in a swarm room, and spawns a ladder when appropriate if so
        /// </summary>
        private void CheckForSwarmRoomLadderSpawn(NpcListChangedEventArgs e)
        {
            MineShaft ms = e.Location as MineShaft;
            if (ms != null && ms.mustKillAllMonstersToAdvance() && e.Removed != null && e.Removed.Count() > 0)
            {
                NPC r = e.Removed.First();
                StardewValley.Monsters.Monster m = r as StardewValley.Monsters.Monster;
                if (m != null && !CheckForMonstersInThisLocation(ms))
                {
                    Vector2 p = new Vector2(r.GetBoundingBox().Center.X, r.GetBoundingBox().Center.Y) / 64f;
                    p.X = ((int)p.X);
                    p.Y = ((int)p.Y);
                    r.Name = "ignoreMe";
                    Rectangle tileRect = new Rectangle((int)p.X * 64, (int)p.Y * 64, 64, 64);
                    if (!ms.isTileOccupied(p, "ignoreMe") && ms.isTileOnClearAndSolidGround(p) && !Game1.player.GetBoundingBox().Intersects(tileRect) && ms.doesTileHaveProperty((int)p.X, (int)p.Y, "Type", "Back") != null && ms.doesTileHaveProperty((int)p.X, (int)p.Y, "Type", "Back").Equals("Stone"))
                    {
                        ms.createLadderAt(p, "hoeHit");
                        return;
                    }
                    if (ms.mustKillAllMonstersToAdvance() && !CheckForMonstersInThisLocation(ms))
                    {
                        FieldInfo tileBeneathLadderField = typeof(StardewValley.Locations.MineShaft).GetField("netTileBeneathLadder", BindingFlags.NonPublic | BindingFlags.Instance);
                        Netcode.NetVector2 tileBeneathLadder = (Netcode.NetVector2)tileBeneathLadderField.GetValue(ms);
                        p = new Vector2(((int)tileBeneathLadder.X), ((int)tileBeneathLadder.Y));
                        ms.createLadderAt(p, "newArtifact");
                        if (ms.mustKillAllMonstersToAdvance() && Game1.player.currentLocation == ms)
                        {
                            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MineShaft.cs.9484"));
                        }
                    }
                }
            }
        }

        #endregion
    }
}
