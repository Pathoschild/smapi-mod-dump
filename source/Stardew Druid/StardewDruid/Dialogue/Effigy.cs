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
using StardewDruid.Character;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile;

namespace StardewDruid.Dialogue
{
    public class Effigy : StardewDruid.Dialogue.Dialogue
    {
        public bool lessonGiven;
        public string questFeedback;

        public override void DialogueApproach()
        {

            if (specialDialogue.Count > 0)
            {

                if (DialogueSpecial())
                {

                    return;

                }

            }
            
            if (Mod.instance.QuestOpen("approachEffigy"))
            {
                
                Mod.instance.CompleteQuest("approachEffigy");

                Mod.instance.CharacterRegister(nameof(Effigy), "FarmCave");

                CharacterData.CharacterDefault(nameof(Effigy), "FarmCave");

                DialogueIntro();

            }
            else
            {
                string str = "Forgotten Effigy: ^Successor.";

                List<Response> responseList = new List<Response>();

                List<string> stringList = QuestData.StageProgress();

                if (stringList.Contains("fates"))
                {

                    responseList.Add(new Response("quests", "(quests) What threatens the valley?"));

                    responseList.Add(new Response("adventure", "(adventure) It's time for a change of scene"));
 
                }
                else if (stringList.Contains("Jester"))
                {

                    responseList.Add(new Response("quests", "(fate) Do you feel a strange presence?"));

                    responseList.Add(new Response("relocate", "(relocate) It's time for a change of scene"));

                }
                else if (stringList.Contains("hidden"))
                    responseList.Add(new Response("quests", "(quests) Is the valley safe?"));
                else if (stringList.Contains("stars"))
                    responseList.Add(new Response("quests", "(lessons) I want to master the power of the stars."));
                else if (stringList.Contains("mists"))
                    responseList.Add(new Response("quests", "(lessons) What can you teach me about the mists?"));
                else if (stringList.Contains("weald"))
                    responseList.Add(new Response("quests", "(lessons) I want to learn more about the weald."));
                if (Mod.instance.CurrentProgress() > 2)
                    responseList.Add(new Response("rites", "(talk) I have some requests."));

                responseList.Add(new Response("none", "(say nothing)"));
                Effigy effigy = this;
                GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);
                returnFrom = null;
                Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
            }
        }

        public override void AnswerApproach(Farmer visitor, string answer)
        {
            switch (answer)
            {
                case "intro":
                    DelayedAction.functionAfterDelay(DialogueQuery, 100);
                    break;
                case "quests":
                case "journey":
                    new Quests(npc).Approach();
                    break;
                case "adventure":
                    new Adventure(npc).Approach();
                    break;
                case "rites":
                    new Rites(npc).Approach();
                    break;
                case "relocate":
                    DelayedAction.functionAfterDelay(DialogueRelocate, 100);
                    break;

            }
        }

        public override bool DialogueSpecial()
        {

            string str = "I sense a change";

            List<Response> responseList = new List<Response>();

            KeyValuePair<string, string> dialogue = specialDialogue.First().ShallowClone();

            if (dialogue.Value != npc.currentLocation.Name)
            {

                return false;

            }

            switch (dialogue.Key)
            {

                case "Aquifer":

                    responseList.Add(new Response("journey", "The rite disturbed the bats. ALL the bats."));

                    break;

                case "Graveyard":

                    responseList.Add(new Response("journey", "The graveyard has a few less shadows."));

                    break;

                case "Infestation":

                    responseList.Add(new Response("journey", "I defeated the Pumpkin Slime. Now I'm covered in his gunk."));

                    break;

                case "Demetrius":

                    str = "I had a peculiar visitor";

                    responseList.Add(new Response("Demetrius", "Did you meet Demetrius?"));

                    specialDialogue.Clear();

                    break;

                case "swordEarth":

                    responseList.Add(new Response("journey", "The tree gave me a branch shaped like a sword."));

                    break;

                case "swordWater":

                    responseList.Add(new Response("journey", "I went to the pier and... was that a bolt of lightning?"));

                    break;

                case "swordStars":

                    responseList.Add(new Response("journey", "I found the lake of flames."));

                    break;

                case "beachOne":
                case "beachTwo":
                case "beachThree":
                case "beachFour":
                case "beachFive":
                case "beachSix":

                    str = Event.Scene.Beach.DialogueIntros(dialogue.Key);

                    responseList = Event.Scene.Beach.DialogueSetups(dialogue.Key);

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
                case "journey":

                    specialDialogue.Clear();

                    new Quests(npc).Approach();

                    break;

                case "Demetrius":

                    DelayedAction.functionAfterDelay(DialogueDemetrius, 100);
                    
                    break;

                case "beachOne":
                case "beachTwo":
                case "beachThree":
                case "beachFour":
                case "beachFive":
                case "beachSix":

                    specialDialogue.Clear();

                    Event.Scene.Beach.DialogueResponses(npc,answer);

                    break;

            }


        }

