/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Framework.Borders;
using HoverLabels.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.GameData.FruitTrees;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels;
internal class FruittreeLabel : BaseLabel
{
    FruitTree hoverTree;
    FruitTreeData treeFruit;
    public FruittreeLabel(int? priority=null) : base(priority)
    {
    }

    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return Game1.currentLocation.terrainFeatures.ContainsKey(cursorTile)
            && Game1.currentLocation.terrainFeatures[cursorTile] is FruitTree;
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        this.hoverTree = Game1.currentLocation.terrainFeatures[cursorTile] as FruitTree;
        this.treeFruit = hoverTree.GetData();
    }
    public override void GenerateLabel()
    {
        SObject fruit = ModEntry.GetObjectWithId(treeFruit.Fruit.First().ItemId);
        string FruitName = fruit.DisplayName;
        AddBorder(new ItemLabel(fruit));

        // Not fully grown
        if (hoverTree.daysUntilMature.Value > 0)
        {
            int days = this.hoverTree.daysUntilMature.Value;
            AddBorder(I18n.LabelFruittreeGrow(days));
        }
        // Fully grown
        else if (this.hoverTree.fruit.Count() > 0)
        {
            int fruitAmount = this.hoverTree.fruit.Count();
            ItemLabel itemLabel = new ItemLabel(fruit, I18n.LabelFruittreeAmount(fruitAmount));
            AddBorder(itemLabel);
        }
    }
}
