/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using StardewDruid.Character;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Minigames;
using StardewValley.Objects;
using System.Collections.Generic;

namespace StardewDruid.Dialogue
{
    public class Rites
    {
        public NPC npc;

        public Rites(NPC NPC) => npc = NPC;

        public void Approach()
        {
            
            if (npc is StardewDruid.Character.Shadowtin)
            {

                DelayedAction.functionAfterDelay(DialogueDiscuss, 100);

            }

            DelayedAction.functionAfterDelay(RitesApproach, 100);

        }

        public void RitesApproach()
        {
           
            string intro = "The Forgotten Effigy: The traditions live on.";
            
            if(npc is StardewDruid.Character.Jester)
            {

                intro = "The Jester of Fate: You assume I know anything.";

            }

            if (npc is StardewDruid.Character.Shadowtin)
            {

                intro = "Shadowtin Bear: Treasure hunter, at your service.";

            }

            List<Response> responseList = new List<Response>();
            
            QuestData.RitesProgress();
            
            Mod.instance.CurrentBlessing();

            if (!(npc is StardewDruid.Character.Shadowtin))
            {

                responseList.Add(new Response("blessing", "(rite) I want to practice a different rite"));

                int num = Mod.instance.AttuneableWeapon();

                if (num != -1 && num != 999)
                {

                    responseList.Add(new Response("attune", "(attune) I want to dedicate this " + Game1.player.CurrentTool.Name + " (manage attunement)"));

                }

            }

            responseList.Add(new Response("discuss", " (talk) I want to discuss some things with you"));

            if (npc is StardewDruid.Character.Effigy)
            {

                if (Context.IsMultiplayer && Context.IsMainPlayer)
                {

                    responseList.Add(new Response("farmhands", "(farmhands) I want to share what I've learned with others"));

                }

                //responseList.Add(new Response("magnetism", "(magnetism) I want the stones and sticks to obey!"));

            }

            responseList.Add(new Response("cancel", "(nevermind)"));

            GameLocation.afterQuestionBehavior questionBehavior = new(RitesAnswer);
            
            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);
        
        }

        public void RitesAnswer(Farmer visitor, string answer)
        {
            switch (answer)
            {
                case "blessing":
                    DelayedAction.functionAfterDelay(DialogueBlessing, 100);
                    break;
                case "discuss":
                    DelayedAction.functionAfterDelay(DialogueDiscuss, 100);
                    break;
                case "farmhands":
                    DelayedAction.functionAfterDelay(DialogueFarmhands, 100);
                    break;
                case "attune":
                    DelayedAction.functionAfterDelay(DialogueAttune, 100);
                    break;
                /*case "magnetism":
                    DelayedAction.functionAfterDelay(ResetMagnetism, 100);
                    break;*/
            }
        }

