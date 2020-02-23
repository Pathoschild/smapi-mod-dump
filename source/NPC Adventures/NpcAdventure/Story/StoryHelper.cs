using NpcAdventure.Loader;
using StardewValley.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Story
{
    public class StoryHelper
    {
        public const int QUEST_ID_PREFIX = 1000;
        public const int ADVENTURE_QUEST_TYPE = 16;
        private readonly IContentLoader contentLoader;

        public StoryHelper(IContentLoader contentLoader)
        {
            this.contentLoader = contentLoader;
        }

        private Dictionary<int, string> GetQuestsSource()
        {
            return this.contentLoader.Load<Dictionary<int, string>>("Data/Quests");
        }

        public string[] GetRawQuestData(int id)
        {
            Dictionary<int, string> questDataSource = this.GetQuestsSource();

            if (!questDataSource.ContainsKey(id))
                return null;

            return questDataSource[id].Split('/');
        }

        public bool GetRawQuestData(int id, out string[] questData)
        {
            questData = this.GetRawQuestData(id);

            if (questData == null)
                return false;

            return true;
        }

        public Quest GetQuestById(int id)
        {
            Quest quest;

            if (!this.GetRawQuestData(id, out string[] parsed))
                return null;


            switch (parsed[0])
            {
                case "Location":
                    quest = new GoSomewhereQuest(parsed[4]);
                    break;
                default:
                    quest = new Quest();
                    break;
            }
             
            quest.id.Value = QUEST_ID_PREFIX + id;
            quest.questType.Value = ADVENTURE_QUEST_TYPE;
            quest.questTitle = parsed[1];
            quest.questDescription = parsed[2];
            quest.currentObjective = parsed[3];
            quest.completionString.Value = parsed[4];
            quest.moneyReward.Value = int.Parse(parsed[5]);
            quest.canBeCancelled.Value = parsed[6].Equals("true");

            return quest;
        }

        public Quest GetNextQuestFor(int id)
        {
            if (!this.GetRawQuestData(id, out string[] parsed))
                return null;

            if (parsed.Length > 7)
            {
                return this.GetQuestById(int.Parse(parsed[7]));
            }

            return null;
        }

        public string GetQuestType(int id)
        {
            if (!this.GetRawQuestData(id, out string[] parsed))
                return null;

            return parsed[0];
        }

        public static int ResolveId(int id)
        {
            if (id > QUEST_ID_PREFIX)
                return id - QUEST_ID_PREFIX;

            return id;
        }
    }
}
