/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/


using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid.Dialogue
{
    public class Dialogue
    {
        public StardewDruid.Character.Character npc;
        public string returnFrom;
        public Dictionary<string, List<string>> specialDialogue = new Dictionary<string, List<string>>();

        public virtual void DialogueApproach()
        {
            if (this.specialDialogue.Count > 0)
            {
                this.DialogueSpecial();
            }
            else
            {
                string str = "Welcome";
                List<Response> responseList = new List<Response>();
                responseList.Add(new Response("none", "(say nothing)"));
                StardewDruid.Dialogue.Dialogue dialogue = this;
                GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);
                this.returnFrom = null;
                Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
            }
        }

        public virtual void DialogueSpecial()
        {
            KeyValuePair<string, List<string>> keyValuePair = this.specialDialogue.First<KeyValuePair<string, List<string>>>();
            string str = keyValuePair.Value[0];
            List<Response> responseList = new List<Response>();
            for (int index = 1; index < keyValuePair.Value.Count; ++index)
                responseList.Add(new Response("special", keyValuePair.Value[index]));
            responseList.Add(new Response("none", "(say nothing)"));
            StardewDruid.Dialogue.Dialogue dialogue = this;
            // ISSUE: virtual method pointer
            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerSpecial);
            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);
        }

        public virtual void AnswerSpecial(Farmer visitor, string answer)
        {
            KeyValuePair<string, List<string>> keyValuePair = this.specialDialogue.First<KeyValuePair<string, List<string>>>();
            if (!(answer == "special"))
                return;
            this.AnswerApproach(visitor, keyValuePair.Key);
            this.specialDialogue.Remove(keyValuePair.Key);
        }

        public virtual void AnswerApproach(Farmer visitor, string answer)
        {
        }
    }
}
