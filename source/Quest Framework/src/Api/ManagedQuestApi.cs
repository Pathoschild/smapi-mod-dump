using QuestFramework.Framework;
using QuestFramework.Offers;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Api
{
    internal class ManagedQuestApi : IManagedQuestApi
    {
        public string Category { get; }
        public QuestManager QuestManager { get; }
        public QuestOfferManager ScheduleManager { get; }

        public ManagedQuestApi(string category, QuestManager questManager, QuestOfferManager scheduleManager)
        {
            this.Category = category;
            this.QuestManager = questManager;
            this.ScheduleManager = scheduleManager;
        }

        public void AcceptQuest(string fullQuestName)
        {
            if (!fullQuestName.Contains('@'))
            {
                fullQuestName = $"{fullQuestName}@{this.Category}";
            }

            this.QuestManager.AcceptQuest(fullQuestName);
        }

        public CustomQuest GetById(int id)
        {
            return this.QuestManager.GetById(id);
        }

        public void RegisterQuest(CustomQuest quest)
        {
            quest.OwnedByModUid = this.Category;

            this.QuestManager.RegisterQuest(quest);
        }

        public void CompleteQuest(string questName)
        {
            int questId = this.QuestManager.ResolveGameQuestId($"{questName}@{this.Category}");

            if (questId > -1 && Game1.player.hasQuest(questId))
                Game1.player.completeQuest(questId);
        }

        public void OfferQuest(QuestOffer offer)
        {
            if (!offer.QuestName.Contains('@'))
            {
                offer.QuestName = $"{offer.QuestName}@{this.Category}";
            }

            this.ScheduleManager.AddOffer(offer);
        }

        public IEnumerable<QuestOffer> GetTodayQuestOffers(string source)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Unable to get today quest schedules when world is not ready!");

            return this.ScheduleManager.GetMatchedOffers(source);
        }

        public IEnumerable<QuestOffer<TAttributes>> GetTodayQuestOffers<TAttributes>(string source)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Unable to get today quest schedules when world is not ready!");

            return this.ScheduleManager.GetMatchedOffers<TAttributes>(source);
        }
    }
}
