/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using NpcAdventure.Utils;
using NpcAdventure.StateMachine.StateFeatures;
using StardewModdingAPI.Events;
using StardewValley;
using StardewModdingAPI;
using System.Collections.Generic;
using NpcAdventure.Dialogues;

namespace NpcAdventure.StateMachine.State
{
    internal class AvailableState : CompanionState, IActionPerformer, IDialogueDetector
    {
        private Dialogue acceptalDialogue;
        private Dialogue rejectionDialogue;
        private Dialogue suggestionDialogue;
        private bool recruitRequestsEnabled;

        public bool CanPerformAction { get => this.recruitRequestsEnabled && this.StateMachine.CompanionManager.CanRecruit(); }

        private int doNotAskUntil;

        public AvailableState(CompanionStateMachine stateMachine, IModEvents events, IMonitor monitor) : base(stateMachine, events, monitor) { }

        public override void Entry()
        {
            this.recruitRequestsEnabled = true;
            this.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
            this.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            var dialogueStack = this.StateMachine.Companion.CurrentDialogue;
            if (e.IsMultipleOf(15) && this.suggestionDialogue != null && !dialogueStack.Contains(this.suggestionDialogue) && dialogueStack.Count == 0)
            {
                // Push suggest dialogue again when companion mind is free and we have created that suggestion dialogue and this dialogue is not already in stack
                this.StateMachine.Companion.CurrentDialogue.Push(this.suggestionDialogue);
                this.monitor.Log($"Adventure suggest dialogue for {this.StateMachine.Companion.Name} pushed into stack!");
            }
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (this.recruitRequestsEnabled == false && e.NewTime > this.doNotAskUntil)
                this.recruitRequestsEnabled = true;

            int heartLevel = this.StateMachine.CompanionManager.Farmer.getFriendshipHeartLevelForNPC(this.StateMachine.Companion.Name);
            int threshold = this.StateMachine.CompanionManager.Config.HeartSuggestThreshold;
            bool married = Helper.IsSpouseMarriedToFarmer(this.StateMachine.Companion, this.StateMachine.CompanionManager.Farmer);
            bool matchesTimeRange = e.NewTime < 2200 && e.NewTime > (married ? 800 : 1000);

            if (!this.StateMachine.SuggestedToday
                && this.StateMachine.CanSuggestToday
                && this.CanPerformAction
                && matchesTimeRange
                && this.suggestionDialogue == null
                && heartLevel >= threshold
                && Game1.random.NextDouble() < this.GetSuggestChance())
            {
                Dialogue d = this.StateMachine.Dialogues.GenerateDialogue("companionSuggest");
                Farmer f = this.StateMachine.CompanionManager.Farmer;

                if (d == null)
                    return; // No dialogue defined, nothing to suggest

                // Add reaction on adventure suggestion acceptance/rejectance question
                d.answerQuestionBehavior = new Dialogue.onAnswerQuestion((whichResponse) =>
                {
                    List<Response> opts = d.getResponseOptions();
                    NPC n = this.StateMachine.Companion;

                    if (opts[whichResponse].responseKey == "Yes")
                    {
                        // Farmer accepted suggestion of adventure. Let's go to find a some trouble!
                        this.acceptalDialogue = new Dialogue(this.StateMachine.Dialogues.GetFriendSpecificDialogueText(f, "companionSuggest_Yes"), n);
                        DialogueProvider.DrawDialogue(this.acceptalDialogue);
                    }
                    else
                    {
                        // Farmer not accepted for this time. Farmer can't ask to follow next 2 hours
                        this.recruitRequestsEnabled = false;
                        this.doNotAskUntil = Game1.timeOfDay + 200;
                        DialogueProvider.DrawDialogue(
                            new Dialogue(this.StateMachine.Dialogues.GetFriendSpecificDialogueText(f, "companionSuggest_No"), n)
                        );
                    }

                    this.suggestionDialogue = null;
                    this.StateMachine.SuggestedToday = true;

                    return false;
                });

                this.suggestionDialogue = d;
                this.monitor.Log($"Added adventure suggest dialogue to {this.StateMachine.Companion.Name}");
            }
            else if (this.suggestionDialogue != null)
            {
                if (e.NewTime >= 2200 || heartLevel <= 4)
                {
                    // Remove suggestion dialogue when it'S over 22:00 or friendship heart level decreased under recruit heart threshold
                    DialogueProvider.RemoveDialogueFromStack(this.StateMachine.Companion, this.suggestionDialogue);
                    this.suggestionDialogue = null;
                    this.monitor.Log($"Removed adventure suggest dialogue from {this.StateMachine.Companion.Name}");
                }
            }
        }

