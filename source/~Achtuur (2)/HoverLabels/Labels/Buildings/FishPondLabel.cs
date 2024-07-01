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
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Buildings;
internal class FishPondLabel : BuildingLabel
{
    FishPond hoverPond;

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return Game1.currentLocation.IsFarm 
            && Game1.currentLocation.IsOutdoors
            && Game1.getFarm().buildings.Any(b => b is FishPond && b.GetRect().Contains(cursorTile));
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        hoverPond = this.hoverBuilding as FishPond;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();
        // add fish name to label name
        if (hoverPond.fishType is null || hoverPond.fishType.Value is null || hoverPond.fishType.Value.Length == 0)
            return;

        SObject fish = new(hoverPond.fishType.Value, 1);
        //this.Name += $" ({fish.DisplayName})";
        ResetBorders();
        AddBorder(new ItemLabel(fish.QualifiedItemId, "Fish Pond"));

        AddBorder($"Occupancy: {hoverPond.currentOccupants}/{hoverPond.maxOccupants}");

        if (this.hoverPond.neededItem.Value is not null)
        {
            AddBorder(I18n.LabelFishpondQuest(fish.DisplayName));
            AppendLabelToBorder($"> {this.hoverPond.neededItem.Value.DisplayName} (x{this.hoverPond.neededItemCount.Value})");
        }
        
    }
}
