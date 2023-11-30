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
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Dialogue
{
    public class Dialogue
    {

        public StardewDruid.Character.Character npc;

        public string returnFrom;

        public Dictionary<string, List<string>> specialDialogue = new();

        public Dialogue()
        {

        }

        public virtual void DialogueApproach()
        {

            if (specialDialogue.Count > 0)
            {

                DialogueSpecial();

                return;

            }

            string question = "Welcome";

            List<Response> choices = new();

            choices.Add(new Response("none", "(say nothing)"));

            GameLocation.afterQuestionBehavior behaviour = new(AnswerApproach);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

            return;

        }

        public virtual void DialogueSpecial()
        {

            KeyValuePair<string, List<string>> special = specialDialogue.First();

            string question = special.Value[0];

            List<Response> choices = new()
            {
                new Response("special", special.Value[1]),

                new Response("none", "(say nothing)")
            };

            GameLocation.afterQuestionBehavior behaviour = new(AnswerSpecial);

            Game1.player.currentLocation.createQuestionDialogue(question, choices.ToArray(), behaviour, npc);

            return;

        }

        public virtual void AnswerSpecial(Farmer visitor, string answer)
        {

            KeyValuePair<string, List<string>> special = specialDialogue.First();

            if (answer == "special")
            {

                AnswerApproach(visitor, special.Key);

                specialDialogue.Remove(special.Key);

            }

            return;
             
        }

        public virtual void AnswerApproach(Farmer visitor, string answer)
        {
            return;

        }

    }

}
