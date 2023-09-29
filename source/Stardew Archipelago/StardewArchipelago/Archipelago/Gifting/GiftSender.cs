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
using System.Linq;
using Archipelago.Gifting.Net.Service;
using Microsoft.Xna.Framework;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Archipelago.Gifting
{
    public class GiftSender
    {
        private readonly IMonitor _monitor;
        private readonly ArchipelagoClient _archipelago;
        private readonly IGiftingService _giftService;
        internal GiftGenerator GiftGenerator { get; }

        public GiftSender(IMonitor monitor, ArchipelagoClient archipelago, StardewItemManager itemManager, IGiftingService giftService)
        {
            _monitor = monitor;
            _archipelago = archipelago;
            _giftService = giftService;
            GiftGenerator = new GiftGenerator(itemManager);
        }

        public void SendGift(string slotName, bool isTrap)
        {
            try
            {
                if (!_archipelago.PlayerExists(slotName))
                {
                    Game1.chatBox?.addMessage($"Could not find player named {slotName}", Color.Gold);
                    return;
                }

                var giftObject = Game1.player.ActiveObject;
                if (!GiftGenerator.TryCreateGiftItem(Game1.player.ActiveObject, isTrap, out var giftItem,
                        out var giftTraits))
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
                    Game1.chatBox?.addMessage(
                        $"The tax is {taxRate * 100}% of the item's value of {itemValue}g, so you must pay {tax}g to gift it",
                        Color.Gold);
                    return;
                }


                var success = _giftService.SendGift(giftItem, giftTraits, slotName, out var giftId);
                _monitor.Log(
                    $"Sending gift of {giftItem.Amount} {giftItem.Name} to {slotName} with {giftTraits.Length} traits. [ID: {giftId}]",
                    LogLevel.Info);
                if (!success)
                {
                    _monitor.Log($"Gift Failed to send properly", LogLevel.Error);
                    Game1.chatBox?.addMessage($"Unknown Error occurred while sending gift.", Color.Red);
                    return;
                }

                Game1.player.ActiveObject = null;
                Game1.player.Money -= tax;
                Game1.chatBox?.addMessage(
                    $"{slotName} will receive your gift of {giftItem.Amount} {giftItem.Name} within 1 business day",
                    Color.Gold);
                Game1.chatBox?.addMessage($"You have been charged a tax of {tax}g", Color.Gold);
                Game1.chatBox?.addMessage($"Thank you for using Joja Prime", Color.Gold);
            }
            catch (Exception ex)
            {
                _monitor.Log($"Unknown error occurred while attempting to process gift command.{Environment.NewLine}Message: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
                Game1.chatBox?.addMessage($"Could complete gifting operation. Check SMAPI for error details.", Color.Red);
                return;
            }
        }
    }
}
