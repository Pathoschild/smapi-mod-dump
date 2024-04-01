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
using StardewValley.Locations;
using StardewValley.Minigames;
using StardewValley.Objects;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System;

namespace StardewDruid.Dialogue
{
    public class Adventure
    {

        public StardewDruid.Character.Character npc;

        public Adventure(StardewDruid.Character.Character NPC)
        {
            npc = NPC;
        }

        public void Approach()
        {

            DelayedAction.functionAfterDelay(DialogueApproach, 100);
        }

        public void DialogueApproach() {

            List<Response> responseList = new List<Response>();
  
            string str = "Forgotten Effigy: Now that you have vanquished the twisted spectres of the past, it is safe for me to roam the wilds of the Valley once more.";

            if (npc is StardewDruid.Character.Shadowtin)
            {

                str = "Shadowtin Bear: What do you propose?";

            }
            else if (npc is StardewDruid.Character.Jester)
            {

                str = "The Jester of Fate: I'm ready to explore the world.";

            }

            responseList.Add(new Response("inventory", "(inventory) I want to check your inventory "));

            responseList.Add(new Response("relocate", "(move) I have a task for you"));

            if (npc.netFollowActive.Value)
            {

                string part = "(finish) Time to head home.";

                if (npc is StardewDruid.Character.Shadowtin)
                {

                    part = "(finish) That's enough treasure hunting for now.";

                }
                else if (npc is StardewDruid.Character.Jester)
                {

                    part = "(finish) That's enough adventure for today.";

                }

                responseList.Add(new Response("partways", part));

                if (npc.netStandbyActive.Value)
                {

                    string cont = "(continue) Thank you for keeping watch.";

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        cont = "(continue) My business has concluded.";

                    }
                    else if (npc is StardewDruid.Character.Jester)
                    {

                        cont = "(continue) I hope you thought hard about what you did. Lets head out.";

                    }

                    responseList.Add(new Response("continue", cont));
                }
                else
                {

                    string standby = "(standby) Can you stand guard for a moment?";

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                       standby = "(standby) I have some business to attend to.";

                    }
                    else if (npc is StardewDruid.Character.Jester)
                    {

                        standby = "(standby) How about you pause for some self-reflection.";

                    }

                    responseList.Add(new Response("standby", standby));

                }

            }

