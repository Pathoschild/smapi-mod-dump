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
using FollowerNPC.CompanionStates;

namespace FollowerNPC
{
    public class CompanionsManager
    {

        // Companion Objects //
        public CompanionStateMachine currentCompanion;
        public Dictionary<string, CompanionStateMachine> possibleCompanions;
        public Random r;
        // ***************** //

        // Companion Parameters //
        public int companionHeartThreshold;
        // ******************** //

        // Farmer Objects //
        public Farmer farmer;
        // ************** //

        // Constants //
        public const int recruitYesID = 592800;
        public const int recruitNoID = 9249200;
        public const int actionDismissID = 4736775;
        public const int actionContinueID = 7298075;
        public const int actionSpecialID = 2191988;
        // ********* //

        public CompanionsManager()
        {
            // Define Companion Parameters //
            companionHeartThreshold = ModEntry.config.heartThreshold;
            // *************************** //

            // Subscribe to Events //
            ModEntry.modHelper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            ModEntry.modHelper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            // ******************* //

            r = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
        }

        #region Helpers

        #region Events

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            farmer = Game1.player;

            InitializeCompanionStateMachines();
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            ResetCompanionStateMachines();
        }
        #endregion

        #region Companion Handling

        private void InitializeCompanionStateMachines()
        {
            string[] characterNames =
            {
                "Abigail", "Alex", "Elliott", "Emily", "Haley", "Harvey", "Leah", "Maru", "Penny", "Sam", "Sebastian",
                "Shane"
            };
            if (possibleCompanions == null)
            {
                possibleCompanions = new Dictionary<string, CompanionStateMachine>();
                foreach (string characterName in characterNames)
                {
                    possibleCompanions.Add(characterName, new CompanionStateMachine(this, characterName));
                    possibleCompanions[characterName].NewDaySetup();
                }
            }
            else
            {
                foreach (string characterName in characterNames)
                {
                    possibleCompanions[characterName].Reset(this, characterName);
                    possibleCompanions[characterName].NewDaySetup();
                }
            }
        }

        private void ResetCompanionStateMachines()
        {
            farmer.dialogueQuestionsAnswered.Remove(recruitYesID);
            farmer.dialogueQuestionsAnswered.Remove(recruitNoID);
            farmer.dialogueQuestionsAnswered.Remove(actionContinueID);
            farmer.dialogueQuestionsAnswered.Remove(actionDismissID);
            farmer.dialogueQuestionsAnswered.Remove(actionSpecialID);

            currentCompanion = null;
            string[] characterNames =
            {
                "Abigail", "Alex", "Elliott", "Emily", "Haley", "Harvey", "Leah", "Maru", "Penny", "Sam", "Sebastian",
                "Shane"
            };
            foreach (string characterName in characterNames)
            {
                possibleCompanions[characterName].Dispose();
            }
        }

        public void CompanioinRecruited(string name)
        {
            foreach (KeyValuePair<string, CompanionStateMachine> csm in possibleCompanions)
            {
                if (!name.Equals(csm.Key))
                {
                    RecruitableState state = csm.Value.currentState as RecruitableState;
                    if (state != null)
                        csm.Value.UndoRecruitable();
                }
             }
        }

        public Dialogue GenerateDialogue(string scenario, string companionName, bool tryRepeatIfFailed)
        {
            List<string> ret = new List<string>();
            bool repeat = false;
            if (!possibleCompanions.TryGetValue(companionName, out CompanionStateMachine csm))
                return null;
            NPC companion = csm.companion;

            GetDialogue:
            // If this companion is married to the farmer
            if (farmer.spouse != null && farmer.spouse.Equals(companionName))
            {
                string recruitFriendKey = "companion-" + scenario + "-Friend";
                string recruitSpouseKey = "companion-" + scenario + "-Spouse";
                string recruitSpouseOverrideKey = "companion-" + scenario + "-SpouseOverride";

                // If there are SpouseOverride dialogue(s)
                if (GetAnyDialogueValuesForDialogueKey(companionName, recruitSpouseOverrideKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], companion);
                }

                // Else, look for Spouse and Friend strings
                else if (GetAnyDialogueValuesForDialogueKey(companionName, recruitSpouseKey, ref ret) |
                         GetAnyDialogueValuesForDialogueKey(companionName, recruitFriendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], companion);
                }
            }
            // Otherwise, if they are just a friend
            else
            {
                string recruitFriendKey = "companion-" + scenario + "-Friend";

                // Look for Friend strings
                if (GetAnyDialogueValuesForDialogueKey(companionName, recruitFriendKey, ref ret))
                {
                    return new Dialogue(ret[r.Next(ret.Count)], companion);
                }
            }

            if (tryRepeatIfFailed && ret.Count == 0 && !repeat)
            {
                repeat = true;
                foreach (KeyValuePair<string, string> kvp in csm.script)
                    csm.companion.Dialogue[kvp.Key] = kvp.Value;
                goto GetDialogue;
            }
            return null;
        }

        private bool GetAnyDialogueValuesForDialogueKey(string companionName, string dialogueKey, ref List<string> dialogueValues)
        {
            bool ret = false;
            if (!possibleCompanions.TryGetValue(companionName, out CompanionStateMachine csm))
                return false;

            // If there is a, or mulitple, strings
            string multiValue = null;
            string singleValue = null;
            if (csm.companion.Dialogue.TryGetValue(dialogueKey + "1", out multiValue) ||
                csm.companion.Dialogue.TryGetValue(dialogueKey, out singleValue))
            {
                // If there are multiple strings
                if (multiValue != null)
                {
                    ret = true;
                    dialogueValues.Add(multiValue);
                    int i = 2;
                    while (csm.companion.Dialogue.TryGetValue(dialogueKey + i.ToString(),
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
        #endregion

        #endregion
    }
}
