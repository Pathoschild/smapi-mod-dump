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
    public class Jester : StardewDruid.Dialogue.Dialogue
    {
        public bool lessonGiven;

        public override void DialogueApproach()
        {

            if (specialDialogue.Count > 0)
            {

                DialogueSpecial();

            }
            else if (Mod.instance.QuestOpen("approachJester"))
            {

                DialogueIntro();
            
            }
            else
            {
                
                string str = "Jester gives you a mischievious look.";

                List<Response> responseList = new List<Response>();

                List<string> stringList = QuestData.StageProgress();

                if (!stringList.Contains("fates"))
                {

                    Game1.drawDialogue(npc,"Not yet, farmer. Not yet.");

                    Mod.instance.CastMessage("Complete more quests to unlock Jester content");

                }

                string questText = "(quests) I'm curious about what you have planned for today";

                if (stringList.Contains("ether"))
                {

                    questText = "(quests) Let's continue our search for the undervalley";

                }

                responseList.Add(new Response("quests", questText));

                responseList.Add(new Response("adventure", "(adventure) Let's talk adventure."));

                responseList.Add(new Response("rites", "(talk) I want to talk about some things."));

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
                case "Thanatoshi":
                    DelayedAction.functionAfterDelay(ReplyThanatoshi, 100);
                    break;
                case "afterQuarry":
                    DelayedAction.functionAfterDelay(ReplyAfterQuarry, 100);
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
                    DelayedAction.functionAfterDelay(ReplyAccept, 100);
                    return;
                case "refuse":
                    DelayedAction.functionAfterDelay(ReplyRefuse, 100);
                    return;
            }

            Game1.drawDialogue(npc, "(The strange cat shrugs back)");

        }

        public void DialogueIntro()
        {
            
            List<Response> responseList = new List<Response>();
            string str = "(The strange cat looks at you expectantly)";
            responseList.Add(new Response("introtwo", "Hello Kitty, are you far from home?"));
            responseList.Add(new Response("cancel", "(You hold out your empty hands and shrug)"));
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerIntro);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void DialogueIntroTwo()
        {
            List<Response> responseList = new List<Response>();
            string str = "Strange Cat: ^Far and not so far. I'm easily lost in this world. The patterns of the earth are strange enough, but the behaviour of humans... I'm muddled.";
            responseList.Add(new Response("introthree", "A cat can easily get lost out here."));
            responseList.Add(new Response("introthree", "An otherworldly visitor might be disorientated by the natural laws of this world, laws that keep it ordered and safe."));
            responseList.Add(new Response("introthree", "Forest magic can really mess with one's outlook. It's wack."));
            responseList.Add(new Response("introthree", "(Say nothing and pretend the cat can't talk)"));
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerIntro);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void DialogueIntroThree()
        {
            List<Response> responseList = new List<Response>();
            string str = "Strange Cat: ^Well farmer, you have the scent of destiny about you, and some otherworldly ability too. If I, the Jester of Fate, teach you my special tricks, will you help me find my way?";
            responseList.Add(new Response("accept", "Well I could use a big cat on the farm."));
            responseList.Add(new Response("accept", "A representative of fate? This is truly fortuitous. I accept your proposal."));
            responseList.Add(new Response("refuse", "I'm not making any deals with a strange cat on a bridge built by forest spirits!"));
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerIntro);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void ReplyAccept()
        {
            Game1.drawDialogue(npc, "Great! Now go across this bridge and descend into that dark dangerous dungeon over there. I'll meet you back on the farm, in that warmer, safer cave with the walking wood man in it.");
            CompleteIntro();
        }

        public void ReplyRefuse()
        {
            Game1.drawDialogue(npc, "Hehehe... I like you already! But you cannot escape this Fate, literally, and, well literally. When you've finished exploring the cave over there, come find me on your farm, in the cave with the walking wood man.");
            CompleteIntro();
        }

        public void CompleteIntro()
        {


            Mod.instance.CompleteQuest("approachJester");

            QuestData.NextProgress();

            Mod.instance.CastMessage("Jester has moved to the farm cave", -1);

            CharacterData.RelocateTo(nameof(Jester), "FarmCave");

        }

        public void ReplyAfterQuarry()
        {
            Game1.drawDialogue(npc, "The monsters that came out of that portal... they come from a realm adjacent to this one. " +
                "(Jester smirks) I tried to go through myself, but something barred me from passing through. " +
                "If I'm to pursue this mystery further, I'll have to figure out another way to the Ethereal plane. ");
        }

        public void ReplyThanatoshi()
        {
            Game1.drawDialogue(npc, "Thanatoshi is one of my distant kin. He fought in this valley, a long time ago, but I've never had the chance to ask him about why or what happened... he vanished.(Jester stares through you) It seems a dusty statue in a dungeon is all that remains of the deadly Thanatoshi...");
        }

    }
}
