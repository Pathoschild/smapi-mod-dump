/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid.Dialogue
{
    public class Quests
    {
        public NPC npc;

        public Quests(NPC NPC)
        {
            
            npc = NPC;

        }

        public void Approach()
        {
            string stage = QuestData.StageProgress().Last();

            if (npc is StardewDruid.Character.Effigy)
            {
                switch (stage)
                {
                    case "none":
                    case "weald":
                    case "mists":
                    case "stars":

                        DelayedAction.functionAfterDelay(ProgressQuests, 100);

                        return;

                    case "hidden":

                        DelayedAction.functionAfterDelay(HiddenQuests, 100);

                        return;

                    case "Jester":

                        DelayedAction.functionAfterDelay(JesterQuest, 100);

                        return;

                }
            
            }

            if (npc is StardewDruid.Character.Jester)
            {

                switch (stage)
                {

                    case "fates":
                    case "ether":

                        DelayedAction.functionAfterDelay(ProgressQuests, 100);

                        return;
                }
            
            }

            if (!Mod.instance.limits.Contains(npc.Name) && Context.IsMainPlayer)
            {
                
                DelayedAction.functionAfterDelay(CycleQuests, 100);
            
            }
            else
            {

                ReturnTomorrow();

            }

        }

        public void ReturnTomorrow()
        {
            
            if (npc is StardewDruid.Character.Effigy)
            {

                Game1.drawDialogue(npc, "Return to me tomorrow. I will listen for the voices of the wild and tell you what I hear.");

            }
            else if (npc is StardewDruid.Character.Jester)
            {

                Game1.drawDialogue(npc, "Prrr... (Jester is deep in thought about tomorrow's possibilities)");

            }
            else // Shadowtin
            {
                
                Game1.drawDialogue(npc, "We need a good rest before we confront the shadows of tomorrow.");

            }

                
        }

        public void ProgressQuests()
        {

            string quest = QuestData.NextProgress();

            if (quest == "none" || Mod.instance.QuestComplete(quest))
            {
                
                ReturnTomorrow();
            
            }
            else
            {
                string str = Mod.instance.QuestDiscuss(quest);

                Mod.instance.CastMessage("Druid journal has been updated");

                Game1.drawDialogue(npc, str);

            }

        }

        public void HiddenQuests()
        {
            
            QuestData.NextProgress();

            Game1.drawDialogue(npc, "Those with a twisted connection to the otherworld may remain tethered to the Valley long after their mortal vessel wastes away. " +
                "Strike them with bolt and flame to draw out and disperse their corrupted energies. " +
                "(Check your quest log for new challenges)");
        
        }

        public void JesterQuest()
        {
           
            if (QuestData.NextProgress() == "approachJester")
            {
                
                string str = "Fortune gazes upon you... but it can't be her. One of her kin perhaps.";
                
                List<Response> responseList = new List<Response>()
                {
                    new Response("Jester", "What do you mean?")
                
                };
                
                GameLocation.afterQuestionBehavior questionBehavior = new(AnswerJester);
                
                Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);

                return;
            
            }
            
            CycleQuests();

        }

        public void AnswerJester(Farmer visitor, string answer)
        {
            Game1.drawDialogue(npc,
                "I have said little of why the monarchs fell into their long slumber, and why the circle of druids was established here to care for the sacred places in their stead. " +
                "Amongst all the knowledge I still posess, these particular secrets are obscured. I can only say that traces of the remain towards the eastern face of the Mountain. " +
                "Be careful. The secrets of the Mountain's past are known to be guarded by the Fates themselves.");
            /*Game1.drawDialogue(npc, "I felt the industry of the forest spirits the night they toiled on the span across the mountain ravine. " +
                "They restored not only a bridge over land but between two destinies. " +
                "Should you decide to cross, a fateful encounter awaits you.");*/
        }

        public void CycleQuests()
        {
            string intro = "An old threat has re-emerged. Be careful, they may have increased in power since your last confrontation.";

            string accept = "(accept) I am ready to face the renewed threat";

            List<Response> responseList = new List<Response>();

            if (npc is StardewDruid.Character.Jester)
            {
                intro = "Come on, time for something fun! I like popping slimes.";

                accept = "(accept) I'm keen for a challenge.";
            }
            else if (npc is StardewDruid.Character.Shadowtin)
            {
                intro = "It might benefit your cause to investigate the Crypt from time to time. My former colleagues can be tenacious.";

                accept = "(accept) I will patrol the valley for signs of invaders.";

            }

            responseList.Add(new Response("accept", accept));

            if (npc is StardewDruid.Character.Effigy && Mod.instance.QuestComplete("challengeSandDragon") && !Mod.instance.QuestOpen("challengeSandDragonTwo"))
            {
                responseList.Add(new Response("dragon", "(dragon fight) I want a rematch with that ghost dragon!"));
            }

            if (npc is StardewDruid.Character.Jester && !Mod.instance.QuestOpen("challengeStarsTwo"))
            {
                responseList.Add(new Response("slimes", "(slimes fight) Lets put ol' pumpkin head in his place."));
            }

            if (npc is StardewDruid.Character.Shadowtin && !Mod.instance.QuestOpen("challengeEtherTwo"))
            {
                responseList.Add(new Response("crypt", "(crypt fight) Lets see what the thieves have been up to."));
            }

            responseList.Add(new Response("abort", "I have been unable to proceed against any of these threats. Can we start over? (abort quests)"));

            responseList.Add(new Response("cancel", "(not now) I'll need some time to prepare"));

            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerThreats);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

        }

        public void AnswerThreats(Farmer effigyVisitor, string effigyAnswer)
        {
            
            string dialogueText = "The valley will withstand the threat as it can, as it always has.";
            
            if (npc is StardewDruid.Character.Jester)
            {
                
                dialogueText = "We will leave the Valley to it's fate. Actually I'm not sure if there's a specific Fate who looks after this place.";
                
            }

            if (npc is StardewDruid.Character.Shadowtin)
            {

                dialogueText = "Every moment that we idle the shadow grows longer. I think it has something to do with the rotation of the earth relative to that giant ball of awful you call the sun.";

            }


            switch (effigyAnswer)
            {
                case "accept":
                    
                    Dictionary<string, string> dictionary = QuestData.SecondQuests();
                    
                    List<string> questList = new List<string>();
                    
                    List<string> validList = new List<string>();
                    
                    dialogueText = "May you be successful against the shadows of the otherworld.";

                    if (npc is StardewDruid.Character.Jester)
                    {

                        dialogueText = "(Jester tries again to mimic the Effigy) May the shadows of the full world be against the successes of others.";

                    }

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        dialogueText = "We will vanquish our enemies and claim their treasures.";

                    }

                    Mod.instance.CastMessage("Druid journal has been updated");
                    
                    foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                    {
                        
                        if (!Mod.instance.QuestGiven(keyValuePair.Key))
                        {
                            
                            Mod.instance.NewQuest(keyValuePair.Key);
                            
                            Mod.instance.limits.Add(npc.Name);
                        
                            break;
                        
                        }

                        if (!Mod.instance.QuestComplete(keyValuePair.Key))
                        {

                            break;

                        }

                        string quest = keyValuePair.Key + "Two";

                        if (!Mod.instance.QuestOpen(quest))
                        {

                            questList.Add(quest);
                        
                        }

                        validList.Add(quest);
                    
                    }

                    if (questList.Count == 0)
                    {

                        questList = validList;

                    }
                     
                    if(questList.Count > 0)
                    {

                        Mod.instance.limits.Add(npc.Name);

                        string quest1 = questList[Game1.random.Next(questList.Count)]; Mod.instance.NewQuest(quest1);

                        //foreach (string quest in questList){ Mod.instance.NewQuest(quest);}

                    }

                    break;
                
                case "dragon":

                    dialogueText = "The Tyrant must be put to rest.";
                    
                    Mod.instance.NewQuest("challengeSandDragonTwo");
                    
                    Mod.instance.limits.Add(npc.Name);
                    
                    Mod.instance.CastMessage("Druid journal has been updated");
                    
                    break;
                
                case "slimes":
                    
                    dialogueText = "Time for me to ready my beam face!";
                    
                    Mod.instance.NewQuest("challengeStarsTwo");
                    
                    Mod.instance.limits.Add(npc.Name);
                    
                    Mod.instance.CastMessage("Druid journal has been updated");
                    
                    break;

                case "crypt":

                    dialogueText = "Now to the final resting place of the humanfolk.";

                    Mod.instance.NewQuest("challengeEtherTwo");

                    Mod.instance.limits.Add(npc.Name);

                    Mod.instance.CastMessage("Druid journal has been updated");

                    break;

                case "abort":
                    
                    using (List<string>.Enumerator enumerator = QuestData.ActiveSeconds().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            
                            string current = enumerator.Current;
                            
                            Mod.instance.RemoveQuest(current);
                        
                        }
                        break;
                    
                    }

            }

            Game1.drawDialogue(npc, dialogueText);

        }

    }

}
