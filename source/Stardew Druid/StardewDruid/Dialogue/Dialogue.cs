/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/


using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Constants;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using static StardewValley.Menus.SocialPage;

namespace StardewDruid.Dialogue
{
    public class Dialogue
    {
        
        public StardewDruid.Character.Character npc;

        public List<string> promptDialogue = new();

        public List<string> questDialogue = new();

        public List<string> archiveDialogue = new();

        public List<string> introDialogue = new();

        public Dictionary<string, DialogueSpecial> specialDialogue = new();

        public Dialogue(StardewDruid.Character.Character NPC)
        {
            
            npc = NPC;
        
        }

        public virtual void DialogueApproach()
        {

            if(promptDialogue.Count > 0)
            {

                string prompt = promptDialogue[0];

                promptDialogue.RemoveAt(0);

                RunSpecialDialogue(prompt);

                return;

            }

            string str = "Welcome";

            if(introDialogue.Count > 0)
            {

                str = introDialogue.First(); 
                
                introDialogue.Clear();

            }

            List<Response> responseList = new List<Response>();

            if (questDialogue.Count > 0)
            {

                responseList.Add(new("quests", "what is the latest quest?"));

            }

            responseList.Add(new("none", "(say nothing)"));

            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);

            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);

        }

        public virtual void AnswerApproach(Farmer visitor, string answer)
        {

            switch (answer)
            {

                case "quests":

                    if(questDialogue.Count == 0 && archiveDialogue.Count > 0)
                    {

                        questDialogue = new(archiveDialogue);

                        archiveDialogue.Clear();

                    }

                    if(questDialogue.Count > 0)
                    {

                        for(int q = questDialogue.Count - 1;  q >= 0; q--)
                        {
                            
                            string questId = questDialogue[q];

                            questDialogue.RemoveAt(q);

                            archiveDialogue.Add(questId);

                            if (Mod.instance.save.progress.ContainsKey(questId))
                            {

                                RunSpecialDialogue(questId);

                                return;

                            }
 
                        }
                    
                    }

                    DelayedAction.functionAfterDelay(DialogueApproach, 100);

                    break;

            }

            return;

        }

        public virtual void RunSpecialDialogue(string prompt)
        {

            DialogueSpecial specialEntry = specialDialogue[prompt];

            if(specialEntry.responses.Count > 0)
            {

                GameLocation.afterQuestionBehavior questionBehavior = new(RespondSpecialDialogue);

                List<Response> responseList = new();

                int respondIndex = 1;

                foreach (string respond in specialEntry.responses)
                {

                    responseList.Add(new(prompt+"|"+respondIndex.ToString(), respond));

                }

                Game1.player.currentLocation.createQuestionDialogue(specialEntry.intro, responseList.ToArray(), questionBehavior, npc);

                return;

            }

            if (specialEntry.questId != null)
            {

                Mod.instance.questHandle.DialogueCheck(specialEntry.questId, specialEntry.questContext, npc);

            }

            npc.CurrentDialogue.Push(new(npc, "0", specialEntry.intro));

            //specialDialogue.Clear();

            Game1.drawDialogue(npc);

            DelayedAction.functionAfterDelay(DialogueApproach, 100);

        }

        public virtual void RespondSpecialDialogue(Farmer visitor, string dialogueId)
        {

            string[] ids = dialogueId.Split('|');

            DialogueSpecial special = specialDialogue[ids[0]];

            int id = Convert.ToInt32(dialogueId);

            if(special.outros.Count >= id)
            {

                introDialogue.Add(special.outros[id]);

            }
            else if(special.outros.Count > 0)
            {

                introDialogue.Add(special.outros.First());

            }

            if(special.questId != null)
            {

                Mod.instance.questHandle.DialogueCheck(special.questId, special.questContext, npc, Convert.ToInt32(ids[1]));

            }

            DelayedAction.functionAfterDelay(DialogueApproach,100);

            //specialDialogue.Clear();

        }


        public virtual void AddSpecialDialogue(string eventId, DialogueSpecial special)
        {

            specialDialogue.Clear();

            specialDialogue[eventId] = special;

            if (special.prompt)
            {

                promptDialogue.Add(eventId);

            }

            if(special.questId != null)
            {

                questDialogue.Add(eventId);

            }

        }

        public virtual void RemoveSpecialDialogue(string eventId)
        {

            if (promptDialogue.Contains(eventId))
            {

                promptDialogue.Remove(eventId);

            }

            if (questDialogue.Contains(eventId))
            {

                questDialogue.Remove(eventId);

            }

            if (archiveDialogue.Contains(eventId))
            {

                archiveDialogue.Remove(eventId);

            }

            if (specialDialogue.ContainsKey(eventId))
            {

                specialDialogue.Remove(eventId);

            }

        }

    }

    public class DialogueSpecial
    {

        public string intro;

        public List<string> responses = new();

        public List<string> outros = new();

        public enum type{

            none,
            special,
            quest,
            active,

        }

        public bool prompt;

        public string questId;

        public int questContext;

    }

}
