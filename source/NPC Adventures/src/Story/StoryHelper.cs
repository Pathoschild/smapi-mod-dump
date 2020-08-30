using NpcAdventure.Loader;
using NpcAdventure.Story.Quests;
using QuestFramework;
using QuestFramework.Api;
using QuestFramework.Extensions;
using QuestFramework.Quests;
using StardewValley;
using StardewValley.Quests;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.Story
{
    public class StoryHelper
    {
        public const int QUEST_ID_PREFIX = 1000;
        public const int ADVENTURE_QUEST_TYPE = 16;
        private readonly IContentLoader contentLoader;
        private readonly IManagedQuestApi questApi;

        public StoryHelper(IContentLoader contentLoader, IManagedQuestApi questApi)
        {
            this.contentLoader = contentLoader;
            this.questApi = questApi;
        }

        private Dictionary<int, string> GetQuestsSource()
        {
            return this.contentLoader.LoadData<int, string>("Data/Quests");
        }

        public string[] GetRawQuestData(int id)
        {
            Dictionary<int, string> questDataSource = this.GetQuestsSource();

            if (!questDataSource.ContainsKey(id))
                return null;

            return questDataSource[id].Split('/');
        }

        public void LoadQuests(IGameMaster master)
        {
            foreach (var questRaw in this.GetQuestsSource())
            {
                var definition = questRaw.Value.Split('/');
                CustomQuest quest;

                if (definition[0] == "Recruitment")
                {
                    quest = new RecruitmentQuest(master, this.contentLoader)
                    {
                        CustomTypeId = RecruitmentQuest.TYPE_ID,
                    };
                }
                else
                {
                    quest = new CustomQuest();
                }

                quest.Name = $"adventure{questRaw.Key}";
                this.SetQuestDetails(quest, definition);
                this.questApi.RegisterQuest(quest);
            }
        }

        private void SetQuestDetails(CustomQuest quest, string[] definition)
        {
            quest.BaseType = definition[0] == "Location" ? QuestType.Location : QuestType.Custom;
            quest.Title = definition[1];
            quest.Description = definition[2];
            quest.Objective = definition[3];
            quest.Trigger = definition[4];
            quest.Reward = int.Parse(definition[5]);
            quest.Cancelable = definition[6].Equals("true");

            if (definition.Length > 7)
            {
                quest.NextQuests.AddRange(
                    definition[7].Split(' ').Select(n => "adventure" + n));
            }
        }

        public bool GetRawQuestData(int id, out string[] questData)
        {
            questData = this.GetRawQuestData(id);

            if (questData == null)
                return false;

            return true;
        }

        internal void AcceptQuest(int questId)
        {
            this.questApi.AcceptQuest($"adventure{questId}");
        }

        internal void CompleteQuest(int questId)
        {
            this.questApi.CompleteQuest($"adventure{questId}");
        }

        internal bool IsAdventureQuest(Quest quest, int id = -1)
        {
            bool isAdventure = quest.IsManaged()
                && quest.AsManagedQuest().OwnedByModUid == NpcAdventureMod.Manifest.UniqueID
                && quest.AsManagedQuest().Name.StartsWith("adventure");

            if (id == -1)
                return isAdventure;

            return isAdventure && quest.AsManagedQuest().Name == $"adventure{id}";
        }

        internal void SanitizeOldAdventureQuestsInLog()
        {
            foreach (int questId in this.GetQuestsSource().Keys)
            {
                int realQuestId = 1000 + questId;

                if (Game1.player.hasQuest(realQuestId))
                {
                    Game1.player.removeQuest(realQuestId);
                    this.questApi.AcceptQuest($"adventure{questId}", silent: true);
                }
            }
        }
    }
}
