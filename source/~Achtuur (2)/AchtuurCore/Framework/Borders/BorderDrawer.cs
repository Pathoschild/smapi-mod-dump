/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Borders;

public class BorderDrawer
{
    protected List<Border> borders = new List<Border>();

    public BorderDrawer() : this(Enumerable.Empty<Border>())
    {
    }

    public BorderDrawer(IEnumerable<Border> borders)
    {
        this.borders = borders.ToList();
    }

    public void SetBorder(IEnumerable<Border> borders)
    {
        this.borders = borders.ToList();
    }

    public void AddBorder(Label label_text)
    {
        List<Label> list = new List<Label>() { label_text };
        this.AddBorder(list);
    }

    public void AddBorder(IEnumerable<Label> label_texts)
    {
        Border border = new(label_texts);
        borders.Add(border);
    }

    public void AddBorder(IEnumerable<Border> borders)
    {
        foreach(Border border in borders)
            AddBorder(border);
    }

    public void AddBorder(Border border)
    {
        borders.Add(border);
    }

    public void Reset()
    {
        borders.Clear();
    }

    public Vector2 BorderSize()
    {
        Vector2 max = Vector2.Zero;
        foreach (Border border in borders)
        {
            if (border.IsEmpty)
                continue;
            max.X = Math.Max(max.X, border.TotalWidth);
            max.Y = Math.Max(max.Y, border.TotalHeight);
        }
        return max;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 offset, float? fixed_width=null, float? fixed_height=null)
    {
        Vector2 total_offset = offset;
        float width = fixed_width ?? BorderSize().X;
        for (int i = 0; i < borders.Count; i++)
        {
            if (i == 0)
            {
                borders[i].TopBorder = BorderType.Top;
            }
            if (i == borders.Count - 1)
            {
                borders[i].BottomBorder = BorderType.Bottom;
            }
            borders[i].FixedWidth = width;
            borders[i].Draw(spriteBatch, total_offset);
            
            // debug draw
            if (ModEntry.DebugDraw)
                spriteBatch.DrawBorder(total_offset + new Vector2(32), new Vector2(borders[i].TotalWidth, borders[i].TotalHeight), bordersize: 5);

            // border draws from top left, so shift offset to the bottom of the border
            // this will then be the top of the next border
            total_offset.Y += borders[i].BottomRight.Y;
            borders[i].FixedWidth = null; // reset fixed witdth to initial state
        }
    }
}