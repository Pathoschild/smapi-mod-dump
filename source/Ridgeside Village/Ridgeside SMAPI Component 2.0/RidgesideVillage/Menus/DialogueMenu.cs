/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;


namespace RidgesideVillage
{
    internal class DialogueBoxWithActions : DialogueBox
    {
        private List<Action> ResponseActions;

        internal DialogueBoxWithActions(string dialogue, List<Response> responses, List<Action> Actions) : base(dialogue, responses)
        {
            this.ResponseActions = Actions;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            int responseIndex = this.selectedResponse;
            base.receiveLeftClick(x, y, playSound);
            //Log.Debug($"selected response {responseIndex}");
            if(base.safetyTimer <= 0 && responseIndex > -1 && responseIndex < this.ResponseActions.Count && this.ResponseActions[responseIndex] != null)
            {
                this.ResponseActions[responseIndex]();
            }
        }
    }
}
