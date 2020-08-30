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
