using System;
using System.Collections.Generic;
using System.Reflection;
using StardewValley;
using StardewValley.Menus;

namespace FollowerNPC.CompanionStates
{
    class RecruitableState: CompanionState
    {
        private bool recruitDialoguePushed;
        Random r;

        public RecruitableState(CompanionStateMachine sm) : base(sm)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            r = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed + Game1.timeOfDay);
            if (stateMachine.manager.currentCompanion == null)
            {
                Dialogue d = stateMachine.manager.GenerateDialogue("Recruit", stateMachine.companion.Name, true);
                stateMachine.recruitDialogue = d ?? throw new Exception(
                        "Tried to push a recruit dialogue, but there were no recruit strings for this character!");
                if (CheckForMissingResponseKeys(d))
                    throw new Exception("There were missing dialogue response keys for this character!");
                stateMachine.companion.CurrentDialogue.Push(d);
                recruitDialoguePushed = true;
            }

            ModEntry.modHelper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
            ModEntry.modHelper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
            ModEntry.modHelper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        public override void ExitState()
        {
            base.ExitState();
            r = null;
            ModEntry.modHelper.Events.GameLoop.TimeChanged -= GameLoop_TimeChanged;
            ModEntry.modHelper.Events.GameLoop.DayEnding -= GameLoop_DayEnding;
            ModEntry.modHelper.Events.Display.MenuChanged -= Display_MenuChanged;
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            if (!recruitDialoguePushed && stateMachine.manager.currentCompanion == null)
            {
                Dialogue d = stateMachine.manager.GenerateDialogue("Recruit", stateMachine.companion.Name, true);
                stateMachine.recruitDialogue = d ?? throw new Exception(
                        "Tried to push a recruit dialogue, but there were no recruit strings for this character!");
                if (CheckForMissingResponseKeys(d))
                    throw new Exception("There were missing dialogue response keys for this character!");
                stateMachine.companion.CurrentDialogue.Push(d);
                recruitDialoguePushed = true;
            }
        }

        private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            stateMachine.NewDaySetup();
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            if (e.OldMenu != null)
            {
                DialogueBox db = (e.OldMenu as DialogueBox);
                if (db != null)
                {
                    Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                    if (d != null && d.speaker != null && d.speaker.Equals(stateMachine.companion) &&
                        d.Equals(stateMachine.recruitDialogue))
                    {
                        if (Game1.timeOfDay < 2200 && 
                            stateMachine.manager.farmer.dialogueQuestionsAnswered.Contains(CompanionsManager.recruitYesID))
                        {
                            stateMachine.Recruit();
                        }
                        else
                        {
                            stateMachine.Reject();
                        }
                    }
                }
            }
        }
    }
}
