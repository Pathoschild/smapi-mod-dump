using NpcAdventure.StateMachine.StateFeatures;
using NpcAdventure.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using System.Reflection;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using NpcAdventure.Buffs;
using StardewModdingAPI;
using NpcAdventure.AI;
using Microsoft.Xna.Framework.Graphics;
using NpcAdventure.Events;
using NpcAdventure.Internal;

namespace NpcAdventure.StateMachine.State
{
    internal class RecruitedState : CompanionState, IRequestedDialogueCreator, IDialogueDetector
    {
        private AI_StateMachine ai;
        private Dialogue dismissalDialogue;
        private Dialogue currentLocationDialogue;

        public bool CanCreateDialogue { get; private set; }
        private BuffManager BuffManager { get; set; }
        public ISpecialModEvents SpecialEvents { get; }

        public RecruitedState(CompanionStateMachine stateMachine, IModEvents events, ISpecialModEvents specialEvents, IMonitor monitor) : base(stateMachine, events, monitor)
        {
            this.BuffManager = new BuffManager(stateMachine.Companion, stateMachine.CompanionManager.Farmer, stateMachine.ContentLoader, this.monitor);
            this.SpecialEvents = specialEvents;
        }

        public override void Entry()
        {
            this.ai = new AI_StateMachine(this.StateMachine, this.StateMachine.CompanionManager.Hud, this.Events, this.monitor);

            if (this.StateMachine.Companion.doingEndOfRouteAnimation.Value)
                this.FinishScheduleAnimation();

            this.StateMachine.Companion.faceTowardFarmerTimer = 0;
            this.StateMachine.Companion.movementPause = 0;
            this.StateMachine.Companion.followSchedule = false;
            this.StateMachine.Companion.Schedule = null;
            this.StateMachine.Companion.controller = null;
            this.StateMachine.Companion.temporaryController = null;
            this.StateMachine.Companion.eventActor = true;
            this.StateMachine.Companion.farmerPassesThrough = true;

            if (this.StateMachine.Companion.isMarried() && Patches.SpouseReturnHomePatch.recruitedSpouses.IndexOf(this.StateMachine.Companion.Name) < 0)
            {
                // Avoid returning recruited wife/husband to FarmHouse when is on Farm and it's after 1pm
                Patches.SpouseReturnHomePatch.recruitedSpouses.Add(this.StateMachine.Companion.Name);
            }

            this.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            this.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
            this.Events.Player.Warped += this.Player_Warped;
            this.SpecialEvents.RenderedLocation += this.SpecialEvents_RenderedLocation;

            if (DialogueHelper.GetVariousDialogueString(this.StateMachine.Companion, "companionRecruited", out string dialogueText))
                this.StateMachine.Companion.setNewDialogue(dialogueText);
            this.CanCreateDialogue = true;

            foreach (string skill in this.StateMachine.Metadata.PersonalSkills)
            {
                string text = this.StateMachine.ContentLoader.LoadString($"Strings/Strings:skill.{skill}", this.StateMachine.Companion.displayName)
                        + Environment.NewLine
                        + this.StateMachine.ContentLoader.LoadString($"Strings/Strings:skillDescription.{skill}").Replace("#", Environment.NewLine);
                this.StateMachine.CompanionManager.Hud.AddSkill(skill, text);
            }

            this.StateMachine.CompanionManager.Hud.AssignCompanion(this.StateMachine.Companion);
            this.BuffManager.AssignBuffs();
            this.ai.Setup();
        }

        private void SpecialEvents_RenderedLocation(object sender, ILocationRenderedEventArgs e)
        {
            this.ai.Draw(e.SpriteBatch);
        }

        /// <summary>
        /// Animate last sequence of current schedule animation
        /// </summary>
        private void FinishScheduleAnimation()
        {
            // Prevent animation freeze glitch
            this.StateMachine.Companion.Sprite.standAndFaceDirection(this.StateMachine.Companion.FacingDirection);

            // And then play finish animation "end of route animation" when companion is recruited
            // Must be called via reflection, because they are private members of NPC class
            this.StateMachine.Reflection.GetMethod(this.StateMachine.Companion, "finishEndOfRouteAnimation").Invoke();
            this.StateMachine.Companion.doingEndOfRouteAnimation.Value = false;
            this.StateMachine.Reflection.GetField<Boolean>(this.StateMachine.Companion, "currentlyDoingEndOfRouteAnimation").SetValue(false);
        }

