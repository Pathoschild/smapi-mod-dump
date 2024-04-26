/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley;

namespace StardewWebApi.Game.Items;

public enum ItemRequestType
{
    ItemId = 1,
    DisplayName = 2
}

public class ItemRequest
{
    private ItemRequest(string nameOrId, ItemRequestType type, int amount = 1, int quality = 0)
    {
        switch (type)
        {
            case ItemRequestType.DisplayName:
                DisplayName = nameOrId;
                break;

            case ItemRequestType.ItemId:
            default:
                ItemId = nameOrId;
                break;
        }

        RequestType = type;
        Amount = amount;
        Quality = quality;
    }

    public static ItemRequest ByItemId(string id, int amount = 1, int quality = 0)
    {
        return new(id, ItemRequestType.ItemId, amount, quality);
    }

    public static ItemRequest ByDisplayName(string name, int amount = 1, int quality = 0)
    {
        return new(name, ItemRequestType.DisplayName, amount, quality);
    }

    public ItemRequestType RequestType { get; }
    public string? ItemId { get; }
    public string? DisplayName { get; }
    public int Amount { get; }
    public int Quality { get; }

    public Item? GetItem()
    {
        return RequestType switch
        {
            ItemRequestType.DisplayName => ItemUtilities.GetItemByDisplayName(DisplayName!, Amount, Quality),
            _ => ItemUtilities.GetItemByFullyQualifiedId(ItemId!, Amount, Quality),
        };
    }
}