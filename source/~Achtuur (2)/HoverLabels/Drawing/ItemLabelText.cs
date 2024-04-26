/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Drawing;
internal class ItemLabelText: LabelText
{
    private float Scale => (ModEntry.Instance.Config.SmallItemIconLabel) ? .75f : 1f;
    private Vector2 SpriteDim => new Vector2(64, 64) * Scale;
    public override Vector2 DrawSize => new Vector2(SpriteDim.X + textDim.X + Margin(), Math.Max(textDim.Y, SpriteDim.Y));
    private Vector2 textDim => Font.MeasureString(Text);

    Item item;

    public ItemLabelText(string qualified_id): base(string.Empty)
    {
        item = ItemRegistry.Create(qualified_id);
        Text = this.item.DisplayName;
    }

    public ItemLabelText(Item item): base(string.Empty)
    {
        this.item = item;
        Text = this.item.DisplayName;
    }
    public ItemLabelText(string qualified_id, string desc): base(string.Empty)
    {
        item = ItemRegistry.Create(qualified_id);
        Text = desc;
    }

    public ItemLabelText(Item item, string desc): base(string.Empty)
    {
        this.item = item;
        Text = desc;
    }

    public void SetFont(SpriteFont font)
    {
        this.Font = font;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        // draw item icon
        Vector2 item_pos = position - new Vector2(LabelText.Margin()) * 0.5f/Scale;
        item.drawInMenu(spriteBatch, item_pos, Scale, 1f, 1, StackDrawType.Draw, Color.White, drawShadow: false);

        // draw text
        Vector2 text_offset = new Vector2(SpriteDim.X + Margin()/2, SpriteDim.Y/2 - textDim.Y/2);
        base.Draw(spriteBatch, position + text_offset);
    }
}
