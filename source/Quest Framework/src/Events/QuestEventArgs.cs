using QuestFramework.Extensions;
using QuestFramework.Quests;
using StardewValley.Quests;
using System;
namespace QuestFramework.Events
{
    public class QuestEventArgs : EventArgs
    {
        internal QuestEventArgs(Quest whichQuest)
        {
            this.WhichQuest = whichQuest;
        }

        /// <summary>
        /// Which vanilla SDV quest
        /// </summary>
        public Quest WhichQuest { get; }

        /// <summary>
        /// Is this quest managed by Quest Framework?
        /// </summary>
        public bool IsManaged => this.WhichQuest.IsManaged();

        /// <summary>
        /// Get managed quest for this vanilla SDV quest
        /// </summary>
        /// <returns>null if the quest is not managed</returns>
        public CustomQuest GetManagedQuest()
        {
            return this.WhichQuest.AsManagedQuest();
        }
    }
}
