/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/


using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid.Dialogue
{
    public class Dialogue
    {
        
        public StardewDruid.Character.Character npc;
        
        public string returnFrom;
        
        public Dictionary<string,string> specialDialogue;

        public Dialogue()
        {

            specialDialogue = new();

        }

        public virtual void DialogueApproach()
        {
            
            if (specialDialogue.Count > 0)
            {

                if (DialogueSpecial())
                {

                    return;

                }

            }

            string str = "Welcome";

            List<Response> responseList = new List<Response>()
            {
                new("none", "(say nothing)"),
                
            };

            GameLocation.afterQuestionBehavior questionBehavior = new(AnswerApproach);

            returnFrom = null;

            Game1.player.currentLocation.createQuestionDialogue(str, responseList.ToArray(), questionBehavior, npc);

        }

        public virtual bool DialogueSpecial()
        {

            specialDialogue.Clear();

            return false;

        }

        public virtual void AnswerSpecial(Farmer visitor, string answer)
        {

        }

        public virtual void AnswerApproach(Farmer visitor, string answer)
        {

        }

        public virtual void AddSpecial(string name, string dialogue = null)
        {

            specialDialogue.Clear();

            if(dialogue != null)
            {
 
                specialDialogue.Add(dialogue, npc.currentLocation.Name);

            }

            if (Context.IsMultiplayer && Context.IsMainPlayer)
            {

                Mod.instance.EventQuery(
                    new() { name = name, value = dialogue, },
                    "DialogueSpecial"
                );

            }

        }

    }
}
