/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Menus;
using Unlockable_Bundles.Lib.AdvancedPricing;
using static Unlockable_Bundles.ModEntry;
using Unlockable_Bundles.Lib.WalletCurrency;
using Unlockable_Bundles.NetLib;

namespace Unlockable_Bundles.Lib.ShopTypes
{
    public class Inventory
    {

        public static void Initialize()
        {
        }

        public static bool hasEnoughItems(Farmer who, KeyValuePair<string, int> requirement)
        {
            string currentKey = "";
            int countedItems = 0;
            try {
                foreach (var item in requirement.Key.Split(",")) {
                    currentKey = item;
                    countedItems += getItemCount(who, item);

                }
            } catch {
                Monitor.LogOnce($"Unlockable requirement key contains a invalid item id: {(requirement.Key == currentKey ? requirement.Key : requirement.Key + " -> " + currentKey)}", LogLevel.Error);
            }

            return countedItems >= requirement.Value;
        }

        public static void removeAllRequiredItems(Farmer who, IEnumerable<KeyValuePair<string, int>> requirementSet)
        {
            foreach (var requirement in requirementSet)
                removeItemsOfRequirement(who, requirement);
        }

        public static void removeItemsOfRequirement(Farmer who, KeyValuePair<string, int> requirement)
        {
            int removedItems = 0;

            foreach (var item in requirement.Key.Split(",")) {
                int left = requirement.Value - removedItems;

                var count = getItemCount(who, item);
                if (count < left) {
                    subtractItems(who, item, count);
                    removedItems += count;
                } else {
                    subtractItems(who, item, left);
                    return;
                }
            }
        }

        public static int getItemCount(Farmer who, string key)
        {
            if (key.ToLower() == "money")
                return who.Money;

            var item = Unlockable.parseItem(Unlockable.getIDFromReqSplit(key), 0, Unlockable.getQualityFromReqSplit(key));
            if (item.QualifiedItemId == "(O)858")
                return who.QiGems;

            else if (item.QualifiedItemId == "(O)73")
                return Game1.netWorldState.Value.GoldenWalnuts;

            else if (WalletCurrencyHandler.getCurrencyItemMatch(item.QualifiedItemId, out var match, out var currency, out var relevantPlayer)) {
                var wallet = ModData.getWalletCurrency(currency.Id, relevantPlayer);
                return wallet / match.Value;

            } else {
                var relevant = getRelevantInventory(who, item);
                return relevant.Sum(el => el.Value.Stack);
            }
        }

        public static Dictionary<int, Item> getRelevantInventory(Farmer who, Item item)
        {
            var relevantInventory = new Dictionary<int, Item>();

            for (int i = 0; i < who.Items.Count; i++)
                if (who.Items[i] is not null && isItemValid(item, who.Items[i]))
                    relevantInventory.Add(i, who.Items[i]);

            return relevantInventory;
        }

        public static bool isItemValid(Item priceItem, Item compareItem)
        {
            if (compareItem.Quality < priceItem.Quality)
                return false;

            if (compareItem.QualifiedItemId == priceItem.QualifiedItemId)
                return true;

            else if (priceItem is AdvancedPricingItem apItem)
                if (apItem.ContextTags.All(tag => compareItem.HasContextTag(tag)))
                    return true;

            return false;
        }

        public static void subtractItems(Farmer who, string key, int amount)
        {
            if (key.ToLower() == "money") {
                who.Money -= amount;
                return;
            }

            var item = Unlockable.parseItem(Unlockable.getIDFromReqSplit(key), quality: Unlockable.getQualityFromReqSplit(key));

            if (item.QualifiedItemId == "(O)858") {
                who.QiGems -= amount;
                return;
            } else if (item.QualifiedItemId == "(O)73") {
                Game1.netWorldState.Value.GoldenWalnuts -= amount;
                return;
            } else if (WalletCurrencyHandler.getCurrencyItemMatch(item.QualifiedItemId, out var match, out var currency, out var relevantPlayer)) {
                WalletCurrencyHandler.addWalletCurrency(currency, relevantPlayer, -(amount * match.Value), true, true);
                return;
            }

            var relevantInventory = getRelevantInventory(who, item);

            //We want to take the least valuable items out first, so we sort by quality and sellToStorePrice
            var sortedInventry = from e in relevantInventory orderby e.Value.Quality, e.Value.sellToStorePrice() ascending select e;

            foreach (var el in sortedInventry) {
                if (who.Items[el.Key].Stack > amount) {
                    who.Items[el.Key].Stack -= amount;
                    return;
                }

                amount -= who.Items[el.Key].Stack;
                who.Items[el.Key] = null;


                if (amount <= 0)
                    return;
            }
        }

        public static bool addExceptionItem(Farmer who, string id, int value)
        {
            if (id.ToLower() == "money") {
                who.Money += value;
                return true;
            }

            if (id == "(O)858" || id == "858") {
                who.QiGems += value;
                return true;
            } else if (id == "(O)73" || id == "73") {
                Game1.netWorldState.Value.GoldenWalnuts += value;
                return true; ;
            }

            return false;
        }
    }
}
