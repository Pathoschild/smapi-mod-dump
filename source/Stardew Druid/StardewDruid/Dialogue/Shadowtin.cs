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
using StardewValley.Buildings;
using StardewValley.Locations;
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Dialogue
{
    public class Shadowtin : StardewDruid.Dialogue.Dialogue
    {
        public bool lessonGiven;

        public override void DialogueApproach()
        {
            if (specialDialogue.Count > 0)
            {

                DialogueSpecial();

            }
            else if (Mod.instance.QuestOpen("approachShadowtin"))
            {

                DialogueIntro();
            
            }
            else
            {
                
                string str = "Shadowtin's ethereal eyes shine through a cold metal mask";

                List<Response> responseList = new List<Response>();

                List<string> stringList = QuestData.StageProgress();

                if (!stringList.Contains("complete"))
                {

                    Game1.drawDialogue(npc, "Am I supposed to know you?");

                    Mod.instance.CastMessage("Complete more quests to unlock Shadowtin content");

                }

                string questText = "(quests) What are the latest treasure prospects?";

                responseList.Add(new Response("quests", questText));

                responseList.Add(new Response("adventure", "(adventure) Lets talk about our partnership."));

                responseList.Add(new Response("rites", "(talk) I want to talk about some things"));

                responseList.Add(new Response("none", "(say nothing)"));

                GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);
                
                returnFrom = null;
                
                Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
            
            }
       
        }

        public override void AnswerApproach(Farmer visitor, string answer)
        {

            switch (answer)
            {
                case "rites":
                    new Rites(npc).Approach();
                    break;
                case "quests":
                case "journey":
                    new Quests(npc).Approach();
                    break;
                case "adventure":
                    new Adventure(npc).Approach();
                    break;
                case "none":
                    Game1.drawDialogue(npc, "The shadow warrior continues to watch you.");
                    break;

            }

        }

        public void AnswerIntro(Farmer visitor, string answer)
        {

            switch (answer)
            {
                case "introtwo":
                    DelayedAction.functionAfterDelay(DialogueIntroTwo, 100);
                    return;
                case "introthree":
                    DelayedAction.functionAfterDelay(DialogueIntroThree, 100);
                    return;
                case "accept":
                    ReplyAccept();
                    return;
                case "refuse":
                    ReplyRefuse();
                    return;
            }

            Game1.drawDialogue(npc, "The shadow warrior continues to watch you.");

        }

        public void DialogueIntro()
        {
            List<Response> responseList = new List<Response>();
            string str = "Masked Shadow: Hail to you, Dragon master.";
            responseList.Add(new Response("introtwo", "Greetings, shadow warrior. I am " + Game1.player.Name));
            responseList.Add(new Response("cancel", "(ignore him)"));
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerIntro);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void DialogueIntroTwo()
        {
            List<Response> responseList = new List<Response>();
            string str = "Masked Shadow: Shadowtin Bear, professional treasure hunter, ready for hire. If you're amenable to a new partnership, I'd be honoured to join your crew.";
            responseList.Add(new Response("introthree", "I might be open to the idea, once you return all the dragon treasures your shady friends took."));
            responseList.Add(new Response("introthree", "I have no reason to trust a thief who answers to the whims of Lord Deep"));
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerIntro);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void DialogueIntroThree()
        {
            List<Response> responseList = new List<Response>();
            string str = "Shadowtin Bear: I can't answer for my former colleagues, or return the treasures or ether they pilfered, but I suspect the Deep one has deceived us, and I have an idea that I'll find the truth of it if I follow you.";
            //They've already given everything we gathered to the Lord, except for the few things I've retained for study, as my desire is to discover and reveal the lost history of my folk, a history shrouded by the sweetened words of the Deep one.I believe the truth will be revealed on the path set before you.
            responseList.Add(new Response("accept", "I've heard enough! Welcome to the circle of druids."));
            responseList.Add(new Response("refuse", "I have a better idea. Shadowshift back to your master and tell him I'm coming for him. Soon."));
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerIntro);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void ReplyAccept()
        {
            Game1.drawDialogue(npc, "You have shown wisdom, Dragon master. I make my services and research available to you. You'll find me in the farmcave your companions loiter within.");
            CompleteIntro();
        }

        public void ReplyRefuse()
        {
            Game1.drawDialogue(npc, "The surface expedition has failed, and my shadowhome will offer no protection from the wrath of Deep. I'll consult your companions instead. I'm sure they will see the opportunities you do not.");
            CompleteIntro();
        }

        public void CompleteIntro()
        {   
            if(npc.currentLocation is not FarmCave)
            {
                Mod.instance.CastMessage("Shadowtin has moved to the farm cave", -1);
            }

            Mod.instance.CompleteQuest("approachShadowtin");

            QuestData.NextProgress();

            CharacterData.RelocateTo("Shadowtin", "FarmCave");

        }


    }

}
