/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using System.Text.RegularExpressions;
using StardewValley;
using StardewValley.Extensions;

namespace StardewWebApi.Game.Items;

public static class ItemUtilities
{
    public static List<Item> GetAllItems()
    {
        return ItemRegistry.ItemTypes
            .SelectMany(it => it.GetAllData().Select(id => it.CreateItem(id)))
            .ToList();
    }

    public static List<Item> GetAllItemsByType(string itemType)
    {
        // Wrap it in parenteses if it isn't already, since item type identifiers require them
        if (!Regex.IsMatch(itemType, @"^\(.*\)$")) itemType = $"({itemType})";

        return ItemRegistry.ItemTypes
            .Where(it => it.Identifier == itemType)
            .SelectMany(it => it.GetAllData().Select(id => it.CreateItem(id)))
            .ToList();
    }

    public static Item? GetItemByTypeAndId(string itemType, string itemId, int amount = 1, int quality = 0)
    {
        // Wrap it in parenteses if it isn't already, since item type identifiers require them
        if (!Regex.IsMatch(itemType, @"^\(.*\)$")) itemType = $"({itemType})";

        return GetItemByFullyQualifiedId($"{itemType}{itemId}");
    }

    public static Item? GetItemByFullyQualifiedId(string itemId, int amount = 1, int quality = 0)
    {
        var itemMetadata = ItemRegistry.ResolveMetadata(itemId);

        return itemMetadata?.Exists() == true
            ? itemMetadata.CreateItem(amount, quality)
            : null;
    }

    public static Item? GetItemByDisplayName(string itemName, int amount = 1, int quality = 0)
    {
        itemName = itemName.ToLower();

        try
        {
            var parsedItemData = ItemRegistry.ItemTypes.Select(it =>
            {
                return it.GetAllData().FirstOrDefault(i =>
                    i.DisplayName.ToLower() == itemName
                );
            }).FirstOrDefault(i => i is not null);

            return parsedItemData is not null
                ? GetItemByFullyQualifiedId(parsedItemData.QualifiedItemId, amount, quality)
                : null;
        }
        catch
        {
            return null;
        }
    }
}