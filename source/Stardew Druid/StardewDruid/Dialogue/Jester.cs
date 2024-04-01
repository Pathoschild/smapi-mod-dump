/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Force.DeepCloner;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.Minigames.BoatJourney;
using System.Security.Cryptography;

namespace StardewDruid.Dialogue
{
    public class Jester : StardewDruid.Dialogue.Dialogue
    {
        public bool lessonGiven;

        public override void DialogueApproach()
        {

            if (specialDialogue.Count > 0)
            {

                if (DialogueSpecial())
                {

                    return;

                }

            }
            
            if (Mod.instance.QuestOpen("approachJester"))
            {

                DialogueIntro();
            
            }
            else
            {
                
                string str = "Jester gives you a mischievious look.";

                List<Response> responseList = new List<Response>();

                List<string> stringList = QuestData.StageProgress();

                if (!stringList.Contains("Jester"))
                {

                    str = "Not yet, farmer. Not yet.";

                    npc.CurrentDialogue.Push(new(npc, "0", str));

                    Game1.drawDialogue(npc);

                    Mod.instance.CastMessage("Complete more quests to unlock Jester content");

                    return;

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


            }

        }

        public override bool DialogueSpecial()
        {

            string str = "I sense a change";

            List<Response> responseList = new List<Response>();

            KeyValuePair<string, string> dialogue = specialDialogue.First().ShallowClone();

            specialDialogue.Clear();

            if (dialogue.Value != npc.currentLocation.Name)
            {

                return false;

            }

            switch (dialogue.Key)
            {
                case "Thanatoshi":

                    str = "I hope he found peace...";

                    responseList.Add(new Response("Thanatoshi", "Grim. Dark. I Love this place."));

                    responseList.Add(new Response("Thanatoshi", "Do you know this figure?"));

                    responseList.Add(new Response("Thanatoshi", "I have a strange foreboding about this."));

                    break;

                case "ThanatoshiTwo":

                    str = "Jester of Fate:" +
                    "^I wasn't expecting the rite to produce a portal to the Undervalley. " +
                    "I don't think even Fortumei could have foreseen that.";

                    responseList.Add(new Response("ThanatoshiTwo", "Did you learn anything about the fallen one ?"));

                    break;

                case "ThanatoshiThree":

                    str = "Jester of Fate:^Thank you for helping me put Thanatoshi to rest.";

                    responseList.Add(new Response("quests", "I'm sorry about your kinsman."));

                    responseList.Add(new Response("quests", "I think this cutlass is to blame"));

                    break;

                case "townOne":
                case "townTwo":
                case "townThree":
                case "townFour":
                case "townFive":
                case "townSix":

                    str = Event.Scene.Town.DialogueIntros(dialogue.Key);

                    responseList = Event.Scene.Town.DialogueSetups(dialogue.Key);

                    break;

                default:

                    return false;

            }

            responseList.Add(new Response("none", "(say nothing)"));

            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerSpecial);

            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);

            return true;

        }

        public override void AnswerSpecial(Farmer visitor, string answer)
        {

            switch (answer)
            {
                case "quests":

                    new Quests(npc).Approach();
                    break;

                case "Thanatoshi":
                    DelayedAction.functionAfterDelay(ReplyThanatoshi, 100);
                    break;

                case "ThanatoshiTwo":
                    DelayedAction.functionAfterDelay(ReplyThanatoshiTwo, 100);
                    break;

                case "townOne":
                case "townTwo":
                case "townThree":
                case "townFour":
                case "townFive":
                case "townSix":

                    specialDialogue.Clear();

                    Event.Scene.Town.DialogueResponses(npc, answer);

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

            string str = "(The strange cat shrugs back)";

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

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

            string str = "Great! Now go across this bridge and descend into that dark dangerous dungeon over there. I'll meet you back on the farm, in that warmer, safer cave with the walking wood man in it.";

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

            CompleteIntro();
        }

        public void ReplyRefuse()
        {

            string str = "Hehehe... I like you already! But you cannot escape this Fate, literally, and, well literally. When you've finished exploring the cave over there, come find me on your farm, in the cave with the walking wood man.";

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

            CompleteIntro();
        }

        public void CompleteIntro()
        {


            Mod.instance.CompleteQuest("approachJester");

            QuestData.NextProgress();

            Mod.instance.CastMessage("Jester has moved to the farm cave", -1);

            CharacterData.RelocateTo(nameof(Jester), "FarmCave");

        }

        public void ReplyThanatoshi()
        {

            string str = "Thanatoshi is one of my distant kin. He fought in this valley, a long time ago, but that was before my time. " +
                "I've never had the chance to ask him about why or what happened... he vanished. " +
                "(Jester stares through you) It seems a dusty statue in a dungeon is all that remains of the deadly Thanatoshi...";

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

        }
        public void ReplyThanatoshiTwo()
        {

            string str = "The monsters that came out of that portal... they come from a realm adjacent to this one. " +
                "(Jester smirks) I tried to go through myself, but something barred me from passing through. " +
                "If I'm to pursue this mystery further, I'll have to figure out another way to the Ethereal plane. ";

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);
        }


    }

}
