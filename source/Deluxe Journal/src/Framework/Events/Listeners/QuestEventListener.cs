/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;
using StardewValley.Quests;

namespace DeluxeJournal.Framework.Listeners
{
    /// <summary>Quest event listener base class.</summary>
    internal abstract class QuestEventListener : Quest
    {
        public QuestEventListener(int questType)
        {
            this.questType.Value = questType;

            // Automatically remove if cleanup failed
            daysLeft.Value = 0;
        }

        /// <summary>Listen to a call to Quest.checkIfComplete for this quest type. The parameter usage varies.</summary>
        protected abstract void OnChecked(NPC? npc, int index, int count, Item? item, string? str);

        public override bool isSecretQuest()
        {
            return true;
        }

        public override bool checkIfComplete(NPC? npc = null, int index = -1, int count = -1, Item? item = null, string? str = null)
        {
            OnChecked(npc, index, count, item, str);
            return false;
        }
    }
}
