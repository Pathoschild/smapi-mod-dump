using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JoysOfEfficiency.Utils;
using StardewValley;
using StardewValley.Objects;

namespace JoysOfEfficiency.Patches
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal class CraftingRecipePatcher
    {
        internal static bool Prefix(ref CraftingRecipe __instance)
        {
            Dictionary<int, int> recipeList = Util.Helper.Reflection.GetField<Dictionary<int, int>>(__instance, "recipeList").GetValue();
            foreach (KeyValuePair<int, int> kv in recipeList)
            {
                int index = kv.Key;
                int count = kv.Value;
                int toConsume;
                foreach (Item playerItem in new List<Item>(Game1.player.Items))
                {
                    //Search for player inventory
                    if (playerItem != null && (playerItem.ParentSheetIndex == index || playerItem.Category == index))
                    {
                        toConsume = Math.Min(playerItem.Stack, count);
                        playerItem.Stack -= toConsume;
                        count -= toConsume;
                        if (playerItem.Stack == 0)
                        {
                            Game1.player.removeItemFromInventory(playerItem);
                        }
                    }
                }

                List<Chest> chests = Util.GetNearbyChests(Game1.player);
                foreach (Chest chest in new List<Chest>(chests))
                {
                    //Search for chests
                    foreach (Item chestItem in chest.items)
                    {
                        if (chestItem != null && (chestItem.ParentSheetIndex == index || chestItem.Category == index))
                        {
                            toConsume = Math.Min(chestItem.Stack, count);
                            chestItem.Stack -= toConsume;
                            count -= toConsume;
                            if (chestItem.Stack == 0)
                            {
                                chest.items.Remove(chestItem);
                            }
                        }
                    }
                    
                }
            }
            return false;
        }
    }
}
