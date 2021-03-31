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

namespace QuestFramework.Quests
{
    public interface ICompletionArgs
    {
        /// <summary>
        /// Interaction with NPC
        /// </summary>
        NPC Npc { get; }

        /// <summary>
        /// First number to test
        /// </summary>
        int Number1 { get; }

        /// <summary>
        /// Second number to test
        /// </summary>
        int Number2 { get; }

        /// <summary>
        /// Which item was used for check quest completion
        /// </summary>
        Item Item { get; }

        /// <summary>
        /// Text to test
        /// </summary>
        string String { get; }
    }
}
