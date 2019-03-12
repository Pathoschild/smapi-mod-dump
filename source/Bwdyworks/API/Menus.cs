using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.API
{
    public class Menus
    {
        public void AskQuestion(string question, StardewValley.Response[] responses, StardewValley.GameLocation.afterQuestionBehavior callback)
        {
            Game1.currentLocation.lastQuestionKey = "bwdy_question";
            Game1.currentLocation.createQuestionDialogue(question, responses, callback);
        }

        public void ShowDialogue(string dialogue, StardewValley.NPC NPC = null)
        {
            //if NPC is provided we'll use their portrait
            if (NPC != null) Game1.drawDialogue(NPC, dialogue);
        }
    }
}
