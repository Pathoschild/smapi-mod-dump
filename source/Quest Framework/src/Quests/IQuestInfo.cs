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
using StardewValley.Quests;

namespace QuestFramework.Quests
{
    /// <summary>
    /// Quest information contains the vanilla quest representant
    /// of the managed (custom) quest.
    /// </summary>
    public interface IQuestInfo
    {
        Quest VanillaQuest { get; }
        Farmer Farmer { get; }
    }
}