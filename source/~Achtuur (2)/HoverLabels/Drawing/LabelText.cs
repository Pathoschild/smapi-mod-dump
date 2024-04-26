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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Drawing;
public class LabelText
{
    protected SpriteFont Font;
    public string Text { get; set; }

    public virtual Vector2 DrawSize => Font.MeasureString(Text);

    public float Width => DrawSize.X + 2*Margin();
    public float Height => DrawSize.Y + 2*Margin();

    public static float Margin()
    {
        if (ModEntry.Instance.Config.CompactLabels)
            return 10f;
        return 12f;
    }
    public LabelText(string text)
    {
        this.Font = Game1.smallFont;
        this.Text = text;
    }


    public virtual void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.DrawString(Font, Text, position + new Vector2(2, 3), Game1.textShadowColor);
        spriteBatch.DrawString(Font, Text, position, Game1.textColor);
    }
}
