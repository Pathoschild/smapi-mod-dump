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
using StardewDruid.Map;
using StardewValley.Objects;

namespace StardewDruid.Dialogue
{
    public class Effigy : Dialogue
    {

        public bool lessonGiven;

        public string questFeedback;

        public Effigy()
        {

        }

        public override void DialogueApproach()
        {

            if (specialDialogue.Count > 0)
            {

                DialogueSpecial();

                return;

            }

            if (Mod.instance.QuestOpen("approachEffigy"))
            {

                Mod.instance.CompleteQuest("approachEffigy");

                Mod.instance.CharacterRegister("Effigy", "FarmCave");

                DialogueIntro();

                return;

            }

            string effigyQuestion = "Forgotten Effigy: ^Successor.";

            List<Response> effigyChoices = new();

            if (Map.QuestData.ChallengeCompleted())
            {

                effigyChoices.Add(new Response("threats", "What challenges may I undertake for the Circle?"));

                if (Context.IsMainPlayer)
                {

                    effigyChoices.Add(new Response("relocate", "It's time for a change of scene"));

                }

            }
            else if (Map.QuestData.JourneyCompleted())
            {

                effigyChoices.Add(new Response("challenge", "Does the valley have need of me?"));

            }
            else
            {

                effigyChoices.Add(new Response("journey", "I've come for a lesson"));

            }

            if (Mod.instance.HasBlessing("earth"))
            {

                effigyChoices.Add(new Response("business", "I have some requests (manage rites)"));

            }

            effigyChoices.Add(new Response("none", "(say nothing)"));

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerApproach);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public override void AnswerApproach(Farmer effigyVisitor, string effigyAnswer)
        {

            switch (effigyAnswer)
            {

                case "intro":

                    DelayedAction.functionAfterDelay(DialogueQuery, 100);

                    break;

                case "journey":

                    DelayedAction.functionAfterDelay(ReplyJourney, 100);

                    break;

                case "challenge":

                    DelayedAction.functionAfterDelay(ReplyChallenge, 100);

                    break;

                case "threats":

                    DelayedAction.functionAfterDelay(ReplyThreats, 100);

                    break;

                case "relocate":

                    DelayedAction.functionAfterDelay(DialogueRelocate, 100);

                    break;


                case "ancestor":

                    DelayedAction.functionAfterDelay(ReplyAncestor, 100);

                    break;

                case "Demetrius":

                    DelayedAction.functionAfterDelay(DialogueDemetrius, 100);

                    break;

                case "business":

                    DelayedAction.functionAfterDelay(DialogueBusiness, 100);

                    break;

            }

            return;

        }

        // =============================================================
        // Challenge / Lessons
        // =============================================================

        
        public void DialogueIntro()
        {

            string question = "So the successor appears, and has demonstrated remarkable potential. I am the Effigy of the Sleeping Kings, and the sole remnant of my circle of Druids.";

            List<Response> choices = new()
            {
                new Response("intro", "Who stuck you in the ceiling?"),

                new Response("none", "(say nothing)")
            };

            GameLocation.afterQuestionBehavior behaviour = new(AnswerApproach);

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

        }

        public void DialogueQuery()
        {

            List<Response> effigyChoices = new();

            string effigyQuestion = "Forgotten Effigy: ^I was crafted by the first farmer of the valley, a powerful friend of the otherworld. If you intend to succeed him, you will need to learn many lessons.";

            effigyChoices.Add(new Response("journey", "Ok. What is the first lesson? (start journey)"));

            if (Game1.year >= 2)
            {

                effigyChoices.Add(new Response("ancestor", "I'll have you know, the first farmer was my ancestor, and my family has practiced his craft for generations (unlock all)"));

            };

            effigyChoices.Add(new Response("none", "(say nothing)"));

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerApproach);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

        }

