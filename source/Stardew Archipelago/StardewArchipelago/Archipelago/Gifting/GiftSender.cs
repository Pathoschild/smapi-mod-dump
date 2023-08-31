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
using Microsoft.Xna.Framework;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Archipelago.Gifting
{
    public class GiftSender
    {
        private  IMonitor _monitor;
        private ArchipelagoClient _archipelago;
        private StardewItemManager _itemManager;
        private IGiftingService _giftService;
        internal GiftGenerator GiftGenerator { get; }

        public GiftSender(IMonitor monitor, ArchipelagoClient archipelago, StardewItemManager itemManager, IGiftingService giftService)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _itemManager = itemManager;
            _giftService = giftService;
            GiftGenerator = new GiftGenerator(_itemManager);
        }

        public void SendGift(string slotName)
        {
            if (!_archipelago.PlayerExists(slotName))
            {
                Game1.chatBox?.addMessage($"Could not find player named {slotName}", Color.Gold);
                return;
            }

            var giftObject = Game1.player.ActiveObject;
            if (!GiftGenerator.TryCreateGiftItem(Game1.player.ActiveObject, out var giftItem, out var giftTraits))
            {
                // TryCreateGiftItem will log the reason if it fails
                return;
            }

            var isValidRecipient = _giftService.CanGiftToPlayer(slotName, giftTraits.Select(x => x.Trait));
            if (!isValidRecipient)
            {
                Game1.chatBox?.addMessage($"{slotName} cannot receive this gift", Color.Gold);
                return;
            }

            var itemValue = giftObject.Price * giftObject.Stack;
            var taxRate = _archipelago.SlotData.BankTax;
            var tax = (int)Math.Round(taxRate * itemValue);

            if (Game1.player.Money < tax)
            {
                Game1.chatBox?.addMessage($"You cannot afford Joja Prime for this item", Color.Gold);
                Game1.chatBox?.addMessage($"The tax is {taxRate * 100}% of the item's value of {itemValue}g, so you must pay {tax}g to gift it", Color.Gold);
                return;
            }


            var success = _giftService.SendGift(giftItem, giftTraits, slotName, out var giftId);
            _monitor.Log($"Sending gift of {giftItem.Amount} {giftItem.Name} to {slotName} with {giftTraits.Length} traits. [ID: {giftId}]", LogLevel.Info);
            if (!success)
            {
                _monitor.Log($"Gift Failed to send properly", LogLevel.Error);
                Game1.chatBox?.addMessage($"Unknown Error occurred while sending gift.", Color.Red);
            }

            Game1.player.ActiveObject = null;
            Game1.player.Money -= tax;
            Game1.chatBox?.addMessage($"{slotName} will receive your gift of {giftItem.Amount} {giftItem.Name} within 1 business day", Color.Gold);
            Game1.chatBox?.addMessage($"You have been charged a tax of {tax}g", Color.Gold);
            Game1.chatBox?.addMessage($"Thank you for using Joja Prime", Color.Gold);
        }
    }
}
