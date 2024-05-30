/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zombifier/My_Stardew_Mods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.Inventories;
using System.Collections.Generic;

namespace ExtraMachineConfig; 

static class Utils {
  // Removes items with the specified ID from the inventory.
  // This differs from ReduceId is that itemId can also be category IDs.
  public static bool RemoveItemFromInventoryById(IInventory inventory, string itemId, int count) {
    return RemoveItemFromInventory(inventory, item => CraftingRecipe.ItemMatchesForCrafting(item, itemId), count);
  }

  // Removes items with the specified tags from the inventory.
  public static bool RemoveItemFromInventoryByTags(IInventory inventory, string itemTags, int count) {
    return RemoveItemFromInventory(inventory, item => ItemContextTagManager.DoesTagQueryMatch(itemTags, item.GetContextTags()), count);
  }

  // TODO: Port functionality from ExtraFuelConfig
  public static bool RemoveItemFromInventory(IInventory inventory, Func<Item, bool> func, int count) {
    for (int index = 0; index < inventory.Count; ++index) {
      if (inventory[index] != null && func(inventory[index])) {
        if (inventory[index].Stack > count) {
          inventory[index].Stack -= count;
          return true;
        }
        count -= inventory[index].Stack;
        inventory[index] = (Item)null;
      }
      if (count <= 0) {
        return true;
      }
    }
    return false;
  }

  public static int getItemCountInListByTags(IList<Item> list, string itemTags) {
    int num = 0;
    for (int i = 0; i < list.Count; i++) {
      if (list[i] != null && ItemContextTagManager.DoesTagQueryMatch(itemTags, list[i].GetContextTags())) {
        num += list[i].Stack;
      }
    }
    return num;
  }

}
