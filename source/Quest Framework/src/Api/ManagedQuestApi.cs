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
        private readonly IMonitor monitor;

        public string ModUid { get; }
        public QuestManager QuestManager { get; }
        public QuestOfferManager QuestOfferManager { get; }
        public HookManager HookManager { get; }

        private static IMonitor Monitor => QuestFrameworkMod.Instance.Monitor;

        public ManagedQuestApi(string modUid, QuestManager questManager, QuestOfferManager questOfferManager, HookManager hookManager, IMonitor monitor)
        {
            this.ModUid = modUid;
            this.QuestManager = questManager;
            this.QuestOfferManager = questOfferManager;
            this.HookManager = hookManager;
            this.monitor = monitor;
        }

        public void AcceptQuest(string fullQuestName, bool silent = false)
        {
            if (!fullQuestName.Contains('@'))
            {
                fullQuestName = $"{fullQuestName}@{this.ModUid}";
            }

            this.QuestManager.AcceptQuest(fullQuestName, silent);
        }

        [Obsolete]
        public CustomQuest GetById(int id)
        {
            return this.GetQuestById(id);
        }

        public CustomQuest GetQuestById(int id)
        {
            var managedQuest = this.QuestManager.GetById(id);

            if (managedQuest?.OwnedByModUid != this.ModUid)
                this.monitor.LogOnce($"Mod {this.ModUid} accessed to quest `{managedQuest.Name}` managed by mod `{managedQuest.OwnedByModUid}`");

            return managedQuest;
        }

        public CustomQuest GetQuestByName(string questName)
        {
            if (!questName.Contains('@'))
            {
                questName = $"{questName}@{this.ModUid}";
            }

            return this.GetQuestById(this.QuestManager.ResolveGameQuestId(questName));
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

            this.QuestOfferManager.AddOffer(offer);
        }

        public IEnumerable<QuestOffer> GetTodayQuestOffers(string source)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Unable to get today quest schedules when world is not ready!");

            return this.QuestOfferManager.GetMatchedOffers(source);
        }

        public IEnumerable<QuestOffer<TAttributes>> GetTodayQuestOffers<TAttributes>(string source)
        {
            if (!Context.IsWorldReady)
                throw new InvalidOperationException("Unable to get today quest schedules when world is not ready!");

            return this.QuestOfferManager.GetMatchedOffers<TAttributes>(source);
        }

        public void ExposeGlobalCondition(string conditionName, Func<string, CustomQuest, bool> conditionHandler)
        {
            string fullConditionName = $"{this.ModUid}/{conditionName}";

            this.HookManager.Conditions[fullConditionName] = conditionHandler;
            Monitor.Log($"Exposed custom global condition `{fullConditionName}`");
        }
    }
}
