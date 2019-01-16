using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FollowerNPC.CompanionStates
{
    public class CompanionStateMachine
    {
        private enum eStates
        {
            resetState = 0,
            unavailableState = 1,
            availableState = 2,
            recruitableState = 3,
            recruitedState = 4
        }

        public CompanionsManager manager;
        public NPC companion;
        public CompanionState[] states;
        public CompanionState currentState;

        public Dictionary<string, string> script;
        public string[] availabilityDays;

        public Dialogue recruitDialogue;
        public Dialogue actionDialogue;
        public Dialogue locationDialogue;
        public Dialogue automaticDismissDialogue;

        public CompanionStateMachine(CompanionsManager manager, string name)
        {
            this.manager = manager;
            this.companion = Game1.getCharacterFromName(name);
            this.states = new CompanionState[5];
            this.states[0] = new ResetState(this);
            this.states[1] = new UnavailableState(this);
            this.states[2] = new AvailableState(this);
            this.currentState = new ResetState(this);

            script = ModEntry.modHelper.Content.Load<Dictionary<string, string>>("assets/" + name + "Companion.json",
                ContentSource.ModFolder);
            availabilityDays = (string[])(typeof(ModConfig).GetField(name).GetValue(ModEntry.config));
        }

        public void Reset(CompanionsManager manager, string name)
        {
            this.manager = manager;
            this.companion = Game1.getCharacterFromName(name);
            this.states = new CompanionState[5];
            this.states[0] = new ResetState(this);
            this.states[1] = new UnavailableState(this);
            this.states[2] = new AvailableState(this);
            this.currentState = new ResetState(this);

            script = ModEntry.modHelper.Content.Load<Dictionary<string, string>>("assets/" + name + "Companion.json",
                ContentSource.ModFolder);
            availabilityDays = (string[])(typeof(ModConfig).GetField(name).GetValue(ModEntry.config));
        }

        public void Dispose()
        {
            for(int i = 0; i < states.Length; i++)
                states[i]?.ExitState();
            states = null;

            this.manager = null;
            this.companion = null;
            this.currentState = null;
            this.script.Clear();
            this.script = null;
            this.availabilityDays = null;

            recruitDialogue = null;
            actionDialogue = null;
            locationDialogue = null;
            automaticDismissDialogue = null;
        }

        private void QuickChangeState(eStates newState)
        {
            try
            {
                currentState.ExitState();
                currentState = states[(int) newState];
                currentState.EnterState();
            }
            catch (Exception e)
            {
                EmergencyExit(e);
            }
        }

        #region State Changes

        public void ActionDismiss()
        {
            QuickChangeState(eStates.resetState);
            ResetState resetState = (ResetState) currentState;
            resetState.ReIntegrateCompanion();
            manager.currentCompanion = null;
            QuickChangeState(eStates.unavailableState);
        }

        public void AutomaticDismiss()
        {
            QuickChangeState(eStates.resetState);
            ResetState resetState = (ResetState)currentState;
            resetState.ReIntegrateCompanion();
            manager.currentCompanion = null;
            QuickChangeState(eStates.unavailableState);
        }
        
        public void DayEndingDismiss()
        {
            QuickChangeState(eStates.resetState);
            ResetState resetState = (ResetState)currentState;
            resetState.ReIntegrateCompanion();
            manager.currentCompanion = null;
            QuickChangeState(eStates.unavailableState);
        }

        public void NewDaySetup()
        {
            string dayOfWeek = Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
            if (availabilityDays.Contains(dayOfWeek))
            {
                foreach (KeyValuePair<string, string> kvp in script)
                    companion.Dialogue[kvp.Key] = kvp.Value;
                QuickChangeState(eStates.availableState);
            }
            else
            {
                QuickChangeState(eStates.unavailableState);
            }
        }

        public void MakeRecruitable()
        {
            RecruitableState newState = new RecruitableState(this);
            this.states[3] = newState;
            QuickChangeState(eStates.recruitableState);
        }

        public void Recruit()
        {
            manager.farmer.DialogueQuestionsAnswered.Remove(CompanionsManager.recruitYesID);

            RecruitedState newState = new RecruitedState(this);
            this.states[4] = newState;
            QuickChangeState(eStates.recruitedState);
            manager.currentCompanion = this;
            manager.CompanioinRecruited(companion.Name);
        }

        public void Reject()
        {
            manager.farmer.DialogueQuestionsAnswered.Remove(CompanionsManager.recruitYesID);
            manager.farmer.DialogueQuestionsAnswered.Remove(CompanionsManager.recruitNoID);

            QuickChangeState(eStates.unavailableState);
        }

        public void UndoRecruitable()
        {
            QuickChangeState(eStates.availableState);
            while (companion.CurrentDialogue.Count != 0)
                companion.CurrentDialogue.Pop();
        }

        public void EmergencyExit(Exception e)
        {
            ModEntry.monitor.Log(e.Message, LogLevel.Error);
            QuickChangeState(eStates.resetState);
            ResetState resetState = (ResetState)currentState;
            resetState.ReIntegrateCompanion();
            manager.currentCompanion = null;
            QuickChangeState(eStates.unavailableState);
        }

        #endregion
    }

    public class CompanionState
    {
        public CompanionStateMachine stateMachine;

        public CompanionState(CompanionStateMachine sm)
        {
            stateMachine = sm;
        }

        public virtual void EnterState()
        {

        }

        public virtual void ExitState()
        {

        }

        public virtual void Update(UpdateTickedEventArgs e)
        {

        }

        protected bool GetAnyDialogueValuesForDialogueKey(string dialogueKey, ref List<string> dialogueValues)
        {
            bool ret = false;

            // If there is a, or mulitple, strings
            string multiValue = null;
            string singleValue = null;
            if (stateMachine.companion.Dialogue.TryGetValue(dialogueKey + "1", out multiValue) ||
                stateMachine.companion.Dialogue.TryGetValue(dialogueKey, out singleValue))
            //if (stateMachine.script.TryGetValue(dialogueKey + "1", out multiValue) ||
            //    stateMachine.script.TryGetValue(dialogueKey, out singleValue))
            {
                // If there are multiple strings
                if (multiValue != null)
                {
                    ret = true;
                    dialogueValues.Add(multiValue);
                    int i = 2;
                    while (stateMachine.companion.Dialogue.TryGetValue(dialogueKey + i.ToString(),
                        out multiValue))
                    {
                        i++;
                        dialogueValues.Add(multiValue);
                    }
                }
                // If there is only one string
                else if (singleValue != null)
                {
                    ret = true;
                    dialogueValues.Add(singleValue);
                }
            }
            return ret;
        }

        protected bool CheckForMissingResponseKeys(Dialogue d)
        {
            List<NPCDialogueResponse> responseKeys = d.getNPCResponseOptions();
            if (responseKeys != null)
            {
                foreach (NPCDialogueResponse npcdr in responseKeys)
                {
                    if (!stateMachine.companion.Dialogue.TryGetValue(npcdr.responseKey, out string dialogueValue))
                    {
                        if (!stateMachine.script.TryGetValue(npcdr.responseKey, out dialogueValue))
                            return true;
                        stateMachine.companion.Dialogue[npcdr.responseKey] = dialogueValue;
                    }
                }
            }
            return false;
        }

    }

}