        public void DialogueIntro()
        {
            string str = "So the successor appears, and has demonstrated remarkable potential. I am the Effigy of the First Farmer, and the sole remnant of my circle of Druids.";
            List<Response> responseList = new List<Response>()
              {
                new Response("intro", "Who stuck you in the ceiling?"),
                new Response("none", "(say nothing)")
              };
            Effigy effigy = this;
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void DialogueQuery()
        {
            List<Response> responseList = new List<Response>();
            string str = "Forgotten Effigy: ^I was crafted by the first farmer of the valley, a powerful friend of the otherworld. If you intend to succeed him, you will need to learn many lessons.";
            responseList.Add(new Response("quests", "(start journey) Ok. What is the first lesson?"));
            responseList.Add(new Response("none", "(say nothing)"));
            Effigy effigy = this;
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);
            returnFrom = null;
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void DialogueDemetrius()
        {
            string str = "Forgotten Effigy: ^I concealed myself for a time, then I spoke to him in the old tongue of the Calico shamans.";
            List<Response> responseList = new List<Response>()
              {
                new Response("descended", "Do you think Demetrius is descended from the shaman tradition?!"),
                new Response("offended", "Wow, he must have been offended. Demetrius is a man of modern science and sensibilities."),
                new Response("return", "Nope, not going to engage with this.")
              };
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerDemetrius);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void AnswerDemetrius(Farmer visitor, string answer)
        {
            if (answer == "return")
            {
                returnFrom = "demetrius";
                Effigy effigy = this;
                DelayedAction.functionAfterDelay(DialogueApproach, 100);
            }
            else
            {
                string str = Game1.player.caveChoice.Value != 1 ? "I can smell the crisp, sandy scent of the Calico variety of mushroom. The shamans would eat them to... enter a trance-like state." : "... ... He came in with a feathered mask on, invoked a rite of summoning, threw Bat feed everywhere, and then left just as quickly as he entered. His shamanic heritage is very... particular.";
 
                npc.CurrentDialogue.Push(new(npc, "0", str));

                Game1.drawDialogue(npc);

            }
       }

        public void DialogueRelocate()
        {
            List<Response> responseList = new List<Response>();

            string str = "Forgotten Effigy: Where shall I await your command?";

            if (npc.DefaultMap == "FarmCave")
            {

                string farm = "My farm would benefit from your gentle stewardship. (The Effigy will garden around scarecrows on the farm)";

                responseList.Add(new Response("Farm", farm));
            }

            if (npc.DefaultMap == "Farm" || npc.netFollowActive.Value)
            {

                string shelter = "Shelter within the farm cave for the while.";

                responseList.Add(new Response("FarmCave", shelter));

            }

            responseList.Add(new Response("return", "(nevermind)"));

            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerRelocate);

            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        
        }


        public void AnswerRelocate(Farmer visitor, string answer)
        {

            string str = "(nevermind isn't a place, successor)";
            string title = "The Effigy";

            switch (answer)
            {
                case "FarmCave":

                    str = "I will return to where I may feel the rumbling energies of the Valley's leylines.";

                    CharacterData.RelocateTo(npc.Name, "FarmCave");

                    Mod.instance.CastMessage(title + " has moved to the farm cave", -1);
                    break;

                case "Farm":

                    str = "I will take my place amongst the posts and furrows of my old master's home.";

                    CharacterData.RelocateTo(npc.Name, "Farm");

                    Mod.instance.CastMessage(title + " now roams the farm", -1);

                    break;


            }

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

        }

    }

}
