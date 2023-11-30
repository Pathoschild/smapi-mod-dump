/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.VisualBasic;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Locations;
using StardewValley.Projectiles;
using StardewDruid.Cast.Fates;
using System.Reflection.PortableExecutable;
using StardewDruid.Event.World;
using System.Diagnostics;
using StardewDruid.Map;

namespace StardewDruid.Dialogue
{
    public class Jester : Dialogue
    {

        public bool lessonGiven;

        public Jester()
        {

        }

        public override void DialogueApproach()
        {

            if (specialDialogue.Count > 0)
            {

                DialogueSpecial();

                return;

            }

            if (Mod.instance.CharacterMap("Jester") == "Mountain")
            {

                DialogueIntro();

                return;

            }

            if (Mod.instance.QuestOpen("approachJester"))
            {

                Mod.instance.CompleteQuest("approachJester");

                Mod.instance.CharacterRegister("Jester", "FarmCave");

            }

            string question = "(Jester gives you a mischievious look)";

            List<Response> choices = new();

            if (!Map.QuestData.JesterCompleted())
            {

                choices.Add(new Response("lesson", "I'm curious about what you have planned for today (lessons)"));

            }
            else if (Context.IsMainPlayer)
            {
                
                choices.Add(new Response("relocate", "I've got an idea for you (relocate/follow)"));

            }

            if (Mod.instance.BlessingList().ContainsKey("fates"))
            {
                
                choices.Add(new Response("effects", "I want to know more about the Fates (manage effects)"));

            }

            choices.Add(new Response("none", "(say nothing)"));

            GameLocation.afterQuestionBehavior behaviour = new(AnswerApproach);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

            return;

        }

        public override void AnswerApproach(Farmer visitor, string answer)
        {
            
            switch (answer)
            {

                case "introtwo":

                    DelayedAction.functionAfterDelay(DialogueIntroTwo, 100);

                    break;

                case "introthree":

                    DelayedAction.functionAfterDelay(DialogueIntroThree, 100);

                    break;

                case "accept":

                    DelayedAction.functionAfterDelay(ReplyAccept, 100);

                    break;

                case "refuse":

                    DelayedAction.functionAfterDelay(ReplyRefuse, 100);

                    break;

                case "lesson":

                    DelayedAction.functionAfterDelay(ReplyLesson, 100);

                    break;

                case "relocate":

                    DelayedAction.functionAfterDelay(DialogueRelocate, 100);

                    break;

                case "effects":

                    DelayedAction.functionAfterDelay(DialogueEffects, 100);

                    break;

                case "afterQuarry":

                    DelayedAction.functionAfterDelay(ReplyAfterQuarry, 100);

                    break;

            }

            return;

        }

        // =============================================================
        // Challenge
        // =============================================================

        public void DialogueIntro()
        {

            Mod.instance.CompleteQuest("approachEffigy");

            List<Response> choices = new();

            string question = "(The strange cat looks at you expectantly)";

            choices.Add(new Response("introtwo", "Hello Kitty, are you far from home?"));

            choices.Add(new Response("cancel", "(You hold out your empty hands and shrug)"));

            GameLocation.afterQuestionBehavior behaviour = new(AnswerApproach);

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

        }

        public void DialogueIntroTwo()
        {

            Mod.instance.CompleteQuest("approachEffigy");

            List<Response> choices = new();

            string question = "Strange Cat: ^Far and not so far. I get lost easily in the material world. " +
                "The patterns of the earth, the behaviour of mortals, they're so silent and unyielding. I'm muddled.";

            choices.Add(new Response("introthree", "An otherworldly visitor might be disorientated by the natural laws of this world, laws that keep it ordered and safe."));

            choices.Add(new Response("introthree", "Forest magic can really mess with one's perception of nature. It's wack."));

            choices.Add(new Response("introthree", "(Say nothing and pretend the cat can't talk)"));

            GameLocation.afterQuestionBehavior behaviour = new(AnswerApproach);

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

        }

