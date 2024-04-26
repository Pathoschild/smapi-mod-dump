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
using AchtuurCore.Framework;
using AchtuurCore.Utility;
using HoverLabels.Drawing;
using HoverLabels.Framework;
using HoverLabels.Labels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels;
internal class LabelOverlay : Overlay
{
    internal readonly Vector2 baseOffset = new Vector2(75f, 75f);

    BorderDrawer borderDrawer = new BorderDrawer();

    public LabelOverlay() : base()
    {
        borderDrawer = new();
    }

    public override void Enable()
    {
        base.Enable();
    }

    public override void Disable()
    {
        base.Disable();
    }

    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {
        if (!ModEntry.Instance.LabelManager.HasLabel())
            return;

        // Draw additional stuff from label first, so other stuff is drawn over it
        ModEntry.Instance.LabelManager.CurrentLabel.DrawOnOverlay(spriteBatch);
        Vector2 cursorPos = ModEntry.Instance.Helper.Input.GetCursorPosition().AbsolutePixels;
        this.DrawLabel(spriteBatch, cursorPos);
    }

    public void DrawLabel(SpriteBatch spriteBatch, Vector2 cursorPos)
    {
        // add labels to border drawer
        borderDrawer.Reset();
        List<Border> nonEmptyBorders = ModEntry.Instance.LabelManager.GetLabelContents()
            .Where(b => !b.IsEmpty).ToList();
        borderDrawer.AddBorder(nonEmptyBorders);

        // Get coordinates of cursor on screen
        Vector2 offset = GetOffset(cursorPos);
        Vector2 cursorCoords = AchtuurCore.Utility.Drawing.GetPositionScreenCoords(cursorPos) + offset;
        cursorCoords -= new Vector2(64); // displace by tile size to look a bit nicer

        // draw border
        borderDrawer.Draw(spriteBatch, cursorCoords);
    }

    private Vector2 GetOffset(Vector2 cursorPos)
    {
        // Label defaults to bottom right of cursor
        Vector2 offset = baseOffset;

        Rectangle visibleRect = AchtuurCore.Utility.Drawing.GetVisibleArea();
        Vector2 visibleCoords = new(visibleRect.Width + visibleRect.X, visibleRect.Height + visibleRect.Y);

        Vector2 labelSize = borderDrawer.BorderSize();

        // Overflow on right side -> put label on left
        if (cursorPos.X + labelSize.X + baseOffset.X >= visibleCoords.X - 2f * Game1.tileSize)
            offset.X -= baseOffset.X * 1.1f + labelSize.X;

        // Overflow on bottom side -> put label on top
        if (cursorPos.Y + labelSize.Y + baseOffset.Y >= visibleCoords.Y - 3f * Game1.tileSize)
            offset.Y -= baseOffset.Y * 1.1f + labelSize.Y;

        // If putting label on top causes overflow on top size, go back to default offset
        // Overflow on bottom part of screen is preferred
        if (cursorPos.Y + offset.Y <= visibleRect.Y)
            offset.Y = baseOffset.Y;

        return offset;
    }
}
