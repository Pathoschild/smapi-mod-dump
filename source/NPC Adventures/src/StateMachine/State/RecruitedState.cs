using NpcAdventure.StateMachine.StateFeatures;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using NpcAdventure.Buffs;
using StardewModdingAPI;
using NpcAdventure.AI;
using NpcAdventure.Events;
using NpcAdventure.Dialogues;
using NpcAdventure.Utils;

namespace NpcAdventure.StateMachine.State
{
    internal class RecruitedState : CompanionState, IActionPerformer, IDialogueDetector
    {
        private AI_StateMachine ai;
        private Dialogue dismissalDialogue;
        private Dialogue currentLocationDialogue;
        private Dialogue recruitedDialogue;
        private int dialoguePushTime;
        private int timeOfRecruit;

        public bool CanPerformAction { get; private set; }
        private BuffManager BuffManager { get; set; }
        public ISpecialModEvents SpecialEvents { get; }
        public int TimeToBye { get; private set; }

        public RecruitedState(CompanionStateMachine stateMachine, IModEvents events, ISpecialModEvents specialEvents, IMonitor monitor) : base(stateMachine, events, monitor)
        {
            this.BuffManager = new BuffManager(stateMachine.Companion, stateMachine.CompanionManager.Farmer, stateMachine.ContentLoader, this.monitor);
            this.SpecialEvents = specialEvents;
            this.TimeToBye = 2200; // Companions auto-dismiss at 10pm, except married (see end of Entry() method)
        }

        public override void Entry()
        {
            this.ai = new AI_StateMachine(this.StateMachine, this.StateMachine.CompanionManager.Hud, this.Events, this.monitor);
            this.timeOfRecruit = Game1.timeOfDay;

            if (this.StateMachine.Companion.doingEndOfRouteAnimation.Value)
                this.FinishScheduleAnimation();

            this.FixProblemsWithNPC();

            this.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            this.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
            this.Events.Player.Warped += this.Player_Warped;
            this.Events.Input.ButtonPressed += this.Input_ButtonPressed;
            this.SpecialEvents.RenderedLocation += this.SpecialEvents_RenderedLocation;

            this.recruitedDialogue = this.StateMachine.Dialogues.GenerateDialogue("companionRecruited");
            this.CanPerformAction = true;

            if (this.recruitedDialogue != null)
                this.StateMachine.Companion.CurrentDialogue.Push(this.recruitedDialogue);

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

            if (this.BuffManager.HasProsthetics())
            {
                var key = this.StateMachine.CompanionManager.Config.ChangeBuffButton;
                var desc = this.StateMachine.ContentLoader.LoadString("Strings/Strings:prosteticsChangeButton", key, this.StateMachine.Companion.displayName);
                this.StateMachine.CompanionManager.Hud.AddKey(key, desc);
            }

            if (Helper.IsSpouseMarriedToFarmer(this.StateMachine.Companion, this.StateMachine.CompanionManager.Farmer))
            {
                this.TimeToBye = 2400; // Extend adventuring time to midnight for Wife/Husband
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.IsDown(this.StateMachine.CompanionManager.Config.ChangeBuffButton))
            {
                this.BuffManager.ChangeBuff();
            }
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

            this.StateMachine.Companion.eventActor = false;
            this.StateMachine.Companion.farmerPassesThrough = false;
            this.CanPerformAction = false;

            this.SpecialEvents.RenderedLocation -= this.SpecialEvents_RenderedLocation;
            this.Events.Input.ButtonPressed -= this.Input_ButtonPressed;
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
            this.UpdateFriendship(e.NewTime);

            if (e.NewTime >= this.TimeToBye)
            {
                NPC companion = this.StateMachine.Companion;
                Dialogue dismissalDialogue = new Dialogue(
                    this.StateMachine.Dialogues.GetFriendSpecificDialogueText(
                        this.StateMachine.CompanionManager.Farmer, "companionDismissAuto"), companion);
                this.dismissalDialogue = dismissalDialogue;
                this.StateMachine.Companion.doEmote(24);
                this.StateMachine.Companion.updateEmote(Game1.currentGameTime);
                DialogueProvider.DrawDialogue(dismissalDialogue);
            }

            // Fix spawn ladder if area is infested and all monsters is killed but NPC following us
            if (this.StateMachine.Companion.currentLocation is MineShaft mines && mines.mustKillAllMonstersToAdvance())
            {
                var monsters = from c in mines.characters where c.IsMonster select c;
                if (monsters.Count() == 0)
                {
                    Vector2 vector2 = this.StateMachine.Reflection.GetProperty<Vector2>(mines, "tileBeneathLadder").GetValue();
                    if (mines.getTileIndexAt(Utility.Vector2ToPoint(vector2), "Buildings") == -1)
                        mines.createLadderAt(vector2, "newArtifact");
                }
            }

            // Try to push new or change location dialogue randomly until or no location dialogue was pushed
            int until = this.dialoguePushTime + (Game1.random.Next(1, 5) * 10);
            if ((e.NewTime > until || this.currentLocationDialogue == null))
                this.TryPushLocationDialogue(this.StateMachine.Companion.currentLocation);

            // Remove recruited dialogue if this dialogue not spoken until a hour from while companion was recruited
            if (this.recruitedDialogue != null && e.NewTime > this.timeOfRecruit + 100)
            {
                // TODO: Use here Remove old dialogue method when rebased onto branch or merged branch which has this util
                Stack<Dialogue> temp = new Stack<Dialogue>(this.StateMachine.Companion.CurrentDialogue.Count);

                while (this.StateMachine.Companion.CurrentDialogue.Count > 0)
                {
                    Dialogue d = this.StateMachine.Companion.CurrentDialogue.Pop();

                    if (!d.Equals(this.recruitedDialogue))
                        temp.Push(d);
                    else
                        this.monitor.Log($"Recruited dialogue was removed from {this.StateMachine.Name}'s stack due to NPC was recruited a hour ago and dialogue still not spoken.");
                }

                while (temp.Count > 0)
                    this.StateMachine.Companion.CurrentDialogue.Push(temp.Pop());
            }
        }

