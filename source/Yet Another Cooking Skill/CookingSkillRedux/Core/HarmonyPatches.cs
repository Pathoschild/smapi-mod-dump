/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using BirbCore.Attributes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Enchantments;
using StardewValley.Extensions;
using StardewValley.GameData;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Locations;
using StardewValley.Internal;
using StardewValley.Inventories;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;
using System.Linq;
using Microsoft.VisualBasic;

namespace CookingSkill.Core
{

    [HarmonyPatch(typeof(CraftingPage), "clickCraftingRecipe")]
    class ClickCraftingRecipe_patch
    {
        [HarmonyLib.HarmonyPrefix]
        public static bool ClickCraftingRecipe(CraftingPage __instance, ClickableTextureComponent c, bool playSound, ref int ___currentCraftingPage, ref Item ___heldItem, ref bool ___cooking)
        {
            //do not change anything not cooking related - pointlessly dangerous
            if(!___cooking)
            {
                return true;
            }
            ModEntry.Instance.Monitor.Log("YACS Starting click crafting recipe prefix - should not happen if bettercrafting is instaleld", LogLevel.Trace);
            CraftingRecipe craftingRecipe = __instance.pagesOfCraftingRecipes[__instance.currentCraftingPage][c];
            Item item = craftingRecipe.createItem();
            var player = Game1.getFarmer(Game1.player.UniqueMultiplayerID);
            List<KeyValuePair<string, int>> list = null;
            if (___cooking && item.Quality == 0)
            {
                //don't allow the player to force a certain quality dish -
                //if inventory is full do not allow the craft - display full inventory toaster
                if (player.isInventoryFull() && ___heldItem != null)
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    return false;
                }
                list = new List<KeyValuePair<string, int>>();
                list.Add(new KeyValuePair<string, int>("917", 1));
                if (CraftingRecipe.DoesFarmerHaveAdditionalIngredientsInInventory(list, GetContainerContents(__instance._materialContainers)))
                {
                    item.Quality = 2;

                }
                else
                {
                    list = null;
                }

                /// /////////////////////////////
                /// Custom Code
                /// for Cooking
                if (craftingRecipe is not null && craftingRecipe.isCookingRecipe)
                {
                    var consumed_items = FigureOutItems(craftingRecipe, __instance._materialContainers);
                    CookingSkill.Core.Events.PreCook(craftingRecipe, item);
                    CookingSkill.Core.Events.PostCook(craftingRecipe, item, consumed_items, player);
                }
                
                ////////////////////////////////////
            }

            if (___heldItem == null)
            {
                craftingRecipe.consumeIngredients(__instance._materialContainers);
                ___heldItem = item;
                if (playSound)
                {
                    Game1.playSound("coin");
                }
            }
            else
            {
                if (!(___heldItem.Name == item.Name) || !___heldItem.getOne().canStackWith(item.getOne()) || ___heldItem.Stack + craftingRecipe.numberProducedPerCraft - 1 >= ___heldItem.maximumStackSize())
                {
                    item.Stack = craftingRecipe.numberProducedPerCraft;
                    if (player.couldInventoryAcceptThisItem(item))
                    {
                        player.addItemToInventoryBool(item);
                    }
                    else { return false; }
                }
                else
                {
                    ___heldItem.Stack += craftingRecipe.numberProducedPerCraft;
                }
                craftingRecipe.consumeIngredients(__instance._materialContainers);
                if (playSound)
                {
                    Game1.playSound("coin");
                }
            }

            if (list != null)
            {
                if (playSound)
                {
                    Game1.playSound("breathin");
                }

                CraftingRecipe.ConsumeAdditionalIngredients(list, __instance._materialContainers);
                if (!CraftingRecipe.DoesFarmerHaveAdditionalIngredientsInInventory(list, GetContainerContents(__instance._materialContainers)))
                {
                    Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Seasoning_UsedLast"));
                }
            }

            player.checkForQuestComplete(null, -1, -1, item, null, 2);
            if (!___cooking && player.craftingRecipes.ContainsKey(craftingRecipe.name))
            {
                player.craftingRecipes[craftingRecipe.name] += craftingRecipe.numberProducedPerCraft;
            }

