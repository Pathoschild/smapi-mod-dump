using QuestFramework.Offers;
using QuestFramework.Quests;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework
{
    class QuestOfferManager
    {
        private readonly HookManager hookManager;
        private readonly QuestManager questManager;

        public List<QuestOffer> Offers { get; }

        public QuestOfferManager(HookManager hookManager, QuestManager questManager)
        {
            this.Offers = new List<QuestOffer>();
            this.hookManager = hookManager;
            this.questManager = questManager;
        }

        public void AddOffer(QuestOffer schedule)
        {
            if (QuestFrameworkMod.Instance.Status < State.LAUNCHING)
                throw new InvalidOperationException($"Unable to add quest schedule in state `{QuestFrameworkMod.Instance.Status}`.");

            this.Offers.Add(schedule);
            QuestFrameworkMod.InvalidateCache();
        }

        public IEnumerable<QuestOffer> GetMatchedOffers(string source)
        {
            return from offer in this.Offers
                   let context = this.questManager.Fetch(offer.QuestName)
                   let isRelevantOffer = offer.OfferedBy == source && context != null
                   where isRelevantOffer
                         && this.hookManager.CheckConditions(offer.When, context)
                         && (offer.OnlyMainPlayer == Context.IsMainPlayer || offer.OnlyMainPlayer == false)
                   select offer;
        }

        public IEnumerable<QuestOffer<TAttributes>> GetMatchedOffers<TAttributes>(string source)
        {
            return this.GetMatchedOffers(source)
                .Select(offer => offer.AsOfferWithDetails<TAttributes>())
                .Where(offer => offer != null);
        }
    }
}