        private float GetSuggestChance()
        {
            int firendship = this.StateMachine.CompanionManager.Farmer.getFriendshipLevelForNPC(this.StateMachine.Companion.Name);
            bool married = Helper.IsSpouseMarriedToFarmer(this.StateMachine.Companion, this.StateMachine.CompanionManager.Farmer);
            float chance = (0.066f * firendship / 100 / 4) + (Game1.random.Next(-100, 100) / 1000);

            if (married && Game1.random.NextDouble() > 0.7f)
                chance /= 2;

            this.monitor.VerboseLog($"Suggestion chance for {this.StateMachine.Companion.Name}: {chance}");

            return chance;
        }

        public override void Exit()
        {
            if (this.StateMachine.Companion.CurrentDialogue.Contains(this.suggestionDialogue))
            {
                DialogueProvider.RemoveDialogueFromStack(this.StateMachine.Companion, this.suggestionDialogue);
                this.monitor.Log($"EXIT STATE: Removed adventure suggest dialogue from {this.StateMachine.Companion.Name}'s stack.");
            }

            this.Events.GameLoop.UpdateTicked -= this.GameLoop_UpdateTicked;
            this.Events.GameLoop.TimeChanged -= this.GameLoop_TimeChanged;
            this.recruitRequestsEnabled = false;
            this.acceptalDialogue = null;
            this.rejectionDialogue = null;
            this.suggestionDialogue = null;
        }

        private void ReactOnAnswer(NPC n, Farmer leader)
        {
            if (leader.getFriendshipHeartLevelForNPC(n.Name) < this.StateMachine.CompanionManager.Config.HeartThreshold || Game1.timeOfDay >= 2200)
            {
                Dialogue rejectionDialogue = new Dialogue(
                    this.StateMachine.Dialogues.GetFriendSpecificDialogueText(
                        leader, Game1.timeOfDay >= 2200 ? "companionRejectedNight" : "companionRejected"), n);

                this.rejectionDialogue = rejectionDialogue;
                DialogueProvider.DrawDialogue(rejectionDialogue);
            }
            else
            {
                Dialogue acceptalDialogue = new Dialogue(
                    this.StateMachine.Dialogues.GetFriendSpecificDialogueText(leader, "companionAccepted"), n);

                this.acceptalDialogue = acceptalDialogue;
                DialogueProvider.DrawDialogue(acceptalDialogue);
            }
        }

        public bool PerformAction(Farmer who, GameLocation location)
        {
            NPC companion = this.StateMachine.Companion;
            string question = this.StateMachine.ContentLoader.LoadString("Strings/Strings:askToFollow", companion.displayName);

            if (companion.isMoving())
            {
                companion.Halt();
                companion.facePlayer(who);
            }

            location.createQuestionDialogue(question, location.createYesNoResponses(), (_, answer) =>
            {
                if (answer == "Yes")
                {
                    if (!this.StateMachine.Companion.doingEndOfRouteAnimation.Value)
                    {
                        this.StateMachine.Companion.Halt();
                        this.StateMachine.Companion.facePlayer(who);
                    }
                    this.ReactOnAnswer(this.StateMachine.Companion, who);
                }
            }, null);

            return true;
        }

        public void OnDialogueSpoken(Dialogue speakedDialogue)
        {
            if (speakedDialogue == this.acceptalDialogue)
            {
                this.StateMachine.CompanionManager.Farmer.changeFriendship(40, this.StateMachine.Companion);
                this.StateMachine.Recruit();
            }
            else if (speakedDialogue == this.rejectionDialogue)
            {
                this.StateMachine.MakeUnavailable();
            }
        }
    }
}
