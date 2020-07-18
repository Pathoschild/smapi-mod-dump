using QuestFramework.Offers;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework
{
    class QuestOfferManager
    {
        private readonly HookManager hookManager;

        public List<QuestOffer> Offers { get; }

        public QuestOfferManager(HookManager hookManager)
        {
            this.Offers = new List<QuestOffer>();
            this.hookManager = hookManager;
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
            return from schedule in this.Offers
                   where schedule.OfferedBy == source
                     && this.hookManager.CheckConditions(schedule.When)
                     && (schedule.OnlyMainPlayer == Context.IsMainPlayer || schedule.OnlyMainPlayer == false)
                   select schedule;
        }

        public IEnumerable<QuestOffer<TAttributes>> GetMatchedOffers<TAttributes>(string source)
        {
            return this.GetMatchedOffers(source)
                .Select(schedule => schedule.AsOfferWithDetails<TAttributes>())
                .Where(schedule => schedule != null);
        }
    }
}
