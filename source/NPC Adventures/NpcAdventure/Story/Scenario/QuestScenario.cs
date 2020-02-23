using NpcAdventure.Events;
using NpcAdventure.Loader;
using NpcAdventure.Story.Messaging;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Quests;
using System;
using System.Linq;

namespace NpcAdventure.Story.Scenario
{
    internal class QuestScenario : BaseScenario
    {
        private readonly ISpecialModEvents events;
        private readonly IContentLoader contentLoader;
        private readonly IMonitor monitor;
        const string TYPE_RECRUITMENT = "Recruitment";

        public QuestScenario(ISpecialModEvents events, IContentLoader contentLoader, IMonitor monitor)
        {
            this.events = events;
            this.contentLoader = contentLoader;
            this.monitor = monitor;
        }

        public override void Dispose()
        {
            this.events.QuestCompleted -= this.Events_QuestCompleted;
            this.events.ReloadObjective -= this.Events_ReloadObjective;
            this.GameMaster.MessageReceived -= this.GameMaster_MessageReceived;
        }

        public override void Initialize()
        {
            this.events.QuestCompleted += this.Events_QuestCompleted;
            this.events.ReloadObjective += this.Events_ReloadObjective;
            this.GameMaster.MessageReceived += this.GameMaster_MessageReceived;
        }

        private void Events_ReloadObjective(object sender, IQuestReloadObjectiveArgs e)
        {
            if (e.Quest.questType.Value != StoryHelper.ADVENTURE_QUEST_TYPE)
                return; // Update only mod's kind of quests

            int id = StoryHelper.ResolveId(e.Quest.id.Value);
            string type = this.StoryHelper.GetQuestType(id);
            var fresh = this.StoryHelper.GetQuestById(id);

            if (type.Equals(TYPE_RECRUITMENT))
            {
                // Update current objective for recruitment quest type (example: 2/10 companions recruited)
                var ps = this.GameMaster.Data.GetPlayerState();

                e.Quest.currentObjective = this.contentLoader.LoadString($"Strings/Strings:questObjective.recruitment", ps.recruited.Count, e.Quest.completionString.Value);
            }

            // Refresh strings
            e.Quest.questTitle = fresh.questTitle;
            e.Quest.questDescription = fresh.questDescription;
        }

        private void Events_QuestCompleted(object sender, IQuestCompletedArgs e)
        {
            if (e.Quest.questType.Value != StoryHelper.ADVENTURE_QUEST_TYPE)
                return; // Check only for mod's kind of quests

            int currentObjectiveId = e.Quest.id.Value - StoryHelper.QUEST_ID_PREFIX;
            Quest nextQuest = this.StoryHelper.GetNextQuestFor(currentObjectiveId);

            if (nextQuest != null)
            {
                nextQuest.showNew.Value = true;
                nextQuest.accept();
                Game1.player.questLog.Add(nextQuest);
                Game1.addHUDMessage(new HUDMessage(this.contentLoader.LoadString("Strings/Strings:objectiveUpdate"), 2));
                this.monitor.Log($"Next quest added: #{nextQuest.id.Value} '{nextQuest.questTitle}' by #{e.Quest.id.Value} '{e.Quest.questTitle}'");
            }

            this.GameMaster.Data.GetPlayerState().completedQuests.Add(currentObjectiveId);
            this.GameMaster.SyncData();
            this.monitor.Log($"Quest complete: #{e.Quest.id.Value} '{e.Quest.questTitle}'", LogLevel.Info);
        }
        
        /// <summary>
        /// Check if any quest is completed from recieved GameMaster event massage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameMaster_MessageReceived(object sender, IGameMasterEventArgs e)
        {
            if (e.Player.UniqueMultiplayerID != Game1.player.UniqueMultiplayerID)
                return; // Only client must to complete quest and inform a server about it (via NetFields in player's questlog internally in vanilla game)

            var ps = this.GameMaster.Data.GetPlayerState();

            if (e.Message is RecruitMessage recruitMessage && !ps.recruited.Contains(recruitMessage.CompanionName))
            {
                ps.recruited.Add(recruitMessage.CompanionName);
                this.GameMaster.SyncData();
            }

            foreach (Quest quest in e.Player.questLog)
            {
                if (quest.questType.Value == StoryHelper.ADVENTURE_QUEST_TYPE)
                {
                    this.CheckForComplete(quest, e.Message);
                }
            }
        }

        private void CheckForComplete(Quest quest, IGameMasterMessage completionMessage = null)
        {
            bool completed;

            switch (this.StoryHelper.GetQuestType(StoryHelper.ResolveId(quest.id.Value)))
            {
                case TYPE_RECRUITMENT:
                    // Is recruitment quest done? Check via compare count of current recruited companions stat with goal from quest
                    var ps = this.GameMaster.Data.GetPlayerState();
                    completed = int.TryParse(quest.completionString.Value, out int goal) && ps.recruited.Count >= goal;
                    break;
                default:
                    // Is basic quest done? Check via completion string
                    completed = quest.completionString.Value.Equals(completionMessage?.Name);
                    break;
            }

            if (completed)
                quest.questComplete();
        }
    }
}
