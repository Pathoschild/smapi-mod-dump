/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Offers;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestFramework.Framework
{
    class QuestOfferManager
    {
        private readonly ConditionManager conditionManager;
        private readonly QuestManager questManager;
        private readonly PerScreen<List<QuestOffer>> _offers;

        public List<QuestOffer> Offers => this._offers.Value;

        public QuestOfferManager(ConditionManager conditionManager, QuestManager questManager)
        {
            this._offers = new PerScreen<List<QuestOffer>>(() => new List<QuestOffer>());
            this.conditionManager = conditionManager;
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
                         && this.conditionManager.CheckConditions(offer.When, context)
                         && offer.CheckAdditionalCondition(context)
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
