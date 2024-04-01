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

                case "fates":
                case "ether":

                    if (npc is StardewDruid.Character.Jester)
                    {
                        DelayedAction.functionAfterDelay(ProgressQuests, 100);
                        return;
                    }

                    break;

                case "town":
                case "beach":
                case "woods":
                    
                    if (Context.IsMainPlayer)
                    {

                        DelayedAction.functionAfterDelay(HeartQuests, 100);

                    }

                    return;

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
            string str = "Return to me tomorrow. I will listen for the voices of the wild and tell you what I hear.";

            if (npc is StardewDruid.Character.Shadowtin)
            {

                 str = "We need a good rest before we confront the shadows of tomorrow.";

            }
            else if (npc is StardewDruid.Character.Jester)
            {

                str = "Prrr... (Jester is deep in thought about tomorrow's possibilities)";

            }

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

        }

        public void HeartQuests()
        {

            string intro = "I have something to ask you";

            string stage = QuestData.StageProgress().Last();

            List<Response> responseList = new List<Response>();

            switch (stage)
            {

                case "beach":

                    if (npc is StardewDruid.Character.Effigy)
                    {

                        intro = "We all have somewhere to be today.";

                        responseList.Add(new Response("accept", "(personal quest) You have somewhere to be?"));

                    }

                    if (npc is StardewDruid.Character.Jester)
                    {
                        
                        intro = "Wood-face looks like he could do with some cheer and company.";

                        responseList.Add(new Response("request", "(take hint) I'll go see what I can do for our friend."));

                    }

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        intro = "The animated scarecrow appears to have become concerned with matters beyond the scope of it's directive. " +
                            "From the way it lingers near the borders of the farm, I suspect it prepares to abandon it's post. Figuratively speaking of course. " +
                            "Though there's still the option to fix it to an actual post. But would it be more stuck-up or less?";

                        responseList.Add(new Response("request", "(take hint) I'll ask the Effigy what it intends."));

                    }

                    break;

                case "town":

                    if (npc is StardewDruid.Character.Effigy)
                    {

                        intro = "Our leonine ally has spent enough time in melancholy and idleness. A beast of it's stature requires proper exercise and stimulation.";

                        responseList.Add(new Response("request", "(take hint) Jester might like to go for a walk."));
                    
                    }

                    if (npc is StardewDruid.Character.Jester)
                    {

                        intro = "Hey farmer. Do you feel like things have been pretty intense lately?";

                        responseList.Add(new Response("accept", "(personal quest) We've been through a lot. You've come a long way since getting lost on the mountain."));

                    }

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        intro = "Something's up with your cat friend.";

                        responseList.Add(new Response("request", "(take hint) That cat's always up to something. I'll go see what."));

                    }

                    break;

                case "woods":

                    if (npc is StardewDruid.Character.Effigy)
                    {
                        intro = "The shadowfolk appears eager to contract you into one of his schemes. " +
                            "Perhaps he'll prove he is worth the opportunity you've afforded him. " +
                            "Or perhaps he'll prove that prejudice against his kind is still justified. " +
                            "I will trust in your judgement either way.";

                        responseList.Add(new Response("request", "(take hint) Thanks, I'll go see what Shadowtin requires."));
                    }

                    if (npc is StardewDruid.Character.Jester)
                    {
                        intro = "Metal-face told me he is looking for a professional for a job. I profess to null, apparently.";

                        responseList.Add(new Response("request", "(take hint) Don't take it personally. I'll go talk to him."));

                    }

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        intro = "So Dragon master, I've got a few leads for us.";

                        responseList.Add(new Response("accept", "(personal quest) I'm open to your ideas"));

                    }

                    break;

            }

            responseList.Add(new Response("quests", "(quests) Is there anything else that needs my attention?"));

            GameLocation.afterQuestionBehavior questionBehavior = new(HeartResponse);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

        }

        public void HeartResponse(Farmer visitor, string answer)
        {

            switch (answer)
            {

                case "accept":

                    DelayedAction.functionAfterDelay(ProgressQuests, 100);

                    break;

                case "quests":

                    if (!Mod.instance.limits.Contains(npc.Name) && Context.IsMainPlayer)
                    {

                        DelayedAction.functionAfterDelay(CycleQuests, 100);

                    }
                    else
                    {

                        ReturnTomorrow();

                    }

                    break;
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
                string str = Map.QuestData.QuestList()[quest].questDiscuss;

                Mod.instance.CastMessage("Druid journal has been updated");

                npc.CurrentDialogue.Push(new(npc, "0", str));

                Game1.drawDialogue(npc);
            }

        }

        public void HiddenQuests()
        {
            
            QuestData.NextProgress();

            string str = "Those with a twisted connection to the otherworld may remain tethered to the Valley long after their mortal vessel wastes away. " +
                "Strike them with bolt and flame to draw out and disperse their corrupted energies. " +
                "(Check your quest log for new challenges. You only need to complete one challenge to progress your Druid journey)";

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);


        }

        public void JesterQuest()
        {

            string str = "Fortune gazes upon you... but it can't be her. One of her kin perhaps.";
                
            List<Response> responseList = new List<Response>()
            {
                new Response("quests", "(challenges) Is the valley safe?"),
                new Response("Jester", "(main quest) What do you mean?"),
                
            };
                
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerJester);
                
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);

        }

        public void AnswerJester(Farmer visitor, string answer)
        {

            switch (answer)
            {

                case "Jester":

                    QuestData.NextProgress();

                    string str = "I have said little of why the monarchs fell into their long slumber, and why the circle of druids was established here to care for the sacred places in their stead. " +
                        "I know the truth is hidden within the core of my being, but the path to it is obscured. The only knowledge I am certain of is that answers may lie towards the eastern face of the Mountain. " +
                        "As the new leader of the circle, it's up to you to seek out the truth of the past. Be careful. Such secrets are known to be guarded by the Fates themselves.";

                    npc.CurrentDialogue.Push(new(npc, "0", str));

                    Game1.drawDialogue(npc);
                    return;

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

        public void AnswerThreats(Farmer visitor, string answer)
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


            switch (answer)
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

                        string quest1 = questList[Game1.random.Next(questList.Count)]; 
                        
                        Mod.instance.NewQuest(quest1);

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

                    dialogueText = "Let's see what the old gang's been up to";

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

            npc.CurrentDialogue.Push(new(npc, "0", dialogueText));

            Game1.drawDialogue(npc);

        }

    }

}
