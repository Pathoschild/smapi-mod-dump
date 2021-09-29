/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

namespace QuestFramework.Quests
{
    /// <summary>
    /// Provides methods for update runtime basic quest info like objective, title and description
    /// on the vanilla quest proxy instance for the managed quest.
    /// </summary>
    public interface IQuestInfoUpdater
    {
        /// <summary>
        /// Update current objective queried by <see cref="StardewValley.Quests.Quest.currentObjective"/>
        /// for the managed quest on their vanilla quest proxy
        /// </summary>
        /// <param name="questData"></param>
        /// <param name="objective"></param>
        void UpdateObjective(IQuestInfo questData, ref string objective);

        /// <summary>
        /// Update current quest description queried by <see cref="StardewValley.Quests.Quest.questDescription"/>
        /// for the managed quest on their vanilla quest proxy
        /// </summary>
        /// <param name="questData"></param>
        /// <param name="description"></param>
        void UpdateDescription(IQuestInfo questData, ref string description);

        /// <summary>
        /// Update current quest title queried by <see cref="StardewValley.Quests.Quest.questTitle"/>
        /// for the managed quest on their vanilla quest proxy
        /// </summary>
        /// <param name="questData"></param>
        /// <param name="title"></param>
        void UpdateTitle(IQuestInfo questData, ref string title);
    }
}
