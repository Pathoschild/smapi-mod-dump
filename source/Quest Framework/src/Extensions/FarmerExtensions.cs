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
using StardewValley;
using StardewValley.Quests;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Extensions
{
    /// <summary>
    /// Extensions API for <see cref="StardewValley.Farmer"/> class
    /// </summary>
    public static class FarmerExtensions
    {
        /// <summary>
        /// Get all managed quests which are assigned in player's quest log.
        /// </summary>
        /// <param name="farmer">StardewValley farmer</param>
        public static IEnumerable<Quest> GetManagedQuestLog(this Farmer farmer)
        {
            return from quest in farmer.questLog
                   where quest.IsManaged()
                   select quest;
        }

        /// <summary>
        /// Convert SDV quests enumaration to managed custom quests.
        /// Converted enumeration contains only managed quests by Quest Framework.
        /// </summary>
        /// <param name="questLog">SDV questlog enumaration</param>
        public static IEnumerable<CustomQuest> AsManaged(this IEnumerable<Quest> questLog)
        {
            return questLog.Where(q => q.IsManaged())
                .Select(q => q.AsManagedQuest());
        }

        public static void AddQuestQuiet(this Farmer farmer, int questId)
        {
            if (farmer.hasQuest(questId))
                return;
            
            Quest questFromId = Quest.getQuestFromId(questId);
            
            if (questFromId == null)
                return;

            questFromId.showNew.Value = false;
            farmer.questLog.Add(questFromId);
        }
    }
}
