/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace StardustCore.Events.Preconditions.PlayerSpecific
{
    public class AnsweredDialogueOptions:EventPrecondition
    {
        public List<int> answeredOptions;

        public AnsweredDialogueOptions()
        {
            this.answeredOptions = new List<int>();
        }

        public AnsweredDialogueOptions(int Options)
        {
            this.answeredOptions = new List<int>();
            this.answeredOptions.Add(Options);
        }

        public AnsweredDialogueOptions(List<int> Options)
        {
            this.answeredOptions = Options.ToList();
        }

        public override string ToString()
        {
            return this.precondition_answeredDialogueOptions();
        }

        /// <summary>
        /// The player has answered with the dialogue options of these choices.
        /// </summary>
        /// <param name="IDS"></param>
        /// <returns></returns>
        public string precondition_answeredDialogueOptions()
        {
            StringBuilder b = new StringBuilder();
            b.Append("q ");
            for (int i = 0; i < this.answeredOptions.Count; i++)
            {
                b.Append(this.answeredOptions[i]);
                if (i != this.answeredOptions.Count - 1)
                {
                    b.Append(" ");
                }
            }
            return b.ToString();
        }

        public override bool meetsCondition()
        {
            foreach(int i in this.answeredOptions)
            {
                if (Game1.player.DialogueQuestionsAnswered.Contains(i) == false) return false;
            }
            return true;
        }
    }
}
