using NpcAdventure.Loader;
using QuestFramework.Quests;

namespace NpcAdventure.Story.Quests
{
    class RecruitmentQuest : CustomQuest, IQuestObserver
    {
        public const int TYPE_ID = 4582100;

        public RecruitmentQuest(IGameMaster gameMaster, IContentLoader contentLoader) : base()
        {
            this.GameMaster = gameMaster;
            this.ContentLoader = contentLoader;
        }

        public IGameMaster GameMaster { get; }
        public IContentLoader ContentLoader { get; }

        public bool CheckIfComplete(IQuestInfo questData, ICompletionArgs completion)
        {
            int goal = int.Parse(this.Trigger.ToString());
            var ps = this.GameMaster.Data.GetPlayerState();

            return ps.recruited.Count >= goal;
        }

        public void UpdateDescription(IQuestInfo questData, ref string description)
        {
            
        }

        public void UpdateObjective(IQuestInfo questData, ref string objective)
        {
            var ps = this.GameMaster.Data.GetPlayerState();

            objective = this.ContentLoader.LoadString($"Strings/Strings:questObjective.recruitment", ps.recruited.Count, this.Trigger);
        }

        public void UpdateTitle(IQuestInfo questData, ref string title)
        {
            
        }
    }
}
