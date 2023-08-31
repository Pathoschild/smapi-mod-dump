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

namespace Unlockable_Bundles.Lib.ShopTypes
{
    public class Inventory
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;
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

            var obj = Unlockable.parseItem(Unlockable.getIDFromReqSplit(key));
            int quality = Unlockable.getQualityFromReqSplit(key);
            if (obj.QualifiedItemId == "(O)858")
                return who.QiGems;

            else if (obj.QualifiedItemId == "(O)73")
                return Game1.netWorldState.Value.GoldenWalnuts;

            else
                return who.Items.Sum(e => e is not null
                                         && e.QualifiedItemId == obj.QualifiedItemId
                                         && e.Quality >= quality
                                         ? e.Stack
                                         : 0);
        }

        public static void subtractItems(Farmer who, string key, int amount)
        {
            if (key.ToLower() == "money") {
                who.Money -= amount;
                return;
            }

            var obj = Unlockable.parseItem(Unlockable.getIDFromReqSplit(key));
            int quality = Unlockable.getQualityFromReqSplit(key);

            if (obj.QualifiedItemId == "(O)858") {
                who.QiGems -= amount;
                return;
            } else if (obj.QualifiedItemId == "(O)73") {
                Game1.netWorldState.Value.GoldenWalnuts -= amount;
                return;
            }

            var relevantInventory = new Dictionary<int, int>();
            for (int i = 0; i < who.Items.Count; i++)
                if (who.Items[i] is not null
                    && who.Items[i].QualifiedItemId == obj.QualifiedItemId
                    && who.Items[i].Quality >= quality)
                    relevantInventory.Add(i, who.Items[i].Quality);

            //We want to take the least valuable items out first, so we sort by quality
            var sortedInventry = (from e in relevantInventory orderby e.Value ascending select e);

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