        public void DialogueIntroThree()
        {

            List<Response> choices = new();

            string question = "Strange Cat: ^Well farmer, you have the scent of destiny about you, and some otherworldly ability too. " +
                "If I, the Jester of Fate, teach you my special techniques, will you help me find my way?";

            choices.Add(new Response("accept", "A representative of fate? This is truly fortuitous. I accept your proposal."));

            choices.Add(new Response("refuse", "I'm not making any deals with a strange cat on a bridge built by forest spirits!"));

            GameLocation.afterQuestionBehavior behaviour = new(AnswerApproach);

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

        }

        public void ReplyAccept()
        {

            Game1.drawDialogue(npc, "Great! Now go across this bridge and descend into that dark dangerous tunnel over there. " +
                "You'll find a shrine to one of my kin. " +
                "I'll meet you back on the farm, in that warmer, safer cave with the walking wood man in it. " +
                "(You will need to have gained the Golden Scythe from the quarry tunnel before starting Jester's lessons)");

            CompleteIntro();

        }

        public void ReplyRefuse()
        {

            Game1.drawDialogue(npc, "Hehehe... I like you already! But you cannot escape this Fate, literally, and, well literally. " +
                "When you've finished exploring the cave over there, come find me on your farm, in the cave with the walking wood man. " +
                "(You will need to have gained the Golden Scythe from the quarry tunnel before starting Jester's lessons)");

            CompleteIntro();

        }

        public void CompleteIntro()
        {

            Mod.instance.CastMessage("Jester has moved to the farm cave", -1);

            (npc as Character.Jester).SwitchRoamMode();

            Mod.instance.CompleteQuest("approachJester");

            Mod.instance.CharacterRegister("Jester", "FarmCave");

            npc.WarpToDefault();

        }

        public void ReplyLesson()
        {

            if (lessonGiven)
            {

                Game1.drawDialogue(npc,"Prrr... (Jester is deep in thought about tomorrow's possibilities)");

                return;

            }
            
            if (!Game1.player.mailReceived.Contains("gotGoldenScythe"))
            {

                Game1.drawDialogue(npc, "For a mortal to even comprehend the forces of destiny, they'll need an instrument of fate itself. Past the bridge where we first met is a cave. " +
                    "If I recall my family history correctly, there's a shrine to one of my kin inside, and maybe something you can use! " +
                    "(You will need to have gained the Golden Scythe from the quarry tunnel before starting Jester's lessons)");

                return;

            }

            if (!Mod.instance.BlessingList().ContainsKey("fates"))
            {

                Mod.instance.UpdateBlessing("fates");

                Mod.instance.ChangeBlessing("fates");

            }

            switch (Mod.instance.BlessingList()["fates"])
            {

                case 0: // whisk

                    Game1.drawDialogue(npc, "Now you can travel like the Others do. It's fun, but also a bit, well, (Jester grins). I'm sure you'll get used to it. " +
                        "New effect: send out a warp projectile that can be triggered by using the action button while holding the rite button. Uses cursor targetting by default. " +
                        "Requires Solar or Void Essence in inventory. Additional effect: Hold the rite button without moving to warp to the furthest exit in the direction you're facing.");

                    Mod.instance.NewQuest("lessonWhisk");

                    break;

                case 1: // villagers

                    Game1.drawDialogue(npc, "(Jester's eyes sparkle) Magic tricks! Fates are known for being the best at making others happy. Or soaked. Do you have essence from the otherworld? You will need either solar or void on hand. " +
                        "New effect: greet villagers with a special effect for a random amount of friendship. Consumes 1 Solar or Void Essence in inventory");

                    Mod.instance.NewQuest("lessonTrick");

                    break;

                case 2: // machines

                    Game1.drawDialogue(npc, "So I checked out your 'machines'. Well built but... kind of clunky. What happens if you put something from the otherworld in them? " +
                        "New effect: enchant one of various types of farm machine to produce a random object without inputs. Consumes 1 Solar or Void Essence in inventory");

                    Mod.instance.NewQuest("lessonEnchant");

                    break;

                case 3: // gravity

                    Game1.drawDialogue(npc, "(Jester looks determined) Alright farmer, time for some serious power. You want to take on big game? Nothing escapes a locus of fate, not even fate itself. " +
                        "New effect: Create gravity wells at the base of scarecrows and monsters. " +
                        "Hold the rite button to invest essense and increase the effect of the well. Uses cursor targetting by default.");

                    Mod.instance.NewQuest("lessonGravity");

                    break;

                case 4: // daze

                    Game1.drawDialogue(npc, "(Jester sighs) The fane creatures of this world cannot understand their own mundane purpose. " +
                        "New effect: Jester attacks and Gravity wells incur a debuff that dazzles enemies. Warp strikes against dazed enemies can be triggered by using the action button.");

                    Mod.instance.NewQuest("lessonDaze");

                    break;

                default: // quest

                    if (Mod.instance.QuestGiven("challengeFates"))
                    {
                        Game1.drawDialogue(npc, "You're the only one with the ability to help me. Perform a rite of the stars at the abandoned quarry.");
                    }
                    else
                    {
                        Game1.drawDialogue(npc, "Now I need your help. (Jester adopts a solemn tone). Every creature of fate is born with a divine purpose. " +
                            "Of mine, there is little foretold, except a fragment of knowledge revealed to the oldest of my kin. (Jester's eyes narrow to slits) " +
                            "My path involves an ancient entity, with a name that has been kept hidden from me. " +
                            "The entity entered the valley once, in a time long past, and the site of their arrival is the abandoned quarry. " +
                            "It seems the secrets there are sealed with the power of the Stars. " +
                            "(Jester resumes his gleeful countenance) You possess the blessing of the stars, so you can perform the rite! See you there! " +
                        "(You have received a new quest)");

                        Mod.instance.NewQuest("challengeFates");
                    }

                    break;

            }
            if (Mod.instance.BlessingList()["fates"] <= 4)
            {

                lessonGiven = true;

                Mod.instance.LevelBlessing("fates");

                Game1.currentLocation.playSoundPitched("yoba", 600);

            }

        }

