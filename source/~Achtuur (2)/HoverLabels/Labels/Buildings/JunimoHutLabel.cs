/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using HoverLabels.Framework;
using StardewValley.Buildings;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using AchtuurCore.Extensions;
using Microsoft.Xna.Framework.Graphics;
using AchtuurCore.Utility;
using AchtuurCore.Framework;
using StardewValley.Objects;
using HoverLabels.Labels.Objects;
using SObject = StardewValley.Object;

namespace HoverLabels.Labels.Buildings;
internal class JunimoHutLabel : BuildingLabel
{
    JunimoHut hoverHut;
    public JunimoHutLabel(int? priority = null) : base(priority)
    {
    }

    protected override void ResetLabel()
    {
        base.ResetLabel();
        hoverHut = null;
    }
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        // Assume that greenhouse can only be on farm and outdoors
        if (!Game1.currentLocation.IsFarm || !Game1.currentLocation.IsOutdoors)
            return false;

        IEnumerable<JunimoHut> farmHuts = Game1.getFarm().buildings.Where(b => b is JunimoHut).Cast<JunimoHut>();
        return farmHuts.Any(hut => hut.GetRect().Contains(cursorTile));
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
        this.hoverHut = this.hoverBuilding as JunimoHut;
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();

        Chest hutInventory = hoverHut.output.Value;
        IEnumerable<Item> items = hutInventory.items.Where(item => item.ParentSheetIndex != SObject.prismaticShardIndex);

        // If difference in length, prismatic shard was filtered out
        if (hutInventory.items.Count != items.Count())
            this.Name += " (Prismatic)";

        IEnumerable<string> inventoryContents = ChestLabel.ListInventoryContents(items, ModEntry.IsShowDetailButtonPressed());
        Description = inventoryContents.ToList();

        string showAllMsg = ChestLabel.GetShowAllMessage(hutInventory.items);
        if (showAllMsg is not null)
            Description.Add(showAllMsg);

        if (!ModEntry.IsAlternativeSortButtonPressed() && inventoryContents.Count() > 1)
            Description.Add(I18n.LabelChestAltsort(ModEntry.GetAlternativeSortButtonName()));

        Description.Add(I18n.LabelShowrange(ModEntry.GetShowDetailButtonName()));
    }

    public override void DrawOnOverlay(SpriteBatch spriteBatch)
    {
        if (!ModEntry.IsShowDetailButtonPressed())
            return;

        IEnumerable<Vector2> hutRange = GetHutRangeRect(hoverHut).GetTiles();
        Overlay.DrawTiles(spriteBatch, hutRange, tileTexture: Overlay.GreenTilePlacementTexture);

    }

    private static Rectangle GetHutRangeRect(JunimoHut hut)
    {
        return new Rectangle(hut.tileX.Value - 7, hut.tileY.Value - 7, 17, 17);
    }
}
