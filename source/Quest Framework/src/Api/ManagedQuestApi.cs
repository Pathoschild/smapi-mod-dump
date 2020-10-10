/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

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
        public string ModUid { get; }
        public QuestManager QuestManager { get; }
        public QuestOfferManager ScheduleManager { get; }
        public HookManager HookManager { get; }

        private static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;

        public ManagedQuestApi(string modUid, QuestManager questManager, QuestOfferManager scheduleManager, HookManager hookManager)
        {
            this.ModUid = modUid;
            this.QuestManager = questManager;
            this.ScheduleManager = scheduleManager;
            this.HookManager = hookManager;
        }

        public void AcceptQuest(string fullQuestName, bool silent = false)
        {
            if (!fullQuestName.Contains('@'))
            {
                fullQuestName = $"{fullQuestName}@{this.ModUid}";
            }

            this.QuestManager.AcceptQuest(fullQuestName, silent);
        }

        public CustomQuest GetById(int id)
        {
            return this.QuestManager.GetById(id);
        }

        public void RegisterQuest(CustomQuest quest)
        {
            quest.OwnedByModUid = this.ModUid;

            this.QuestManager.RegisterQuest(quest);
        }

        public void CompleteQuest(string questName)
        {
            int questId = this.QuestManager.ResolveGameQuestId($"{questName}@{this.ModUid}");

            if (questId > -1 && Game1.player.hasQuest(questId))
                Game1.player.completeQuest(questId);
        }

        public void OfferQuest(QuestOffer offer)
        {
            if (!offer.QuestName.Contains('@'))
            {
                offer.QuestName = $"{offer.QuestName}@{this.ModUid}";
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

        public void ExposeGlobalCondition(string conditionName, Func<string, CustomQuest, bool> conditionHandler)
        {
            string fullConditionName = $"{this.ModUid}/{conditionName}";

            this.HookManager.Conditions[fullConditionName] = conditionHandler;
            Monitor.Log($"Exposed custom global condition `{fullConditionName}`");
        }
    }
}