        public void ReplyJourney()
        {


            string reply;

            if (lessonGiven)
            {

                Game1.drawDialogue(npc, "Hmm... return tomorrow after I have consulted the Others.");

                return;

            }

            if (!Mod.instance.QuestComplete("swordEarth"))
            {

                if (!Mod.instance.QuestGiven("swordEarth"))
                {
                    Mod.instance.NewQuest("swordEarth");

                    Game1.currentLocation.playSoundPitched("discoverMineral", 600);

                }

                Game1.drawDialogue(npc, "Seek the patronage of the two Kings. Find the giant malus of the southern forest and perform a rite below it's boughs. " +
                    $"Cast: {Mod.instance.CastControl()} with an appropriate tool or weapon in hand to perform a rite. Hold the button to increase the range of the effect.");

                return;

            }

            if (!Mod.instance.QuestComplete("challengeEarth"))
            {

                if (!Mod.instance.BlessingList().ContainsKey("earth"))
                {

                    Mod.instance.UpdateBlessing("earth");

                    Mod.instance.ChangeBlessing("earth");

                }

                switch (Mod.instance.BlessingList()["earth"])
                {

                    case 0: // explode weeds

                        reply = "Good. You are now a subject of the two kingdoms, and bear authority over the weed and the twig. Use this new power to drive out decay and detritus. Return tomorrow for another lesson. " +
                            $"New effect: explode weeds and twigs. Remotely greet animals, pets and villagers once a day. Hold the button to increase the range of the effect";

                        Mod.instance.NewQuest("lessonVillager");

                        break;

                    case 1: // bush, water, grass, stump, boulder

                        reply = "The valley springs into new life. Go now, sample its hidden bounty, and prepare to face those who guard its secrets. " +
                            $"New effect: extract foragables from large bushes, wood from trees, fibre and seeds from grass and small fish from water. Might spawn monsters";

                        Mod.instance.NewQuest("lessonCreature");

                        break;

                    case 2: // lawn, dirt, trees

                        reply = "Years of stagnation have starved the valley of it's wilderness. Go now, and recolour the barren spaces. " +
                            $"New effect: sprout trees, grass, seasonal forage and flowers in empty spaces";

                        Mod.instance.NewQuest("lessonForage");

                        break;

                    case 3: // hoed

                        reply = "Your connection to the earth deepens. You may channel the power of the Two Kings for your own purposes. " +
                            $"New effect: increase the growth rate and quality of growing crops. Convert planted wild seeds into random cultivations. Fertilise trees and uptick the growth rate of fruittrees";

                        Mod.instance.NewQuest("lessonCrop");

                        break;

                    case 4: // rockfall

                        reply = "Be careful in the mines. The deep earth answers your call, both above and below you. " +
                            $"New effect: shake loose rocks free from the ceilings of mine shafts. Explode gem ores";

                        Mod.instance.NewQuest("lessonRockfall");

                        break;

                    default: // quest

                        if (Mod.instance.QuestGiven("challengeEarth"))
                        {
                            reply = "Stop dallying. Return when the mountain is cleansed.";
                        }
                        else
                        {
                            reply = "A trial presents itself. Foulness seeps from the mountain springs. Cleanse the source with the King's blessing. " +
                            $"(You have received a new quest)";

                            Mod.instance.NewQuest("challengeEarth");

                        }

                        break;

                }

                if (Mod.instance.BlessingList()["earth"] <= 4)
                {

                    lessonGiven = true;

                    Mod.instance.LevelBlessing("earth");

                    Game1.currentLocation.playSoundPitched("discoverMineral", 600);

                }

                Game1.drawDialogue(npc, reply);

                return;

            }


            if (!Mod.instance.QuestComplete("swordWater"))
            {
                
                reply = "The Voice Beyond the Shore harkens to you now. Perform a rite at the furthest pier, and behold her power.";

                if (!Mod.instance.QuestGiven("swordWater"))
                {

                    Mod.instance.NewQuest("swordWater");

                    Game1.currentLocation.playSoundPitched("thunder_small", 1200);

                }

                Game1.drawDialogue(npc, reply);

                return;

            }

            if (!Mod.instance.QuestComplete("challengeWater"))
            {

                if (!Mod.instance.BlessingList().ContainsKey("water"))
                {

                    Mod.instance.UpdateBlessing("water");

                    Mod.instance.ChangeBlessing("water");

                }

                switch (Mod.instance.BlessingList()["water"])
                {

                    case 0: // warp totems

                        reply = "Good. The Lady Beyond the Shore has answered your call. Find the shrines to the patrons of the Valley, and strike them to draw out a portion of their essence. Do the same to any obstacle in your way. " +
                        $"New effect: strike warp shrines once a day to extract totems, and boulders and stumps to extract resources";

                        Mod.instance.NewQuest("lessonTotem");

                        break;

                    case 1: // scarecrow, rod, craftable, campfire

                        reply = "The Lady is fascinated by the industriousness of humanity. Combine your artifice with her blessing and reap the rewards. " +
                            $"New effect: strike scarecrows, campfires and lightning rods to activate special functions. Villager firepits will work too";

                        Mod.instance.NewQuest("lessonCookout");

                        break;

                    case 2: // fishspot

                        reply = "The denizens of the deep water serve the Lady. Go now, and test your skill against them. " +
                            $"New effect: strike deep water to produce a fishing-spot that yields rare species of fish";

                        Mod.instance.NewQuest("lessonFishspot");

                        break;

                    case 3: // stump, boulder, enemy

                        reply = "Your connection to the plane beyond broadens. Call upon the Lady's Voice to destroy your foes. " +
                            $"New effect: expend high amounts of stamina to instantly destroy enemies";

                        Mod.instance.NewQuest("lessonSmite");

                        break;

                    case 4: // portal

                        reply = "Are you yet a master of the veil between worlds? Focus your will to breach the divide. " +
                            $"New effect:  strike candle torches to create monster portals. Every candle included in the rite increases the challenge. Only works in remote outdoor locations like the backwoods";

                        Mod.instance.NewQuest("lessonPortal");

                        break;

                    default: // quest

                        if (Mod.instance.QuestGiven("challengeWater"))
                        {
                            reply = "Deal with the shadows first.";

                        }
                        else
                        {

                            reply = "A new trial presents itself. Creatures of shadow linger in the hollowed grounds of the village. Smite them with the Lady's blessing. " +
                            $"(You have received a new quest)";

                            Mod.instance.NewQuest("challengeWater");

                        }

                        break;

                }

                if (Mod.instance.BlessingList()["water"] <= 4)
                {

                    lessonGiven = true;

                    Mod.instance.LevelBlessing("water");

                    Game1.currentLocation.playSoundPitched("thunder_small", 1200);

                }

                Game1.drawDialogue(npc, reply);

                return;

            }
            
            if (!Mod.instance.QuestComplete("swordStars"))
            {
                reply = "Your name is known within the celestial plane. Travel to the lake of flames. Retrieve the final vestige of the first farmer.";

                if (!Mod.instance.QuestGiven("swordStars"))
                {
                    Mod.instance.NewQuest("swordStars");

                }

                Game1.drawDialogue(npc, reply);

                return;

            }
            
            if (!Mod.instance.QuestComplete("challengeStars"))
            {


                if (!Mod.instance.BlessingList().ContainsKey("stars"))
                {

                    Mod.instance.UpdateBlessing("stars");

                    Mod.instance.ChangeBlessing("stars");

                    //Mod.instance.DecorateCave();

                }

                switch (Mod.instance.BlessingList()["stars"])
                {

                    case 0: // warp totems

                        reply = "Excellent. The Stars Beyond the Expanse have chosen a new champion. Shatter the thickened wild so that new life may find root there. " +
                        $"New effect: call down a shower of fireballs that increase in number and range as the cast is held";

                        Mod.instance.NewQuest("lessonMeteor");

                        Mod.instance.LevelBlessing("stars");

                        lessonGiven = true;

                        Game1.currentLocation.playSoundPitched("Meteorite", 1200);

                        break;

                    default:

                        if (Mod.instance.QuestGiven("challengeStars"))
                        {
                            reply = "Only you can deal with the threat to the forest";

                        }
                        else
                        {
                            reply = "Your last trial awaits. The southern forest reeks of our mortal enemy. Rain judgement upon the slime with the blessing of the Stars. " +
                                    "(You have received a new quest)";

                            Mod.instance.NewQuest("challengeStars");

                            Game1.currentLocation.playSoundPitched("Meteorite", 1200);

                        }

                        break;
                }

                Game1.drawDialogue(npc, reply);

                return;

            }

            reply = "Your power rivals that of the first farmer. I have nothing further to teach you. When the seasons change, the valley may call upon your aid once again. " +
            "(Thank you for playing with StardewDruid. Credits: Neosinf/StardewDruid, PathosChild/SMAPI, ConcernedApe/StardewValley)";

            Game1.drawDialogue(npc, reply);

            Game1.currentLocation.playSound("yoba");

        }

