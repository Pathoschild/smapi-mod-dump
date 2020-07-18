using QuestFramework.Quests;
using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Hooks
{
    public class Hook
    {
        public string When { get; set; }
        public Actions Action { get; set; }
        public Dictionary<string, string> Has { get; set; }

        internal CustomQuest managedQuest;

        public void Execute(ICompletionArgs args)
        {
            if (this.managedQuest == null)
                return;

            var manager = QuestFrameworkMod.Instance.QuestManager;
            int questId = manager.ResolveGameQuestId(this.managedQuest.GetFullName());

            switch (this.Action)
            {
                case Actions.Accept:
                    manager.AcceptQuest(this.managedQuest.GetFullName());
                    break;
                case Actions.CheckIfComplete:
                    var quest = Game1.player.questLog.Where(q => q.id.Value == questId).FirstOrDefault();

                    if (quest != null)
                        quest.checkIfComplete(args.Npc, args.Number1, args.Number2, args.Item, args.String);
                    break;
                case Actions.Complete:
                    if (questId > -1 && Game1.player.hasQuest(questId))
                        Game1.player.completeQuest(questId);
                    break;
                case Actions.Remove:
                    if (Game1.player.hasQuest(questId))
                        return;
                    
                    Game1.player.removeQuest(questId);
                    this.managedQuest.AsStatefull().ResetState();
                    break;
            }
        }
    }
}
