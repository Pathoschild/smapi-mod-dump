/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using StardewValley.Inventories;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZP_Allergies
{
    public class InventoryUtils
    {
        public static List<Item> InventoryUsedItems(Dictionary<string, Item> beforeConsume, Dictionary<string, Item> afterConsume)
        {
            List<Item> usedItems = new();
            foreach (var pair in beforeConsume)
            {
                int numAfter = 0;
                if (afterConsume.ContainsKey(pair.Key))
                {
                    numAfter = afterConsume[pair.Key].Stack;
                }

                if (pair.Value.Stack - numAfter > 0)
                {
                    // we used some of this item
                    usedItems.Add(pair.Value.getOne());
                }
            }
            return usedItems;
        }

        public static Dictionary<string, Item> GetInventoryItemLookup(IInventory inv)
        {
            Dictionary<string, Item> result = new();
            if (inv is null) return result;

            foreach (Item i in inv)
            {
                if (i == null) continue;
                Item copy = i.getOne();
                copy.Stack = i.Stack;
                result[i.ItemId] = copy;
            }

            return result;
        }

        public static Dictionary<string, Item> GetItemsInAllInventories(List<IInventory> additionalMaterials)
        {
            Dictionary<string, Item> result = GetInventoryItemLookup(Game1.player.Items);

            if (additionalMaterials == null) return result;

            foreach (IInventory inv in additionalMaterials)
            {
                if (inv is null) continue;
                foreach (Item i in inv)
                {
                    if (i == null) continue;
                    Item copy = i.getOne();
                    copy.Stack = i.Stack;
                    result[i.ItemId] = copy;
                }
            }

            return result;
        }

        public static Inventory CopyInventory(IInventory inventory)
        {
            Inventory result = new();
            if (inventory is null) return result;

            foreach (Item i in inventory)
            {
                if (i is null)
                {
                    result.Add(i);
                    continue;
                }

                Item copy = i.getOne();
                copy.Stack = i.Stack;
                result.Add(copy);
            }


            return result;
        }
    }
}
