/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Archipelago.Gifting.Net.Gifts.Versions.Current;
using Archipelago.Gifting.Net.Service;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Stardew;
using StardewModdingAPI;

namespace StardewArchipelago.Archipelago.Gifting
{
    public class GiftReceiver
    {
        private IMonitor _monitor;
        private ArchipelagoClient _archipelago;
        private IGiftingService _giftService;
        private StardewItemManager _itemManager;
        private Mailman _mail;
        private GiftProcessor _giftProcessor;

        public GiftReceiver(IMonitor monitor, ArchipelagoClient archipelago, IGiftingService giftService, StardewItemManager itemManager, Mailman mail)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _giftService = giftService;
            _itemManager = itemManager;
            _mail = mail;
            _giftProcessor = new GiftProcessor(monitor, archipelago, itemManager);
        }

        public void ReceiveAllGifts()
        {
            var gifts = _giftService.GetAllGiftsAndEmptyGiftbox();
            if (!gifts.Any())
            {
                return;
            }

            var giftAmounts = new Dictionary<ReceivedGift, int>();
            var giftIds = new Dictionary<string, ReceivedGift>();
            foreach (var (id, gift) in gifts)
            {
                ParseGift(gift, giftAmounts, giftIds);
            }

            foreach (var (receivedGift, amount) in giftAmounts)
            {
                var amountRemaining = amount;
                while (amountRemaining > 0)
                {
                    amountRemaining = SendGiftMail(giftIds, receivedGift, amountRemaining);
                }
            }
        }

        /// <summary>
        /// Sends a Gift Mail for the received gift item and the specified amount.
        /// </summary>
        /// <param name="giftIds"></param>
        /// <param name="receivedGift"></param>
        /// <param name="amount"></param>
        /// <returns>The amount of item remaining that needs to be sent after this gift, if the amount was too high</returns>
        private int SendGiftMail(Dictionary<string, ReceivedGift> giftIds, ReceivedGift receivedGift, int amount)
        {
            var relatedGiftIds = giftIds.Where(x => x.Value == receivedGift).Select(x => x.Key).ToArray();
            var senderGame = _archipelago.GetPlayerGame(receivedGift.SenderName);
            var item = _itemManager.GetItemByName(receivedGift.ItemName);
            var amountInGift = amount;
            if (amount > 999)
            {
                amountInGift = 999;
            }

            var amountRemainingAfterGift = amount - amountInGift;
            var mailKey = GetMailKey(relatedGiftIds, amountRemainingAfterGift);
            var embed = GetEmbed(item, amount);
            _mail.SendArchipelagoGiftMail(mailKey, receivedGift.ItemName, receivedGift.SenderName, senderGame, embed);

            return amountRemainingAfterGift;
        }

        private void ParseGift(Gift gift, Dictionary<ReceivedGift, int> giftAmounts, Dictionary<string, ReceivedGift> giftIds)
        {
            if (!_giftProcessor.TryMakeStardewItem(gift, out var item, out var amount))
            {
                if (!gift.IsRefund)
                {
                    _giftService.RefundGift(gift);
                }

                return;
            }

            var key = new ReceivedGift(item, gift.SenderSlot, _archipelago.GetPlayerName(gift.SenderSlot));
            if (!giftAmounts.ContainsKey(key))
            {
                giftAmounts.Add(key, 0);
            }

            giftAmounts[key] += amount;
            giftIds.Add(gift.ID, key);
        }

        private string GetEmbed(StardewItem item, int amount)
        {
            if (item == null || amount <= 0)
            {
                return "";
            }

            return $"%item object {item.Id} {amount} %%";
        }

        private string GetMailKey(IEnumerable<string> ids, int amount)
        {
            return $"APGift;{string.Join(";", ids)};{amount}";
        }
    }
}
