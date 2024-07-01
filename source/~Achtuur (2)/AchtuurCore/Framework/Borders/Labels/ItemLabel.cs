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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Framework.Borders;

public class ItemLabel: Label
{
    private float Scale => 1f;
    private Vector2 SpriteDim => new Vector2(64, 64) * Scale;

    /// <summary>
    /// Width of the **drawn** label. 
    /// <c>Margin()</c> is added here since that is the distance between the sprite and the text.
    /// </summary>
    public override float Width => SpriteDim.X + textDim.X;

    /// <summary>
    /// Height of the **drawn** label. Text is moved to be centered, which is why some calculations are performed there.
    /// </summary>
    public override float Height => Math.Max(textDim.Y, SpriteDim.Y) + ExtraHeight;
    
    /// <summary>
    /// Make a little extra room at the bottom if the quality/stack is displayed.
    /// </summary>
    private float ExtraHeight => (Item.Quality != 0 || Item.Stack > 1) ? Margin() : 0;

    private Vector2 textDim => Font.MeasureString(Text);

    public Item Item;

    public Color SpriteColor;

    public ItemLabel(string qualified_id): this(ItemRegistry.Create(qualified_id), null)
    {
    }
    public ItemLabel(string qualified_id, string desc): this(ItemRegistry.Create(qualified_id), desc)
    {
    }

    public ItemLabel(Item item): this(item, null)
    {
    }

    public ItemLabel(Item item, string desc): base(string.Empty)
    {
        this.Item = item;
        if (desc is not null && desc.Length >= 0)
        {
            Text = desc;
        }
        else
        {
            SetItemNameAsDescription();
        }
        this.SpriteColor = Color.White;
    }

    public void SetFont(SpriteFont font)
    {
        this.Font = font;
    }

    public void SetColor(Color color)
    {
        this.SpriteColor = color;
    }
    public void SetItemNameAsDescription()
    {
        this.Text = string.Join("\n", Item.DisplayName.Split(' '));
        //this.Text = Item.DisplayName;
    }

    public void HideDescription()
    {
        Text = "";
    }

    public override void Draw(SpriteBatch sb, Vector2 position)
    {
        // draw item icon
        Vector2 item_pos = position - Label.MarginSize * 0.5f/Scale;
        Vector2 text_offset = new Vector2(SpriteDim.X + Margin()/2, SpriteDim.Y/2 - textDim.Y / 2f);
        if (text_offset.Y < 0)
        {
            item_pos.Y += Math.Abs(text_offset.Y);
            text_offset.Y = 0;
        }

        Item.drawInMenu(sb, item_pos, Scale, 1f, 1, StackDrawType.Draw, SpriteColor, drawShadow: false);
        base.Draw(sb, item_pos + text_offset);

        //sb.DrawBorder(item_pos, SpriteDim, color: Color.Magenta);
        //sb.DrawBorder(item_pos + text_offset, textDim, color: Color.Green);
    }
}
