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
using StardewValley.Objects;
using System.Collections.Generic;

namespace StardewDruid.Dialogue
{
    public class Effigy : StardewDruid.Dialogue.Dialogue
    {
        public bool lessonGiven;
        public string questFeedback;

        public override void DialogueApproach()
        {
            if (specialDialogue.Count > 0)
                DialogueSpecial();
            else if (Mod.instance.QuestOpen("approachEffigy"))
            {
                Mod.instance.CompleteQuest("approachEffigy");
                Mod.instance.CharacterRegister(nameof(Effigy), "FarmCave");
                DialogueIntro();
            }
            else
            {
                string str = "Forgotten Effigy: ^Successor.";
                List<Response> responseList = new List<Response>();
                List<string> stringList = QuestData.StageProgress();
                if (stringList.Contains("fates") || stringList.Contains("Jester"))
                {
                    responseList.Add(new Response("quests", "What threatens the valley? (quests)"));
                    if (Context.IsMainPlayer)
                        responseList.Add(new Response("relocate", "It's time for a change of scene"));
                }
                else if (stringList.Contains("hidden"))
                    responseList.Add(new Response("quests", "Is the valley safe? (quests)"));
                else if (stringList.Contains("stars"))
                    responseList.Add(new Response("quests", "I want to master the power of the stars (lessons)"));
                else if (stringList.Contains("mists"))
                    responseList.Add(new Response("quests", "What can you teach me about the mists? (lessons)"));
                else if (stringList.Contains("weald"))
                    responseList.Add(new Response("quests", "I want to learn more about the weald (lessons)"));
                if (Mod.instance.CurrentProgress() > 2)
                    responseList.Add(new Response("rites", "I have some requests (manage rites)"));
                responseList.Add(new Response("none", "(say nothing)"));
                Effigy effigy = this;
                GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);
                returnFrom = null;
                Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
            }
        }

        public override void AnswerApproach(Farmer effigyVisitor, string effigyAnswer)
        {
            switch (effigyAnswer)
            {
                case "intro":
                    DelayedAction.functionAfterDelay(DialogueQuery, 100);
                    break;
                case "quests":
                    new Quests(npc).Approach();
                    break;
                case "relocate":
                    DelayedAction.functionAfterDelay(DialogueRelocate, 100);
                    break;
                case "Demetrius":
                    DelayedAction.functionAfterDelay(DialogueDemetrius, 100);
                    break;
                case "rites":
                    new Rites(npc).Approach();
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
            responseList.Add(new Response("quests", "Ok. What is the first lesson? (start journey)"));
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
                new Response("return", "Nope, not going to engage with ")
              };
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerDemetrius);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void AnswerDemetrius(Farmer effigyVisitor, string effigyAnswer)
        {
            if (effigyAnswer == "return")
            {
                returnFrom = "demetrius";
                Effigy effigy = this;
                DelayedAction.functionAfterDelay(DialogueApproach, 100);
            }
            else
                Game1.drawDialogue(npc, Game1.player.caveChoice.Value != 1 ? "I can smell the crisp, sandy scent of the Calico variety of mushroom. The shamans would eat them to... enter a trance-like state." : "... ... He came in with a feathered mask on, invoked a rite of summoning, threw Bat feed everywhere, and then left just as quickly as he entered. His shamanic heritage is very... particular.");
        }

        public void DialogueRelocate()
        {
            
            List<Response> responseList = new List<Response>();
            
            string str = "Forgotten Effigy: ^Now that you have vanquished the twisted spectres of the past, it is safe for me to roam the wilds of the Valley once more. Where shall I await your command?^";
            
            if (npc.DefaultMap == "FarmCave")
            {
                responseList.Add(new Response("Farm", "My farm would benefit from your gentle stewardship. (The Effigy will target scarecrows with Rite of the Earth effects, automatically sewing seeds, fertilising and watering tilled earth around any scarecrow)"));

            }
            
            if (npc.DefaultMap == "Farm")
            {
                responseList.Add(new Response("FarmCave", "Shelter within the farm cave for the while."));

            }
                
            responseList.Add(new Response("return", "(nevermind)"));
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerRelocate);
            returnFrom = null;
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public void AnswerRelocate(Farmer effigyVisitor, string effigyAnswer)
        {
            string str = "(nevermind isn't a place, successor)";
            switch (effigyAnswer)
            {
                case "FarmCave":
                    str = "I will return to where I may feel the rumbling energies of the Valley's leylines.";
                    Mod.instance.CharacterRegister(npc.Name, "FarmCave");
                    npc.WarpToDefault();
                    (npc as StardewDruid.Character.Effigy).SwitchDefaultMode();
                    Mod.instance.CastMessage("The Effigy has moved to the farm cave", -1);
                    break;
                case "Farm":
                    str = "I will take my place amongst the posts and furrows of my old master's home.";
                    Mod.instance.CharacterRegister(npc.Name, "Farm");
                    npc.WarpToDefault();
                    (npc as StardewDruid.Character.Effigy).SwitchRoamMode();
                    Mod.instance.CastMessage("The Effigy now roams the farm", -1);
                    break;
            }
            Game1.drawDialogue(npc, str);
        }

    }
}