        /// <summary>
        /// Update friendship points every whole hour
        /// </summary>
        /// <param name="time">Current time</param>
        private void UpdateFriendship(int time)
        {
            if (time % 100 != 0 || !this.StateMachine.CompanionManager.Config.AllowGainFriendship)
            {
                return; // It's not whole hour? Or gain friendship is disabled? Do nothing!
            }

            var farmer = this.StateMachine.CompanionManager.Farmer;
            var npc = this.StateMachine.Companion;
            var points = 2;

            if (farmer.friendshipData.TryGetValue(npc.Name, out Friendship friendship))
            {
                if (friendship.IsMarried())
                    points = 5;
                else if (friendship.IsDating())
                    points = 3;
            }

            if (npc.isBirthday(Game1.Date.Season, Game1.Date.DayOfMonth))
                points *= 2; // Gain double points if companion has birthday today

            farmer.changeFriendship(points, npc);
            this.monitor.VerboseLog($"You gained {points} friendship points with {npc.Name}");
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.FixProblemsWithNPC();
            this.ai.Update(e);
        }

        /// <summary>
        /// Fix a problems with companion while follows farmer.
        /// </summary>
        private void FixProblemsWithNPC()
        {
            var npc = this.StateMachine.Companion;

            if(npc.controller != null)
            {
                npc.controller.endBehaviorFunction = null;
                npc.controller.pathToEndPoint = null;
                npc.controller = null;
            }

            if (npc.temporaryController != null)
            {
                npc.temporaryController.endBehaviorFunction = null;
                npc.temporaryController.pathToEndPoint = null;
                npc.temporaryController = null;
            }

            npc.movementPause = 0;
            npc.followSchedule = false;
            npc.ignoreScheduleToday = true;
            npc.Schedule = null;
            npc.faceTowardFarmerTimer = 0;
            npc.farmerPassesThrough = true;
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            NPC companion = this.StateMachine.Companion;
            Dictionary<string, string> bubbles = this.StateMachine.ContentLoader.LoadStrings("Strings/SpeechBubbles");

            // Warp companion to farmer if it's needed
            if (companion.currentLocation != e.NewLocation)
                this.ai.ChangeLocation(e.NewLocation);

            // Show above head bubble text for location
            if (Game1.random.NextDouble() > .66f && DialogueProvider.GetAmbientBubbleString(bubbles, companion, e.NewLocation, out string bubble))
                companion.showTextAboveHead(bubble, preTimer: 250);

            // Push new location dialogue
            this.TryPushLocationDialogue(e.NewLocation, true);
        }

