/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Quests
{
    /// <summary>
    /// Quest observer which defines custom quest actions, acting and whatever.
    /// </summary>
    public interface IQuestObserver
    {
        bool CheckIfComplete(IQuestInfo questData, ICompletionArgs completion);
        void UpdateObjective(IQuestInfo questData, ref string objective);
        void UpdateDescription(IQuestInfo questData, ref string description);
        void UpdateTitle(IQuestInfo questData, ref string title);
    }
}
