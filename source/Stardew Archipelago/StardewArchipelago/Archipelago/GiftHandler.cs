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
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Stardew;
using StardewValley;

namespace StardewArchipelago.Archipelago
{
    public class GiftHandler
    {
        private const string gift_key_pattern = "StardewGift {0}";

        public ArchipelagoClient _archipelago;
        private StardewItemManager _itemManager;
        private Mailman _mail;
        private Random _random;

        public GiftHandler()
        {
        }

        public void Initialize(StardewItemManager itemManager, Mailman mail, ArchipelagoClient archipelago)
        {
            _itemManager = itemManager;
            _mail = mail;
            _archipelago = archipelago;
            _random = new Random();
        }

        public bool HandleGiftItemCommand(string message)
        {
            if (_archipelago == null)
            {
                return false;
            }

            var giftPrefix = $"{ChatForwarder.COMMAND_PREFIX}gift ";
            if (!message.StartsWith(giftPrefix))
            {
                return false;
            }

            if (!_archipelago.SlotData.Gifting) // Did I enable that feature?
            {
                Game1.chatBox?.addMessage($"You are not allowed to send gifts from this Archipelago Slot", Color.Gold);
                return true;
            }

            var receiverSlotName = message.Substring(giftPrefix.Length);
            var isValidRecipient = _archipelago.IsStardewValleyPlayer(receiverSlotName);

            if (!isValidRecipient)
            {
                Game1.chatBox?.addMessage($"{receiverSlotName} is not recognized as a Stardew Valley player in this multiworld", Color.Gold);
                return true;
            }

            if (false /*How to check if they have enabled it?*/)
            {
                Game1.chatBox?.addMessage($"This player is not accepting gifts", Color.Gold);
                return true;
            }

#if RELEASE
            if (receiverSlotName == _archipelago.SlotData.SlotName)
            {
                Game1.chatBox?.addMessage($"You cannot send yourself a gift", Color.Gold);
                return true;
            }
#endif
            var giftObject = Game1.player.ActiveObject;
            if (giftObject == null || !_itemManager.ItemExists(giftObject.Name) || _itemManager.GetItemByName(giftObject.Name) is not StardewObject || giftObject.questItem.Value)
            {
                Game1.chatBox?.addMessage($"You cannot gift this item to another player", Color.Gold);
                return true;
            }

            var itemValue = giftObject.Price * giftObject.Stack;
            var tax = (int)Math.Round(_archipelago.SlotData.GiftTax * itemValue);

            if (Game1.player.Money < tax)
            {
                Game1.chatBox?.addMessage($"You cannot afford Joja Prime for this item", Color.Gold);
                Game1.chatBox?.addMessage($"The tax is {_archipelago.SlotData.GiftTax * 100}% of the item's value of {itemValue}g, so you must pay {tax}g to gift it", Color.Gold);
                return true;
            }

            var key = string.Format(gift_key_pattern, receiverSlotName);
            var sender = _archipelago.SlotData.SlotName.Replace(" ", "");
            var amount = giftObject.Stack;
            var itemType = giftObject.bigCraftable.Value ? "bigobject" : "object"; // Stardew object types
            var itemQuality = giftObject.Quality;
            var itemName = giftObject.Name;
            var value = $"{_random.Next(int.MaxValue)} {sender} {receiverSlotName.Replace(" ", "")} {amount} {itemType} {itemQuality} {itemName}";

            _archipelago.AddToStringDataStorage(Scope.Game, key, value);
            Game1.player.ActiveObject = null;
            Game1.player.Money -= tax;

            Game1.chatBox?.addMessage($"{receiverSlotName} will receive your gift of {amount} {itemName} within 1 business day", Color.Gold);
            Game1.chatBox?.addMessage($"You have been charged a tax of {tax}g", Color.Gold);
            Game1.chatBox?.addMessage($"Thank you for using Joja Prime", Color.Gold);

            return true;
        }

        public void ReceiveAllGiftsTomorrow()
        {
            if (_archipelago == null || !_archipelago.SlotData.Gifting)
            {
                return;
            }

            var mySlotName = _archipelago.SlotData.SlotName;
            var slotNameGiftkey = string.Format(gift_key_pattern, mySlotName);
            ReceiveGiftsTomorrow(slotNameGiftkey);

            var myAlias = _archipelago.GetPlayerAlias(mySlotName);
            var aliasGiftKey = string.Format(gift_key_pattern, myAlias);
            ReceiveGiftsTomorrow(aliasGiftKey);
        }

        private void ReceiveGiftsTomorrow(string giftKey)
        {
            if (!_archipelago.StringExistsInDataStorage(Scope.Game, giftKey))
            {
                return;
            }

            var newGifts = _archipelago.ReadStringFromDataStorage(Scope.Game, giftKey)
                .Split(ArchipelagoClient.STRING_DATA_STORAGE_DELIMITER);
            foreach (var newGift in newGifts)
            {
                ReceiveGiftTomorrow(newGift);
            }

            _archipelago.RemoveStringFromDataStorage(Scope.Game, giftKey);
        }

        private void ReceiveGiftTomorrow(string giftString)
        {
            var splitGiftValue = giftString.Split(" ");
            var uniqueId = splitGiftValue[0];
            var senderName = splitGiftValue[1];
            var receiverName = splitGiftValue[2];
            var amount = splitGiftValue[3];
            var itemType = splitGiftValue[4];
            var itemQuality = splitGiftValue[5];
            var itemName = string.Join(" ", splitGiftValue.Skip(6));

            var parsedItem = _itemManager.GetItemByName(itemName);

            var letterEmbedString = $"%item {itemType} {parsedItem.Id} {amount} %%";

            var mailKey = giftString.Replace(" ", "_");
            _mail.SendArchipelagoGiftMail(mailKey, senderName, letterEmbedString);
        }
    }
}
