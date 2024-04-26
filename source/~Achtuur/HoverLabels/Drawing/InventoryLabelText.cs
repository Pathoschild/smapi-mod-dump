/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Drawing;
internal class InventoryLabelText : LabelText
{
    // make these static after testing
    private float Scale => (ModEntry.Instance.Config.SmallItemIconLabel) ? 0.75f : 1f;
    private Vector2 SpriteDim => new Vector2(72, 64) * Scale;
    private int ItemsPerRow => Math.Max(6, (int) Math.Sqrt(items.Count));
    public override Vector2 DrawSize => CalculateDrawSize();
    public int ItemCount => items.Count;

    private bool NoSubIcons => items.All(i => i.Quality == 0 && i.Stack == 1);

    List<Item> items;
    public InventoryLabelText(IEnumerable<Item> items) : base(string.Empty)
    {
        this.items = items.ToList();
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        int col = 0;
        int row = 0;
        foreach (Item item in items)
        {
            Vector2 margin = new Vector2(col * LabelText.Margin(), row * LabelText.Margin());
            Vector2 offset = margin + new Vector2(col * SpriteDim.X, row * SpriteDim.Y);
            item.drawInMenu(spriteBatch, position + offset, Scale, 1f, 1, StackDrawType.Draw, Color.White, drawShadow: false);
            col++;
            if (col >= ItemsPerRow)
            {
                col = 0;
                row++;
            }
        }
    }

    private Vector2 CalculateDrawSize()
    {
        int items_per_row = Math.Min(ItemsPerRow, items.Count);
        int rows = (int)Math.Ceiling(items.Count / (float)items_per_row);
        Vector2 item_size = new Vector2(items_per_row * SpriteDim.X, rows * SpriteDim.Y);
        Vector2 margin = new Vector2(items_per_row * LabelText.Margin(), rows * LabelText.Margin());

        if (NoSubIcons)
            return item_size + margin;

        // little bit of extra space for stack size text
        Vector2 stack_size_margin = new Vector2(LabelText.Margin()); 
        return item_size + margin + stack_size_margin;
    }
}