        public override void Exit()
        {
            this.BuffManager.ReleaseBuffs();
            this.ai.Dispose();

            if (Patches.SpouseReturnHomePatch.recruitedSpouses.IndexOf(this.StateMachine.Companion.Name) >= 0)
            {
                // Allow dissmised wife/husband to return to FarmHouse when is on Farm
                Patches.SpouseReturnHomePatch.recruitedSpouses.Remove(this.StateMachine.Companion.Name);
            }

            this.StateMachine.Companion.eventActor = false;
            this.StateMachine.Companion.farmerPassesThrough = false;
            this.CanCreateDialogue = false;

            this.SpecialEvents.RenderedLocation -= this.SpecialEvents_RenderedLocation;
            this.Events.GameLoop.UpdateTicked -= this.GameLoop_UpdateTicked;
            this.Events.GameLoop.TimeChanged -= this.GameLoop_TimeChanged;
            this.Events.Player.Warped -= this.Player_Warped;

            this.ai = null;
            this.dismissalDialogue = null;
            this.StateMachine.CompanionManager.Hud.Reset();
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            this.StateMachine.Companion.clearSchedule();

            if (e.NewTime >= 2200)
            {
                NPC companion = this.StateMachine.Companion;
                Dialogue dismissalDialogue = new Dialogue(DialogueHelper.GetDialogueString(companion, "companionDismissAuto"), companion);
                this.dismissalDialogue = dismissalDialogue;
                this.StateMachine.Companion.doEmote(24);
                this.StateMachine.Companion.updateEmote(Game1.currentGameTime);
                DialogueHelper.DrawDialogue(dismissalDialogue);
            }

            MineShaft mines = this.StateMachine.Companion.currentLocation as MineShaft;

            // Fix spawn ladder if area is infested and all monsters is killed but NPC following us
            if (mines != null && mines.mustKillAllMonstersToAdvance())
            {
                var monsters = from c in mines.characters where c.IsMonster select c;
                if (monsters.Count() == 0)
                {
                    Vector2 vector2 = this.StateMachine.Reflection.GetProperty<Vector2>(mines, "tileBeneathLadder").GetValue();
                    if (mines.getTileIndexAt(Utility.Vector2ToPoint(vector2), "Buildings") == -1)
                        mines.createLadderAt(vector2, "newArtifact");
                }
            }
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (e.IsMultipleOf(20))
                this.FixProblemsWithNPC();

            this.ai.Update(e);
        }

        private void FixProblemsWithNPC()
        {
            this.StateMachine.Companion.movementPause = 0;
            this.StateMachine.Companion.followSchedule = false;
            this.StateMachine.Companion.Schedule = null;
            this.StateMachine.Companion.controller = null;
            this.StateMachine.Companion.temporaryController = null;
            this.StateMachine.Companion.eventActor = true;
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            NPC companion = this.StateMachine.Companion;
            Farmer farmer = this.StateMachine.CompanionManager.Farmer;
            Dictionary<string, string> bubbles = this.StateMachine.ContentLoader.LoadStrings("Strings/SpeechBubbles");

            // Warp companion to farmer if it's needed
            if (companion.currentLocation != e.NewLocation)
                this.ai.ChangeLocation(e.NewLocation);

            // Show above head bubble text for location
            if (Game1.random.NextDouble() > 66f && DialogueHelper.GetBubbleString(bubbles, companion, e.NewLocation, out string bubble))
                companion.showTextAboveHead(bubble, preTimer: 250);

            // Push new location dialogue
            this.TryPushLocationDialogue(e.NewLocation);
        }