        /// <summary>
        /// Try push location based companion dialogue to companion's dialogue stack
        /// </summary>
        /// <param name="location">Current location</param>
        /// <param name="warped">Warped right now to this location?</param>
        private void TryPushLocationDialogue(GameLocation location, bool warped = false)
        {
            Stack<Dialogue> temp = new Stack<Dialogue>(this.StateMachine.Companion.CurrentDialogue.Count);
            Dialogue newDialogue = this.StateMachine.GenerateLocationDialogue(location, warped ? "Enter" : "");

            if (warped && newDialogue == null)
            {
                // Try generate regular location dialogue if no enter location dialogue not defined or already spoken
                newDialogue = this.StateMachine.GenerateLocationDialogue(location);
            }

            bool isSameDialogue = this.currentLocationDialogue is CompanionDialogue curr
                                  && newDialogue is CompanionDialogue newd
                                  && curr.Kind == newd.Kind;

            if (isSameDialogue || (newDialogue == null && this.currentLocationDialogue == null))
                return;

            // Remove old location dialogue
            while (this.StateMachine.Companion.CurrentDialogue.Count > 0)
            {
                Dialogue d = this.StateMachine.Companion.CurrentDialogue.Pop();

                if (!d.Equals(this.currentLocationDialogue))
                    temp.Push(d);
                else
                    this.monitor.Log($"Old location dialogue was removed from {this.StateMachine.Name}'s stack");
            }

            while (temp.Count > 0)
                this.StateMachine.Companion.CurrentDialogue.Push(temp.Pop());

            this.currentLocationDialogue = newDialogue;

            if (newDialogue != null)
            {
                this.dialoguePushTime = Game1.timeOfDay;
                this.StateMachine.Companion.CurrentDialogue.Push(newDialogue); // Push new location dialogue
                this.monitor.Log($"New location dialogue pushed to {this.StateMachine.Name}'s stack");
            }
        }

        /// <summary>
        /// Perform player's (inter)action with Companion
        /// </summary>
        /// <param name="who">Player</param>
        /// <param name="location">Current location mastered an action</param>
        /// <returns></returns>
        public bool PerformAction(Farmer who, GameLocation location)
        {
            if (this.ai != null && this.ai.PerformAction())
                return true;

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
                    this.StateMachine.Companion.facePlayer(who);
                    this.ReactOnAsk(this.StateMachine.Companion, who, answer); ;
                }
            }, this.StateMachine.Companion);

            return true;
        }

        /// <summary>
        /// Companion reacts on player's ask
        /// </summary>
        /// <param name="companion"></param>
        /// <param name="leader"></param>
        /// <param name="action"></param>
        private void ReactOnAsk(NPC companion, Farmer leader, string action)
        {
            switch (action)
            {
                case "dismiss":
                    Dialogue dismissalDialogue = new Dialogue(
                        this.StateMachine.Dialogues.GetFriendSpecificDialogueText(leader, "companionDismiss"), companion);
                    this.dismissalDialogue = dismissalDialogue;
                    DialogueProvider.DrawDialogue(dismissalDialogue);
                    break;
                case "bag":
                    Chest bag = this.StateMachine.Bag;
                    this.StateMachine.Companion.currentLocation.playSound("openBox");
                    Game1.activeClickableMenu = this.CreateOpenBagMenu(bag);
                    break;
            }
        }

        private IClickableMenu CreateOpenBagMenu(Chest bag)
        {
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                return new ItemGrabMenu(
                    inventory: bag.items,
                    reverseGrab: true,
                    showReceivingMenu: true,
                    highlightFunction: new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                    behaviorOnItemSelectFunction: null,
                    message: this.StateMachine.Companion.displayName,
                    behaviorOnItemGrab: null,
                    canBeExitedWithKey: true,
                    showOrganizeButton: true,
                    source: ItemGrabMenu.source_chest,
                    context: this.StateMachine.Companion);
            }

            return new ItemGrabMenu(
                inventory: bag.items,
                reverseGrab: false,
                showReceivingMenu: true,
                highlightFunction: new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                behaviorOnItemSelectFunction: new ItemGrabMenu.behaviorOnItemSelect(bag.grabItemFromInventory),
                message: this.StateMachine.Companion.displayName,
                behaviorOnItemGrab: new ItemGrabMenu.behaviorOnItemSelect(bag.grabItemFromChest),
                canBeExitedWithKey: true,
                showOrganizeButton: true,
                source: ItemGrabMenu.source_chest,
                context: this.StateMachine.Companion);
        }

        /// <summary>
        /// Handles after companion's dialogue was spoken.
        /// </summary>
        /// <param name="speakedDialogue"></param>
        public void OnDialogueSpoken(Dialogue speakedDialogue)
        {
            if (speakedDialogue == this.dismissalDialogue)
            {
                // After companion speaked a dismissal dialogue dismiss (unrecruit) companion who speaked that
                this.StateMachine.Dismiss(Game1.timeOfDay >= 2200);
            }
        }
    }
}
