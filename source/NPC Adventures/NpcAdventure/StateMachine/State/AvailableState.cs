using NpcAdventure.Utils;
using NpcAdventure.StateMachine.StateFeatures;
using StardewModdingAPI.Events;
using StardewValley;
using StardewModdingAPI;

namespace NpcAdventure.StateMachine.State
{
    internal class AvailableState : CompanionState, IRequestedDialogueCreator, IDialogueDetector
    {
        private Dialogue acceptalDialogue;
        private Dialogue rejectionDialogue;

        public bool CanCreateDialogue { get; private set; }

        public AvailableState(CompanionStateMachine stateMachine, IModEvents events, IMonitor monitor) : base(stateMachine, events, monitor) {}

        public override void Entry()
        {
            this.CanCreateDialogue = true;
        }

        public override void Exit()
        {
            this.CanCreateDialogue = false;
            this.acceptalDialogue = null;
            this.rejectionDialogue = null;
        }

        private void ReactOnAnswer(NPC n, Farmer leader)
        {
            if (leader.getFriendshipHeartLevelForNPC(n.Name) <= 4 || Game1.timeOfDay >= 2200)
            {
                Dialogue rejectionDialogue = new Dialogue(
                    DialogueHelper.GetDialogueString(
                        n, Game1.timeOfDay >= 2200 ? "companionRejectedNight" : "companionRejected"), n);

                this.rejectionDialogue = rejectionDialogue;
                DialogueHelper.DrawDialogue(rejectionDialogue);
            }
            else
            {
                Dialogue acceptalDialogue = new Dialogue(DialogueHelper.GetDialogueString(n, "companionAccepted"), n);

                this.acceptalDialogue = acceptalDialogue;
                DialogueHelper.DrawDialogue(acceptalDialogue);
            }
        }

        public void CreateRequestedDialogue()
        {
            Farmer leader = this.StateMachine.CompanionManager.Farmer;
            NPC companion = this.StateMachine.Companion;
            GameLocation location = this.StateMachine.CompanionManager.Farmer.currentLocation;
            string question = this.StateMachine.ContentLoader.LoadString("Strings/Strings:askToFollow", companion.displayName);

            location.createQuestionDialogue(question, location.createYesNoResponses(), (_, answer) =>
            {
                if (answer == "Yes")
                {
                    if (!this.StateMachine.Companion.doingEndOfRouteAnimation.Value)
                    {
                        this.StateMachine.Companion.Halt();
                        this.StateMachine.Companion.facePlayer(leader);
                    }
                    this.ReactOnAnswer(this.StateMachine.Companion, leader);
                }
            }, null);
        }

        public void OnDialogueSpeaked(Dialogue speakedDialogue)
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
