using QuestFramework.Offers;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Controllers
{
    class MailController : IAssetEditor
    {
        public MailController(QuestManager questManager, QuestOfferManager offerManager, IMonitor monitor)
        {
            this.QuestManager = questManager;
            this.OfferManager = offerManager;
            this.monitor = monitor;
        }

        public QuestManager QuestManager { get; }
        public QuestOfferManager OfferManager { get; }

        private readonly IMonitor monitor;

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data\\Mail");
        }

        public void Edit<T>(IAssetData asset)
        {
            var mails = asset.AsDictionary<string, string>();

            foreach (var offer in this.OfferManager.Offers.Where(o => o.OfferedBy == "Mail"))
            {
                var detailedOffer = offer.AsOfferWithDetails<MailOfferAttributes>();
                var letterId = GetQuestLetterKey(offer.QuestName);
                var questId = this.QuestManager.ResolveGameQuestId(offer.QuestName);
                
                if (questId != -1 || detailedOffer != null) 
                {
                    mails.Data[letterId] = $"{detailedOffer.OfferDetails.Text} %item quest {questId} %%[#]{detailedOffer.OfferDetails.Topic}";
                    this.monitor.Log($"Registered mail for quest offer `{detailedOffer.QuestName}` as letter id `{letterId}`");
                }
            }
        }

        public void ReceiveQuestLetterToMailbox()
        {
            if (!Context.IsWorldReady)
                return;

            foreach (var offer in this.OfferManager.GetMatchedOffers<MailOfferAttributes>("Mail"))
            {
                var whichLetter = GetQuestLetterKey(offer.QuestName);
                var id = this.QuestManager.ResolveGameQuestId(offer.QuestName);

                if (id != -1 && !Game1.player.mailbox.Contains(whichLetter) && !Game1.player.hasQuest(id))
                {
                    Game1.player.mailbox.Add(whichLetter);
                    this.monitor.Log($"Source mail of quest `{offer.QuestName}` (letter id: `{whichLetter}`) is ready in mailbox!");
                }
            }
        }

        public static string GetQuestLetterKey(string questname)
        {
            return $"quest_{questname}.qf";
        }
    }
}