        // =============================================================
        // Relocate
        // =============================================================

        public void DialogueEffects()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            Dictionary<string, int> toggleList = Mod.instance.ToggleList();

            string question = "The Jester of Fate: ^I enjoy answering questions. One of my dearest sisters was a sphinx.";

            List<Response> choices = new()
            {
                new Response("Jester", "Have you learned anything more about your purpose?"),
                new Response("fates", "Can you show me those tricks again? (review)"),
                new Response("return", "(nevermind)")
            };

            GameLocation.afterQuestionBehavior behaviour = new(AnswerEffects);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

            return;

        }

        public void AnswerEffects(Farmer visitor, string answer)
        {

            switch (answer)
            {

                case "Jester":

                    DelayedAction.functionAfterDelay(EffectsJester, 100);

                    break;

                case "fates":

                    DelayedAction.functionAfterDelay(EffectsFates, 100);

                    break;

                case "return":

                    returnFrom = "effects";

                    DelayedAction.functionAfterDelay(DialogueApproach, 100);

                    break;

            }

            return;

        }

        public void EffectsJester()
        {

            Game1.drawDialogue(npc, "I'm as lost as when I started. But, I have found out something about myself, something disturbing, embarrassing, and yet, I must accept it. I like to hide in boxes.");

        }

        public void EffectsFates()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string question = "The Jester of Fate: ^Every fate serves a purpose ordained by Yoba. Some of us are fairies. Some of us weave the cords of destiny into the great tapestry.";

            if (blessingList["earth"] >= 1)
            {

                question += "^Lesson 1. Send out a warp projectile that can be triggered by using the action button while holding the rite button. Additionally, hold the rite button without moving to warp to the furthest exit in the direction you're facing.";
  
            }

            if (blessingList["earth"] >= 2)
            {

                question += "^Lesson 2. Greet villagers with a special effect for a random amount of friendship. Consumes 1 essence.";

            }

            question += "^ ";

            List<Response> choices = new();