        public void ReplyChallenge()
        {

            string reply = "Those with a twisted connection to the otherworld may remain tethered to the Valley long after their mortal vessel wastes away. " +
                "Strike them with bolt and flame to draw out and disperse their corrupted energies. " +
                "(You have received new quests)";

            /*if (!Mod.instance.QuestGiven("effigyOne"))
            {

                Mod.instance.NewQuest("effigyOne");

            }*/
            
            List<string> challengeQuests = Map.QuestData.ChallengeQuests();

            List<string> addQuests = new();

            foreach (string questName in challengeQuests)
            {

                if (!Mod.instance.QuestGiven(questName))
                {

                    addQuests.Add(questName);

                }

            }

            foreach (string questName in addQuests)
            {

                Mod.instance.NewQuest(questName);

            }

            Game1.currentLocation.playSound("yoba");

            lessonGiven = true;

            Game1.drawDialogue(npc, reply);

            return;

        }

        public void ReplyThreats()
        {

            string reply;

            GameLocation.afterQuestionBehavior behaviour;

            List<Response> choices = new();

            if (Mod.instance.QuestOpen("approachJester"))
            {
                reply = "Fortune gazes upon you... but it can't be her. One of her kin perhaps.";

                choices.Add(new Response("Jester", "What do you mean?"));

                behaviour = new(AnswerJester);

                Game1.player.currentLocation.createQuestionDialogue(reply, choices.ToArray(), behaviour, npc);

                return;

            }

            if (lessonGiven)
            {

                reply = "I will scry through the endless chitter of those who watch from the otherworld. Return to me tomorrow for more tidings.";

                Game1.drawDialogue(npc, reply);

                return;

            }

            List<string> activeThreats = Map.QuestData.ActiveSeconds();

            if (activeThreats.Count > 0)
            {

                reply = "Have you dealt with the threat before you?";

                choices.Add(new Response("abort", "I have been unable to proceed against this threat for now. Is there something else?"));

                choices.Add(new Response("cancel", "(oh that's right)"));

            }
            else
            {
                reply = "An old threat has re-emerged. Be careful, they may have increased in power since your last confrontation.";

                choices.Add(new Response("accept", "(accept) I am ready to face the renewed threat"));

                choices.Add(new Response("refuse", "(refuse) The valley must endure without my help"));

                choices.Add(new Response("cancel", "(not now) I'll need some time to prepare"));

            }

            behaviour = new(AnswerThreats);

            Game1.player.currentLocation.createQuestionDialogue(reply, choices.ToArray(), behaviour, npc);

        }

