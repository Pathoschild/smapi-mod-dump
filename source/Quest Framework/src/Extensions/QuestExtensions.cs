/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Quests;
using StardewValley.Quests;

namespace QuestFramework.Extensions
{
    /// <summary>
    /// Extension API for <see cref="StardewValley.Quests.Quest"/> class.
    /// </summary>
    public static class QuestExtensions
    {
        /// <summary>
        /// Returns associated managed quest if the quest is managed by Quest Framework.
        /// </summary>
        /// <param name="quest">Vanilla SDV quest</param>
        /// <returns>null if this quest is not managed by Quest Framework.</returns>
        public static CustomQuest AsManagedQuest(this Quest quest)
        {
            return QuestFrameworkMod.Instance?.QuestManager?.GetById(quest.id.Value);
        }

        /// <summary>
        /// Checks if this quest is managed by Quest Framework.
        /// </summary>
        /// <param name="quest">Vanilla SDV quest</param>
        /// <returns>true if this quest is managed.</returns>
        public static bool IsManaged(this Quest quest)
        {
            return QuestFrameworkMod.Instance?.QuestManager?.IsManaged(quest.id.Value) ?? false;
        }

        /// <summary>
        /// Returns observer of managed quest if the quest is managed and has observer.
        /// </summary>
        /// <param name="quest">Vanilla SDV quest</param>
        /// <returns>null if quest has no observer or the quest is not managed.</returns>
        public static IQuestObserver AsObserver(this Quest quest)
        {
            return AsManagedQuest(quest) as IQuestObserver;
        }
    }
}