            responseList.Add(new Response("return", "(nevermind)"));

            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);

            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);

        }

        public void AnswerApproach(Farmer visitor, string answer)
        {
            switch (answer)
            {
                case "inventory":
                    DelayedAction.functionAfterDelay(ReplyInventory, 100);
                    break;

                case "relocate":
                    
                    DelayedAction.functionAfterDelay(DialogueRelocate, 100);
                    break;

                case "partways":
                    AnswerRelocate(visitor, npc.DefaultMap);
                    break;

                case "standby":
                    DelayedAction.functionAfterDelay(ReplyStandby, 100);
                    break;

                case "continue":
                    DelayedAction.functionAfterDelay(ReplyContinue, 100);
                    break;

            }

        }

        public void ReplyInventory()
        {
            
            int chestIndex = 0;

            string rejection = "That would require an adequate storage device. (Place at least one chest in the farmcave to activate companion inventory).";


            if (npc is StardewDruid.Character.Shadowtin)
            {
                rejection = "First, I require you to craft for me the strongest safebox you can. (Place at least one chest in the farmcave to activate companion inventory).";

                chestIndex = 2;

            }
            else if (npc is StardewDruid.Character.Jester)
            {
               
                rejection = "You mean we can keep some of the stuff we find? (Place at least one chest in the farmcave to activate companion inventory).";
                
                chestIndex = 1;

            }

            List<Chest> chests = new();

            GameLocation farmcave = Game1.getLocationFromName("FarmCave");

            int chestCount = 0;

            foreach (Dictionary<Vector2, StardewValley.Object> dictionary in farmcave.Objects)
            {

                foreach (KeyValuePair<Vector2, StardewValley.Object> keyValuePair in dictionary)
                {

                    if (keyValuePair.Value is Chest)
                    {

                        chests.Add( (Chest)keyValuePair.Value );

                        if (chestCount == chestIndex)
                        {

                            break;

                        }

                        chestCount++;

                    }

                }

            }

            if(chests.Count == 0)
            {

                npc.CurrentDialogue.Push(
                    new(
                        npc, "0", rejection
                    )
                );

                Game1.drawDialogue(npc);

                return;

            }

            Chest chest = chests.Last();

            chest.ShowMenu();

        }

        public void DialogueRelocate()
        {
            
            List<Response> responseList = new List<Response>();

            string str = "Forgotten Effigy: Where shall I await your command?";

            if (npc is StardewDruid.Character.Shadowtin)
            {

                str = "Shadowtin Bear: Are we going on a raid?";

            }
            else if (npc is StardewDruid.Character.Jester)
            {

                str = "The Jester of Fate: Where will we go next!";

            }

            if (Context.IsMainPlayer)
            {

                if (npc.DefaultMap == "FarmCave" || npc.netFollowActive.Value)
                {

                    string farm = "My farm would benefit from your gentle stewardship. (The Effigy will garden around scarecrows on the farm)";

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        farm = "How about you explore the farm. (relocate to farm)";

                    }
                    else if (npc is StardewDruid.Character.Jester)
                    {

                        farm = "There's plenty going on on the farm. (relocate to farm)";

                    }

                    responseList.Add(new Response("Farm", farm));

                }

                if (npc.DefaultMap == "Farm" || npc.netFollowActive.Value)
                {

                    string shelter = "Shelter within the farm cave for the while.";

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        shelter = "I'm sure the farmcave is full of secrets to uncover. (relocate to farmcave)";

                    }
                    else if (npc is StardewDruid.Character.Jester)
                    {

                        shelter = "The cave is where it's all happening. (relocate to farmcave)";

                    }

                    responseList.Add(new Response("FarmCave", shelter));

                }

            }

            if (!npc.netFollowActive.Value && QuestData.StageProgress().Contains("fates"))
            {

                string follow = "Come see the valley of the new farmer. (Effigy will follow you around)";

                if (npc is StardewDruid.Character.Shadowtin)
                {

                    follow = "Let's go on a treasure hunt. (Shadowtin Bear will join you on your adventures)";

                }
                else if (npc is StardewDruid.Character.Jester)
                {

                    follow = "Come on an adventure with me.  (The Jester of Fate will follow you. Jester's melee attack applies the Daze effect.)";

                }

                responseList.Add(new Response("Follow", follow));

            }

            responseList.Add(new Response("return", "(nevermind)"));

            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerRelocate);

            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);

        }

        public void AnswerRelocate(Farmer visitor, string answer)
        {

            string str = "(nevermind isn't a place, successor)";
            string title = "The Effigy";

            if (npc is StardewDruid.Character.Shadowtin)
            {
                str = "I'll try find something productive to do.";
                title = "Shadowtin";

            }
            else if (npc is StardewDruid.Character.Jester)
            {
                str = "(Jester looks away)";
                title = "Jester";

            }

            switch (answer)
            {
                case "FarmCave":

                    str = "I will return to where I may feel the rumbling energies of the Valley's leylines.";

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        str = "I'll be happy to get somewhere dark and shady again.";

                    }
                    else if (npc is StardewDruid.Character.Jester)
                    {

                        str = "(Jester grins) I will be the one that lurks in the dark";

                    }

                    CharacterData.RelocateTo(npc.Name, "FarmCave");

                    Mod.instance.CastMessage(title + " has moved to the farm cave");
                    break;

                case "Farm":

                    str = "I will take my place amongst the posts and furrows of my old master's home.";

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        str = "Lets see how profitable this agricultural venture is.";

                    }
                    else if (npc is StardewDruid.Character.Jester)
                    {

                        str = "Let's see who's around to bother.";

                    }

                    CharacterData.RelocateTo(npc.Name, "Farm");

                    Mod.instance.CastMessage(title + " now roams the farm");

                    break;

                case "Follow":

                    str = "I will see how you put the lessons of the First Farmer to use.";

                    if (npc is StardewDruid.Character.Shadowtin)
                    {

                        str = "Indeed. How about we split the spoils fifty fifty.";

                    }
                    else if (npc is StardewDruid.Character.Jester)
                    {

                        str = "Lead the way, fateful one.";

                    }

                    CharacterData.RelocateTo(npc.Name, "Follow");

                    Mod.instance.CastMessage(title + " joins you on your adventures");

                    break;
            
            }

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

        }

        public void ReplyStandby()
        {
            string str = "Vigilance is a speciality of mine. (The Effigy does its best to remain as stoic and still as a scarecrow)";

            if (npc is StardewDruid.Character.Shadowtin)
            {

                str = "Time to practice sounding my Carnyx.";

            }
            else if (npc is StardewDruid.Character.Jester)
            {

               str = "(Jester sits. He seems deep in thought about everything he's seen so far today. Only once you turn away, does he start to lick himself)";

            }

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

            if (!Context.IsMainPlayer)
            {

                CharacterData.CharacterQuery(npc.Name, "CharacterStandby");

                return;

            }

            npc.ActivateStandby();

        }

        public void ReplyContinue()
        {
            string str = "Thank you for not stuffing me in a ceiling cavity.";

            if (npc is StardewDruid.Character.Shadowtin)
            {

                str = "Did you find anything? Perhaps the entrance to the lair of a treasure hoarding monster?";

            }
            else if (npc is StardewDruid.Character.Jester)
            {

                str = "I knew you would come back for me. It's destiny.";

            }

            npc.CurrentDialogue.Push(new(npc, "0", str));

            Game1.drawDialogue(npc);

            if (!Context.IsMainPlayer)
            {

                CharacterData.CharacterQuery(npc.Name,"CharacterContinue");

                return;

            }

            npc.DeactivateStandby();

        }


    }

}