        public void AnswerThreats(Farmer effigyVisitor, string effigyAnswer)
        {
            string reply;

            switch (effigyAnswer)
            {

                case "accept":

                    Dictionary<string, string> challengeList = Map.QuestData.SecondQuests();

                    List<string> enterList = new();

                    List<string> rotateList = new();

                    reply = "May you be successful against the shadows of the otherworld. (You have received a new quest)";

                    foreach (KeyValuePair<string, string> challenge in challengeList)
                    {

                        string questName = challenge.Key + "Two";

                        if (!Mod.instance.QuestGiven(questName))
                        {

                            enterList.Add(questName);

                        }

                        rotateList.Add(questName);

                    }

                    if (enterList.Count == 0)
                    {

                        enterList = rotateList;

                    }

                    string enterChallenge = enterList[Game1.random.Next(enterList.Count)];

                    Mod.instance.NewQuest(enterChallenge);

                    Game1.currentLocation.playSound("yoba");

                    lessonGiven = true;

                    Game1.drawDialogue(npc, reply);

                    break;

                case "refuse":

                    lessonGiven = true;

                    reply = "The valley will withstand the threat as it can, as it always has.";

                    Game1.drawDialogue(npc, reply);

                    break;

                case "abort":

                    List<string> activeThreats = Map.QuestData.ActiveSeconds();

                    Mod.instance.RemoveQuest(activeThreats[0]);

                    reply = "The valley will withstand the threat as it can, as it always has.";

                    Game1.drawDialogue(npc, reply);

                    break;

                case "cancel":

                    returnFrom = "threats";

                    DelayedAction.functionAfterDelay(DialogueApproach, 100);

                    break;

            }

            return;

        }

        public void ReplyAncestor()
        {
            string reply = "Indeed, I hear the wisdom of the old valley in each word spoken. " +
            "(All rites unlocked at maximum level)";

            Game1.currentLocation.playSound("yoba");

            Mod.instance.UnlockAll();

            //Mod.instance.DecorateCave();

            Game1.drawDialogue(npc, reply);
            //Game1.activeClickableMenu = new DialogueBox(reply);

        }

        public void DialogueDemetrius()
        {

            string effigyQuestion = "Forgotten Effigy: ^I concealed myself for a time, then I spoke to him in the old tongue of the Calico shamans.";

            List<Response> effigyChoices = new()
            {
                new Response("descended", "Do you think Demetrius is descended from the shaman tradition?!"),

                new Response("offended", "Wow, he must have been offended. Demetrius is a man of modern science and sensibilities."),

                new Response("return", "Nope, not going to engage with this.")
            };

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerDemetrius);

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void AnswerDemetrius(Farmer effigyVisitor, string effigyAnswer)
        {

            string reply;

            switch (effigyAnswer)
            {
                case "return":

                    returnFrom = "demetrius";

                    DelayedAction.functionAfterDelay(DialogueApproach, 100);

                    return;

                default: //

                    if (Game1.player.caveChoice.Value == 1)
                    {

                        reply = "... ... He came in with a feathered mask on, invoked a rite of summoning, threw Bat feed everywhere, then ran off singing \"Old man in a frog pond\".";

                    }
                    else
                    {

                        reply = "I can smell the crisp, sandy scent of the Calico variety of mushroom. The shamans would eat them to... enter a trance-like state.";

                    }

                    break;

            }
            Game1.drawDialogue(npc, reply);
            //Game1.activeClickableMenu = new DialogueBox(reply);

        }

        // =============================================================
        // Business
        // =============================================================

