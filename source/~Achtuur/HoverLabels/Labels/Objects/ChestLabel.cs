/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using HoverLabels.Drawing;
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

        InventoryLabelText inventoryLabels = ListInventoryContents(hoverChest.Items, ModEntry.IsShowDetailButtonPressed());
        if (hoverChest.Items.Count > 0)
            AddBorder(inventoryLabels);

        Border control_border = new Border();
        string showAllMsg = GetShowAllMessage(hoverChest.Items);
        if (showAllMsg is not null)
            control_border.AddLabelText(new LabelText(showAllMsg));

        if (!ModEntry.IsAlternativeSortButtonPressed() && hoverChest.Items.Count() > 1)
            control_border.AddLabelText(new LabelText(I18n.LabelChestAltsort(ModEntry.GetAlternativeSortButtonName())));
        AddBorder(control_border);
    }


    /// <summary>
    /// Returns a "name: amount" string for each unique item *name* in <paramref name="inventory"/>
    /// </summary>
    /// <param name="inventory"></param>
    /// <returns></returns>
    public static InventoryLabelText ListInventoryContents(IEnumerable<Item> inventory, bool showAll)
    {
        if (inventory is null || inventory.Count() <= 0)
            return new InventoryLabelText(new List<Item>());

        // Either take entire dict, or first x entries based on button
        int listSize = showAll ? inventory.Count() : ModEntry.Instance.Config.LabelListMaxSize;

        // this will show the items as they are in the chest
        if (!ModEntry.IsAlternativeSortButtonPressed())
            return new InventoryLabelText(inventory.Take(listSize));

        IOrderedEnumerable<Item> orderedItems = inventory
            .OrderByDescending(item => item.Stack)
            .ThenBy(item => item.DisplayName)
            .ThenByDescending(item => item.quality.Value);

        // Loop through items dictionary sorted by name
        return new InventoryLabelText(orderedItems.Take(listSize));
    }

    public static string GetShowAllMessage(IEnumerable<Item> inventoryItems)
    {
        int allContentLength = ListInventoryContents(inventoryItems, showAll: true).ItemCount;
        int inventoryCountListSizeDifference = allContentLength - ModEntry.Instance.Config.LabelListMaxSize;
        if (!ModEntry.IsShowDetailButtonPressed() && inventoryCountListSizeDifference > 0)
            return I18n.LabelPressShowmore(ModEntry.GetShowDetailButtonName(), inventoryCountListSizeDifference);

        return null;
    }
}