        public void DialogueBlessing()
        {
            string intro = "Forgotten Effigy: ^The Kings, the Lady, the Stars, I may entreat them all.";

            List<string> riteList = QuestData.RitesProgress();

            string rite = Mod.instance.CurrentBlessing();

            List<Response> responseList = new List<Response>();

            if (npc is StardewDruid.Character.Effigy)
            {
                if (rite != "weald")
                {
                    responseList.Add(new Response("weald", "Let us pay homage to the Two Kings"));
                }
                    
                if (riteList.Contains("mists") && rite != "mists")
                {
                    responseList.Add(new Response("mists", "Call out to the Lady Beyond The Shore"));
                }
                    
                if (riteList.Contains("stars") && rite != "stars")
                {
                    responseList.Add(new Response("stars", "Look to the Stars for me"));
                }
                    
            }
            if (npc is StardewDruid.Character.Jester)
            {
                intro = "Jester: ^I hear many voices, and some aren't even mine";

                if (rite != "fates")
                {
                    responseList.Add(new Response("fates", "Ask your kin to favour me"));
                }
                    
                if (riteList.Contains("ether") && rite != "ether")
                {
                    responseList.Add(new Response("ether", "I want to reach the Masters of the Ether, the Dragons"));
                }
                    
            }

            responseList.Add(new Response("none", "I don't want anyone's favour (disable)"));

            responseList.Add(new Response("cancel", "(say nothing)"));

            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerBlessing);

            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);

        }

        public void AnswerBlessing(Farmer visitor, string answer)
        {
            string str;
            switch (answer)
            {
                case "weald":
                    Game1.addHUDMessage(new HUDMessage(Mod.instance.CastControl() + " to perform Rite of the Weald"));
                    str = "The Kings of Oak and Holly come again.";
                    break;
                case "mists":
                    Game1.addHUDMessage(new HUDMessage(Mod.instance.CastControl() + " to perform Rite of the Mists"));
                    str = "The Voice Beyond the Shore echoes around us.";
                    break;
                case "stars":
                    Game1.addHUDMessage(new HUDMessage(Mod.instance.CastControl() + " to perform Rite of the Stars"));
                    str = "Life to ashes. Ashes to dust.";
                    break;
                case "fates":
                    Game1.addHUDMessage(new HUDMessage(Mod.instance.CastControl() + " to perform Rite of the Fates"));
                    str = "The Fates peer through the veil. (Jester tries to be as expressionless as the Effigy)";
                    break;
                case "ether":
                    Game1.addHUDMessage(new HUDMessage(Mod.instance.CastControl() + " to perform Rite of the Ether"));
                    str = "You're the master now, Farmer. The ancient ones have retreated from this world.";
                    break;
                case "none":
                    Game1.addHUDMessage(new HUDMessage(Mod.instance.CastControl() + " will do nothing"));
                    str = !(npc is StardewDruid.Character.Jester) ? "The light fades away." : "Well I don't blame you. Much.";
                    break;
                default:
                    str = "(says nothing back).";
                    break;
            }
            if (answer != "cancel")
            {
                Mod.instance.ChangeBlessing(answer);

            }

            npc.CurrentDialogue.Push(
                new(
                    npc, "0",
                    str
                )
            );

            Game1.drawDialogue(npc);

        }

        public void DialogueDiscuss()
        {
            
            List<string> stringList = QuestData.RitesProgress();
            
            Mod.instance.CurrentBlessing();
            
            List<Response> responseList = new List<Response>();
            
            string intro;
            
            if (npc is StardewDruid.Character.Effigy)
            {
                intro = "The Forgotten Effigy: Our traditions are etched into the bedrock of the valley.";

                responseList.Add(new Response("weald", "What role do the Two Kings play?"));

                if (stringList.Contains("mists"))
                {
                    responseList.Add(new Response("mists", "Who is the Voice Beyond the Shore?"));
                }
                    
                if (stringList.Contains("stars"))
                {
                    responseList.Add(new Response("stars", "Do the Stars have names?"));
                }
                    
                if (stringList.Contains("fates"))
                {
                    responseList.Add(new Response("fates", "What do you know of Jester and the Fates?"));
                }
                    
                if (stringList.Contains("ether"))
                {
                    responseList.Add(new Response("ether", "Who were the Masters of the Ether?"));
                }
                    
                responseList.Add(new Response("Effigy", "I want to know more about the First Farmer"));


                if (Mod.instance.characters.ContainsKey("Shadowtin"))
                {

                    responseList.Add(new Response("Shadowtin", "Our circle now has it's own treasure hunter"));

                }

            }
            else if (npc is StardewDruid.Character.Jester)
            {
                
                intro = "The Jester of Fate: ^I enjoy answering questions. One of my dearest sisters was a sphinx.";
                
                responseList.Add(new Response("Jester", "Have you learned anything more about your purpose?"));
               
                responseList.Add(new Response("Effigy", "The Effigy of the First Farmer watches us"));
                
                responseList.Add(new Response("fates", "Tell me more about your kin, the Fates"));
                
                if (stringList.Contains("ether"))
                {
                    
                    responseList.Add(new Response("ether", "Where are the Dragons?"));
                
                }

                if (Mod.instance.characters.ContainsKey("Shadowtin"))
                {

                    responseList.Add(new Response("Shadowtin", "Shadowtin doesn't believe in luck. Or chance. Or fortune."));

                }

            }
            else
            {
                intro = "Shadowtin Bear: ^Certainly. As long as the discussion relates to treasure. Or Dragons.";

                responseList.Add(new Response("ether", "So what dragon treasures have you found?"));

                responseList.Add(new Response("Effigy", "The Effigy is a strange mystical artifact."));

                responseList.Add(new Response("fates", "Jester has no care for treasures."));

                responseList.Add(new Response("Shadowtin", "How did you and the other shadowfolk come into the service of Lord Deep?"));

            }
            
            responseList.Add(new Response("cancel", "(nevermind)"));
            
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerEffects);
            
            Game1.player.currentLocation.createQuestionDialogue(intro, responseList.ToArray(), questionBehavior, npc);
        
        }

        public void AnswerEffects(Farmer visitor, string answer)
        {
            
            switch (answer)
            {
                case "Effigy":
                    DelayedAction.functionAfterDelay(DiscussEffigy, 100);
                    break;

                case "Jester":
                    DelayedAction.functionAfterDelay(DiscussJester, 100);
                    break;

                case "Shadowtin":
                    DelayedAction.functionAfterDelay(DiscussShadowtin, 100);
                    break;

                case "weald":
                    DelayedAction.functionAfterDelay(EffectsWeald, 100);
                    break;

                case "mists":
                    DelayedAction.functionAfterDelay(EffectsMists, 100);
                    break;

                case "stars":
                    DelayedAction.functionAfterDelay(EffectsStars, 100);
                    break;

                case "ether":
                    DelayedAction.functionAfterDelay(EffectsEther, 100);
                    break;

                case "fates":
                    DelayedAction.functionAfterDelay(EffectsFates, 100);
                    break;

            }

        }

        public void EffectsWeald()
        {
            string str = "In times past, the King of Oaks and the King of Holly would war upon the Equinox. Their warring would conclude for the sake of new life in Spring. When need arose, they lent their strength to a conflict from which neither could fully recover. They rest now, dormant. May those awake guard the change of seasons.";
            
            npc.CurrentDialogue.Push(new(npc, "0",str));

            Game1.drawDialogue(npc);

        }

        public void EffectsMists()
        {
            string str = "The Voice is that of the Lady of the Isle of Mists. She is as beautiful and distant as the sunset on the Gem Sea. The first farmer knew her. (The Effigy's eyes flicker a brilliant turqoise). There is a feeling that comes with my memories of that time, a feeling I cannot describe.";
            
            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);
        }

        public void EffectsStars()
        {
            string str = "The Stars have no names that can be uttered by earthly dwellers. They exist high above, and beyond, and care not for the life of our world, though their light sustains much of it. The Stars that have shone on you belong to constellation of Sisters. There is one star... a fallen star. This one the Druids did give a name too. (Effigy's flaming eyes flicker). I have been forbidden to share it.";
            
            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);
        }

        public void EffectsFates()
        {
            string str;
            if (npc is StardewDruid.Character.Effigy)
            {
                str = "The Fates weave the cords of destiny into the great tapestry that is the story of the world. It is said that they each serve a special purpose known only to Yoba, and so they often appear to work by mystery and happenchance, by whim even. (Effigy motions ever so slightly in the direction of Jester) Many have been known to stray from their duty.";
            }
            else if (npc is StardewDruid.Character.Jester)
            {
                str = "Flameface is right. Every Fate has a special role we're given by Fortumei, the greatest of us, priestess of Yoba. Some of us are fairies, and care for the fates of plants and little things. For my contribution, well, I've had some pretty cool moments... (Jester is pensive as his voice trails off)";
            }
            else
            {
                str = "He claims to be a powerful agent of destiny, but he's as blind as a newborn kitten, and naive about the horrors that await him on his quest into the undervalley. I doubt he's been sent by Yoba. I doubt Yoba still cares about any of us.";
            }
            
            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);
        }

        public void EffectsEther()
        {
            string str;
            if (npc is StardewDruid.Character.Effigy)
            {
                str = "I know very little of Dragonkind and their ilk. They were the first servants of Yoba, and perhaps they disappointed their creator. Their bones have become the foundation of the other world, their potent life essence has become the streams of ether that flow through the planes.";
            }
            else if (npc is StardewDruid.Character.Jester)
            {
                str = "We're talking about creatures that could reforge the world itself, Farmer. They don't like our kind, otherfolk or humanfolk. Actually I'm... (Jester's hairs raise across it's backside) kind of scared of them. I'm glad you have my back!";
            }
            else
            {
                str = "I've found this cloak, and this carnyx, and this bear mask. The Dragons would demand the best tributes, from the greatest artisans, with the finest materials available. All Shadowfolk prize such treasures, and it's a very competitive society, so I have to carry mine with me at all times.";
            }
            
            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);
        }


        public void DiscussEffigy()
        {
            string str;
            if (npc is StardewDruid.Character.Effigy)
            {
                str = "The first farmer was blessed by the elderborn, the monarchs of the valley, to cultivate and protect this special land. He used this blessing to construct me, and showed me how I could preserve his techniques for a future successor. Though my friend is long gone, I remain, because the power of the elders remain. For now.";
            }
            else if (npc is StardewDruid.Character.Jester)
            {
                str = "He talks about the first of the valley farmers all the time. They must have have been good friends. Has he asked you to build a pyre yet? (Jester gives a mischievous smirk)";
            }
            else
            {
                str = "Of all the constructs embued with the power of the elderborn, I've never heard of one so loyal to his former master. I've done my own assessment of the quality of his make. The clothes and head-dress are cheap garbage. And threadbare. I suspect a large cat has been kneading them, as the back is scratched and covered in fur. You'll probably need to replace them at some point. Or burn them. The real value in the Effigy is a fashioned inner core that is saturated with elder power. It's the heart, and the brain. A treasure from the elder age.";

            }
            
            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);
        }

        public void DiscussJester()
        {
            string str = "I'm as lost as when I started. But, I have found out something about myself, something embarrassing, even disturbing, and yet, I must accept it. I like to hide in boxes.";
            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);
        }

        public void DiscussShadowtin()
        {
            string str;
            if (npc is StardewDruid.Character.Effigy)
            {
                str = "All manner of otherfolk traded and befriended with the first farmer, but he always had the most trouble with the shadowfolk. It's difficult to see their intentions. It's difficult to see them in any lack of light.";
            }
            else if (npc is StardewDruid.Character.Jester)
            {
                str = "I think I get what he wants, I mean, trinkets and shiny things are great. But they aren't everything. He said he'd help us get to the undervalley, but he doesn't care about my sacred mission. Still, I think he has a part to play for Yoba in our great purpose. (Jester grins) You can just beat him up again if he tries to double-cross us.";
            }
            else
            {
                str = "The folklore of shadows is enscribed on the outer surface of the great vessel. The narrative starts with Lord Deep, before the first of my forefolk is mentioned, and the stories suggest we have always been subservient to him. But I believe those first enscriptions have been tampered with. I know now, from research, that the vessel is dragon-forged. Perhaps we served an ancient one, perhaps Lord Deep rewrote our history. I hope my travels and the treasures we uncover yield answers.";

            }
            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);
        }

        public void DialogueFarmhands()
        {
            
            string str = "Teach them to embrace the source, or seize it.";

            Mod.instance.TrainFarmhands();

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

        }

        public void DialogueAttune()
        {
            List<string> stringList = QuestData.RitesProgress();
            Mod.instance.CurrentBlessing();
            List<Response> responseList = new List<Response>();
            int toolIndex = Mod.instance.AttuneableWeapon();
            string str1 = Mod.instance.AttunedWeapon(toolIndex);
            string str2;
            if (str1 != "reserved")
            {
                if (npc is StardewDruid.Character.Effigy)
                {
                    str2 = "Forgotten Effigy: ^To whom should this " + Game1.player.CurrentTool.Name + " be dedicated to?^";
                    if (str1 != "weald" && stringList.Contains("weald"))
                        responseList.Add(new Response("weald", "To the Two Kings (Rite of the Weald)"));
                    if (str1 != "mists" && stringList.Contains("mists"))
                        responseList.Add(new Response("mists", "To the Lady Beyond the Shore (Rite of the Mists)"));
                    if (str1 != "stars" && stringList.Contains("stars"))
                        responseList.Add(new Response("stars", "To the Stars Themselves (Rite of the Stars)"));
                }
                else
                {
                    str2 = "The Jester of Fate: ^Who do you want to use this " + Game1.player.CurrentTool.Name + " against - I mean in honor of?^";
                    if (str1 != "fates" && stringList.Contains("fates"))
                        responseList.Add(new Response("fates", "To the Folk of Mystery (Rite of the Fates)"));
                    if (str1 != "ether" && stringList.Contains("ether"))
                        responseList.Add(new Response("ether", "To the Masters of the Ether (Rite of the Ether)"));
                }
                responseList.Add(new Response("none", "I want to reclaim it for myself (removes attunement)"));
            }
            else
                str2 = !(npc is StardewDruid.Character.Effigy) ? "The Jester of Fate: ^I don't think " + Game1.player.CurrentTool.Name + " wants to change^" : "Forgotten Effigy: ^This " + Game1.player.CurrentTool.Name + " will serve no other master^";
            responseList.Add(new Response("return", "(nevermind)"));
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerAttune);
            Game1.player.currentLocation.createQuestionDialogue(str2, responseList.ToArray(), questionBehavior, npc);
        }

        public void AnswerAttune(Farmer effigyVisitor, string effigyAnswer)
        {
            string str1 = "This " + Game1.player.CurrentTool.Name + " will serve ";
            string str2;
            switch (effigyAnswer)
            {
                case "return":
                    DelayedAction.functionAfterDelay(RitesApproach, 100);
                    return;
                case "none":
                    string str3 = "This " + Game1.player.CurrentTool.Name + " will no longer serve.";
                    Mod.instance.DetuneWeapon();
                    npc.CurrentDialogue.Push(new(npc, "0", str3));

                    Game1.drawDialogue(npc);
                    return;
                case "stars":
                    str2 = str1 + "the very Stars Themselves";
                    break;
                case "mists":
                    str2 = str1 + "the Lady Beyond the Shore";
                    break;
                case "fates":
                    str2 = str1 + "the Folk of Mystery";
                    break;
                case "ether":
                    str2 = str1 + "the Masters of the Ether";
                    break;
                default:
                    str2 = str1 + "the Two Kings";
                    break;
            }
            Mod.instance.AttuneWeapon(effigyAnswer);
            npc.CurrentDialogue.Push(new(npc, "0", str2));

            Game1.drawDialogue(npc);
        }


        /*public void ResetMagnetism()
        {

            string str = "Now the little objects will listen. (Magnetism has been reset to the base value)";

            Game1.player.MagneticRadius = 128;

            Ring ring1 = Game1.player.leftRing.Value;

            if (ring1 != null)
            {
                switch (ring1.indexInTileSheet.Value)
                {
                    case 518:
                        Game1.player.MagneticRadius += 64;
                        break;
                    case 519:
                    case 527:
                    case 888:
                        Game1.player.MagneticRadius += 128;
                        break;
                }
            }

            Ring ring2 = Game1.player.rightRing.Value;

            if (ring2 != null)
            {
                switch (ring2.indexInTileSheet.Value)
                {
                    case 518:
                        Game1.player.MagneticRadius += 64;
                        break;
                    case 519:
                    case 527:
                    case 888:
                        Game1.player.MagneticRadius += 128;
                        break;
                }

            }

            Game1.drawDialogue(npc, str);

        }*/
    }
}
