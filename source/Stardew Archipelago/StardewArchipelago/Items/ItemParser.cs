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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Items.Mail;
using StardewArchipelago.Stardew;

namespace StardewArchipelago.Items
{
    public class ItemParser
    {
        public const string RESOURCE_PACK_PREFIX = "Resource Pack: ";
        public const string FRIENDSHIP_BONUS_PREFIX = "Friendship Bonus (";
        public const string RECIPE_SUFFIX = " Recipe";

        private StardewItemManager _itemManager;
        private UnlockManager _unlockManager;

        public ItemParser(StardewItemManager itemManager, UnlockManager unlockManager)
        {
            _itemManager = itemManager;
            _unlockManager = unlockManager;
        }

        public LetterAttachment ProcessItem(ReceivedItem receivedItem)
        {
            var itemIsResourcePack = TryParseResourcePack(receivedItem.ItemName, out var stardewItemName, out var resourcePackAmount);
            if (itemIsResourcePack)
            {
                if (stardewItemName == "Money")
                {
                    return new LetterMoneyAttachment(receivedItem, resourcePackAmount);
                }

                var resourcePackItem = GetResourcePackItem(stardewItemName);
                return resourcePackItem.GetAsLetter(receivedItem, resourcePackAmount);
            }

            var itemIsFriendshipBonus = TryParseFriendshipBonus(receivedItem.ItemName, out var numberOfPoints);
            if (itemIsFriendshipBonus)
            {
                return new LetterActionAttachment(receivedItem, LetterActionsKeys.Friendship, numberOfPoints.ToString());
            }

            if (_unlockManager.IsUnlock(receivedItem.ItemName))
            {
                return _unlockManager.PerformUnlock(receivedItem);
            }

            if (receivedItem.ItemName.EndsWith(RECIPE_SUFFIX))
            {
                var itemOfRecipe =
                    receivedItem.ItemName.Substring(0, receivedItem.ItemName.Length - RECIPE_SUFFIX.Length);
                return new LetterCraftingRecipeAttachment(receivedItem, itemOfRecipe);
            }

            if (_itemManager.ItemExists(receivedItem.ItemName))
            {
                var singleItem = GetSingleItem(receivedItem.ItemName);
                return singleItem.GetAsLetter(receivedItem);
            }

            return new LetterInformationAttachment(receivedItem);
            throw new ArgumentException($"Could not process item {receivedItem.ItemName}");
        }

        private bool TryParseResourcePack(string apItemName, out string stardewItemName, out int amount)
        {
            stardewItemName = "";
            amount = 0;
            if (apItemName.StartsWith(RESOURCE_PACK_PREFIX))
            {
                var apItemWithoutPrefix = apItemName.Substring(RESOURCE_PACK_PREFIX.Length);
                return TryParseResourcePack(apItemWithoutPrefix, out stardewItemName, out amount);
            }

            var parts = apItemName.Split(" ");
            if (!int.TryParse(parts[0], out amount))
            {
                return false;
            }

            stardewItemName = apItemName.Substring(apItemName.IndexOf(" ", StringComparison.Ordinal) + 1);
            return true;
        }

        private bool TryParseFriendshipBonus(string apItemName, out int numberOfPoints)
        {
            numberOfPoints = 0;
            if (!apItemName.StartsWith(FRIENDSHIP_BONUS_PREFIX))
            {
                return false;
            }

            var apItemWithoutPrefix = apItemName.Substring(FRIENDSHIP_BONUS_PREFIX.Length);
            var parts = apItemWithoutPrefix.Split(" ");
            if (!double.TryParse(parts[0], out var numberOfHearts))
            {
                return false;
            }

            numberOfPoints = (int)Math.Round(numberOfHearts * 250);

            return true;
        }

        private StardewItem GetSingleItem(string stardewItemName)
        {
            var item = _itemManager.GetItemByName(stardewItemName);
            return item;
        }

        private StardewItem GetResourcePackItem(string stardewItemName)
        {
            if (_itemManager.ItemExists(stardewItemName))
            {
                return _itemManager.GetItemByName(stardewItemName);
            }

            // Sometimes an item is plural because it's a resource pack, but the item is registered with a singular name in-game
            // So I try the alternate version before giving up
            var isPlural = stardewItemName.EndsWith('s');
            var otherVersion = isPlural ? stardewItemName.Substring(0, stardewItemName.Length - 1) : stardewItemName + "s";
            return _itemManager.GetItemByName(otherVersion);
        }
    }
}
