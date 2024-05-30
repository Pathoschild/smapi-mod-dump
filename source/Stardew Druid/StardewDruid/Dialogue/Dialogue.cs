/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/


using StardewDruid.Data;
using StardewDruid.Journal;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Constants;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using static StardewValley.Menus.SocialPage;

namespace StardewDruid.Dialogue
{
    public class Dialogue
    {

        public CharacterData.characters characterType = CharacterData.characters.none;

        public StardewDruid.Character.Character npc = null;

        public List<string> promptDialogue = new();

        public List<string> questDialogue = new();

        public List<string> archiveDialogue = new();

        public List<string> introDialogue = new();

        public Dictionary<string, DialogueSpecial> specialDialogue = new();

        public string currentSpecial;

        public CharacterData.subjects currentSubject;

        public int currentIndex;

        public Dialogue(CharacterData.characters CharacterType)
        {

            characterType = CharacterType;

            if (Mod.instance.characters.ContainsKey(CharacterType))
            {

                npc = Mod.instance.characters[CharacterType];

            }
               
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

            string str = CharacterData.DialogueString(characterType, CharacterData.subjects.approach);

            if(introDialogue.Count > 0)
            {

                str = introDialogue.First(); 
                
                introDialogue.Clear();

            }

            List<Response> responseList = new List<Response>();

            if (questDialogue.Count > 0)
            {

                responseList.Add(new("quests", CharacterData.DialogueString(characterType, CharacterData.subjects.quests)));

            }

            List<CharacterData.subjects> subjects = new()
            {
                CharacterData.subjects.lore,
                CharacterData.subjects.adventure,
                CharacterData.subjects.attune,
            };

            foreach(CharacterData.subjects subject in subjects)
            {
                string option = CharacterData.DialogueString(characterType, subject);

                if (option != null)
                {

                    responseList.Add(new(subject.ToString(), option));


                }

            }

            Response nevermind = new ("none", CharacterData.DialogueString(characterType, CharacterData.subjects.nevermind));

            nevermind.SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape);

            responseList.Add(nevermind);

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

                            if (Mod.instance.save.progress.ContainsKey(questId))
                            {

                                RunSpecialDialogue(questId);

                                return;

                            }
 
                        }
                    
                    }

                    DelayedAction.functionAfterDelay(DialogueApproach, 100);

                    break;

                default:

                    List<CharacterData.subjects> subjects = new()
                    {
                        CharacterData.subjects.lore,
                        CharacterData.subjects.adventure,
                        CharacterData.subjects.attune,
                    };

                    foreach (CharacterData.subjects subject in subjects)
                    {
                        
                        if (answer == subject.ToString())
                        {

                            currentSubject = subject;

                            currentIndex = 0;

                            currentSpecial = subject.ToString();

                            DialogueSpecial generateSpecial = CharacterData.DialogueGenerator(characterType, subject);

                            if(generateSpecial == null)
                            {

                                break;

                            }

                            AddSpecialDialogue(subject.ToString(), generateSpecial);

                            RunSpecialDialogue(subject.ToString());

                        }

                    }

                    break;

            }

            return;

        }

        public virtual void RunSpecialDialogue(string prompt)
        {

            DialogueSpecial specialEntry = specialDialogue[prompt];

            if(specialEntry.responses.Count > 0)
            {

                currentSpecial = prompt;

                DelayedAction.functionAfterDelay(RunSpecialQuestion, 100);

                return;

            }

            if (specialEntry.questId != null)
            {

                Mod.instance.questHandle.DialogueCheck(specialEntry.questId, specialEntry.questContext, characterType);

                questDialogue.Remove(prompt);

                archiveDialogue.Add(prompt);

            }

            RunSpecialAnswer(specialEntry.intro);

        }

        public virtual void RunSpecialQuestion()
        {

            DialogueSpecial specialEntry = specialDialogue[currentSpecial];

            GameLocation.afterQuestionBehavior questionBehavior = new(RespondSpecialDialogue);

            List<Response> responseList = new();

            for (int r = 0; r < specialEntry.responses.Count; r++)
            {

                responseList.Add(new(r.ToString(), specialEntry.responses[r]));

            }

            responseList.Add(new Response("999", CharacterData.DialogueString(characterType, CharacterData.subjects.nevermind)).SetHotKey(Microsoft.Xna.Framework.Input.Keys.Escape));

            Game1.player.currentLocation.createQuestionDialogue(specialEntry.intro, responseList.ToArray(), questionBehavior, npc);

        }

        public virtual void RunSpecialAnswer(string answer)
        {

            Game1.currentSpeaker = npc;

            StardewValley.Dialogue dialogue = new(npc, "0", answer);

            Game1.activeClickableMenu = new DialogueBox(dialogue);

            Game1.player.Halt();

            Game1.player.CanMove = false;


        }


        public virtual void RespondSpecialDialogue(Farmer visitor, string dialogueId)
        {

            string specialId = currentSpecial;

            DialogueSpecial special = specialDialogue[specialId];

            int id = Convert.ToInt32(dialogueId);

            if (id == 999)
            {

                return;

            }

            if (special.lead)
            {

                currentIndex++;

                int answer = Convert.ToInt32(special.answers[id]);

                DialogueSpecial nextEntry = CharacterData.DialogueGenerator(characterType, currentSubject, currentIndex, answer);

                if (nextEntry == null)
                {

                    return;

                }

                AddSpecialDialogue(currentSpecial, nextEntry);

                if (nextEntry.responses.Count > 0)
                {

                    DelayedAction.functionAfterDelay(RunSpecialQuestion, 100);

                    return;

                }

                RunSpecialAnswer(nextEntry.intro);

                return;

            }

            if (special.questId != null)
            {

                Mod.instance.questHandle.DialogueCheck(special.questId, special.questContext, characterType, id);

                questDialogue.Remove(specialId);

                archiveDialogue.Add(specialId);

            }

            if (special.answers.Count > 0)
            {

                RunSpecialAnswer(special.answers[id]);

                return;

            }
            else if(special.outro != null)
            {

                introDialogue.Add(special.outro);

            }

            DelayedAction.functionAfterDelay(DialogueApproach,100);

        }


        public virtual void AddSpecialDialogue(string eventId, DialogueSpecial special)
        {

            if(special == null)
            {

                return;

            }

            specialDialogue[eventId] = special;

            if (special.prompt)
            {

                promptDialogue.Add(eventId);

            }

            if(special.questId != null)
            {
                
                if(special.questContext < 2)
                {
                    
                    questDialogue.Add(eventId);

                }

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

        public List<string> answers = new();

        public string outro;

        public bool prompt;

        public string questId;

        public int questContext;

        public bool lead;

    }

}
