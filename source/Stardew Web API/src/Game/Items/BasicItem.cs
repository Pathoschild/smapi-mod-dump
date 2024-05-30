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

public class BasicItem
{
    private readonly Item _item;

    private BasicItem(Item item)
    {
        _item = item;
    }
    public static BasicItem? FromItem(Item? item) => item is not null ? new(item) : null;
    public static IEnumerable<BasicItem> FromItems(IEnumerable<Item> items) =>
        items.Select(i => FromItem(i)!);

    public string ItemId => _item.ItemId;
    public string TypeDefinitionId => _item.TypeDefinitionId;
    public string QualifiedItemId => _item.QualifiedItemId;
    public string Name => _item.Name;
    public string DisplayName => _item.DisplayName;
    public int Quality => _item.Quality;
    public int Category => _item.Category;
    public int StackSize => _item.Stack;
    public string Url => $"/api/v1/items/id/{QualifiedItemId}";
}