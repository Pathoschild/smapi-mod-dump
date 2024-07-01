/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Brandon22Adams/ToolPouch
**
*************************************************/

using StardewValley;
using StardewValley.Inventories;

namespace ToolPouch
{
    internal static class Extensions
    {
        public static void Sort(this Inventory inv)
        {
            List<Item> itemList = new List<Item>();
            for (int i = 0; i < inv.Count; i++)
            {
                if (inv[i] != null)
                {
                    itemList.Add(inv[i]);
                }
            }
            for (int i = 0; i < inv.Count; i++)
            {
                
                if(i + 1 > itemList.Count)
                {
                    inv[i] = null;
                } 
                else
                {
                    inv[i] = itemList[i];
                }
            }
        }
        public static Item DepositItem(this Inventory inv, Item item)
        {
            // Taken from Farmer.addItemToInventory
            for (int i = 0; i < inv.Count; i++)
            {
                Item item2 = inv[i];
                if (item.canStackWith(item2))
                {
                    int num = item2.addToStack(item);
                    if (num <= 0)
                    {
                        return null;
                    }

                    item.Stack = num;
                }
            }

            for (int j = 0; j < inv.Count; j++)
            {
                if (inv[j] == null)
                {
                    inv[j] = item;
                    return null;
                }
            }

            return item;
        }
    }
}
