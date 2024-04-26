/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Extensions;
using StardewValley.Quests;
using SObject = StardewValley.Object;

namespace HelpWanted;

internal partial class ModEntry
{
    /// <summary>获取随机物品</summary>
    public static string GetRandomItem(string result, List<string>? possibleItems)
    {
        // 获取允许的任务物品列表
        var items = GetRandomItemList(possibleItems);

        // 如果物品列表为空或者物品列表包含原随机结果,则返回原随机结果
        if (items is null || items.Contains(result))
            return result;

        // 遍历物品列表,去掉其中不符合条件的物品
        items = items.Where(item => Game1.objectData.ContainsKey(item)).ToList();
        // 如果物品列表为空,则返回原随机结果,否则返回随机物品
        return !items.Any() ? result : Game1.random.ChooseFrom(items);
    }

    /// <summary>获取随机物品列表</summary>
    private static List<string>? GetRandomItemList(List<string>? possibleItems)
    {
        // 如果模组未启用,或者必须为喜爱物品和必须为喜欢物品均为false,或者今日任务不是物品交付任务,则返回null
        if (config is { MustLikeItem: false, MustLoveItem: false } ||
            Game1.questOfTheDay is not ItemDeliveryQuest itemDeliveryQuest)
            return null;

        // 获取普遍喜爱物品和普遍喜欢物品
        var itemString = Game1.NPCGiftTastes["Universal_Love"];
        if (!config.MustLoveItem)
        {
            itemString += " " + Game1.NPCGiftTastes["Universal_Like"];
        }

        // 获取任务目标NPC的喜爱物品和喜欢物品
        var npcName = itemDeliveryQuest.target.Value;
        if (npcName is null || !Game1.NPCGiftTastes.TryGetValue(npcName, out var data))
            return null;
        var split = data.Split('/');
        if (split.Length < 4) return null;
        itemString += " " + split[1] + (config.MustLoveItem ? "" : " " + split[3]);

        // 如果物品字符串为空,则返回null
        if (string.IsNullOrEmpty(itemString))
            return null;

        // 遍历所有物品,移除不符合条件的物品
        var items = new List<string>(itemString.Split(' '));
        items = items.Where(item =>
            {
                var sObject = new SObject(item, 1);
                return !(!config.AllowArtisanGoods && sObject.Category == SObject.artisanGoodsCategory) &&
                       !(config.MaxPrice > 0 && sObject.Price > config.MaxPrice) &&
                       !(int.TryParse(item, out var value) && value < 0);
            }
        ).ToList();

        // 如果物品列表为空,则返回null
        if (!items.Any()) return null;

        // 如果物品列表不为空,则根据是否忽略原版物品限制执行不同操作
        switch (config.IgnoreVanillaItemRestriction)
        {
            // 如果忽略原版物品限制,则返回该物品列表
            case true:
                return items;
            // 如果不忽略原版物品限制,且可能的物品列表不为空,则对该列表进行筛选        
            case false when possibleItems?.Any() != null:
                possibleItems = possibleItems.Where(item =>
                {
                    var sObject = new SObject(item, 1);
                    return items.Contains(item) &&
                           items.Contains(sObject.Category.ToString()) &&
                           !(!config.AllowArtisanGoods && sObject.Category == SObject.artisanGoodsCategory) &&
                           !(config.MaxPrice > 0 && sObject.Price > config.MaxPrice);
                }).ToList();
                if (possibleItems.Any())
                {
                    return possibleItems;
                }

                break;
        }

        return null;
    }

    public static List<string> GetPossibleCrops(List<string> oldList)
    {
        var newList = GetRandomItemList(oldList);
        return newList is null || !newList.Any() ? oldList : newList;
    }
}