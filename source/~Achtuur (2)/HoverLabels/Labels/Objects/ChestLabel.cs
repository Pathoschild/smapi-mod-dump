/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Objects;
internal class ChestLabel : ObjectLabel
{
    Chest hoverChest;
    public ChestLabel(int? priority = null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        SObject sobj = GetCursorObject(cursorTile);

        return sobj is not null && sobj is Chest;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        hoverChest = hoverObject as Chest;

        IEnumerable<string> inventoryContents = ListInventoryContents(hoverChest.items, ModEntry.IsShowDetailButtonPressed());
        Description = inventoryContents.ToList();

        string showAllMsg = GetShowAllMessage(hoverChest.items);
        if (showAllMsg is not null)
            Description.Add(showAllMsg);

        if (!ModEntry.IsAlternativeSortButtonPressed() && inventoryContents.Count() > 1)
            Description.Add(I18n.LabelChestAltsort(ModEntry.GetAlternativeSortButtonName()));

    }


    /// <summary>
    /// Returns a "name: amount" string for each unique item *name* in <paramref name="inventory"/>
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
    public static IEnumerable<string> ListInventoryContents(IEnumerable<Item> inventory, bool showAll)
    {
        if (inventory is null || inventory.Count() <= 0)
            yield break;

        // Make a dict since multiple items with same quality should be in the same entry
        // to avoid a cluttered list
        Dictionary<string, int> inventoryItems = new Dictionary<string, int>();
        foreach (Item item in inventory)
        {
            if (!inventoryItems.ContainsKey(item.DisplayName))
                inventoryItems.Add(item.DisplayName, 0);
            inventoryItems[item.DisplayName] += item.Stack;
        }

        // Either take entire dict, or first x entries based on button
        int listSize = showAll ? inventoryItems.Count : ModEntry.Instance.Config.LabelListMaxSize;

        // Sort by name or number of items depending on button
        IOrderedEnumerable<KeyValuePair<string, int>> orderedDict;
        if (!ModEntry.IsAlternativeSortButtonPressed())
            orderedDict = inventoryItems.OrderBy(item => item.Key); //sort by name
        else
            orderedDict = inventoryItems.OrderByDescending(item => item.Value).ThenBy(item => item.Key); //sort by amount -> name


        // Loop through items dictionary sorted by name
        foreach ((string name, int amount) in orderedDict.Take(listSize))
        {
            yield return $"{name}: {amount}";
        }
    }

    public static string GetShowAllMessage(IEnumerable<Item> inventoryItems)
    {
        int allContentLength = ListInventoryContents(inventoryItems, showAll: true).Count();
        int inventoryCountListSizeDifference = allContentLength - ModEntry.Instance.Config.LabelListMaxSize;
        if (!ModEntry.IsShowDetailButtonPressed() && inventoryCountListSizeDifference > 0)
            return I18n.LabelPressShowmore(ModEntry.GetShowDetailButtonName(), inventoryCountListSizeDifference);

        return null;
    }
}
