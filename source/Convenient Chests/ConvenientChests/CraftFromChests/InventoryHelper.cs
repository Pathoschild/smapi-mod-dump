/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace ConvenientChests.CraftFromChests {
    public static class InventoryHelper {
        public static string ToString(this IEnumerable<Item> list) => string.Join(", ", list.Select(i => i == null ? "null" : $"{i.Name} x{i.Stack}"));
        
        public static bool TryMoveItem(Item item, IList<Item> inventory) {
            Item stack = inventory.FirstOrDefault(x => x?.canStackWith(item) == true && x.Stack + item.Stack <= item.maximumStackSize());
            int index = inventory.IndexOf(stack);
            if (index == -1)
                return false;

            if (inventory[index] == null)
                inventory[index] = item;

            else
                inventory[index].Stack += item.Stack;

            return true;
        }
    }
}