            if (___cooking)
            {
                player.cookedRecipe(item.ItemId);
                Game1.stats.checkForCookingAchievements();

            }
            else
            {
                Game1.stats.checkForCraftingAchievements();
            }

            if (Game1.options.gamepadControls && ___heldItem != null && player.couldInventoryAcceptThisItem(___heldItem))
            {
                player.addItemToInventoryBool(___heldItem);
                ___heldItem = null;
            }
            return false;
        }

        public static IList<Item> GetContainerContents(List<IInventory> _materialContainers)
        {
            if (_materialContainers == null)
            {
                return null;
            }

            List<Item> list = new List<Item>();
            foreach (IInventory materialContainer in _materialContainers)
            {
                list.AddRange(materialContainer);
            }

            return list;
        }

        public static Dictionary<Item, int> FigureOutItems(CraftingRecipe recipe, List<IInventory> additionalInventories)
        {
            Dictionary<Item, int> items = new Dictionary<Item, int>();
            foreach (KeyValuePair<string, int> ingredient in recipe.recipeList)
            {
                string key = ingredient.Key;
                int num = ingredient.Value;
                bool flag = false;
                for (int num2 = Game1.player.Items.Count - 1; num2 >= 0; num2--)
                {
                    if (CraftingRecipe.ItemMatchesForCrafting(Game1.player.Items[num2], key))
                    {
                        int amount = num;
                        num -= Game1.player.Items[num2].Stack;
                        items.Add(Game1.player.Items[num2], Math.Min(Game1.player.Items[num2].Stack, amount));
                        if (num <= 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                }

                if (additionalInventories == null || flag)
                {
                    continue;
                }

                for (int i = 0; i < additionalInventories.Count; i++)
                {
                    IInventory inventory = additionalInventories[i];
                    if (inventory == null)
                    {
                        continue;
                    }
                    for (int num3 = inventory.Count - 1; num3 >= 0; num3--)
                    {
                        if (CraftingRecipe.ItemMatchesForCrafting(inventory[num3], key))
                        {
                            int num4 = Math.Min(num, inventory[num3].Stack);
                            num -= num4;
                            items.Add(inventory[num3], num4);

                            if (num <= 0)
                            {
                                break;
                            }
                        }
                    }


                    if (num <= 0)
                    {
                        break;
                    }
                }
            }

            return items;
        }

    }


    [HarmonyPatch(typeof(StardewValley.Item), nameof(Item.canStackWith))]
    class CanStackWith_Patch
    {
        [HarmonyLib.HarmonyPostfix]
        private static void Postfix(
        StardewValley.Item __instance, ref bool __result, ref ISalable other)
        {
            //Prevent items with different edibility values from stacking. 
            if (__instance is Object @object && other is Object object2 && object2.Edibility != @object.Edibility)
            {
                __result = false;
                return;
            }
        }
    }


    [HarmonyPatch(typeof(StardewValley.Buildings.Building), "CheckItemConversionRule")]
    class MillItemConversion_patch
    {
        [HarmonyLib.HarmonyPrefix]
        private static bool Prefix(
        StardewValley.Buildings.Building __instance, BuildingItemConversion conversion, ItemQueryContext itemQueryContext)
        {
            if(__instance.buildingType.Value != "Mill") {
                return true;
            }
            ModEntry.Instance.Monitor.Log("Starting to run the logic for Mills", LogLevel.Trace);
            int[] convertAmount;
            int[] currentCount = { 0, 0, 0, 0, 0 };
            List<int[]> consumeCount = new List<int[]>();
            Chest sourceChest = __instance.GetBuildingChest(conversion.SourceChest);
            Chest destinationChest = __instance.GetBuildingChest(conversion.DestinationChest);
            if (sourceChest == null)
            {
                return false;
            }
            foreach (Item item in sourceChest.Items)
            {
                if (item == null)
                {
                    continue;
                }
                bool fail = false;
                foreach (string requiredTag in conversion.RequiredTags)
                {
                    if (!item.HasContextTag(requiredTag))
                    {
                        fail = true;
                        break;
                    }
                }
                if (fail)
                {
                    continue;
                }
                currentCount[item.Quality] += item.Stack;
            }
            convertAmount = GetConversionsNum(currentCount, consumeCount,conversion.RequiredCount, conversion.MaxDailyConversions);
            if (convertAmount.Sum() == 0)
            {
                return false;
            }
            int totalConversions = 0; 
            int[] requiredAmount = { 0, 0, 0, 0, 0 };
            ModEntry.Instance.Monitor.Log($"Will try to produce [{string.Join(", ", convertAmount)}]", LogLevel.Trace);
            for (int j = 0; j < convertAmount.Sum(); j++)
            {
                bool conversionCreatedItem = false;
                for (int i = 0; i < conversion.ProducedItems.Count; i++)
                {
                    GenericSpawnItemDataWithCondition producedItem = conversion.ProducedItems[i];
                    if (GameStateQuery.CheckConditions(producedItem.Condition, __instance.GetParentLocation()))
                    {
                        Item item = ItemQueryResolver.TryResolveRandomItem(producedItem, itemQueryContext);

                        //round off quality
                        double average_quality = (double)(consumeCount[j][1] + consumeCount[j][2]*2 + consumeCount[j][4] * 3) / consumeCount[j].Sum();
                        double chance = average_quality - (int)average_quality;
                        int quality = (int)average_quality;
                        if(Game1.random.NextDouble() < chance)
                        {quality++;}

                        //add +- 1 with random chance
                        int r = Game1.random.Next(15);
                        if(r == 0)
                        {quality++;}
                        else if (r < 5)
                        {quality--;}
                        if (quality >= 3)
                        {quality = 4;}
                        if (quality < 0)
                        { quality = 0; }
                        item.Quality = quality;
                        //currently, quality is the quality of the worst ingredient. Can be possibly changed to some sort of mean.
                        int producedCount = item.Stack;
                        Item item2 = destinationChest.addItem(item);
                        if (item2 == null || item2.Stack != producedCount)
                        {
                            conversionCreatedItem = true;
                        }
                    }
                }
                if (conversionCreatedItem)
                {
                    totalConversions++;
                    for(int k = 0; k < 5; k++)
                    {
                        requiredAmount[k] += consumeCount[j][k];
                    }
                }
            }
            if (totalConversions <= 0)
            {
                return false;
            }
            ModEntry.Instance.Monitor.Log($"Need to delete [{string.Join(", ", requiredAmount)}]", LogLevel.Trace);

            for (int i = 0; i < sourceChest.Items.Count; i++)
            {
                Item item = sourceChest.Items[i];
                if (item == null)
                {
                    continue;
                }
                bool fail = false;
                foreach (string requiredTag in conversion.RequiredTags)
                {
                    if (!item.HasContextTag(requiredTag))
                    {
                        fail = true;
                        break;
                    }
                }
                if (!fail)
                {
                    int consumedAmount = Math.Min(requiredAmount[item.Quality], item.Stack);
                    sourceChest.Items[i] = item.ConsumeStack(consumedAmount);
                    requiredAmount[item.Quality] -= consumedAmount;
                    if (requiredAmount.Sum() <= 0)
                    {
                        break;
                    }
                }
            }
            return false;
        }

        public static int[] GetConversionsNum(int[] currentCount, List<int[]> consumeCounts, int requiredCount, int maxConversions)
        {
            int currCount = 0;
            int[] conversions = { 0, 0, 0, 0, 0 };
            int[] tempCount;
            int[] counts = currentCount;
            //this is stupid but it should work
            if(maxConversions == -1)
            {
                maxConversions = int.MaxValue;
            }
            for (int i = conversions.Length - 1; i >= 0 && maxConversions > 0; i--)
            {
                //all conversions done one by one as items are created one by one in the end
                //"recipes" for conversions stored in the list.
                while (counts[i] + currCount >= requiredCount && maxConversions > 0)
                {
                    conversions[i]++;
                    tempCount = Enumerable.Repeat(0, 5).ToArray();
                    counts[i] -= requiredCount - currCount;
                    tempCount[i] = requiredCount - currCount;
                    int j = i + 1;
                    int prevCount = currCount;
                    while (currCount > 0)
                    {
                        currCount -= Math.Min(counts[j], currCount);
                        tempCount[j] = Math.Min(counts[j], prevCount);
                        counts[j] -= Math.Min(counts[j], prevCount);
                        j++;
                    }
                    maxConversions--;
                    consumeCounts.Add(tempCount);
                }
                currCount += counts[i];
            }
            return conversions;
        }
    }
}