        public void DialogueBusiness()
        {

            string effigyQuestion = "Forgotten Effigy: ^The traditions live on.";

            List<Response> effigyChoices = new();

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string blessing = Mod.instance.ActiveBlessing();

            effigyChoices.Add(new Response("effects", "I'd like to review my training (manage rite effects)"));

            if (blessingList.ContainsKey("water"))
            //if (Mod.instance.HasBlessing("water"))
            {

                effigyChoices.Add(new Response("blessing", "I want to change my patron (change rite)"));

            }

            if (blessingList.ContainsKey("earth") && Context.IsMultiplayer && Context.IsMainPlayer)
            //if (Mod.instance.HasBlessing("earth") && Context.IsMultiplayer && Context.IsMainPlayer)
            {

                effigyChoices.Add(new Response("farmhands", "I want to share what I've learned with others (train farmhands)"));

            }

            int toolIndex = Mod.instance.AttuneableWeapon();

            if (toolIndex != -1)
            {

                effigyChoices.Add(new Response("attune", $"I want to dedicate this {Game1.player.CurrentTool.Name} (manage attunement)"));


            }

            effigyChoices.Add(new Response("none", "(nevermind)"));

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerBusiness);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;
        }

        public void AnswerBusiness(Farmer effigyVisitor, string effigyAnswer)
        {

            switch (effigyAnswer)
            {
                case "blessing":

                    DelayedAction.functionAfterDelay(DialogueBlessing, 100);

                    break;

                case "effects":

                    DelayedAction.functionAfterDelay(DialogueEffects, 100);

                    break;

                case "farmhands":

                    DelayedAction.functionAfterDelay(DialogueFarmhands, 100);

                    break;

                case "attune":

                    DelayedAction.functionAfterDelay(DialogueAttune, 100);

                    break;

            }

            return;

        }

        public void DialogueBlessing()
        {

            string effigyQuestion = "Forgotten Effigy: ^The Kings, the Lady, the Stars, I may entreat them all.";

            List<Response> effigyChoices = new();

            if (Mod.instance.ActiveBlessing() != "earth")
            {

                effigyChoices.Add(new Response("earth", "Seek the Two Kings for me"));

            }

            if (Mod.instance.BlessingList().ContainsKey("water") && Mod.instance.ActiveBlessing() != "water")
            {

                effigyChoices.Add(new Response("water", "Call out to the Lady Beyond The Shore"));

            }

            if (Mod.instance.BlessingList().ContainsKey("stars") && Mod.instance.ActiveBlessing() != "stars")
            {

                effigyChoices.Add(new Response("stars", "Look to the Stars for me"));

            }

            if (Mod.instance.BlessingList().ContainsKey("fates") && Mod.instance.ActiveBlessing() != "fates")
            {

                effigyChoices.Add(new Response("fates", "Leave it to the Fates"));

            }

            if (Mod.instance.ActiveBlessing() != "none")
            {

                effigyChoices.Add(new Response("none", "I don't want anyone's favour (disables all effects)"));

            }

            effigyChoices.Add(new Response("cancel", "(say nothing)"));

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerBlessing);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void AnswerBlessing(Farmer effigyVisitor, string effigyAnswer)
        {

            string reply;

            switch (effigyAnswer)
            {
                case "earth":

                    Game1.addHUDMessage(new HUDMessage($"{Mod.instance.CastControl()} to perform rite of the earth", ""));

                    reply = "The Kings of Oak and Holly come again.";

                    Game1.currentLocation.playSound("discoverMineral");

                    break;

                case "water":

                    Game1.addHUDMessage(new HUDMessage($"{Mod.instance.CastControl()} to perform rite of the water", ""));

                    reply = "The Voice Beyond the Shore echoes around us.";

                    Game1.currentLocation.playSound("thunder_small");

                    break;

                case "stars":

                    Game1.addHUDMessage(new HUDMessage($"{Mod.instance.CastControl()} to perform rite of the stars", ""));

                    reply = "Life to ashes. Ashes to dust.";

                    Game1.currentLocation.playSound("Meteorite");

                    break;

                case "fates":

                    Game1.addHUDMessage(new HUDMessage($"{Mod.instance.CastControl()} to perform rite of the fates", ""));

                    reply = "The Fates peer through the veil.";

                    Game1.currentLocation.playSound("Meteorite");

                    break;

                case "none":

                    Game1.addHUDMessage(new HUDMessage($"{Mod.instance.CastControl()} will do nothing", ""));

                    reply = "The light fades away.";

                    Game1.currentLocation.playSound("ghost");

                    break;

                default: // "cancel"

                    reply = "(says nothing back).";

                    break;

            }

            if (effigyAnswer != "cancel")
            {

                Mod.instance.ChangeBlessing(effigyAnswer);

            }

            //Mod.instance.DecorateCave();

            Game1.drawDialogue(npc, reply);
            //Game1.activeClickableMenu = new DialogueBox(reply);

        }

        public void DialogueEffects()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            Dictionary<string, int> toggleList = Mod.instance.ToggleList();

            string effigyQuestion = "Forgotten Effigy: ^Our traditions are etched into the bedrock of the valley.";

            if (returnFrom == "forget")
            {

                effigyQuestion = "Forgotten Effigy: ^The druid's life is... full of random surprises... but may you not suffer any more of this kind.";


            }

            if (returnFrom == "remember")
            {

                effigyQuestion = "Forgotten Effigy: ^Let the essence of life itself enrich your world.";

            }

            List<Response> effigyChoices = new()
            {
                new Response("earth", "What role do the Two Kings play?")
            };

            if (blessingList.ContainsKey("water"))
            {

                effigyChoices.Add(new Response("water", "Who is the Voice Beyond the Shore?"));

            }

            if (blessingList.ContainsKey("stars"))
            {

                effigyChoices.Add(new Response("stars", "Do the Stars have names?"));

            }

            if (blessingList.ContainsKey("fates"))
            {

                effigyChoices.Add(new Response("fates", "What do you know of the Fates?"));

            }

            if (blessingList["earth"] >= 2)
            {
                if (toggleList.Count < 4)
                {
                    effigyChoices.Add(new Response("disable", "I'd rather forget something that happened (disable effects)"));

                }
                if (toggleList.Count > 0)
                {

                    effigyChoices.Add(new Response("enable", "I want to relearn something (enable effects)"));

                }

            }

            effigyChoices.Add(new Response("return", "(nevermind)"));

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerEffects);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void AnswerEffects(Farmer effigyVisitor, string effigyAnswer)
        {

            switch (effigyAnswer)
            {

                case "earth":

                    DelayedAction.functionAfterDelay(EffectsEarth, 100);

                    break;

                case "water":

                    DelayedAction.functionAfterDelay(EffectsWater, 100);

                    break;

                case "stars":

                    DelayedAction.functionAfterDelay(EffectsStars, 100);

                    break;

                case "fates":

                    DelayedAction.functionAfterDelay(EffectsFates, 100);

                    break;
                case "disable":

                    DelayedAction.functionAfterDelay(DialogueDisable, 100);

                    break;

                case "enable":

                    DelayedAction.functionAfterDelay(DialogueEnable, 100);

                    break;

                case "return":

                    returnFrom = "effects";

                    DelayedAction.functionAfterDelay(DialogueApproach, 100);

                    break;

            }

            return;

        }

        public void EffectsEarth()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string effigyQuestion = "Forgotten Effigy: ^The King of Oaks and the King of Holly war upon the Equinox. One will rule with winter, one with summer.";

            if (blessingList["earth"] >= 1)
            {

                effigyQuestion += "^Lesson 1. Explode weeds and twigs. Greet Villagers, Pets and Animals once a day.";

            }

            if (blessingList["earth"] >= 2)
            {

                effigyQuestion += "^Lesson 2. Extract foragables from the landscape. Might attract monsters.";

            }

            effigyQuestion += "^ ";

            List<Response> effigyChoices = new();

            GameLocation.afterQuestionBehavior effigyBehaviour;

            if (blessingList["earth"] >= 3)
            {

                effigyChoices.Add(new Response("next", "Next ->"));

                effigyBehaviour = new(EffectsEarthTwo);

            }
            else
            {

                effigyChoices.Add(new Response("return", "It's all clear now"));

                effigyBehaviour = new(ReturnEffects);

            }

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void EffectsEarthTwo(Farmer effigyVisitor, string effigyAnswer)
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string effigyQuestion = "Forgotten Effigy: ^Lesson 3. Sprout trees, grass, seasonal forage and flowers in empty spaces.";

            if (blessingList["earth"] >= 4)
            {

                effigyQuestion += "^Lesson 4. Increase the growth rate and quality of growing crops. Convert planted wild seeds into random cultivations.";

            }

            if (blessingList["earth"] >= 5)
            {

                effigyQuestion += "^Lesson 5. Shake loose rocks free from the ceilings of mine shafts. Explode gem ores.";

            }

            effigyQuestion += "^ ";

            List<Response> effigyChoices = new()
            {
                new Response("return", "It's all clear now")
            };

            GameLocation.afterQuestionBehavior effigyBehaviour = new(ReturnEffects);

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void EffectsWater()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string effigyQuestion = "Forgotten Effigy: ^The Voice is that of the Lady of the Isle of Mists. She is as ancient and powerful as the sunset on the Gem Sea.";

            if (blessingList["water"] >= 1)
            {

                effigyQuestion += "^Lesson 1. Strike warp shrines, stumps, logs and boulders to extract resources.";

            }

            if (blessingList["water"] >= 2)
            {

                effigyQuestion += "^Lesson 2. Strike scarecrows, campfires and lightning rods to activate special functions.";

            }

            effigyQuestion += "^ ";

            List<Response> effigyChoices = new();

            GameLocation.afterQuestionBehavior effigyBehaviour;

            if (blessingList["water"] >= 3)
            {

                effigyChoices.Add(new Response("next", "Next ->"));

                effigyBehaviour = new(EffectsWaterTwo);

            }
            else
            {

                effigyChoices.Add(new Response("return", "It's all clear now"));

                effigyBehaviour = new(ReturnEffects);

            }

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void EffectsWaterTwo(Farmer effigyVisitor, string effigyAnswer)
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string effigyQuestion = "Forgotten Effigy: ^Lesson 3. Strike deep water to produce a fishing-spot that yields rare species of fish.";


            if (blessingList["water"] >= 4)
            {

                effigyQuestion += "^Lesson 4. Expend high amounts of stamina to smite enemies with bolts of power.";

            }

            if (blessingList["water"] >= 5)
            {

                effigyQuestion += "^Lesson 5. Strike candle torches placed in remote outdoor locations to produce monster portals.";

            }

            effigyQuestion += "^ ";

            List<Response> effigyChoices = new()
            {
                new Response("return", "It's all clear now")
            };

            GameLocation.afterQuestionBehavior effigyBehaviour = new(ReturnEffects);

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void EffectsStars()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string effigyQuestion = "Forgotten Effigy: ^The Stars have no names that can be uttered by earthly dwellers. They exist high above, and beyond, and care not for the life of our world, though their light sustains much of it. ^Yet... there is one star... a fallen star. That has a name. A name that we dread to speak.";

            List<Response> effigyChoices = new()
            {
                new Response("return", "It's all clear now")
            };

            GameLocation.afterQuestionBehavior effigyBehaviour = new(ReturnEffects);

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void EffectsFates()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            string effigyQuestion = "Forgotten Effigy: ^The Fates are the most whimsical of Other folk. " +
                "It is said that they each serve a special purpose known only to Yoba, and so they often appear to work by mystery and happenchance. " +
                "Though immortal, this purpose gives them... a definitive life.";

            List<Response> effigyChoices = new()
            {
                new Response("return", "It's all clear now")
            };

            GameLocation.afterQuestionBehavior effigyBehaviour = new(ReturnEffects);

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }


        public void ReturnEffects(Farmer effigyVisitor, string effigyAnswer)
        {

            DelayedAction.functionAfterDelay(DialogueEffects, 100);

            return;

        }

        public void DialogueDisable()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            Dictionary<string, int> toggleList = Mod.instance.ToggleList();

            List<string> disabledEffects = Mod.instance.DisabledEffects();

            string effigyQuestion = "Forgotten Effigy: ^Is there a lesson you'd rather forget.";

            List<Response> effigyChoices = new();

            if (!toggleList.ContainsKey("forgetSeeds") && !disabledEffects.Contains("Seeds") && blessingList["earth"] >= 3)
            {

                effigyChoices.Add(new Response("forgetSeeds", "I end up with seeds in my boots everytime I run through the meadow. It's ANNOYING."));

            }

            if (!toggleList.ContainsKey("forgetFish") && !disabledEffects.Contains("Fish"))
            {

                effigyChoices.Add(new Response("forgetFish", "I got slapped in the face by a flying fish today."));

            }

            if (!toggleList.ContainsKey("forgetWildspawn") && !disabledEffects.Contains("Wildspawn"))
            {

                effigyChoices.Add(new Response("forgetWildspawn", "Why does a bat sleep in every damn tree on this farm. Can't they live in this cave instead?"));

            }

            if (!toggleList.ContainsKey("forgetTrees") && !disabledEffects.Contains("Trees") && blessingList["earth"] >= 3)
            {

                effigyChoices.Add(new Response("forgetTrees", "I was almost at Clint's by 3:50pm when a tree sprouted. I collided with it before I could retrieve my upgraded Axe!"));

            }

            if (effigyChoices.Count == 0)
            {

                effigyQuestion = "Forgotten Effigy: ^Your mind is already completely empty.";

                effigyChoices.Add(new Response("return", "(sure...)"));

            }
            else
            {

                effigyChoices.Add(new Response("return", "(nevermind)"));

            }

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerDisable);

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void AnswerDisable(Farmer effigyVisitor, string effigyAnswer)
        {

            switch (effigyAnswer)
            {
                case "return":

                    returnFrom = "nevermind";

                    break;

                default: //

                    Mod.instance.ToggleEffect(effigyAnswer);

                    returnFrom = "forget";

                    break;

            }

            DelayedAction.functionAfterDelay(DialogueEffects, 100);

        }

        public void DialogueEnable()
        {

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            Dictionary<string, int> toggleList = Mod.instance.ToggleList();

            List<string> disabledEffects = Mod.instance.DisabledEffects();

            string effigyQuestion = "Forgotten Effigy: ^The mind is open.";

            List<Response> effigyChoices = new();

            if (toggleList.ContainsKey("forgetSeeds") && !disabledEffects.Contains("Seeds"))
            {

                effigyChoices.Add(new Response("forgetSeeds", "There's a time to reap and a time to sow. I want to reap seeds from wild grass, sell them, and buy a Sow."));

            }

            if (toggleList.ContainsKey("forgetFish") && !disabledEffects.Contains("Fish"))
            {

                effigyChoices.Add(new Response("forgetFish", "I miss the way the fish dance to the rhythm of the rite"));

            }

            if (toggleList.ContainsKey("forgetWildspawn") && !disabledEffects.Contains("Wildspawn"))
            {

                effigyChoices.Add(new Response("forgetWildspawn", "I miss the feeling of being watched from every bush."));

            }

            if (toggleList.ContainsKey("forgetTrees") && !disabledEffects.Contains("Trees"))
            {

                effigyChoices.Add(new Response("forgetTrees", "Stuff Clint. I want to impress Emily with the magic sprout trick."));

            }

            if (effigyChoices.Count == 0)
            {

                effigyQuestion = "Forgotten Effigy: ^You already remember everything I taught you.";

                effigyChoices.Add(new Response("return", "(sure...)"));

            }
            else
            {

                effigyChoices.Add(new Response("return", "(nevermind)"));

            }

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerEnable);

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

            return;

        }

        public void AnswerEnable(Farmer effigyVisitor, string effigyAnswer)
        {

            switch (effigyAnswer)
            {
                case "return":

                    returnFrom = "nevermind";

                    break;

                default: //

                    Mod.instance.ToggleEffect(effigyAnswer);

                    returnFrom = "remember";

                    break;

            }

            DelayedAction.functionAfterDelay(DialogueEffects, 100);

        }

        public void DialogueFarmhands()
        {

            string reply = "Teach them to embrace the source, or seize it.";

            Mod.instance.TrainFarmhands();

            Game1.drawDialogue(npc, reply);
            //Game1.activeClickableMenu = new DialogueBox(reply);

            return;

        }

        public void DialogueAttune()
        {

            string effigyQuestion;

            List<Response> effigyChoices = new();

            int toolIndex = Mod.instance.AttuneableWeapon();

            string attunement = Mod.instance.AttunedWeapon(toolIndex);

            Dictionary<string, int> blessingList = Mod.instance.BlessingList();

            effigyQuestion = $"Forgotten Effigy: ^To whom should this {Game1.player.CurrentTool.Name} be dedicated to?^";

            if (attunement != "reserved")
            {

                if (attunement != "earth" && blessingList.ContainsKey("earth"))
                {

                    effigyChoices.Add(new Response("earth", $"To the Two Kings (Rite of the Earth)"));


                }

                if (attunement != "water" && blessingList.ContainsKey("water"))
                {

                    effigyChoices.Add(new Response("water", $"To the Lady Beyond the Shore (Rite of the Water)"));


                }

                if (attunement != "stars" && blessingList.ContainsKey("stars"))
                {

                    effigyChoices.Add(new Response("stars", $"To the Stars Themselves (Rite of the Stars)"));

                }

                if (attunement != "fates" && blessingList.ContainsKey("fates"))
                {

                    effigyChoices.Add(new Response("fates", $"To the Folk of Mystery (Rite of the Fates)"));

                }

                if (attunement != "none")
                {

                    effigyChoices.Add(new Response("none", "I want to reclaim it for myself (removes attunement)"));


                }

            }
            else
            {

                effigyQuestion = $"Forgotten Effigy: ^This {Game1.player.CurrentTool.Name} will not attune to anything else.";

            }

            effigyChoices.Add(new Response("return", "(nevermind)"));

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerAttune);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

        }

        public void AnswerAttune(Farmer effigyVisitor, string effigyAnswer)
        {

            string reply = $"This {Game1.player.CurrentTool.Name} will serve ";

            switch (effigyAnswer)
            {
                case "return":

                    returnFrom = "attune";

                    DelayedAction.functionAfterDelay(DialogueApproach, 100);

                    return;

                case "none":

                    reply = $"This {Game1.player.CurrentTool.Name} will no longer serve.";

                    Mod.instance.DetuneWeapon();

                    Game1.drawDialogue(npc, reply);

                    return;

                case "stars": reply += "the very Stars Themselves"; Game1.currentLocation.playSound("Meteorite"); break;

                case "water": reply += "the Lady Beyond the Shore"; Game1.currentLocation.playSound("thunder_small"); break;

                case "fates": reply += "the Folk of Mystery"; Game1.currentLocation.playSound("healSound"); break;

                default: reply += "the Two Kings"; Game1.currentLocation.playSound("discoverMineral"); break; //earth

            }

            Mod.instance.AttuneWeapon(effigyAnswer);

            Game1.drawDialogue(npc, reply);

            return;

        }

        // =============================================================
        // Relocate
        // =============================================================

        public void DialogueRelocate()
        {

            string effigyQuestion;

            List<Response> effigyChoices = new();

            effigyQuestion = $"Forgotten Effigy: ^Now that you have vanquished the twisted spectres of the past, it is safe for me to roam the wilds of the Valley once more. Where shall I await your command?^";

            if (npc.DefaultMap == "FarmCave")
            {

                effigyChoices.Add(new Response("Farm", "My farm would benefit from your gentle stewardship. (The Effigy will target scarecrows with Rite of the Earth effects, automatically sewing seeds, fertilising and watering tilled earth around any scarecrow)"));

            }

            if (npc.DefaultMap == "Farm")
            {

                effigyChoices.Add(new Response("FarmCave", "Shelter within the farm cave for the while."));

            }

            effigyChoices.Add(new Response("return", "(nevermind)"));

            GameLocation.afterQuestionBehavior effigyBehaviour = new(AnswerRelocate);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(effigyQuestion, effigyChoices.ToArray(), effigyBehaviour, npc);

        }

        public void AnswerRelocate(Farmer effigyVisitor, string effigyAnswer)
        {

            string reply = "(nevermind isn't a place, successor)";

            switch (effigyAnswer)
            {
                case "FarmCave":

                    reply = "I will return to where I may feel the rumbling energies of the Valley's leylines.";

                    //npc.roamMode = false;

                    (npc as Character.Effigy).SwitchDefaultMode();

                    Mod.instance.CharacterRegister(npc.Name, "FarmCave");

                    npc.WarpToDefault();

                    Mod.instance.CastMessage("The Effigy has moved to the farm cave", -1);

                    break;

                case "Farm": // Farm

                    reply = "I will take my place amongst the posts and furrows of my old master's home.";

                    //npc.roamMode = true;

                    (npc as Character.Effigy).SwitchRoamMode();

                    Mod.instance.CharacterRegister(npc.Name, "Farm");

                    npc.WarpToDefault();

                    Mod.instance.CastMessage("The Effigy now roams the farm", -1);

                    break;

            }

            Game1.drawDialogue(npc, reply);

        }


        public void AnswerJester(Farmer visitor, string answer)
        {

            switch (answer)
            {

                case "Jester":

                    DelayedAction.functionAfterDelay(ReplyJester, 100);

                    break;
            
            }

            return;

        }

        public void ReplyJester()
        {

            Game1.drawDialogue(npc, "I felt the industry of the forest spirits the night they toiled on the span across the mountain ravine. " +
                "They restored not only a bridge over land but between two destinies. " +
                "Should you decide to cross, a fateful encounter awaits you. " +
                "(Should be worth checking out the bridge to the Quarry)");

            //Mod.instance.NewQuest("approachJester");

        }


    }
}