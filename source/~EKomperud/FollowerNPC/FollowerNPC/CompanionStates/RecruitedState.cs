using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using FollowerNPC.AI_States;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace FollowerNPC.CompanionStates
{
    public class RecruitedState: CompanionState
    {
        public AI_StateMachine ai;
        public CompanionBuff buff;
        public Stack<Dialogue> combatWithheldDialogue;
        public Dictionary<string, bool> seenDialogueForLocations;

        Random r;

        private string locationDialogueName;
        private bool locationDialoguePushed;
        private bool actionDialoguePushed;

        public RecruitedState(CompanionStateMachine sm) : base(sm)
        {

        }

        public override void EnterState()
        {
            base.EnterState();

            ai = new AI_StateMachine(this);
            combatWithheldDialogue = new Stack<Dialogue>();
            buff = CompanionBuff.InitializeBuffFromCompanionName(stateMachine.companion.Name, stateMachine.manager.farmer,
                stateMachine.manager);
            seenDialogueForLocations = new Dictionary<string, bool>();
            r = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed + Game1.timeOfDay);
            Patches.companion = stateMachine.companion.Name;
            stateMachine.companion.faceTowardFarmerTimer = 0;
            stateMachine.companion.movementPause = 0;

            stateMachine.companion.controller = null;
            stateMachine.companion.temporaryController = null;

            ModEntry.modHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            ModEntry.modHelper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            ModEntry.modHelper.Events.GameLoop.DayEnding += GameLoop_DayEnding;

            ModEntry.modHelper.Events.Player.Warped += Player_Warped;

            ModEntry.modHelper.Events.Display.MenuChanged += Display_MenuChanged;

            ModEntry.modHelper.Events.World.NpcListChanged += World_NpcListChanged; ;

        }

        public override void ExitState()
        {
            base.ExitState();

            ai?.Dispose();
            ai = null;
            buff?.RemoveAndDisposeCompanionBuff();
            buff = null;
            combatWithheldDialogue?.Clear();
            combatWithheldDialogue = null;
            seenDialogueForLocations?.Clear();
            seenDialogueForLocations = null;
            r = null;
            Patches.companion = null;

            ModEntry.modHelper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            ModEntry.modHelper.Events.GameLoop.TimeChanged -= GameLoop_TimeChanged;
            ModEntry.modHelper.Events.GameLoop.DayEnding -= GameLoop_DayEnding;

            ModEntry.modHelper.Events.Player.Warped -= Player_Warped;

            ModEntry.modHelper.Events.Display.MenuChanged -= Display_MenuChanged;

            ModEntry.modHelper.Events.World.NpcListChanged -= World_NpcListChanged;
        }

        #region Event Handlers
        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady ||
                (Game1.activeClickableMenu != null && !Game1.IsMultiplayer))
                return;

            if (ai?.currentState != null)
            {
                ai.currentState.Update(e);
            }

            if (buff != null)
            {
                buff.Update();
            }

        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            try
            {
                TryPushCompanionActionDialogue();
                CheckForAutomaticCompanionDismissal();
            }
            catch (Exception exception)
            {
                stateMachine.EmergencyExit(exception);
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            stateMachine.DayEndingDismiss();
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if (!Context.IsWorldReady || stateMachine.companion.currentLocation == null)
                return;

            Farmer f = stateMachine.manager.farmer;
            NPC c = stateMachine.companion;
            if (!f.isRidingHorse() && f.currentLocation.Equals(e.NewLocation))
            {
                Game1.warpCharacter(c, f.currentLocation, f.getTileLocation());
                TryPushLocationDialogue();
            }
            else
            {
                WarpButWaitForFarmer(e.NewLocation.Name, Point.Zero,
                    new Action(TryPushLocationDialogue));
            }
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu != null)
            {
                DialogueBox db = (e.OldMenu as DialogueBox);
                if (db != null)
                {
                    Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                    if (d != null && d.speaker != null && d.speaker.Equals(stateMachine.companion))
                    {
                        if (d.Equals(stateMachine.actionDialogue))
                        {
                            HandleActionDialogue();
                        }

                        else if (d.Equals(stateMachine.locationDialogue))
                        {
                            seenDialogueForLocations[stateMachine.companion.currentLocation.Name] = true;
                            locationDialoguePushed = false;
                        }

                        else if (d.Equals(stateMachine.automaticDismissDialogue))
                        {
                            stateMachine.AutomaticDismiss();
                        }

                        stateMachine.companion.faceTowardFarmerTimer = 0;
                        stateMachine.companion.movementPause = 0;
                    }
                }
            }
        }

        private void World_NpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            CheckForSwarmRoomLadderSpawn(e);
            CheckForCombatWithheldDialogue(e);
        }
        #endregion

        #region Dialogue
        private void HandleActionDialogue()
        {
            Farmer f = stateMachine.manager.farmer;
            if (f.dialogueQuestionsAnswered.Contains(CompanionsManager.actionContinueID))
            {
                f.dialogueQuestionsAnswered.Remove(CompanionsManager.actionContinueID);
                stateMachine.companion.faceTowardFarmerTimer = 0;
                actionDialoguePushed = false;
            }
            else if (f.dialogueQuestionsAnswered.Contains(CompanionsManager.actionDismissID))
            {
                f.dialogueQuestionsAnswered.Remove(CompanionsManager.actionDismissID);
                stateMachine.ActionDismiss();
            }
            else if (f.dialogueQuestionsAnswered.Contains(CompanionsManager.actionSpecialID))
            {
                f.dialogueQuestionsAnswered.Remove(CompanionsManager.actionSpecialID);
                buff.SpecialAction();
                stateMachine.companion.faceTowardFarmerTimer = 0;
                actionDialoguePushed = false;
            }
        }

        private void TryPushCompanionActionDialogue()
        {
            if (!actionDialoguePushed && stateMachine.companion.CurrentDialogue.Count == 0 &&
                !locationDialoguePushed && combatWithheldDialogue.Count == 0)
            {
                NPC c = stateMachine.companion;
                Farmer f = stateMachine.manager.farmer;
                bool hbk = (bool)typeof(NPC).GetField("hasBeenKissedToday", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(c);
                // check spouse for null
                if (f.spouse != null && f.spouse.Equals(c.Name) && f.getFriendshipHeartLevelForNPC(c.Name) > 9 && !hbk)
                    return;

                Dialogue d = GenerateAnActionDialogue();
                stateMachine.actionDialogue = d ?? throw new Exception(
                        "Tried to push an action dialogue, but there were no action strings for this character!");
                if (CheckForMissingResponseKeys(d))
                    throw new Exception("There were missing dialogue response keys for this character!");
                stateMachine.companion.CurrentDialogue.Push(stateMachine.actionDialogue);
                actionDialoguePushed = true;
            }
        }

        private Dialogue GenerateAnActionDialogue()
        {
            List<string> ret = new List<string>();
            bool repeat = false;

            GetDialogue:
            // If this companion is married to the farmer
            if (stateMachine.manager.farmer.spouse != null && stateMachine.manager.farmer.spouse.Equals(stateMachine.companion.Name))
            {
                string recruitFriendKey = "Companion-Action-Friend";
                string recruitSpouseKey = "Companion-Action-Spouse";
                string recruitSpouseOverrideKey = "Companion-Action-SpouseOverride";

                // If there are SpouseOverride dialogue(s)
                if (GetAnyDialogueValuesForDialogueKey(recruitSpouseOverrideKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }

                // Else, look for Spouse and Friend strings
                else if (GetAnyDialogueValuesForDialogueKey(recruitSpouseKey, ref ret) |
                         GetAnyDialogueValuesForDialogueKey(recruitFriendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }
            }
            // Otherwise, if they are just a friend
            else
            {
                string recruitFriendKey = "Companion-Action-Friend";

                // Look for Friend strings
                if (GetAnyDialogueValuesForDialogueKey(recruitFriendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }
            }

            if (ret.Count == 0 && !repeat)
            {
                repeat = true;
                foreach (KeyValuePair<string, string> kvp in stateMachine.script)
                    stateMachine.companion.Dialogue[kvp.Key] = kvp.Value;
                goto GetDialogue;
            }
            return null;
        }

        private void TryPushLocationDialogue()
        {
            NPC c = stateMachine.companion;
            bool monstersInLocation = CheckForMonstersInThisLocation(c.currentLocation);

            // If there is combat withheld dialogue, and there are no monsters in this current location...
            if (combatWithheldDialogue.Count != 0 && !monstersInLocation)
            {
                PushCombatWithheldDialogueToCompanionDialogue();
            }

            // If this is a dungeon and there are enemies, remove current dialogue
            MineShaft ms = c.currentLocation as MineShaft;
            Woods w = c.currentLocation as Woods;
            if ((ms != null || w != null) && monstersInLocation)
            {
                PushCompanionDialogueToCombatWithheldDialogue();
            }
            // Else, proceed as normal!
            else
            {
                // If there is location-dialogue for this location...
                Dialogue d = GenerateALocationDialogue();
                if (d != null)
                {
                    // And it hasn't been seen yet...
                    if (!seenDialogueForLocations.TryGetValue(c.currentLocation.Name, out bool visited) || !visited)
                    {
                        // If a location dialogue has already been pushed though...
                        if (locationDialoguePushed)
                        {
                            // Return if it is the dialogue for this location...
                            if (locationDialogueName != null && locationDialogueName.Equals(c.currentLocation.Name))
                                return;
                            // Or remove it if it's from another location
                            RemoveLocationDialogueFromCompanion();
                        }
                        stateMachine.companion.CurrentDialogue.Push(d);
                        stateMachine.locationDialogue = d;
                        locationDialoguePushed = true;
                        locationDialogueName = c.currentLocation.Name;
                        seenDialogueForLocations[c.currentLocation.Name] = false;
                    }
                }
                // If there isn't location-dialogue for this location...
                else
                {
                    // and we're not in a mineshaft, and the companion has another location's dialogue pushed, remove it.
                    if (ms == null && locationDialoguePushed)
                    {
                        RemoveLocationDialogueFromCompanion();
                    }
                }
            }
        }

        private Dialogue GenerateALocationDialogue()
        {
            List<string> ret = new List<string>();
            string location = stateMachine.companion.currentLocation.Name;

            // If this companion is married to the farmer
            if (stateMachine.manager.farmer.spouse != null && stateMachine.manager.farmer.spouse.Equals(stateMachine.companion.Name))
            {
                string friendKey = "companion-"+location+"-Friend";
                string spouseKey = "companion-"+location+"-Spouse";
                string spouseOverrideKey = "companion-"+location+"-SpouseOverride";

                // If there are SpouseOverride dialogue(s)
                if (GetAnyDialogueValuesForDialogueKey(spouseOverrideKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }

                // Else, look for Spouse and Friend strings
                else if (GetAnyDialogueValuesForDialogueKey(spouseKey, ref ret) |
                         GetAnyDialogueValuesForDialogueKey(friendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }
            }
            // Otherwise, if they are just a friend
            else
            {
                string friendKey = "companion-" + location + "-Friend";

                // Look for Friend strings
                if (GetAnyDialogueValuesForDialogueKey(friendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }
            }
            return null;
        }

        private bool RemoveLocationDialogueFromCompanion()
        {
            Stack<Dialogue> temp = new Stack<Dialogue>(stateMachine.companion.CurrentDialogue.Count);
            Dialogue t;
            bool ret = false;
            while (stateMachine.companion.CurrentDialogue.Count != 0)
            {
                t = stateMachine.companion.CurrentDialogue.Pop();
                if (!t.Equals(stateMachine.locationDialogue))
                    temp.Push(t);
                else
                    ret = true;
            }
            while (temp.Count != 0)
                stateMachine.companion.CurrentDialogue.Push(temp.Pop());
            stateMachine.locationDialogue = null;
            locationDialoguePushed = false;
            return ret;
        }

        private void CheckForCombatWithheldDialogue(NpcListChangedEventArgs e)
        {
            MineShaft ms = e.Location as MineShaft;
            if (combatWithheldDialogue.Count > 0 && ms != null && e.Removed != null && e.Removed.Count() > 0)
            {
                Monster m = GetAMonsterFromNPCList(e.Removed);
                if (m != null && !CheckForMonstersInThisLocation(ms))
                {
                    PushCombatWithheldDialogueToCompanionDialogue();
                }
            }
        }

        private void PushCompanionDialogueToCombatWithheldDialogue()
        {
            if (stateMachine.companion.CurrentDialogue.Count != 0)
            {
                while (stateMachine.companion.CurrentDialogue.Count != 0)
                    combatWithheldDialogue.Push(stateMachine.companion.CurrentDialogue.Pop());
            }
        }

        private void PushCombatWithheldDialogueToCompanionDialogue()
        {
            if (combatWithheldDialogue.Count != 0)
            {
                while (combatWithheldDialogue.Count != 0)
                    stateMachine.companion.CurrentDialogue.Push(combatWithheldDialogue.Pop());
            }
        }

        private void CheckForAutomaticCompanionDismissal()
        {
            if (Game1.timeOfDay >= 2200)
            {
                stateMachine.manager.farmer.dialogueQuestionsAnswered.Remove(CompanionsManager.recruitYesID);
                stateMachine.manager.farmer.dialogueQuestionsAnswered.Remove(CompanionsManager.recruitNoID);
                stateMachine.manager.farmer.dialogueQuestionsAnswered.Remove(CompanionsManager.actionDismissID);
                stateMachine.manager.farmer.dialogueQuestionsAnswered.Remove(CompanionsManager.actionContinueID);
                Dialogue d = GenerateAnAutomaticDismissDialogue();
                stateMachine.automaticDismissDialogue = d ?? throw new Exception("Tried to push a dismissal dialogue, but there were no dismissal strings for this character!");
                if (CheckForMissingResponseKeys(d))
                    throw new Exception("There were missing dialogue response keys for this character!");
                while (stateMachine.companion.CurrentDialogue.Count != 0)
                    stateMachine.companion.CurrentDialogue.Pop();
                stateMachine.companion.CurrentDialogue.Push(d);
                stateMachine.automaticDismissDialogue = d;
                Game1.drawDialogue(stateMachine.companion);
            }
        }

        private Dialogue GenerateAnAutomaticDismissDialogue()
        {
            List<string> ret = new List<string>();
            bool repeat = false;

            GetDialogue:
            // If this companion is married to the farmer
            if (stateMachine.manager.farmer.spouse != null && stateMachine.manager.farmer.spouse.Equals(stateMachine.companion.Name))
            {
                string friendKey = "Companion-AutomaticDismissal-Friend";
                string spouseKey = "Companion-AutomaticDismissal-Spouse";
                string spouseOverrideKey = "Companion-AutomaticDismissal-SpouseOverride";

                // If there are SpouseOverride dialogue(s)
                if (GetAnyDialogueValuesForDialogueKey(spouseOverrideKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }

                // Else, look for Spouse and Friend strings
                else if (GetAnyDialogueValuesForDialogueKey(spouseKey, ref ret) |
                         GetAnyDialogueValuesForDialogueKey(friendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }
            }
            // Otherwise, if they are just a friend
            else
            {
                string friendKey = "Companion-AutomaticDismissal-Friend";

                // Look for Friend strings
                if (GetAnyDialogueValuesForDialogueKey(friendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], stateMachine.companion);
                }
            }

            if (ret.Count == 0 && !repeat)
            {
                repeat = true;
                goto GetDialogue;
            }
            return null;
        }
        #endregion

        #region Misc Helpers
        private void CheckForSwarmRoomLadderSpawn(NpcListChangedEventArgs e)
        {
            MineShaft ms = e.Location as MineShaft;
            if (ms != null && ms.mustKillAllMonstersToAdvance() && e.Removed != null && e.Removed.Count() > 0)
            {
                Monster m = GetAMonsterFromNPCList(e.Removed);
                if (m != null && !CheckForMonstersInThisLocation(ms))
                {
                    Vector2 p = new Vector2(m.GetBoundingBox().Center.X, m.GetBoundingBox().Center.Y) / 64f;
                    p.X = ((int)p.X);
                    p.Y = ((int)p.Y);
                    m.Name = "ignoreMe";
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

        private Monster GetAMonsterFromNPCList(IEnumerable<NPC> npcs)
        {
            foreach (NPC n in npcs)
            {
                if ((n as Monster) != null)
                    return n as Monster;
            }
            return null;
        }

        private async void WarpButWaitForFarmer(String location, Point tileLocation, Action afterWarpAction)
        {
            //await Task.Run(() => Timer(50));
            while (!stateMachine.manager.farmer.currentLocation.Name.Equals(location))
                await Task.Run(() => Timer(15));
            location = location != null ? location : stateMachine.manager.farmer.currentLocation.Name;
            tileLocation = tileLocation != Point.Zero ? tileLocation : stateMachine.manager.farmer.getTileLocationPoint();
            //if (companion.currentLocation != null)
            Game1.warpCharacter(stateMachine.companion, location, tileLocation);
            afterWarpAction.Invoke();
        }

        private int Timer(int milliseconds)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (watch.ElapsedMilliseconds < milliseconds) ;
            return 0;
        }
        #endregion
    }
}
