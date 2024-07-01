/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Extensions;
using AchtuurCore.Framework.Borders;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Labels.Buildings;
internal class MillLabel : BuildingLabel
{
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return ModEntry.IsPlayerOnFarm()
            && BuildingLabel.GetFarmBuildings().Any(b => b.GetData().Name.Contains("mill") && b.GetRect().Contains(cursorTile));
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        int labelMaxSize = ModEntry.GetLabelSizeLimit();

        List<Item> inputItems = hoverBuilding.GetBuildingChest("Input").Items.Where(item => item is not null).ToList();
        List<Item> outputItems = hoverBuilding.GetBuildingChest("Output").Items.Where(item => item is not null).ToList();

        if (outputItems.Count > 0)
        {
            AddBorder(I18n.Ready() + ":");
            this.GenerateInventoryDescription(outputItems);
        }

        if (inputItems.Count > 0)
        {
            AddBorder(I18n.Processing() + ":");
            this.GenerateInventoryDescription(inputItems);
        }
    }

    /// <summary>
    /// Returns number of items that have been displayed in label
    /// </summary>
    /// <param name="items"></param>
    /// <param name="labelSize"></param>
    /// <returns></returns>
    public void GenerateInventoryDescription(IEnumerable<Item> items)
    {
        //Dictionary<string, int> groupedItems = items
        //    .GroupBy(item => item.DisplayName)
        //    .ToDictionary(group => group.Key, group => group.Sum(item => item.Stack));
            
        //var orderedItems = groupedItems
        //    .OrderByDescending(entry => entry.Value)
        //    .ThenBy(entry => entry.Key);

        //foreach ((string name, int amount) in orderedItems.Select(x => (x.Key, x.Value)))
        //{
        //    this.Description.Add($"> {name}: {amount}");
        //}

        var orderedItems = items
            .OrderByDescending(item => item.Stack)
            .ThenBy(item => item.DisplayName);
    
        foreach (Item item in orderedItems)
        {
            ItemLabel itemlabel = new(item);
            AppendLabelToBorder(itemlabel);
        }
    }
}