            GameLocation.afterQuestionBehavior behaviour;

            if (blessingList["fates"] >= 3)
            {

                choices.Add(new Response("next", "Next ->"));

                behaviour = new(EffectsFatesTwo);

            }
            else
            {

                choices.Add(new Response("return", "It's all clear now"));

                behaviour = new(ReturnEffects);

            }

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

            return;

        }

        public void EffectsFatesTwo(Farmer visitor, string answer)
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string question = "The Jester of Fate: ^Lesson 3. Enchant one of various types of farm machine to produce a random object without inputs. Consumes 1 essence.";

            if (blessingList["fates"] >= 4)
            {

                question += "^Lesson 4. Create gravity wells at the base of scarecrows and monsters. Uses directional and cursor targetting. Hold the rite button to invest essense and increase the effect of the well. Consumes 1 essence.";

            }

            if (blessingList["fates"] >= 5)
            {

                question += "^Lesson 5. Jester attacks and Gravity wells incur a debuff that dazzles enemies. Warp strikes against dazed enemies can be triggered by using the action button while holding the rite button.";

            }

            question += "^ ";

            List<Response> choices = new()
            {
                new Response("return", "It's all clear now")
            };

            GameLocation.afterQuestionBehavior behaviour = new(ReturnEffects);

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

            return;

        }

        public void ReturnEffects(Farmer visitor, string answer)
        {

            DelayedAction.functionAfterDelay(DialogueEffects, 100);

            return;

        }

        // =============================================================
        // Relocate
        // =============================================================

        public void DialogueRelocate()
        {

            List<Response> choices = new();

            string question = "The Jester of Fate: What do you propose?";

            bool follow = npc.priorities.Contains("track");

            if (npc.DefaultMap == "FarmCave" || follow)
            {

                choices.Add(new Response("Farm", "There's plenty going on on the farm. (relocate to farm)"));

            }

            if (npc.DefaultMap == "Farm" || follow)
            {

                choices.Add(new Response("FarmCave", "The cave is where it's all happening. (relocate to farmcave)"));

            }

            if (!follow)
            {

                choices.Add(new Response("Follow", "Come on an adventure with me.  (The Jester of Fate will follow you around, and target nearby enemies with a powerful attack that applies a daze debuff)"));

            }

            choices.Add(new Response("return", "(nevermind)"));

            GameLocation.afterQuestionBehavior behaviour = new(AnswerRelocate);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

        }

        public void AnswerRelocate(Farmer visitor, string answer)
        {

            string reply = "(Jester looks away)";

            switch (answer)
            {
                case "FarmCave":

                    reply = "I'm not bothered either way.";

                    (npc as Character.Jester).SwitchDefaultMode();

                    Mod.instance.CharacterRegister("Jester", "FarmCave");

                    npc.WarpToDefault();

                    Mod.instance.CastMessage("Jester has moved to the farm cave", -1);

                    return;

                case "Farm": // Farm

                    reply = "Let's see who's around to bother.";

                    (npc as Character.Jester).SwitchRoamMode();

                    Mod.instance.CharacterRegister("Jester", "Farm");

                    npc.WarpToDefault();

                    Mod.instance.CastMessage("Jester now roams the farm", -1);

                    break;

                case "Follow":

                    reply = "Lead the way, fateful one.";

                    (npc as Character.Jester).SwitchFollowMode();

                    Mod.instance.CastMessage("Jester joins you on your adventures", -1);

                    break;

            }

            Game1.drawDialogue(npc, reply);

        }

        // =============================================================
        // Relocate
        // =============================================================

        public void ReplyAfterQuarry()
        {

            Game1.drawDialogue(npc,
                "The monsters that came out of that portal... they come from a realm adjacent to this one. " +
                "(Jester smirks) Something barred us from passing through the other way. " +
                "If I'm to pursue this mystery further, I'll have to figure out how to get there from another way. " +
                "(Thank you for playing Stardew Druid: Rite of the Fates! Subscribe or join discord to get updates on the next installment - Neosinf)");

        }


    }

}
