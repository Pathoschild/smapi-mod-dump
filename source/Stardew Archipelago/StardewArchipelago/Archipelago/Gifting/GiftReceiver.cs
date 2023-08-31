/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archipelago.Gifting.Net;
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

            var giftItems = new Dictionary<(string, string), int>();
            var giftIds = new Dictionary<Guid, (string, string)>();
            foreach (var (id, gift) in gifts)
            {
                ParseGift(gift, giftItems, giftIds);
            }

            foreach (var ((itemName, senderName), amount) in giftItems)
            {
                var relatedGiftIds = giftIds.Where(x => x.Value == (itemName, senderName)).Select(x => x.Key);
                var mailKey = GetMailKey(relatedGiftIds);
                var firstGift = gifts[relatedGiftIds.First()];
                var senderGame = _archipelago.GetPlayerGame(senderName);
                var item = _itemManager.GetItemByName(itemName);
                var embed = GetEmbed(item, amount);
                _mail.SendArchipelagoGiftMail(mailKey, firstGift.Item.Name, senderName, senderGame, embed);
            }
        }

        private void ParseGift(Gift gift, Dictionary<(string, string), int> giftItems, Dictionary<Guid, (string, string)> giftIds)
        {
            if (!_giftProcessor.TryMakeStardewItem(gift, out var item, out var amount))
            {
                if (!gift.IsRefund)
                {
                    _giftService.RefundGift(gift);
                    return;
                }
            }

            var key = (item, gift.SenderName);
            if (!giftItems.ContainsKey(key))
            {
                giftItems.Add(key, 0);
            }

            giftItems[key] += amount;
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

        private string GetMailKey(IEnumerable<Guid> ids)
        {
            return $"APGift;{string.Join(";", ids)}";
        }
    }
}
