/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Quests
{
    /// <summary>
    /// Quest completion arguments passed from StardewValley `checkForCompleteQuest` method type.
    /// </summary>
    public class CompletionArgs : ICompletionArgs
    {
        /// <summary>
        /// Create completion args wrapper
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="number1"></param>
        /// <param name="number2"></param>
        /// <param name="item"></param>
        /// <param name="str"></param>
        public CompletionArgs(NPC npc = null, int number1 = -1, int number2 = -1, Item item = null, string str = null)
        {
            this.Npc = npc;
            this.Number1 = number1;
            this.Number2 = number2;
            this.Item = item;
            this.String = str;
        }

        /// <summary>
        /// Interaction with NPC
        /// </summary>
        public NPC Npc { get; }

        /// <summary>
        /// First number to test
        /// </summary>
        public int Number1 { get; }

        /// <summary>
        /// Second number to test
        /// </summary>
        public int Number2 { get; }

        /// <summary>
        /// Which item was used for check quest completion
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Text to test
        /// </summary>
        public string String { get; }
    }
}
