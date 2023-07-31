/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Extensions;
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
    Mill hoverMill;
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return ModEntry.IsPlayerOnFarm()
            && BuildingLabel.GetFarmBuildings().Any(b => b is Mill && b.GetRect().Contains(cursorTile));
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        hoverMill = hoverBuilding as Mill;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        int labelMaxSize = ModEntry.GetLabelSizeLimit();
        List<Item> inputItems = hoverMill.input.Value.items.Where(item => item is not null).ToList();
        List<Item> outputItems = hoverMill.output.Value.items.Where(item => item is not null).ToList();

        if (outputItems.Count > 0)
        {
            this.Description.Add("Finished:");
            this.GenerateInventoryDescription(outputItems);
        }

        if (inputItems.Count > 0)
        {
            this.Description.Add("Currently Processing:");
            this.GenerateInventoryDescription(inputItems);
        }
    }

    /// <summary>
    /// Returns number of items that have been displayed in label
    /// </summary>
    /// <param name="items"></param>
    /// <param name="labelSize"></param>
    /// <returns></returns>
    public int GenerateInventoryDescription(IEnumerable<Item> items)
    {
        Dictionary<string, int> groupedItems = items
            .GroupBy(item => item.DisplayName)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Stack));
            
        var orderedItems = groupedItems
            .OrderByDescending(entry => entry.Value)
            .ThenBy(entry => entry.Key);

        foreach ((string name, int amount) in orderedItems.Select(x => (x.Key, x.Value)))
        {
            this.Description.Add($"> {name}: {amount}");
        }

        return groupedItems.Count;
    }
}