        private Dialogue GenerateLocationDialogue(GameLocation location, NPC companion)
        {
            if (DialogueHelper.GenerateStaticDialogue(companion, location, "companionOnce") is CompanionDialogue dialogueOnce
                && !this.StateMachine.CompanionManager.Farmer.hasOrWillReceiveMail(dialogueOnce.Tag))
            {
                // Remember only once spoken dialogue
                dialogueOnce.Remember = true;
                return dialogueOnce;
            }

            // Generate standard companion various dialogue if no once dialogue defined or once dialogue spoken
            return DialogueHelper.GenerateDialogue(companion, location, "companion");
        }

        private bool TryPushLocationDialogue(GameLocation location)
        {
            NPC companion = this.StateMachine.Companion;
            Stack<Dialogue> temp = new Stack<Dialogue>(this.StateMachine.Companion.CurrentDialogue.Count);
            Dialogue newDialogue = this.GenerateLocationDialogue(location, companion);

            if ((newDialogue == null && this.currentLocationDialogue == null) || (newDialogue != null && newDialogue.Equals(this.currentLocationDialogue)))
                return false;

            // Remove old location dialogue
            while (this.StateMachine.Companion.CurrentDialogue.Count > 0)
            {
                Dialogue d = this.StateMachine.Companion.CurrentDialogue.Pop();

                if (!d.Equals(this.currentLocationDialogue))
                    temp.Push(d);
            }

            while (temp.Count > 0)
                this.StateMachine.Companion.CurrentDialogue.Push(temp.Pop());

            this.currentLocationDialogue = newDialogue;

            if (newDialogue != null)
            {
                this.StateMachine.Companion.CurrentDialogue.Push(newDialogue); // Push new location dialogue
                return true;
            }

            return false;
        }

        public void CreateRequestedDialogue()
        {
            if (this.ai != null && this.ai.PerformAction())
                return;

            Farmer leader = this.StateMachine.CompanionManager.Farmer;
            GameLocation location = this.StateMachine.CompanionManager.Farmer.currentLocation;
            string question = this.StateMachine.ContentLoader.LoadString("Strings/Strings:recruitedWant");
            Response[] responses =
            {
                new Response("bag", this.StateMachine.ContentLoader.LoadString("Strings/Strings:recruitedWant.bag")),
                new Response("dismiss", this.StateMachine.ContentLoader.LoadString("Strings/Strings:recruitedWant.dismiss")),
                new Response("nothing", this.StateMachine.ContentLoader.LoadString("Strings/Strings:recruitedWant.nothing")),
            };

            location.createQuestionDialogue(question, responses, (_, answer) => {
                if (answer != "nothing")
                {
                    this.StateMachine.Companion.Halt();
                    this.StateMachine.Companion.facePlayer(leader);
                    this.ReactOnAsk(this.StateMachine.Companion, leader, answer);
                }
            }, this.StateMachine.Companion);
        }

        private void ReactOnAsk(NPC companion, Farmer leader, string action)
        {
            switch (action)
            {
                case "dismiss":
                    Dialogue dismissalDialogue = new Dialogue(DialogueHelper.GetDialogueString(companion, "companionDismiss"), companion);
                    this.dismissalDialogue = dismissalDialogue;
                    DialogueHelper.DrawDialogue(dismissalDialogue);
                    break;
                case "bag":
                    Chest bag = this.StateMachine.Bag;
                    this.StateMachine.Companion.currentLocation.playSound("openBox");
                    Game1.activeClickableMenu = new ItemGrabMenu(bag.items, false, true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems), new ItemGrabMenu.behaviorOnItemSelect(bag.grabItemFromInventory), this.StateMachine.Companion.displayName, new ItemGrabMenu.behaviorOnItemSelect(bag.grabItemFromChest), false, true, true, true, true, 1, null, -1, this.StateMachine.Companion);
                    break;
            }
        }

        public void OnDialogueSpeaked(Dialogue speakedDialogue)
        {
            if (speakedDialogue == this.dismissalDialogue)
            {
                // After companion speaked a dismissal dialogue dismiss (unrecruit) companion who speaked that
                this.StateMachine.Dismiss(Game1.timeOfDay >= 2200);
            }
        }
    }
}
